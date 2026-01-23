using AcadShared.Extensions.AutoCad;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using CSharpFunctionalExtensions;
using Shared.Contracts;

namespace NutsonCivilPlugin.AddPipeOnPV;

/// <summary>
/// Класс для добавления труб на виде профиля
/// </summary>
/// <param name="doc">Документ AutoCAD</param>
/// <param name="modelDataProvider">Провайдер данных модели</param>
public class AddPipeOnPV(Document doc, ISelectionService<ProfileView> modelDataProvider)
{
    private readonly Document _doc = doc;
    private readonly ISelectionService<ProfileView> _modelDataProvider = modelDataProvider;

    /// <summary>
    /// Выполняет основную работу по добавлению труб на виде профиля
    /// </summary>
    public void Work()
    {
        using var loc = _doc.LockDocument();
        using var tr = _doc.TransactionManager.StartTransaction();
        ProfileView profileView = default!;
        _modelDataProvider.RequestSelection()
             .ToResult("Selected object is not ProfileView")
             .Tap(pv => profileView = pv)
             .Map(pv => pv.AlignmentId.As<Alignment>(tr))
             .Map(al => al != null ? GetVertexPoints(al) : [])
             .Map(pnts => GetCrossingPipes(_doc, pnts, tr))
             .Bind(pipes => ProccessCrossingPipes(profileView, pipes, tr));

        tr.Commit();
    }

    private static Result<List<PipeOverride>> ProccessCrossingPipes(
        ProfileView profileView,
        List<Pipe> pipeAddedOnProfileView,
        Transaction tr
    )
    {
        var pipes = AddPipesOnProfileView(profileView, pipeAddedOnProfileView);
        return OverridePipeViewOnProfileView(profileView, pipes, tr);
    }

    private static Result<List<PipeOverride>> OverridePipeViewOnProfileView(
        ProfileView profileView,
        List<Pipe> pipeAddedOnProfileView,
        Transaction tr
    )
    {
        var styleId = CivilApplication.ActiveDocument.Styles.PipeStyles["Пересекаемая труба"];
        var res = profileView
            .PipeOverrides.Select(po => (po, pipe: po.PipeId.As<ProfileViewPart>(tr)))
            .Where(pO => pipeAddedOnProfileView.Any(p => p.Id == pO.pipe?.ModelPartId))
            .Select(pair => pair.po)
            .ToList();
        foreach (var po in res)
        {
            po.OverrideStyleId = styleId;
        }

        return res;
    }

    private static List<Pipe> AddPipesOnProfileView(
        ProfileView profileView,
        List<Pipe> crossingPipes
    )
    {
        var res = crossingPipes
            .OfType<Pipe>()
            .Where(p => !p.GetProfileViewsDisplayingMe().Contains(profileView.Id));
        foreach (var pipe in res)
        {
            pipe.AddToProfileView(profileView.Id);
        }

        return res.ToList();
    }

    private static List<Pipe> GetCrossingPipes(
        Document doc,
        Point3dCollection point3DCollection,
        Transaction tr
    )
    {
        TypedValue[] filter = [new(0, "AECC_PIPE")];
        var selectionFilter = new SelectionFilter(filter);
        var res = doc.Editor.SelectFence(point3DCollection, selectionFilter);
        return res?.Value.GetObjectIds()
                .Select(id => id.As<Pipe>(tr, OpenMode.ForWrite))
                .OfType<Pipe>()
                .ToList() ?? [];
    }

    /// <summary>
    /// Получает коллекцию точек вершин трассы
    /// </summary>
    /// <param name="alignment">Трасса</param>
    /// <returns>Коллекция точек вершин</returns>
    private static Point3dCollection GetVertexPoints(Alignment alignment)
    {
        var points = alignment
            .Entities.OfType<AlignmentCurve>()
            .OrderBy(en => en.StartStation)
            .Select(ent => new Point3d(ent.StartPoint.X, ent.StartPoint.Y, 0));

        return new Point3dCollection([.. points, alignment.EndPoint]);
    }
}
