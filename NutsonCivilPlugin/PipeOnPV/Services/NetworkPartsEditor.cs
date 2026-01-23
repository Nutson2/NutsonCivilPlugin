using AcadShared.Extensions.AutoCad;
using AcadShared.Extensions.Civil;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using CSharpFunctionalExtensions;
using Shared.Extensions.Civil;
using Shared.Models;

namespace NutsonCivilPlugin.PipeOnPV.Services;

public class NetworkPartsEditor(Document doc)
    : INetworkPartsEditor
{
    private readonly Document _doc = doc;

    public Result AlignPipes(
        List<PipeModel> pipes,
        StructureModel startStructure,
        AlignPipesSettings settings)
    {
        using var loc = _doc.LockDocument();
        using var tr = _doc.TransactionManager.StartTransaction();

        var startSructureId = startStructure.Id.ToObjectId();
        bool isFirstPipe = true;
        foreach (var pipeModel in pipes)
        {
            var pipe = pipeModel.Id.ToObjectId().As<Pipe>(tr);
            pipe.SetPipeDirection(startSructureId);

            var (lowestPipe, elevation) =
                startSructureId.As<Structure>(tr)?.GetLowestPipe(tr) ?? (pipe, 0);

            if (isFirstPipe)
            {
                isFirstPipe = false;
                var pipeElevation = settings.IsRelatingTypeCovering
                    ? pipe.CalculateElevationForCovering(
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
                    + lowestPipe.GetOffset(settings.PointForConnecting)
                    - pipe.GetOffset(settings.PointForConnecting)
                    + pipeModel.OffsetFromPreviousPipe;
            }

            var slope = pipeModel.Slope;

            pipe.SetStartElevation(elevation);
            pipe.SetEndElevationBySlope(slope);

            startSructureId = pipe.EndStructureId;
        }

        tr.Commit();
        _doc.Window.Focus();

        return Result.Success();
    }
}
