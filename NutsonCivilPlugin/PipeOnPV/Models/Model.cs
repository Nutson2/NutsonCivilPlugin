using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CSharpFunctionalExtensions;
using Shared.Extensions.AutoCad;

namespace NutsonCivilPlugin.PipeOnPV.Models;

/// <summary>
/// Модель для представления части сети
/// </summary>
public partial class Model<T> : ObservableObject
    where T : Part
{
    public T NetworkPart { get; }

    [ObservableProperty]
    private string _name;

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

    /// <summary>
    /// Инициализирует новый экземпляр класса Model
    /// </summary>
    /// <param name="networkPart">Часть сети</param>
    /// <param name="partSettings">Настройки частей</param>
    public Model(T networkPart, List<PartFamilyModel> partSettings, Transaction tr)
    {
        NetworkPart = networkPart;
        Name = networkPart.Name;
        PartFamilies = partSettings;
        SelectedPartFamily = partSettings.First(pf => pf.Id == networkPart.PartFamilyId);
        SelectedPartSize = SelectedPartFamily.PartSizes.FirstOrDefault(ps =>
            ps.Name == networkPart.PartSizeName
        );
        var surface = networkPart.RefSurfaceId.As<TinSurface>(tr);
        SelectedSurface = new BaseEntity(surface.Name, surface.Id);
    }

    /// <summary>
    /// Устанавливает семейство части
    /// </summary>
    /// <param name="pf">Семейство части</param>
    /// <param name="psName">Имя размера части</param>
    public void SetPartFamily(Transaction tr) =>
        NetworkPart
            .Id.As<T>(tr, OpenMode.ForWrite)
            .AsMaybe()
            .Execute(part =>
                part.SwapPartFamilyAndSize(SelectedPartFamily.Id, SelectedPartSize.Id)
            );
}
