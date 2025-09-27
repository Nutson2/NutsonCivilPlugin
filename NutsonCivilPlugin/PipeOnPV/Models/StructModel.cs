using Autodesk.Civil.DatabaseServices;

namespace NutsonCivilPlugin.PipeOnPV.Models;

public class StructModel : Model<Structure>
{
    public StructModel(Structure networkPart, List<PartFamilyModel> partSettings)
        : base(networkPart, partSettings) { }
}
