using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NutsonCivilPlugin.PipeOnPV.Models;

public partial class PipeModel : Model<Pipe>
{
    [ObservableProperty]
    private double _slope;

    [ObservableProperty]
    private double _offsetFromPreviousPipe;

    public PipeModel(Pipe networkPart, List<PartFamilyModel> partSettings, Transaction tr)
        : base(networkPart, partSettings, tr) { }
}
