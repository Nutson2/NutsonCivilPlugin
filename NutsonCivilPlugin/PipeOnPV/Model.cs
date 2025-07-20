using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NutsonCivilPlugin.PipeOnPV;

/// <summary>
/// Модель для представления части сети
/// </summary>
public partial class Model : ObservableObject
{
    [ObservableProperty]
    private Part _part = null!;

    [ObservableProperty]
    private string _partFamily = null!;

    [ObservableProperty]
    private string _partSize = null!;

    [ObservableProperty]
    private List<string> _listPartSize = [];

    private readonly Dictionary<string, List<string>> _partSettings;
    
    /// <summary>
    /// Список типов семейств частей
    /// </summary>
    public List<string> ListPartFamilyTypes { get; set; }

    partial void OnPartFamilyChanged(string value) => ListPartSize = _partSettings[value];

    /// <summary>
    /// Инициализирует новый экземпляр класса Model
    /// </summary>
    /// <param name="networkPart">Часть сети</param>
    /// <param name="partSettings">Настройки частей</param>
    public Model(Part networkPart, Dictionary<string, List<string>> partSettings)
    {
        _partSettings = partSettings;

        Part = networkPart;
        ListPartFamilyTypes = [.. _partSettings.Keys];

        PartFamily =
            Part.GetType().GetProperty("PartFamilyName")?.GetValue(Part) as string
            ?? "Ошибка определения типа семейства";

        PartSize = Part.PartType != PartType.StructNull ? Part.PartSizeName : PartFamily;
    }

    /// <summary>
    /// Устанавливает семейство части
    /// </summary>
    /// <param name="pf">Семейство части</param>
    /// <param name="psName">Имя размера части</param>
    public void SetPartFamily(PartFamily pf, string psName)
    {
        var psId = !ListPartSize.Contains(psName) ? pf[0] : pf[psName];

        Part.SwapPartFamilyAndSize(pf.Id, psId);
        PartFamily = pf.Name;
        PartSize = psName;
    }
}
