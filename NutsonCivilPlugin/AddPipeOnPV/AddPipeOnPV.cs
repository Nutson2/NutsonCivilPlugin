using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using CSharpFunctionalExtensions;
using Shared;

namespace NutsonCivilPlugin.AddPipeOnPV;

public class AddPipeOnPV(Document doc, ModelDataProvider modelDataProvider)
{
    private readonly Document _doc = doc;
    private readonly ModelDataProvider _modelDataProvider = modelDataProvider;

    public void Work()
    {
        var profileViewId = _modelDataProvider.RequestSelection<ProfileView>(_doc);
        if (profileViewId == ObjectId.Null)
        {
            return;
        }

        using var loc = _doc.LockDocument();
        using var tr = _doc.TransactionManager.StartTransaction();

        var crossingPipeIds = profileViewId
            .As<ProfileView>()
            .AsMaybe()
            .Map((pv) => pv.AlignmentId.As<Alignment>())
            .Map((al) => GetVertexPoints(al))
            .Map((pnts) => GetCrossingPipes(_doc, pnts))
            .ToResult("fdfdf")
            .BindZip(
                (_) => profileViewId.As<ProfileView>(OpenMode.ForWrite).AsMaybe().ToResult("dsd")
            )
            .BindZip((pipes, pv) => ProccessCrossingPipes(pv, pipes));

        tr.Commit();
    }

    private Result<List<PipeOverride>> ProccessCrossingPipes(
        ProfileView profileView,
        List<Pipe?> pipeAddedOnProfileView
    )
    {
        var pipes = AddPipesOnProfileView(profileView, pipeAddedOnProfileView);
        return OverridePipeViewOnProfileView(profileView, pipes);
    }

    private Result<List<PipeOverride>> OverridePipeViewOnProfileView(
        ProfileView profileView,
        List<Pipe> pipeAddedOnProfileView
    )
    {
        var styleId = CivilApplication.ActiveDocument.Styles.PipeStyles["Пересекаемая труба"];
        var res = profileView
            .PipeOverrides.Select(po => (po, pipe: po.PipeId.As<ProfileViewPart>()))
            .Where(pO => pipeAddedOnProfileView.Any(p => p.Id == pO.pipe?.ModelPartId))
            .Select(pair => pair.po)
            .ToList();
        foreach (var po in res)
        {
            po.OverrideStyleId = styleId;
            //PipeProfileLabel.Create(po.PipeId, profileView.Id);
        }
        return res;
    }

    private List<Pipe> AddPipesOnProfileView(ProfileView profileView, List<Pipe?> crossingPipes)
    {
        var res =
            crossingPipes.Count == 0
                ? []
                : crossingPipes
                    .OfType<Pipe>()
                    .Where(p => !p.GetProfileViewsDisplayingMe().Contains(profileView.Id))
                    .ToList();
        foreach (var pipe in res)
        {
            pipe.AddToProfileView(profileView.Id);
        }

        return res;
    }

    private List<Pipe?> GetCrossingPipes(Document doc, Point3dCollection point3DCollection)
    {
        TypedValue[] filter = { new(0, "AECC_PIPE") };
        var selectionFilter = new SelectionFilter(filter);
        var res = doc.Editor.SelectFence(point3DCollection, selectionFilter);
        return res
            .Value.GetObjectIds()
            .Cast<ObjectId>()
            .Select(id => id.As<Pipe>(OpenMode.ForWrite))
            .ToList();
    }

    private Point3dCollection GetVertexPoints(Alignment alignment)
    {
        var points = alignment
            .Entities.OfType<AlignmentCurve>()
            .OrderBy(en => en.StartStation)
            .Select(ent => new Point3d(ent.StartPoint.X, ent.StartPoint.Y, 0));

        return new Point3dCollection([.. points, alignment.EndPoint]);
    }
}
