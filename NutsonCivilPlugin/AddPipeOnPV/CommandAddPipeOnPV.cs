using Autodesk.AutoCAD.Runtime;
using Shared;

namespace NutsonCivilPlugin.AddPipeOnPV;

/// <summary>
/// Команда для добавления труб на виде профиля
/// </summary>
public class CommandAddPipeOnPV : CivilCommand
{
    /// <summary>
    /// Выполняет команду для добавления труб на виде профиля
    /// </summary>
    [CommandMethod("AddPipeOnPV")]
    public override void Execute()
    {
        var doc = Application.DocumentManager.MdiActiveDocument;
        var modelDataProvider = new ModelDataProvider();
        var addPipeOnPV = new AddPipeOnPV(doc, modelDataProvider);
        addPipeOnPV.Work();
    }
}
