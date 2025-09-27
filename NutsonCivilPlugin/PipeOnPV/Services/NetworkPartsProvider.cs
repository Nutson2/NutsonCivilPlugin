using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;

namespace NutsonCivilPlugin.PipeOnPV.Services;

/// <summary>
/// Провайдер для работы с частями сети
/// </summary>
public class NetworkPartsProvider
{
    private readonly Document _doc;

    /// <summary>
    /// Инициализирует новый экземпляр класса NetworkPartsProvider
    /// </summary>
    public NetworkPartsProvider()
    {
        _doc = Application.DocumentManager.MdiActiveDocument;
    }

    /// <summary>
    /// Запрос у пользователя на выбор вида профиля
    /// </summary>
    /// <param name="profileView">Вид профиля</param>
    /// <returns>Список частей сети</returns>
    public List<Part> GetNetworkPartsFromPV(ProfileView? profileView, Transaction tr)
    {
        if (profileView is null)
        {
            return [];
        }

        var alignmentPV = profileView.AlignmentId.As<Alignment>(tr);

        var FirstEntity =
            alignmentPV?.Entities.EntityAtId(alignmentPV.Entities.FirstEntity) as AlignmentLine;

        var LastEntity =
            alignmentPV?.Entities.EntityAtId(alignmentPV.Entities.LastEntity) as AlignmentLine;

        var startPoint = FirstEntity?.StartPoint;
        var endPoint = LastEntity?.EndPoint;

        if (startPoint is null || endPoint is null)
        {
            return [];
        }

        var startStructure = GetStructureAtPoint(startPoint.Value, profileView, tr);
        var endStructure = GetStructureAtPoint(endPoint.Value, profileView, tr);

        if (
            startStructure is null
            || endStructure is null
            || startStructure.NetworkId != endStructure.NetworkId
        )
        {
            return [];
        }

        double minLength = 0;
        var partsIdOnPV = Network.FindShortestNetworkPath(
            startStructure.Id,
            endStructure.Id,
            ref minLength
        );

        var networksPartOnPv = partsIdOnPV
            .Cast<ObjectId>()
            .Select(id => id.As<Part>(tr))
            .OfType<Part>();

        return [.. networksPartOnPv, endStructure];
    }

    /// <summary>
    /// Получение структуры по точке
    /// </summary>
    /// <param name="Point">Точка</param>
    /// <param name="profileView">Вид профиля</param>
    /// <returns>Структура</returns>
    private Structure? GetStructureAtPoint(Point2d Point, ProfileView profileView, Transaction tr)
    {
        double offset = 2;
        var point3DCollection = new Point3dCollection(
            [
                new Point3d(Point.X - offset, Point.Y, 0),
                new Point3d(Point.X, Point.Y + offset, 0),
                new Point3d(Point.X + offset, Point.Y, 0),
                new Point3d(Point.X, Point.Y - offset, 0),
            ]
        );

        TypedValue[] filter = { new(0, "AECC_STRUCTURE") };
        var selectionFilter = new SelectionFilter(filter);
        var res = _doc.Editor.SelectCrossingPolygon(point3DCollection, selectionFilter);

        return res.Status != PromptStatus.OK || res.Value.Count < 0
            ? null
            : res
                .Value.GetObjectIds()
                .Cast<ObjectId>()
                .Select(id => id.As<Structure>(tr))
                .OfType<Structure>()
                .Where(s => s.GetProfileViewsDisplayingMe().Contains(profileView!.Id))
                .FirstOrDefault();
    }
}
