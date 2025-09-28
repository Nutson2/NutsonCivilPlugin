using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using CommunityToolkit.Mvvm.ComponentModel;
using NutsonCivilPlugin.PipeOnPV.Models;

namespace NutsonCivilPlugin.PipeOnPV.ViewModels;

public partial class BasePartViewModel<TModel, T> : ObservableObject
    where TModel : Model<T>
    where T : Part
{
    [ObservableProperty]
    private List<TModel> _parts;
    public List<BaseEntity> Surfaces { get; set; }

    internal void Proccess(Transaction tr)
    {
        Parts.ForEach(p => p.SetPartFamily(tr));
    }
}
