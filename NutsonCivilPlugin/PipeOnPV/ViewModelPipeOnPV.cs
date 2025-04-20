using System.Collections;
using System.Collections.ObjectModel;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using CommunityToolkit.Mvvm.ComponentModel;
using NutsonCivilPlugin.AddPipeOnPV;

namespace NutsonCivilPlugin.PipeOnPV;

public partial class ViewModelPipeOnPV : ObservableObject
{
    private readonly Document _doc;

    [ObservableProperty]
    private ProfileView? _profileView;

    private Network? _network;
    private NetworkSettings? _networkSettings;

    private List<Part> allPartsFromPV = new();

    public ObservableCollection<Model> modelPipes = new();
    public ObservableCollection<Model> modelStructures = new();

    public ViewModelPipeOnPV()
    {
        _doc = Application.DocumentManager.MdiActiveDocument;
    }

    /// <summary>
    /// Запрос у пользователя на выбор вида профиля
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    private List<Part> GetNetworkPartsFromPV(Transaction tr, ProfileView? profileView)
    {
        if (profileView is null)
        {
            return [];
        }

        var alignmentPV = profileView.AlignmentId.As<Alignment>();

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

        var startStructure = GetStructureAtPoint(tr, startPoint.Value);
        var endStructure = GetStructureAtPoint(tr, endPoint.Value);

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
            .Select(id => id.As<Part>())
            .OfType<Part>();

        return [.. networksPartOnPv, endStructure];
    }

    /// <summary>
    /// Подготовка коллекций труб и колодцев с вида профиля для отображения в форме
    /// </summary>
    /// <returns></returns>
    public bool GetNetworkPartsFromPV()
    {
        using var tr = _doc.TransactionManager.StartTransaction();

        var ProfileViewId = Helper.SelectPV(_doc);

        ProfileView = ProfileViewId.As<ProfileView>();

        allPartsFromPV = GetNetworkPartsFromPV(tr, ProfileView);
        if (allPartsFromPV.Count == 0)
        {
            return false;
        }

        _network = allPartsFromPV.First().NetworkId.As<Network>();
        if (_network is null)
        {
            return false;
        }

        _networkSettings = new NetworkSettings(_doc, _network);

        PreparePartsToShow(modelPipes, DomainType.Pipe);
        PreparePartsToShow(modelStructures, DomainType.Structure);

        return true;
    }

    public void PreparePartsToShow(ObservableCollection<Model> collection, DomainType domainType)
    {
        collection.Clear();

        var PartFamily = _networkSettings!.GetPartFamilys(domainType);

        allPartsFromPV
            .Where(p => p.Domain == domainType)
            .Select(p => new Model(p, PartFamily))
            .ToList()
            .ForEach(collection.Add);
    }

    private Structure? GetStructureAtPoint(Transaction tr, Point2d Point)
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
                .Select(id => id.As<Structure>())
                .OfType<Structure>()
                .Where(s => s.GetProfileViewsDisplayingMe().Contains(ProfileView!.Id))
                .FirstOrDefault();
    }

    public void SetPartFamily(IList selectedPipes, string PartFamilyName, string PartSizeName)
    {
        using var loc = _doc.LockDocument();
        using var tr = _doc.TransactionManager.StartTransaction();

        var partFamily = _networkSettings?.partsList[PartFamilyName].As<PartFamily>();

        foreach (Model modelPipe in selectedPipes)
        {
            modelPipe.Part = modelPipe.Part.Id.As<Part>(OpenMode.ForWrite)!;
            modelPipe.SetPartFamily(partFamily!, PartSizeName);
        }

        _doc.Window.Focus();
    }
}
