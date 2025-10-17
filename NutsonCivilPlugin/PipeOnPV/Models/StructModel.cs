using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NutsonCivilPlugin.PipeOnPV.Models;

public partial class StructModel : Model<Structure>
{
    [ObservableProperty]
    private double _heighOfBottomPart;

    [ObservableProperty]
    private double _fullHeigh;

    public StructModel(Structure networkPart, List<PartFamilyModel> partSettings, Transaction tr)
        : base(networkPart, partSettings, tr) { }
}
