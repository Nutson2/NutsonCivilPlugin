using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

namespace NutsonCivilPlugin.PipeOnPV
{
    class NetworkSettings
    {
        public PartsList partsList;
        public readonly Dictionary<string, List<string>> pipePartfamily;
        public readonly Dictionary<string, List<string>> structurePartfamily;
        public  NetworkSettings(Document doc, Network network)
        {
            ObjectId partListId = network.PartsListId;
            using (Transaction tr = doc.TransactionManager.StartTransaction())
            {
                partsList = (PartsList)tr.GetObject(partListId, OpenMode.ForRead);

                pipePartfamily=GetNetworkPartFamily(tr, partsList,DomainType.Pipe);
                structurePartfamily = GetNetworkPartFamily(tr, partsList, DomainType.Structure);
            }
        }
        public Dictionary<string, List<string>> GetPartFamilys(DomainType domainType)
        {
            if (domainType==DomainType.Pipe)
            {
                return pipePartfamily;
            }
            else
            {
                return structurePartfamily;
            }
        }
        private Dictionary<string, List<string>> GetNetworkPartFamily(Transaction tr, PartsList partsList, DomainType domainType)
        {
            Dictionary<string, List<string>> DomainPartFamily=new Dictionary<string, List<string>>();

            ObjectIdCollection PartFamilyIds = partsList.GetPartFamilyIdsByDomain(domainType);
            foreach (ObjectId PFid in PartFamilyIds)
            {
                PartFamily pf = (PartFamily)tr.GetObject(PFid, OpenMode.ForRead);
                List<string> PartSize = new List<string>();
                for (int i = 0; i < pf.PartSizeCount; i++)
                {
                    PartSize partSize = (PartSize)tr.GetObject(pf[i], OpenMode.ForRead);
                    PartSize.Add(partSize.Name);
                }
                DomainPartFamily.Add(pf.Name, PartSize);
            }
            return DomainPartFamily;
        }
    }

}
