using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using NutsonCivilPlugin.PipeOnPV;

namespace NutsonCivilPlugin
{
    public class NutsonRibbonTab:IExtensionApplication
    {
        [CommandMethod("NutsonRibbon")]
        public void NutsonRibbon()
        {
            RibbonControl ribbon = ComponentManager.Ribbon;
            if (ribbon!=null)
            {
                RibbonTab NutsonTab = ribbon.FindTab( "Nutson");
                if (NutsonTab!=null)
                {
                    ribbon.Tabs.Remove(NutsonTab);
                }

                NutsonTab = new RibbonTab();
                NutsonTab.Title = "NCP";
                NutsonTab.Id = "Nutson";
                ribbon.Tabs.Add(NutsonTab);
                addContentonTab(NutsonTab);
            }

        }

        public void addContentonTab(RibbonTab ribbon)
        {
            RibbonPanelSource ribbonPanelSource = new RibbonPanelSource();
            ribbonPanelSource.Title = "Работа с видом профиля";
            RibbonPanel ribbonPanel = new RibbonPanel();
            ribbonPanel.Source = ribbonPanelSource;
            ribbon.Panels.Add(ribbonPanel);


            RibbonButton buttonPipeOnPV = new RibbonButton();
            buttonPipeOnPV.Name = "PipeOnPV";
            buttonPipeOnPV.Text = "text_PipeOnPV";
            buttonPipeOnPV.ShowText = true;
            buttonPipeOnPV.Size = RibbonItemSize.Large;
            buttonPipeOnPV.CommandHandler = new CommandPipeOnPV();

            ribbonPanelSource.Items.Add(buttonPipeOnPV);



        }

        public void Initialize()
        {
            NutsonRibbon();

        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }
    }

}
