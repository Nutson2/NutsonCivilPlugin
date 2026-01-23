using AcadShared.Extensions.AutoCad;
using AcadShared.Extensions.Civil;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using CSharpFunctionalExtensions;
using Shared.Contracts;
using Shared.Extensions.Civil;
using Shared.Models;

namespace NutsonCivilPlugin.PipeOnPV.Services;
public class NetworkSettingsProvider(Document doc) : INetworkSettingsProvider
{
    private readonly Document _doc = doc;

    public Result<NetworkSettingsModel> GetSettings(ObjectIdModel networkId)
    {
        var tr = _doc.TransactionManager.TopTransaction;

        var res = networkId.ToObjectId().As<Network>(tr)
             .AsMaybe()
             .ToResult("NetworkId is invalid")
             .Map(nw => nw.PartsListId.As<PartsList>(tr))
             .EnsureNotNull("PartsListId is invalid")
             .Map(partsList => new NetworkSettingsModel
             (
                GetNetworkPartFamily(partsList, DomainType.Pipe, tr),
                GetNetworkPartFamily(partsList, DomainType.Structure, tr))
             );

        return res;
    }

    private static List<PartFamilyModel> GetNetworkPartFamily(PartsList partsList, DomainType domainType, Transaction tr)
    {
        var res = partsList
               .GetPartFamilyIdsByDomain(domainType)
               .Cast<ObjectId>()
               .Select(id => id.As<PartFamily>(tr))
               .OfType<PartFamily>()
               .Select(pf => CreatePartFamilyModel(pf, tr))
               .ToList();

        return res;
    }

    private static PartFamilyModel CreatePartFamilyModel(PartFamily pf, Transaction tr)
    {
        var res = pf.GetPartSizeIds()
               .Select(id => id.As<PartSize>(tr))
               .OfType<PartSize>()
               .Select(ps => new PartSizeModel(ps.Name, ps.Id.ToModel()))
               .ToList();

        return new(pf.Name, pf.Id.ToModel(), res);
    }
}