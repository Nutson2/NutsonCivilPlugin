using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NutsonCivilPlugin.PipeOnPV;

public partial class Model : ObservableObject
{
    [ObservableProperty]
    private Part _part = null!;

    [ObservableProperty]
    private string _partFamily = null!;

    [ObservableProperty]
    private string _partSize = null!;

    [ObservableProperty]
    private List<string> _listPartSize = new();

    private readonly Dictionary<string, List<string>> _partSettings;
    public List<string> ListPartFamilyTypes { get; set; }

    partial void OnPartFamilyChanged(string value) => ListPartSize = _partSettings[value];

    public void SetPartFamily(PartFamily pf, string psName)
    {
        var psId = !ListPartSize.Contains(psName) ? pf[0] : pf[psName];

        Part.SwapPartFamilyAndSize(pf.Id, psId);
        PartFamily = pf.Name;
        PartSize = psName;
    }

    public Model(Part networkPart, Dictionary<string, List<string>> partSettings)
    {
        _partSettings = partSettings;

        Part = networkPart;
        ListPartFamilyTypes = new List<string>(_partSettings.Keys);

        try
        {
            PartFamily = (string)Part.GetType().GetProperty("PartFamilyName").GetValue(Part);
        }
        catch (System.Exception)
        {
            PartFamily = "Ошибка определения типа семейства";
        }

        PartSize = Part.PartType != PartType.StructNull ? Part.PartSizeName : PartFamily;
    }
}
