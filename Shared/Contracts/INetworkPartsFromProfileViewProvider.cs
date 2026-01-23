using CSharpFunctionalExtensions;
using Shared.Models;

namespace Shared.Contracts;
public interface INetworkPartsFromProfileViewProvider
{
    Result<List<PartModel>> GetNetworkPartsFromPV();
}