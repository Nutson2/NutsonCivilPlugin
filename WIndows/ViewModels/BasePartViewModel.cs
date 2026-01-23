using CommunityToolkit.Mvvm.ComponentModel;
using Shared.Models;
using Windows.Models;

namespace NutsonCivilPlugin.PipeOnPV.ViewModels;

public partial class BasePartViewModel<TModel, T> : ObservableObject
    where TModel : PartVM<T>
    where T : PartModel
{
    [ObservableProperty]
    private List<TModel> _parts = [];

    [ObservableProperty]
    private List<BaseEntity> _surfaces = [];

    //internal void Proccess(Transaction tr)
    //{
    //    Parts.ForEach(p => p.SetPartFamily(tr));
    //}
}
