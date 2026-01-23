using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using NutsonCivilPlugin.PipeOnPV.Services;
using Shared;
using Shared.Contracts;
using Shared.Models;
using Windows.ViewModels;

namespace NutsonCivilPlugin.PipeOnPV.ViewModels;

/// <summary>
/// Модель представления для работы с трубами на виде профиля
/// </summary>
public partial class ViewModelPipeOnPV(
    INetworkPartsFromProfileViewProvider networkPartsFromProfileViewProvider,
    INetworkSettingsProvider networkSettingsProvider,
    IElementProvider<List<BaseEntity>> surfaceProvider,
    INetworkPartsEditor networkPartsEditor)
    : ObservableObject
{
    private readonly INetworkPartsFromProfileViewProvider _networkPartsFromProfileViewProvider = networkPartsFromProfileViewProvider;
    private readonly INetworkSettingsProvider _networkSettingsProvider = networkSettingsProvider;
    private readonly INetworkPartsEditor _networkPartsEditor = networkPartsEditor;

    [ObservableProperty]
    private int _tabIndex;

    public NetworkSettingsModel? NetworkSettings { get; private set; }
    public List<BaseEntity> Surfaces { get; set; } = surfaceProvider.GetElements();
    public PipesViewModel PipesVM { get; } = new();
    public StructuresViewModel StructuresVM { get; } = new();

    [RelayCommand]
    private void SelectProfileView()
    {
        _networkPartsFromProfileViewProvider.GetNetworkPartsFromPV()
            .Ensure(parts => parts.Count > 0, "Не удалось определить элементы")
            .BindZip(parts => _networkSettingsProvider.GetSettings(parts[0].Id))
            .Tap(tuple => tuple.ApplyTo((parts, settings) =>
            {
                NetworkSettings = settings;
                parts.ForEach(part => FillParts(part));
            }));

    }
    private void FillParts(PartModel part)
    {
        switch (part)
        {
            case PipeModel pipe:
                PipesVM.Parts.Add(new(pipe, NetworkSettings!.PipePartfamily));
                break;
            case StructureModel structure:
                StructuresVM.Parts.Add(new(structure, NetworkSettings!.StructurePartfamily));
                break;
            default:
                break;
        }
    }

    [RelayCommand]
    private void ModifyElements()
    {
        _networkPartsEditor.AlignPipes(
            PipesVM.Parts.ConvertAll(p => p.NetworkPart),
            StructuresVM.Parts.FirstOrDefault().NetworkPart,
            PipesVM.AlignPipesSettings);
    }
}
