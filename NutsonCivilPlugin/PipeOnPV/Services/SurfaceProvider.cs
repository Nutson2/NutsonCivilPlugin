using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;

namespace NutsonCivilPlugin.PipeOnPV.Services;

public class SurfaceProvider
{
    private readonly Document _aDoc;
    private readonly CivilDocument _civilDoc;
    private readonly Database _database;

    public SurfaceProvider(Document aDoc, CivilDocument civilDoc)
    {
        _aDoc = aDoc;
        _civilDoc = civilDoc;
        _database = _aDoc.Database;
    }

    public List<TinSurface> GetSurfaces()
    {
        using var lockDock = _aDoc.LockDocument();
        using var tr = _database.TransactionManager.StartTransaction();

        var surfacesCollection = _civilDoc
            .GetSurfaceIds()
            .Cast<ObjectId>()
            .Select(id => id.As<TinSurface>(tr))
            .OfType<TinSurface>()
            .ToList();

        tr.Commit();
        return surfacesCollection;
    }
}
