using System.Collections.Generic;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace NutsonCivilPlugin.PipeOnPV
{
    class Model
    {
        private Part part;
        private string partFamily;
        private string partSize;
        private Dictionary<string, List<string>> partSettings;
        public List<string> ListPartFamilyTypes { get; set; }
        private List<string> listPartSize;
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
            }
        }

        public Part Part
        {
            get { return part; }
            set 
                { 
                    part = value;
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
        public string PartFamily
        {
            get { return partFamily; }
            set 
            { 
                partFamily = value;

                ListPartSizes= partSettings[partFamily];
            }
        }

        public string PartSize
        {
            get { return partSize; }
            set { partSize = value; }
        }
        public void SetPartFamily(PartFamily pf, string psName)
        {
            ObjectId psId;
            if (psName == String.Empty) { psId = (ObjectId)pf[0]; } else { psId = (ObjectId)pf[psName]; }

            part.SwapPartFamilyAndSize(pf.Id, psId);
            PartFamily = pf.Name;
            PartSize = part.PartSizeName;
        }
        public Model(Part networkPart, Dictionary<string, List<string>> PartSettings)
        {
            listPartSize = new List<string>();
            partSettings = PartSettings;
            ListPartFamilyTypes= new List<string>(partSettings.Keys);
            Part = networkPart;
        }
    }

}
