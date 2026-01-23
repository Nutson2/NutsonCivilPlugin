using CommunityToolkit.Mvvm.ComponentModel;
using Shared.Models;

namespace Windows.Models;

public partial class StructureVM : PartVM<StructureModel>
{
    [ObservableProperty]
    private double _heighOfBottomPart;

    [ObservableProperty]
    private double _fullHeigh;

    public StructureVM(StructureModel networkPart, List<PartFamilyModel> partSettings)
        : base(networkPart, partSettings) { }
}
