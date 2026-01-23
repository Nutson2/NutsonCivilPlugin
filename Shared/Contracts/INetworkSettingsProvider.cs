using CSharpFunctionalExtensions;
using Shared.Models;

namespace Shared.Contracts;
public interface INetworkSettingsProvider
{
    Result<NetworkSettingsModel> GetSettings(ObjectIdModel networkId);
}