using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using CSharpFunctionalExtensions;

namespace NutsonCivilPlugin.PipeOnPV.Models;

/// <summary>
/// Класс для работы с настройками сети
/// </summary>
public class NetworkSettings
{
    public List<PartFamilyModel> PipePartfamily { get; private set; }
    public List<PartFamilyModel> StructurePartfamily { get; private set; }
    public PartsList PartsList { get; private set; } = null!;

    public static Result<NetworkSettings> Create(ObjectId networkId, Transaction tr)
    {
        return networkId
            .As<Network>(tr)
            .AsMaybe()
            .ToResult("NetworkId is invalid")
            .Map(nw => nw.PartsListId.As<PartsList>(tr))
            .EnsureNotNull("PartsListId is invalid")
            .Map(partsList => new NetworkSettings
            {
                PartsList = partsList,
                PipePartfamily = GetNetworkPartFamily(partsList, DomainType.Pipe, tr),
                StructurePartfamily = GetNetworkPartFamily(partsList, DomainType.Structure, tr),
            });
    }

    private NetworkSettings()
    {
        PipePartfamily = [];
        StructurePartfamily = [];
    }

    /// <summary>
    /// Получает словарь семейств частей по типу домена
    /// </summary>
    /// <param name="domainType">Тип домена (труба или структура)</param>
    /// <returns>Словарь семейств частей и их размеров</returns>
    public List<PartFamilyModel> GetPartFamilys(DomainType domainType) =>
        domainType == DomainType.Pipe ? PipePartfamily : StructurePartfamily;

    private static List<PartFamilyModel> GetNetworkPartFamily(
        PartsList partsList,
        DomainType domainType,
        Transaction tr
    ) =>
        partsList
            .GetPartFamilyIdsByDomain(domainType)
            .Cast<ObjectId>()
            .Select(id => id.As<PartFamily>(tr))
            .OfType<PartFamily>()
            .Select(pf => PartFamilyModel.Create(pf, tr))
            .ToList();
}
