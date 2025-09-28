using System.ComponentModel;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using NutsonCivilPlugin.PipeOnPV.Models;
using NutsonCivilPlugin.PipeOnPV.Services;
using Shared;

namespace NutsonCivilPlugin.PipeOnPV.ViewModels;

/// <summary>
/// Модель представления для работы с трубами на виде профиля
/// </summary>
public partial class ViewModelPipeOnPV : ObservableObject
{
    private readonly Document _doc;
    private readonly ModelDataProvider _modelDataProvider;
    private readonly NetworkPartsProvider _networkPartsProvider;
    private readonly SurfaceProvider _surfaceProvider;

    [ObservableProperty]
    private ProfileView _profileView;

    [ObservableProperty]
    private int _tabIndex;

    public PipesViewModel PipesVM { get; set; }
    public StructuresViewModel StructuresVM { get; set; }

    /// <summary>
    /// Действие, выполняемое после запроса выбора пользователем
    /// </summary>
    public Action? AfterRequestUserPick { get; internal set; }

    /// <summary>
    /// Действие, выполняемое перед запросом выбора пользователем
    /// </summary>
    public Action? BeforeRequestUserPick { get; internal set; }

    /// <summary>
    /// Действие, выполняемое при закрытии окна
    /// </summary>
    public Action<CancelEventArgs>? OnClosing { get; internal set; }

    /// <summary>
    /// Инициализирует новый экземпляр класса ViewModelPipeOnPV
    /// </summary>
    /// <param name="doc">Документ AutoCAD</param>
    /// <param name="modelDataProvider"></param>
    /// <param name="networkPartsProvider"></param>
    /// <param name="networkSettings"></param>
    public ViewModelPipeOnPV(
        Document doc,
        ModelDataProvider modelDataProvider,
        NetworkPartsProvider networkPartsProvider,
        SurfaceProvider surfaceProvider
    )
    {
        _doc = doc;
        _modelDataProvider = modelDataProvider;
        _networkPartsProvider = networkPartsProvider;
        _surfaceProvider = surfaceProvider;
    }

    [RelayCommand]
    private void SelectProfileView()
    {
        BeforeRequestUserPick?.Invoke();
        try
        {
            using var tr = _doc.TransactionManager.StartOpenCloseTransaction();
            Result
                .Success(_modelDataProvider.RequestSelection<ProfileView>(_doc, tr))
                .EnsureNotNull("Selected object is not ProfileView")
                .Tap(pv => ProfileView = pv)
                .Map(pv => _networkPartsProvider.GetNetworkPartsFromPV(pv, tr))
                .Tap(pv => PreparePartsToShow(pv, tr))
                .Tap(tr.Commit)
                .TapError(error => tr.Abort());
        }
        catch (Exception ex)
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ex.Message);
        }

        AfterRequestUserPick?.Invoke();
    }

    /// <summary>
    /// Подготавливает части для отображения
    /// </summary>
    /// <param name="collection">Коллекция моделей</param>
    /// <param name="domainType">Тип домена (труба или структура)</param>
    public void PreparePartsToShow(List<Part> parts, Transaction tr)
    {
        NetworkSettings
            .Create(parts[0].NetworkId, tr)
            .Tap(ns =>
            {
                var pipeParts = ns.GetPartFamilys(DomainType.Pipe);
                var structParts = ns.GetPartFamilys(DomainType.Structure);

                var (pipes, structures) = parts.Aggregate(
                    (pipes: new List<PipeModel>(), structure: new List<StructModel>()),
                    (acc, part) =>
                    {
                        if (part.Domain == DomainType.Pipe && part is Pipe pipe)
                        {
                            acc.pipes.Add(new(pipe, pipeParts));
                        }
                        else if (part.Domain == DomainType.Structure && part is Structure structure)
                        {
                            acc.structure.Add(new(structure, structParts));
                        }

                        return acc;
                    }
                );
                var baseEntities = _surfaceProvider
                    .GetSurfaces()
                    .ConvertAll(s => new BaseEntity(s.Name, s.Id));

                PipesVM = new() { Parts = pipes, Surfaces = baseEntities };
                StructuresVM = new() { Parts = structures, Surfaces = baseEntities };
            });
    }

    [RelayCommand]
    /// <summary>
    /// Устанавливает семейство части для выбранных труб
    /// </summary>
    /// <param name="selectedPipes">Выбранные трубы</param>
    /// <param name="PartFamilyName">Имя семейства части</param>
    /// <param name="PartSizeName">Имя размера части</param>
    private void ModifyElements()
    {
        using var loc = _doc.LockDocument();
        using var tr = _doc.TransactionManager.StartTransaction();

        if (TabIndex == 0)
        {
            PipesVM.Proccess(tr);
        }
        else
        {
            StructuresVM.Proccess(tr);
        }

        tr.Commit();
        _doc.Window.Focus();
    }
}
