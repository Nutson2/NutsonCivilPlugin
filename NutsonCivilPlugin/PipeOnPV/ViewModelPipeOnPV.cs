using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NutsonCivilPlugin.PipeOnPV;

public partial class ViewModelPipeOnPV : ObservableObject
{
    private readonly Document _doc;

    private readonly ProfileView? _profileView;
    private readonly NetworkSettings? _networkSettings;

    public List<Part> AllPartsFromPV { get; set; }

    public Action? AfterRequestUserPick { get; internal set; }
    public Action? BeforeRequestUserPick { get; internal set; }
    public Action<CancelEventArgs>? OnClosing { get; internal set; }

    public ViewModelPipeOnPV(
        Document doc,
        ProfileView profileView,
        List<Part> allPartsFromPV,
        NetworkSettings networkSettings
    )
    {
        _doc = doc;
        _profileView = profileView;
        _networkSettings = networkSettings;
        AllPartsFromPV = allPartsFromPV;
    }

    public void PreparePartsToShow(ObservableCollection<Model> collection, DomainType domainType)
    {
        collection.Clear();

        var PartFamily = _networkSettings!.GetPartFamilys(domainType);

        AllPartsFromPV
            .Where(p => p.Domain == domainType)
            .Select(p => new Model(p, PartFamily))
            .ToList()
            .ForEach(collection.Add);
    }

    public void SetPartFamily(IList selectedPipes, string PartFamilyName, string PartSizeName)
    {
        using var loc = _doc.LockDocument();
        using var tr = _doc.TransactionManager.StartTransaction();

        var partFamily = _networkSettings?.partsList[PartFamilyName].As<PartFamily>();

        foreach (Model modelPipe in selectedPipes)
        {
            modelPipe.Part = modelPipe.Part.Id.As<Part>(OpenMode.ForWrite)!;
            modelPipe.SetPartFamily(partFamily!, PartSizeName);
        }

        _doc.Window.Focus();
    }
}
