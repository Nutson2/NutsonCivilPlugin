using System.Collections.Generic;

namespace Shared.Models;

public class PointOnPipeShape(string name)
{
    public string Name { get; } = name;

    public static List<PointOnPipeShape> AllTypes { get; } =
        [
            new ("Top with wall thickness"),
            new ("Top"),
            new ("Center"),
            new ("Bottom"),
            new ("Bottom with wall thickness"),
        ];

    public static PointOnPipeShape TopWithWallThickness => AllTypes[0];
    public static PointOnPipeShape Top => AllTypes[1];
    public static PointOnPipeShape Center => AllTypes[2];
    public static PointOnPipeShape Bottom => AllTypes[3];
    public static PointOnPipeShape BottomWithWallThickness => AllTypes[4];

    public override bool Equals(object obj) => obj is PointOnPipeShape other && other.Name == Name;
    public override int GetHashCode() => Name.GetHashCode();
}
