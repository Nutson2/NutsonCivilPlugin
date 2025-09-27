using Autodesk.Civil.DatabaseServices;

namespace NutsonCivilPlugin.PipeOnPV.Models;

public class PipeModel : Model<Pipe>
{
    public PipeModel(Pipe networkPart, List<PartFamilyModel> partSettings)
        : base(networkPart, partSettings) { }
}
