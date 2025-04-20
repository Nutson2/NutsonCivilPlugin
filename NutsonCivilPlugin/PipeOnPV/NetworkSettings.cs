using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

namespace NutsonCivilPlugin.PipeOnPV;

class NetworkSettings
{
    public PartsList partsList;
    public readonly Dictionary<string, List<string>> pipePartfamily;
    public readonly Dictionary<string, List<string>> structurePartfamily;

    public NetworkSettings(Document doc, Network network)
    {
        using var tr = doc.TransactionManager.StartTransaction();

        partsList = network.PartsListId.As<PartsList>() ?? throw new System.Exception();

        pipePartfamily = GetNetworkPartFamily(tr, partsList, DomainType.Pipe);
        structurePartfamily = GetNetworkPartFamily(tr, partsList, DomainType.Structure);
    }

    public Dictionary<string, List<string>> GetPartFamilys(DomainType domainType) =>
        domainType == DomainType.Pipe ? pipePartfamily : structurePartfamily;

    private Dictionary<string, List<string>> GetNetworkPartFamily(
        Transaction tr,
        PartsList partsList,
        DomainType domainType
    ) =>
        partsList
            .GetPartFamilyIdsByDomain(domainType)
            .Cast<ObjectId>()
            .Select(id => id.As<PartFamily>())
            .OfType<PartFamily>()
            .Select(pf =>
                (
                    pf.Name,
                    Enumerable
                        .Range(0, pf.PartSizeCount)
                        .Select(i => pf[i].As<PartSize>()?.Name ?? string.Empty)
                        .ToList()
                )
            )
            .ToDictionary(key => key.Name, val => val.Item2);
}
