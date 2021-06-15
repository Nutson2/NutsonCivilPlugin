using System.Collections.Generic;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NutsonCivilPlugin.PipeOnPV
{
    class Model : INotifyPropertyChanged
    {
        private Part part;
        private string partFamily;
        private string partSize;
        private Dictionary<string, List<string>> partSettings;
        public List<string> ListPartFamilyTypes { get; set; }
        private List<string> listPartSize;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public List<string> ListPartSizes
        {
            get
            {
                return listPartSize;
            }
            set
            {
                listPartSize.Clear();
                listPartSize.AddRange(value);
                OnPropertyChanged();
            }
        }

        public Part Part
        {
            get { return part; }
            set { part = value; }
        }
        public string PartFamily
        {
            get { return partFamily; }
            set
            {
                partFamily = value;
                ListPartSizes = partSettings[partFamily];
                OnPropertyChanged();
            }
        }

        public string PartSize
        {
            get { return partSize; }
            set 
            {
                partSize = value;
                OnPropertyChanged();
            }
        }
        public void SetPartFamily(PartFamily pf, string psName)
        {
            ObjectId psId;
            if (!ListPartSizes.Contains(psName))
            {
                psId = (ObjectId)pf[0];
            }
            else
            {
                psId = (ObjectId)pf[psName];
            }

            part.SwapPartFamilyAndSize(pf.Id, psId);

        }
        public Model(Part networkPart, Dictionary<string, List<string>> PartSettings)
        {
            listPartSize = new List<string>();
            partSettings = PartSettings;
            ListPartFamilyTypes = new List<string>(partSettings.Keys);
            Part = networkPart;

            try
            {
                PartFamily = (string)part.GetType().GetProperty("PartFamilyName").GetValue(part);
            }
            catch (System.Exception)
            {

                PartFamily = "Ошибка определения типа семейства";
            }

            PartSize = part.PartSizeName;

        }
    }

}
