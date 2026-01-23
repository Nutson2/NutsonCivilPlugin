using AcadShared;
using AcadShared.Extensions.AutoCad;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using CSharpFunctionalExtensions;
using Shared;
using Shared.Contracts;
using Shared.Extensions.AutoCad;
using Shared.Models;

namespace NutsonCivilPlugin.PipeOnPV.Services;

/// <summary>
/// Провайдер для работы с частями сети
/// </summary>
public class NetworkPartsFromProfileViewProvider : INetworkPartsFromProfileViewProvider
{
    private readonly Document _doc;
    private readonly ISelectionService<ProfileView> _profileViewProvider;
    private readonly IMapper<PartModel, Part> _mapper;

    /// <summary>
    /// Инициализирует новый экземпляр класса NetworkPartsProvider
    /// </summary>
    public NetworkPartsFromProfileViewProvider(
        ISelectionService<ProfileView> profileViewProvider,
        IMapper<PartModel, Part> mapper)
    {
        _doc = Application.DocumentManager.MdiActiveDocument;
        _profileViewProvider = profileViewProvider;
        _mapper = mapper;
    }

    public Result<List<PartModel>> GetNetworkPartsFromPV()
    {
        using var tr = _doc.TransactionManager.StartOpenCloseTransaction();

        var res = _profileViewProvider.RequestSelection()
           .ToResult("Не выбран вид профиля")
           .Bind(pv => GetNetworkPartsFromPV(pv, tr));

        return res;
    }

    /// <summary>
    /// Запрос у пользователя на выбор вида профиля
    /// </summary>
    /// <param name="profileView">Вид профиля</param>
    /// <returns>Список частей сети</returns>
    private Result<List<PartModel>> GetNetworkPartsFromPV(ProfileView profileView, Transaction tr)
    {
        var alignmentPV = profileView.AlignmentId.As<Alignment>(tr).AsMaybe();

        var startStructure = alignmentPV.Map(al => al.Entities.EntityAtId(al.Entities.FirstEntity) as AlignmentLine)
            .Map(line => line!.StartPoint)
            .Bind(p => GetStructureAtPoint(p, profileView, tr))
            .ToResult("Не удалось определить первый колодец");

        var endStructure = alignmentPV.Map(al => al.Entities.EntityAtId(al.Entities.LastEntity) as AlignmentLine)
            .Map(line => line!.EndPoint)
            .Bind(p => GetStructureAtPoint(p, profileView, tr))
            .ToResult("Не удалось определить последний колодец");

        var res = startStructure.BindZip(_ => endStructure)
            .Ensure(t => t.ApplyTo((ss, es) => ss.NetworkId == es.NetworkId), "dfdf")
            .Bind(t => t.ApplyTo((ss, es) => GetPartsBeetwinStructures(ss, es, tr)));

        return res;
    }

    private Result<List<PartModel>> GetPartsBeetwinStructures(Structure startStructure, Structure endStructure, Transaction tr)
    {
        double minLength = 0;
        var partsIdOnPV = Network.FindShortestNetworkPath(startStructure.Id, endStructure.Id, ref minLength);
        var networksPartOnPv = partsIdOnPV
            .Cast<ObjectId>()
            .Select(id => id.As<Part>(tr))
            .OfType<Part>()
            .Select(_mapper.ToModel);

        return Result.Success<List<PartModel>>([.. networksPartOnPv, _mapper.ToModel(endStructure)]);
    }

    /// <summary>
    /// Получение структуры по точке
    /// </summary>
    /// <param name="Point">Точка</param>
    /// <param name="profileView">Вид профиля</param>
    /// <returns>Структура</returns>
    private Maybe<Structure> GetStructureAtPoint(Point2d Point, ProfileView profileView, Transaction tr)
    {
        double offset = 2;
        var point3d = new Point3d(Point.X, Point.Y, 0);

        var point3DCollection = new Point3dCollection(
            [
                point3d.WithX(Point.X - offset),
                point3d.WithY(Point.Y + offset),
                point3d.WithX(Point.X + offset),
                point3d.WithY(Point.Y - offset),
            ]
        );

        TypedValue[] filter = [new(0, "AECC_STRUCTURE")];
        var selectionFilter = new SelectionFilter(filter);
        var res = _doc.Editor.SelectCrossingPolygon(point3DCollection, selectionFilter);

        return res.Status != PromptStatus.OK || res.Value.Count < 0
            ? null
            : res
                .Value.GetObjectIds()
                .Select(id => id.As<Structure>(tr))
                .OfType<Structure>()
                .FirstOrDefault(s => s.GetProfileViewsDisplayingMe().Contains(profileView!.Id));
    }
}
