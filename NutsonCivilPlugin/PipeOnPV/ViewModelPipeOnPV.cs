using System.Collections.Generic;
using System.ComponentModel;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using System.Collections.ObjectModel;
using Autodesk.Civil.DatabaseServices.Styles;
using System.Collections;

namespace NutsonCivilPlugin.PipeOnPV
{
    class ViewModelPipeOnPV : INotifyPropertyChanged
    {
        private ProfileView _pv;
        private readonly Document doc;
        private Network network;
        public ProfileView ProfileView
        {
            get { return _pv; }
            set
            {
                _pv = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ProfileView"));
                }
            }
        }
        private List<Part> allPartsFromPV;
        private NetworkSettings networkSettings;

        public ObservableCollection<Model> modelPipes;
        public ObservableCollection<Model> modelStructures;

        public event PropertyChangedEventHandler PropertyChanged;
        public ViewModelPipeOnPV()
        {
            doc = Application.DocumentManager.MdiActiveDocument;
            allPartsFromPV = new List<Part>();
            modelPipes = new ObservableCollection<Model>();
            modelStructures = new ObservableCollection<Model>();
        }
        /// <summary>
        /// Запрос у пользователя на выбор вида профиля
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public ProfileView SelectPV(Document doc)
        {
            PromptEntityOptions promptEntityOptions = new PromptEntityOptions("\nВыберите вид профиля");
            promptEntityOptions.AllowNone = false;
            promptEntityOptions.SetRejectMessage("\nНеобходимо выбрать вид профиля");
            promptEntityOptions.AddAllowedClass(typeof(ProfileView), true);

            PromptEntityResult res = doc.Editor.GetEntity(promptEntityOptions);
            if (res.Status == PromptStatus.OK)
            {
                using (Transaction tr = doc.TransactionManager.StartTransaction())
                {
                    ProfileView = (ProfileView)tr.GetObject(res.ObjectId, OpenMode.ForRead);
                    return ProfileView;
                }
            }
            return null;
        }
        private List<Part> GetNetworkPartsFromPV(Document doc, ProfileView profileView)
        {
            List<Part> networksPartOnPv=new List<Part>();

            using (Transaction tr = doc.TransactionManager.StartTransaction())
            {

                Alignment alignmentPV = (Alignment)tr.GetObject(profileView.AlignmentId, OpenMode.ForRead);
                AlignmentLine FirstEntity = (AlignmentLine)alignmentPV.Entities.EntityAtId(alignmentPV.Entities.FirstEntity);
                AlignmentLine LastEntity = (AlignmentLine)alignmentPV.Entities.EntityAtId(alignmentPV.Entities.LastEntity);

                Point2d startPoint = FirstEntity.StartPoint;
                Point2d endPoint = LastEntity.EndPoint;

                Structure startStructure = GetStructureAtPoint(startPoint);
                Structure endStructure = GetStructureAtPoint(endPoint);

                if (startStructure.NetworkId == endStructure.NetworkId)
                {
                    network = (Network)tr.GetObject(startStructure.NetworkId, OpenMode.ForRead);
                }
                double minLength = 0;
                ObjectIdCollection partsIdOnPV = Network.FindShortestNetworkPath(startStructure.Id, endStructure.Id, ref minLength);

                foreach (ObjectId objectId in partsIdOnPV)
                {
                    networksPartOnPv.Add((Part)tr.GetObject(objectId, OpenMode.ForRead));
                }
                networksPartOnPv.Add(endStructure);

            }
            return networksPartOnPv;
        }
        /// <summary>
        /// Подготовка коллекций труб и колодцев с вида профиля для отображения в форме
        /// </summary>
        /// <returns></returns>
        public bool GetNetworkPartsFromPV()
        {
            ProfileView = SelectPV(doc);
            allPartsFromPV = GetNetworkPartsFromPV(doc, ProfileView);
            networkSettings = new NetworkSettings(doc, network);

            PrepairPartsToShow(modelPipes, DomainType.Pipe);
            PrepairPartsToShow(modelStructures, DomainType.Structure);

            return true;
        }


        public void PrepairPartsToShow(ObservableCollection<Model> collection, DomainType domainType)
        {

            Model model;
            collection.Clear();

            Dictionary<string, List<string>> Partfamily= networkSettings.GetPartFamilys(domainType);

            foreach (Part part in allPartsFromPV)
            {
                if (part.Domain==domainType)
                {

                    model = new Model(part, Partfamily);

                    collection.Add(model);
                }
            }

        }
        private Structure GetStructureAtPoint(Point2d Point)
        {
            double offset = 2;
            Point3dCollection point3DCollection = new Point3dCollection(
                                                    new Point3d[]{
                                                            new Point3d(Point.X-offset,Point.Y,0),
                                                            new Point3d(Point.X,Point.Y+offset,0),
                                                            new Point3d(Point.X+offset,Point.Y,0),
                                                            new Point3d(Point.X,Point.Y-offset,0),});

            TypedValue[] filter = { new TypedValue(0, "AECC_STRUCTURE") };
            SelectionFilter selectionFilter = new SelectionFilter(filter);
            PromptSelectionResult res = doc.Editor.SelectCrossingPolygon(point3DCollection, selectionFilter);


            if (res.Status == PromptStatus.OK && res.Value.Count >= 0)
            {
                using (Transaction tr = doc.TransactionManager.StartTransaction())
                {

                    foreach (ObjectId objectId in res.Value.GetObjectIds())
                    {
                        Structure structure = (Structure)tr.GetObject(objectId, OpenMode.ForRead);
                        ObjectIdCollection pvWithStruct = structure.GetProfileViewsDisplayingMe();

                        foreach (ObjectId pvId in pvWithStruct)
                        {
                            if (pvId == ProfileView.Id)
                            {
                                return structure;
                            }
                        }
                    }

                }
            }
            return null;
        }

        public void SetPartFamily(IList selectedPipes,string PartFamilyName, string PartSizeName)
        {

            using (DocumentLock docLock=doc.LockDocument())
            {
                using (Transaction tr = doc.TransactionManager.StartTransaction())
                {
                    PartFamily partFamily;
                    partFamily = (PartFamily)tr.GetObject(networkSettings.partsList[PartFamilyName],OpenMode.ForRead);

                    foreach (Model modelPipe in selectedPipes)
                    {
                        modelPipe.Part = (Part)tr.GetObject(modelPipe.Part.Id, OpenMode.ForWrite);
                        modelPipe.SetPartFamily(partFamily, PartSizeName);

                    }
                    tr.Commit();
                }

            }
            var w=doc.Window;
            w.Focus();
        }
    }

}
