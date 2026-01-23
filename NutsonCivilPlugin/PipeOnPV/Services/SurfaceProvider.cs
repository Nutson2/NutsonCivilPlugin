using AcadShared;
using AcadShared.Extensions.AutoCad;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Shared.Contracts;
using Shared.Models;

namespace NutsonCivilPlugin.PipeOnPV.Services;

public class SurfaceProvider(
        Document doc,
        CivilDocument civilDoc,
        IMapper<BaseEntity,
        TinSurface> mapper)
    : IElementProvider<List<BaseEntity>>
{
    private readonly Document _doc = doc;
    private readonly CivilDocument civilDoc = civilDoc;
    private readonly IMapper<BaseEntity, TinSurface> _mapper = mapper;

    public List<BaseEntity> GetElements()
    {
        var tr = _doc.TransactionManager.TopTransaction;

        var surfacesCollection = civilDoc
            .GetSurfaceIds()
            .Cast<ObjectId>()
            .Select(id => id.As<TinSurface>(tr))
            .OfType<TinSurface>()
            .Select(_mapper.ToModel)
            .ToList();

        return surfacesCollection;
    }
}
