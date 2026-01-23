using CommunityToolkit.Mvvm.ComponentModel;
using Shared.Models;

namespace Windows.Models;

public partial class PipeVM(PipeModel networkPart, List<PartFamilyModel> partSettings)
    : PartVM<PipeModel>(networkPart, partSettings)
{
    [ObservableProperty]
    private double _slope;

    [ObservableProperty]
    private double _offsetFromPreviousPipe;
}
