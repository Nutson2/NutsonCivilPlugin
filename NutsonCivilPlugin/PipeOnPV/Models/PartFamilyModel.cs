using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

namespace NutsonCivilPlugin.PipeOnPV.Models;

public class PartFamilyModel : BaseEntity
{
    public List<PartSizeModel> PartSizes { get; private set; } = [];

    private PartFamilyModel(string Name, ObjectId Id)
        : base(Name, Id) { }

    public static PartFamilyModel Create(PartFamily partFamily, Transaction tr)
    {
        var model = new PartFamilyModel(partFamily.Name, partFamily.Id)
        {
            PartSizes = partFamily
                .GetPartSizeIds()
                .Select(id => id.As<PartSize>(tr))
                .OfType<PartSize>()
                .Select(ps => new PartSizeModel(ps.Name, ps.Id))
                .ToList(),
        };
        return model;
    }

    public override bool Equals(object? obj) =>
        ReferenceEquals(this, obj)
        || (obj is PartFamilyModel other && Id == other.Id && Name == other.Name);

    public override int GetHashCode() => HashCode.Combine(Name, Id);
}
