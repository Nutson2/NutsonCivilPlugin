namespace NutsonCivilPlugin.PipeOnPV.ViewModels;

public class PointOnPipeShape(string name)
{
    public string Name { get; } = name;

    public static List<PointOnPipeShape> AllTypes { get; private set; } =
        [TopWithWallThickness!, Top!, Center!, Bottom!, BottomWithWallThickness!];

    public static PointOnPipeShape TopWithWallThickness => new("Top with wall thickness");
    public static PointOnPipeShape Top => new("Top");
    public static PointOnPipeShape Center => new("Center");
    public static PointOnPipeShape Bottom => new("Bottom");
    public static PointOnPipeShape BottomWithWallThickness => new("Bottom with wall thickness");
}
