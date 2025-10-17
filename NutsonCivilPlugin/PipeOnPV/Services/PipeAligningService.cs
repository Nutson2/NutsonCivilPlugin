using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using CSharpFunctionalExtensions;
using NutsonCivilPlugin.PipeOnPV.Models;
using NutsonCivilPlugin.PipeOnPV.ViewModels;
using Shared.Extensions.AutoCad;
using Shared.Extensions.Civil;

namespace NutsonCivilPlugin.PipeOnPV.Services;

internal static class PipeAligningService
{
    public static Result AlignPipes(
        List<PipeModel> pipes,
        StructModel startStructure,
        AlignPipesSettings settings,
        Transaction tr
    )
    {
        var startSructureId = startStructure.NetworkPart.Id;
        bool isFirstPipe = true;
        foreach (var pipe in pipes)
        {
            pipe.NetworkPart.SetPipeDirection(startSructureId);

            var (lowestPipe, elevation) =
                startSructureId.As<Structure>(tr)?.GetLowestPipe(tr) ?? (pipe.NetworkPart, 0);

            if (isFirstPipe)
            {
                isFirstPipe = false;
                var pipeElevation = settings.IsRelatingTypeCovering
                    ? pipe.NetworkPart.CalculateElevationForCovering(
                        settings.StartCovering,
                        settings.PointForCalculateCover,
                        tr
                    )
                    : settings.StartCovering;

                if (elevation > pipeElevation)
                {
                    elevation = pipeElevation;
                }
            }
            else
            {
                elevation =
                    elevation
                    + settings.PointForConnecting.GetOffset(lowestPipe)
                    - settings.PointForConnecting.GetOffset(pipe.NetworkPart)
                    + pipe.OffsetFromPreviousPipe;
            }

            var slope = pipe.Slope;

            pipe.NetworkPart.SetStartElevation(elevation);
            pipe.NetworkPart.SetEndElevationBySlope(slope);

            startSructureId = pipe.NetworkPart.EndStructureId;
        }

        return Result.Success();
    }
}
