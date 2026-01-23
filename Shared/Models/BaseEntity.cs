namespace Shared.Models;

public record ObjectIdModel(string Handle);

public record BaseEntity(string Name, ObjectIdModel Id);

public record ProfileViewModel(string Name, ObjectIdModel Id) : BaseEntity(Name, Id);

public record PartSizeModel(string Name, ObjectIdModel Id) : BaseEntity(Name, Id);

public record PartFamilyModel(string Name, ObjectIdModel Id, List<PartSizeModel> PartSizes)
    : BaseEntity(Name, Id);

public record NetworkSettingsModel(
    List<PartFamilyModel> PipePartfamily,
    List<PartFamilyModel> StructurePartfamily
);

public record PartModel(
    string Name,
    ObjectIdModel Id,
    PartFamilyModel PartFamily,
    PartSizeModel PartSize,
    ObjectIdModel RefSurfaceId
) : BaseEntity(Name, Id);

public record PipeModel(
    string Name,
    ObjectIdModel Id,
    PartFamilyModel PartFamily,
    PartSizeModel PartSize,
    ObjectIdModel RefSurfaceId,
    double Slope,
    double OffsetFromPreviousPipe
) : PartModel(Name, Id, PartFamily, PartSize, RefSurfaceId);

public record StructureModel(
    string Name,
    ObjectIdModel Id,
    PartFamilyModel PartFamily,
    PartSizeModel PartSize,
    ObjectIdModel RefSurfaceId,
    double HeighOfBottomPart,
    double FullHeigh
) : PartModel(Name, Id, PartFamily, PartSize, RefSurfaceId);
