using CommunityToolkit.Mvvm.ComponentModel;
using Shared.Models;

namespace Windows.Models;

public partial class PartVM<T> : ObservableObject
    where T : PartModel
{
    public PartVM(T model, List<PartFamilyModel> partFamilies)
    {
        NetworkPart = model;
        PartFamilies = partFamilies;
    }
    public T NetworkPart { get; }

    [ObservableProperty]
    private BaseEntity _selectedSurface = null!;

    [ObservableProperty]
    private PartFamilyModel _selectedPartFamily = null!;

    [ObservableProperty]
    private PartSizeModel _selectedPartSize = null!;

    [ObservableProperty]
    private List<PartSizeModel> _selectedPartFamilyPartSizes = [];

    /// <summary>
    /// Список типов семейств частей
    /// </summary>
    public List<PartFamilyModel> PartFamilies { get; set; }

    partial void OnSelectedPartFamilyChanged(PartFamilyModel value) =>
        SelectedPartFamilyPartSizes = PartFamilies.First(pf => pf == value).PartSizes;
}
