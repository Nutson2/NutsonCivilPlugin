using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;

namespace NutsonCivilPlugin.AddPipeOnPV;

class CommandAddPipeOnPV : System.Windows.Input.ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter)
    {
        try
        {
            AddPipeOnPV.Work(Application.DocumentManager.MdiActiveDocument);
        }
        catch (Autodesk.AutoCAD.Runtime.Exception ex)
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ex.ToString());
        }
    }
}

public static class AddPipeOnPV
{
    public static void Work(Document doc)
    {
        var profileViewId = Helper.SelectPV(doc);
        if (profileViewId == ObjectId.Null)
        {
            return;
        }

        using var loc = doc.LockDocument();
        using var tr = doc.TransactionManager.StartTransaction();

        var profileView = profileViewId.As<ProfileView>();
        var alignment = profileView?.AlignmentId.As<Alignment>();
        if (alignment == null)
        {
            return;
        }

        var point3DCollection = GetVertexPoints(alignment);
        var crossingPipeIds = GetCrossingPipes(doc, point3DCollection);

        profileView = profileView?.Id.As<ProfileView>(OpenMode.ForWrite);
        var pipeAddedOnProfileView = AddPipesOnProfileView(profileView, tr, crossingPipeIds);
        OverridePipeViewOnProfileView(profileView, pipeAddedOnProfileView);

        tr.Commit();
    }

    private static void OverridePipeViewOnProfileView(
        ProfileView profileView,
        List<ObjectId> pipeAddedOnProfileView
    )
    {
        var styleId = CivilApplication.ActiveDocument.Styles.PipeStyles["Пересекаемая труба"];

        profileView
            .PipeOverrides.Where(pO => pipeAddedOnProfileView.Contains(pO.PipeId))
            .ForEach(pO => pO.OverrideStyleId = styleId);
    }

    private static List<ObjectId> AddPipesOnProfileView(
        ProfileView profileView,
        Transaction tr,
        List<ObjectId> crossingPipes
    )
    {
        return crossingPipes.Count == 0
            ? []
            : crossingPipes
                .Select(id => id.As<Pipe>(OpenMode.ForWrite))
                .OfType<Pipe>()
                .Where(p => !p.GetProfileViewsDisplayingMe().Contains(profileView.Id))
                .ForEach(p => p.AddToProfileView(profileView.Id))
                .Select(p => p.Id)
                .ToList();
    }

    private static List<ObjectId> GetCrossingPipes(
        Document doc,
        Point3dCollection point3DCollection
    )
    {
        TypedValue[] filter = { new(0, "AECC_PIPE") };
        var selectionFilter = new SelectionFilter(filter);
        var res = doc.Editor.SelectFence(point3DCollection, selectionFilter);
        return res.Value.GetObjectIds().Cast<ObjectId>().ToList();
    }

    private static Point3dCollection GetVertexPoints(Alignment alignment)
    {
        var points = alignment
            .Entities.OfType<AlignmentCurve>()
            .Select(ent => new Point3d(ent.StartPoint.X, ent.StartPoint.Y, 0));

        return new Point3dCollection([.. points, alignment.EndPoint]);
    }
}
