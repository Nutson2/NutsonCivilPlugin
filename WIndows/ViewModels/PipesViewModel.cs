using NutsonCivilPlugin.PipeOnPV.ViewModels;
using Shared.Models;
using Windows.Models;

namespace Windows.ViewModels;

public partial class PipesViewModel : BasePartViewModel<PipeVM, PipeModel>
{
    public AlignPipesSettings AlignPipesSettings { get; } = new();
    public List<PointOnPipeShape> PointOnPipeShapeTypes { get; } = PointOnPipeShape.AllTypes;
    public IEnumerable<PipeVM> SelectedItems { get; internal set; }
}
