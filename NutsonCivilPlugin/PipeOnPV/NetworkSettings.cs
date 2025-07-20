using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

namespace NutsonCivilPlugin.PipeOnPV;

/// <summary>
/// Класс для работы с настройками сети
/// </summary>
public class NetworkSettings
{
    /// <summary>
    /// Список частей сети
    /// </summary>
    public PartsList partsList;
    
    /// <summary>
    /// Словарь семейств труб и их размеров
    /// </summary>
    public readonly Dictionary<string, List<string>> pipePartfamily;
    
    /// <summary>
    /// Словарь семейств структур и их размеров
    /// </summary>
    public readonly Dictionary<string, List<string>> structurePartfamily;

    /// <summary>
    /// Инициализирует новый экземпляр класса NetworkSettings
    /// </summary>
    /// <param name="network">Сеть трубопроводов</param>
    public NetworkSettings(Network network)
    {
        partsList = network.PartsListId.As<PartsList>() ?? throw new Exception();

        pipePartfamily = GetNetworkPartFamily(partsList, DomainType.Pipe);
        structurePartfamily = GetNetworkPartFamily(partsList, DomainType.Structure);
    }

    /// <summary>
    /// Получает словарь семейств частей по типу домена
    /// </summary>
    /// <param name="domainType">Тип домена (труба или структура)</param>
    /// <returns>Словарь семейств частей и их размеров</returns>
    public Dictionary<string, List<string>> GetPartFamilys(DomainType domainType) =>
        domainType == DomainType.Pipe ? pipePartfamily : structurePartfamily;

    private Dictionary<string, List<string>> GetNetworkPartFamily(
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
