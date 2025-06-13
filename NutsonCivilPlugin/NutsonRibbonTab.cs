using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using NutsonCivilPlugin.AddPipeOnPV;
using NutsonCivilPlugin.PipeOnPV;
using NutsonCivilPlugin.PropertySet;

namespace NutsonCivilPlugin;

public class NutsonRibbonTab : IExtensionApplication
{
    private const string TabId = "Nutson";
    private const string TabTitle = "NCP";

    public void Initialize() => Application.Idle += Application_Idle;

    public void Terminate() { }

    [CommandMethod("NutsonRibbon")]
    public void NutsonRibbon()
    {
        var NutsonTab = ComponentManager.Ribbon.FindTab(TabId);
        if (NutsonTab is null)
        {
            NutsonTab = new RibbonTab { Title = TabTitle, Id = TabId };
            ComponentManager.Ribbon.Tabs.Add(NutsonTab);
            AddContentOnTab(NutsonTab);
        }

        Application.Idle -= Application_Idle;
    }

    private void Application_Idle(object sender, EventArgs e) => NutsonRibbon();

    public void AddContentOnTab(RibbonTab ribbon)
    {
        var ribbonPanelSource = new RibbonPanelSource();
        ribbonPanelSource.Title = "Работа с видом профиля";
        var ribbonPanel = new RibbonPanel();
        ribbonPanel.Source = ribbonPanelSource;
        ribbon.Panels.Add(ribbonPanel);

        var buttonPipeOnPV = new RibbonButton()
        {
            Name = "PipeOnPV",
            Text = "Pipe on PV",
            ShowText = true,
            Size = RibbonItemSize.Large,
            CommandHandler = new CommandPipeOnPV(),
        };
        buttonPipeOnPV.LargeImage = ConvertFromBitmap(Properties.Resource.plumbing);
        ribbonPanelSource.Items.Add(buttonPipeOnPV);

        var buttonGetPropSetDef = new RibbonButton()
        {
            Name = "ClearPropSet",
            Text = "Clear Property Set",
            ShowText = true,
            Size = RibbonItemSize.Large,

            CommandHandler = new CommandClearPropertySet(),
        };
        buttonGetPropSetDef.LargeImage = ConvertFromBitmap(Properties.Resource.dust);
        ribbonPanelSource.Items.Add(buttonGetPropSetDef);

        var buttonAddPipeOnPV = new RibbonButton()
        {
            Name = "AddPipeOnPV",
            Text = "Add Pipe On PV",
            ShowText = true,
            Size = RibbonItemSize.Large,

            CommandHandler = new CommandAddPipeOnPV(),
        };
        buttonAddPipeOnPV.LargeImage = ConvertFromBitmap(
            Properties.Resource.pencil_drawing_circles
        );
        ribbonPanelSource.Items.Add(buttonAddPipeOnPV);
    }

    static BitmapSource ConvertFromBitmap(Bitmap bitmap)
    {
        return Imaging.CreateBitmapSourceFromHBitmap(
            bitmap.GetHbitmap(),
            IntPtr.Zero,
            System.Windows.Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions()
        );
    }
}
