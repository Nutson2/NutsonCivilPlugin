using CSharpFunctionalExtensions;
using Shared.Models;

namespace NutsonCivilPlugin.PipeOnPV.Services;
public interface INetworkPartsEditor
{
    Result AlignPipes(List<PipeModel> pipes, StructureModel startStructure, AlignPipesSettings settings);
}