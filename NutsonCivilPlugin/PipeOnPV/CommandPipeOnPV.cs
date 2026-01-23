using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Shared;
using SimpleInjector;

namespace NutsonCivilPlugin.PipeOnPV;

/// <summary>
/// Команда для работы с трубами на виде профиля
/// </summary>
class CommandPipeOnPV : CivilCommand
{
    /// <summary>
    /// Выполняет команду для работы с трубами на виде профиля
    /// </summary>
    [CommandMethod("PipeOnPV")]
    public override void Execute()
    {
        try
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var civilDoc = CivilDocument.GetCivilDocument(doc.Database);

        }
        catch (System.Exception) { }
    }

    public void Execute2()
    {
        var container = new Container();

    }
}
