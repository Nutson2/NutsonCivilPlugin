using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using NutsonCivilPlugin.PipeOnPV.Services;
using NutsonCivilPlugin.PipeOnPV.ViewModels;
using NutsonCivilPlugin.PipeOnPV.Views;
using Shared;

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

            var modelDataProvider = new ModelDataProvider();
            var networkPartsProvider = new NetworkPartsProvider();
            var surfaceProvider = new SurfaceProvider(civilDoc);
            var vm = new ViewModelPipeOnPV(
                doc,
                modelDataProvider,
                networkPartsProvider,
                surfaceProvider
            );
            var formWork = new FormWorkWithPipe(vm);
            formWork.Show();
        }
        catch (System.Exception) { }
    }
}
