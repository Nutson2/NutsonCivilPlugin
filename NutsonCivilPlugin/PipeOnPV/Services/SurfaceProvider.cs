using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Shared.Extensions.AutoCad;

namespace NutsonCivilPlugin.PipeOnPV.Services;

public class SurfaceProvider(CivilDocument civilDoc)
{
    public List<TinSurface> GetSurfaces(Transaction tr)
    {
        var surfacesCollection = civilDoc
            .GetSurfaceIds()
            .Cast<ObjectId>()
            .Select(id => id.As<TinSurface>(tr))
            .OfType<TinSurface>()
            .ToList();

        return surfacesCollection;
    }
}
