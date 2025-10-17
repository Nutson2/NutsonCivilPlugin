using Autodesk.Civil.DatabaseServices;
using NutsonCivilPlugin.PipeOnPV.Models;
using Shared.Models;

namespace NutsonCivilPlugin.PipeOnPV.ViewModels;

public partial class PipesViewModel : BasePartViewModel<PipeModel, Pipe>
{
    public AlignPipesSettings AlignPipesSettings { get; } = new();
    public List<PointOnPipeShape> PointOnPipeShapeTypes { get; } = PointOnPipeShape.AllTypes;
}
