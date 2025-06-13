using Autodesk.AutoCAD.Runtime;
using Shared;

namespace NutsonCivilPlugin.AddPipeOnPV;

public class CommandAddPipeOnPV : CivilCommand
{
    [CommandMethod("AddPipeOnPV")]
    public override void Execute()
    {
        var doc = Application.DocumentManager.MdiActiveDocument;
        var modelDataProvider = new ModelDataProvider();
        var addPipeOnPV = new AddPipeOnPV(doc, modelDataProvider);
        addPipeOnPV.Work();
    }
}
