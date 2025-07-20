using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
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
        var doc = Application.DocumentManager.MdiActiveDocument;
        using var tr = doc.TransactionManager.StartOpenCloseTransaction();

        var modelDataProvider = new ModelDataProvider();
        var networkPartsProvider = new NetworkPartsProvider();

        var profileView = modelDataProvider.RequestSelection<ProfileView>(doc).As<ProfileView>();
        profileView.ThrowIfNull();
        var allPartsFromPV = networkPartsProvider.GetNetworkPartsFromPV(profileView).ToList();

        var _network = allPartsFromPV.First().NetworkId.As<Network>();
        _network.ThrowIfNull();

        var _networkSettings = new NetworkSettings(_network!);
        tr.Commit();

        var vm = new ViewModelPipeOnPV(doc, profileView!, allPartsFromPV, _networkSettings);

        var formWork = new FormWorkWithPipe(vm);

        formWork.Show();
    }
}
