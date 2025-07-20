using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NutsonCivilPlugin.PipeOnPV;

/// <summary>
/// Модель представления для работы с трубами на виде профиля
/// </summary>
public partial class ViewModelPipeOnPV : ObservableObject
{
    private readonly Document _doc;

    private readonly ProfileView? _profileView;
    private readonly NetworkSettings? _networkSettings;

    /// <summary>
    /// Список всех частей на виде профиля
    /// </summary>
    public List<Part> AllPartsFromPV { get; set; }

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
    /// <param name="profileView">Вид профиля</param>
    /// <param name="allPartsFromPV">Список всех частей на виде профиля</param>
    /// <param name="networkSettings">Настройки сети</param>
    public ViewModelPipeOnPV(
        Document doc,
        ProfileView profileView,
        List<Part> allPartsFromPV,
        NetworkSettings networkSettings
    )
    {
        _doc = doc;
        _profileView = profileView;
        _networkSettings = networkSettings;
        AllPartsFromPV = allPartsFromPV;
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

        var partFamily = _networkSettings?.partsList[PartFamilyName].As<PartFamily>();

        foreach (Model modelPipe in selectedPipes)
        {
            modelPipe.Part = modelPipe.Part.Id.As<Part>(OpenMode.ForWrite)!;
            modelPipe.SetPartFamily(partFamily!, PartSizeName);
        }

        _doc.Window.Focus();
    }
}
