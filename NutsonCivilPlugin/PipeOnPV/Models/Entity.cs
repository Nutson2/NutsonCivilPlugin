namespace NutsonCivilPlugin.PipeOnPV.Models;

public class Entity(string Name, ObjectId Id)
{
    public string Name { get; } = Name;
    public ObjectId Id { get; } = Id;
}