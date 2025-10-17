using CommunityToolkit.Mvvm.ComponentModel;
using Shared.Models;

namespace NutsonCivilPlugin.PipeOnPV.ViewModels;

public partial class AlignPipesSettings : ObservableObject
{
    [ObservableProperty]
    private bool _isAlignBySlope;

    [ObservableProperty]
    private bool _isRelatingTypeCovering;

    [ObservableProperty]
    private double _startCovering;

    [ObservableProperty]
    private PointOnPipeShape _pointForCalculateCover = PointOnPipeShape.Bottom;

    [ObservableProperty]
    private PointOnPipeShape _pointForConnecting = PointOnPipeShape.Top;
}
