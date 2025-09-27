using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using Shared;

namespace NutsonCivilPlugin.PipeOnPV;

/// <summary>
/// Модель представления для работы с трубами на виде профиля
/// </summary>
public partial class ViewModelPipeOnPV : ObservableObject
{
    private readonly Document _doc;
    private readonly ModelDataProvider _modelDataProvider;
    private readonly NetworkPartsProvider _networkPartsProvider;

    private ProfileView? _profileView;
    private NetworkSettings? _networkSettings;

    /// <summary>
    /// Список всех частей на виде профиля
    /// </summary>
    [ObservableProperty]
    private List<Part> _allPartsFromPV;

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
        NetworkPartsProvider networkPartsProvider
    )
    {
        _doc = doc;
        _modelDataProvider = modelDataProvider;
        _networkPartsProvider = networkPartsProvider;
    }

    [RelayCommand]
    private void SelectProfileView()
    {
        using var tr = _doc.TransactionManager.StartOpenCloseTransaction();
        Result
            .Success(_modelDataProvider.RequestSelection<ProfileView>(_doc))
            .Ensure((id) => id != ObjectId.Null, "ObjectId is invalid")
            .Map((id) => id.As<ProfileView>(tr))
            .Tap((pv) => _profileView = pv)
            .Map((pv) => _networkPartsProvider.GetNetworkPartsFromPV(pv, tr))
            .Tap((parts) => AllPartsFromPV = parts)
            .Tap(
                (parts) =>
                    _networkSettings = new NetworkSettings(parts.First().NetworkId.As<Network>(tr), tr)
            )
            .Tap(() => tr.Commit())
            .TapError((error) => tr.Abort());

        _profileView = _modelDataProvider.RequestSelection<ProfileView>(_doc).As<ProfileView>(tr);
        AllPartsFromPV = _networkPartsProvider.GetNetworkPartsFromPV(_profileView, tr);
        var _network = AllPartsFromPV.First().NetworkId.As<Network>(tr);
        _networkSettings = new NetworkSettings(_network!, tr);

        tr.Commit();
    }

    /// <summary>
    /// Подготавливает части для отображения
    /// </summary>
    /// <param name="collection">Коллекция моделей</param>
    /// <param name="domainType">Тип домена (труба или структура)</param>
    public void PreparePartsToShow(ObservableCollection<Model> collection, DomainType domainType)
    {
        collection.Clear();

        var PartFamily = _networkSettings!.GetPartFamilys(domainType);

        AllPartsFromPV
            .Where(p => p.Domain == domainType)
            .Select(p => new Model(p, PartFamily))
            .ForEach(collection.Add)
            .ToList();
    }

    /// <summary>
    /// Устанавливает семейство части для выбранных труб
    /// </summary>
    /// <param name="selectedPipes">Выбранные трубы</param>
    /// <param name="PartFamilyName">Имя семейства части</param>
    /// <param name="PartSizeName">Имя размера части</param>
    public void SetPartFamily(IList selectedPipes, string PartFamilyName, string PartSizeName)
    {
        using var loc = _doc.LockDocument();
        using var tr = _doc.TransactionManager.StartTransaction();

        var partFamily = _networkSettings?.partsList[PartFamilyName].As<PartFamily>(tr);

        foreach (Model modelPipe in selectedPipes)
        {
            modelPipe.Part = modelPipe.Part.Id.As<Part>(tr, OpenMode.ForWrite)!;
            modelPipe.SetPartFamily(partFamily!, PartSizeName);
        }

        _doc.Window.Focus();
    }
}
