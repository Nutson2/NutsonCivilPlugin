namespace NutsonCivilPlugin.PipeOnPV.Models;

public class BaseEntity(string Name, ObjectId Id)
{
    public string Name { get; } = Name;
    public ObjectId Id { get; } = Id;

    public override bool Equals(object obj) => obj is BaseEntity other && other.Id == Id;

    public override int GetHashCode() => Id.GetHashCode();
}
