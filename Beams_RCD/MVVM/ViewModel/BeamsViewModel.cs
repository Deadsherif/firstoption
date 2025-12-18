using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using Beams_RCD.Commands;
using Beams_RCD.MVVM.Model;
using Beams_RCD.MVVM.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using Beams_RCD.SelectionFilters;

namespace Beams_RCD.MVVM.ViewModel
{
    public class BeamsViewModel : INotifyPropertyChanged
    {
        #region Imp
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion


        #region Fields
        ObservableCollection<Element> dimStyles;
        ObservableCollection<Element> dimEleStyles;
        ObservableCollection<Family> framingTagsFamily;
        ObservableCollection<Family> floorTagsFamily;
        ObservableCollection<FamilySymbol> detailFamily;
        ObservableCollection<Element> spotElevations;
        Element selectedType;
        Element selectedType_Sec;
        Family selectedBeamTag;
        Family selectedFloorTag;
        FamilySymbol selectedBreakLine;
        Element selectedSpotElevation;
        private SolidColorBrush backGroundColor;
        double sectionMargin;
        string sectionName;
        string sectionName1;
        string sectionName2;
        bool is1_20;



        private static Element Element { get; set; }
        private static List<Element> Elements { get; set; }

        private static bool IsBeam { get; set; }
        #endregion


        #region Properties
        public ICommand DrawButton { get; set; }
        public ICommand CloseButton { get; set; }
        public ICommand SelectButton { get; set; }
        public ICommand CreateSectionButton { get; set; }

        public Element SelectedType
        {
            get { return selectedType; }
            set { selectedType = value; OnPropertyChanged(); }
        }
        public Element SelectedType_Sec
        {
            get { return selectedType_Sec; }
            set { selectedType_Sec = value; OnPropertyChanged(); }
        }
        public ObservableCollection<Family> FramingTagsFamilies
        {
            get { return framingTagsFamily; }
            set { framingTagsFamily = value; OnPropertyChanged(); }
        }
        public ObservableCollection<FamilySymbol> DetailFamilies
        {
            get { return detailFamily; }
            set { detailFamily = value; OnPropertyChanged(); }
        }
        public ObservableCollection<Element> SpotElevations
        {
            get { return spotElevations; }
            set { spotElevations = value; OnPropertyChanged(); }
        }
        public ObservableCollection<Family> FloorTagsFamilies
        {
            get { return floorTagsFamily; }
            set { floorTagsFamily = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Element> DimStyles
        {
            get { return dimStyles; }
            set
            {
                dimStyles = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Element> DimEleStyles
        {
            get { return dimEleStyles; }
            set
            {
                dimEleStyles = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush ButtonBackgroundColor
        {
            get { return backGroundColor; }
            set
            {
                backGroundColor = value;
                OnPropertyChanged();
            }
        }

        public double SectionMargin
        {
            get
            {
                return sectionMargin;
            }
            set
            {
                sectionMargin = value;
                OnPropertyChanged();
            }
        }
        public string SectionName
        {
            get
            {
                return sectionName;
            }
            set
            {
                sectionName = value;
                OnPropertyChanged();
            }
        }
        public string SectionName1
        {
            get
            {
                return sectionName1;
            }
            set
            {
                sectionName1 = value;
                OnPropertyChanged();
            }
        }
        public string SectionName2
        {
            get
            {
                return sectionName2;
            }
            set
            {
                sectionName2 = value;
                OnPropertyChanged();
            }
        }
        public bool Is1_20
        {
            get
            {
                return is1_20;
            }
            set
            {
                is1_20 = value;
                OnPropertyChanged();
            }
        }
        public Family SelectedBeamTag
        {
            get
            {
                return selectedBeamTag;

            }
            set
            {
                selectedBeamTag = value;
                OnPropertyChanged();
            }

        }
        public FamilySymbol SelectedBreakLine
        {
            get
            {
                return selectedBreakLine;

            }
            set
            {
                selectedBreakLine = value;
                OnPropertyChanged();
            }

        }
        public Element SelectedSpotElevation
        {
            get
            {
                return selectedSpotElevation;

            }
            set
            {
                selectedSpotElevation = value;
                OnPropertyChanged();
            }

        }
        public Family SelectedFloorTag
        {
            get
            {
                return selectedFloorTag;

            }
            set
            {
                selectedFloorTag = value;
                OnPropertyChanged();
            }

        }
        #endregion


        #region Constructor
        public BeamsViewModel()
        {
            DimStyles = new ObservableCollection<Element>();
            DimStyles = Command.FinalListOfDimensionStyles;
            DimEleStyles = Command.FinalListOfSecDimensionStyles;
            DetailFamilies = Command.AllDetailComponents;
            DrawButton = new RelayCommand(P => BeamsButton(P));
            CloseButton = new RelayCommand(P => CloseBTN(P));
            SelectButton = new RelayCommand(P => SelectBeamsCommand(P));
            CreateSectionButton = new RelayCommand(P => CreateSectionCommand(P));
            SelectedType = DimStyles[0];
            SelectedType_Sec = DimStyles[0];
            FramingTagsFamilies = Command.AllFramingTags;
            FloorTagsFamilies = Command.AllFlooringTags;
            SpotElevations = Command.AllSpotElevations;
            ButtonBackgroundColor = System.Windows.Media.Brushes.Red;
            Is1_20 = true;

        }

        #endregion


        #region Buttons Actions
        public void BeamsButton(object parameter)
        {
            Document Doc = Command.Doc;
            Autodesk.Revit.DB.View activeView = Doc.ActiveView;
            DimensionType D_Type = View.BeamsView.VM.SelectedType as DimensionType;
            //Get All beams in a certain view
            FilteredElementCollector collector = new FilteredElementCollector(Doc, activeView.Id);

            collector.OfCategory(BuiltInCategory.OST_StructuralFraming);
            List<Element> beamsInActiveView = collector.ToElements().ToList();


            //Get All grids in the project

            var grds = new FilteredElementCollector(Doc).OfClass(typeof(Grid)).ToList();

            List<Grid> AllGridsInProject = new List<Grid>();

            foreach (Element ele in grds)
            {
                AllGridsInProject.Add(ele as Grid);
            }


            // Create First Option Beams

            List<FO_Beam> FO_Beams = new List<FO_Beam>();

            foreach (Element element in beamsInActiveView)
            {
                FO_Beam B = new FO_Beam(element, AllGridsInProject);
                FO_Beams.Add(B);
            }


            //foreach FO_Beam , Apply the dimensions.


            foreach (FO_Beam B in FO_Beams)
            {
                try
                {
                    using (Transaction trans = new Transaction(Doc, "Beam Dimensions"))
                    {

                        trans.Start();

                        if (B.RightAndLeftFaces.Count == 2 && B.TopAndBottomFaces.Count == 2)
                        {

                            Face Face_1 = B.RightAndLeftFaces[0];
                            Face Face_2 = B.RightAndLeftFaces[1];
                            Grid MidGrid = B.CenterGrid;


                            Reference Face1_Reff = Face_1.Reference;
                            Reference Face2_Reff = Face_2.Reference;
                            ReferenceArray ArrayForDimWithinBeam = new ReferenceArray();
                            ArrayForDimWithinBeam.Append(Face1_Reff);
                            ArrayForDimWithinBeam.Append(Face2_Reff);

                            Line Lnew = B.BeamDimensionLine;

                            Dimension Dim = Doc.Create.NewDimension(activeView, Lnew, ArrayForDimWithinBeam, D_Type);
                        }

                        trans.Commit();
                    }

                }
                catch
                {

                }
            }

            foreach (FO_Beam B in FO_Beams)
            {
                try
                {
                    using (Transaction trans = new Transaction(Doc, "Beam Dimensions"))
                    {

                        trans.Start();

                        if (B.RightAndLeftFaces.Count == 2 && B.TopAndBottomFaces.Count == 2 && B.CenterGrid != null)
                        {
                            XYZ PerfectPosition_1 = Helper.GetLeaderEndPosition(B.Beam, 0);
                            XYZ PerfectPosition_2 = Helper.GetLeaderEndPosition(B.Beam, 1);



                            Face Face_1 = B.RightAndLeftFaces[0];

                            Face Face_2 = B.RightAndLeftFaces[1];
                            Grid MidGrid = B.CenterGrid;
                            Reference Grid_Reff = new Reference(MidGrid);

                            ReferenceArray ArrayForDimWithinBeam = new ReferenceArray();

                            Reference Face1_Reff = Face_1.Reference;
                            ArrayForDimWithinBeam.Append(Grid_Reff);
                            ArrayForDimWithinBeam.Append(Face1_Reff);
                            Line Lnew = B.GridBeamDimensionLine;
                            Dimension Dim = Doc.Create.NewDimension(activeView, Lnew, ArrayForDimWithinBeam, D_Type);

                            XYZ LnewCenter = Dim.Origin;


                            double Dis1 = LnewCenter.DistanceTo(PerfectPosition_1);
                            double Dis2 = LnewCenter.DistanceTo(PerfectPosition_2);

                            if (Dis1 < Dis2)
                            {
                                Line BeamPrallelLine = Helper.GetBeamParallelLine(B.Beam, 0);
                                //Intersection Point Method between BeamParallelLine and Lnew
                                XYZ IntersectionPoint = Helper.FindIntersection(BeamPrallelLine, Lnew);
                                Dim.LeaderEndPosition = IntersectionPoint;


                            }
                            else
                            {

                                Line BeamPrallelLine = Helper.GetBeamParallelLine(B.Beam, 1);
                                //Intersection Point Method between BeamParallelLine and Lnew
                                XYZ IntersectionPoint = Helper.FindIntersection(BeamPrallelLine, Lnew);
                                Dim.LeaderEndPosition = IntersectionPoint;


                            }

                            ReferenceArray ArrayForDimWithinBeam2 = new ReferenceArray();

                            Reference Face2_Reff = Face_2.Reference;
                            ArrayForDimWithinBeam2.Append(Grid_Reff);
                            ArrayForDimWithinBeam2.Append(Face2_Reff);
                            Line Lnew2 = B.GridBeamDimensionLine;

                            Dimension Dim2 = Doc.Create.NewDimension(activeView, Lnew2, ArrayForDimWithinBeam2, D_Type);

                            XYZ LnewCenter2 = Dim2.Origin;

              

                            double Dis_1 = LnewCenter2.DistanceTo(PerfectPosition_1);
                            double Dis_2 = LnewCenter2.DistanceTo(PerfectPosition_2);

                            if (Dis_1 < Dis_2)
                            {

                                Line BeamPrallelLine = Helper.GetBeamParallelLine(B.Beam, 0);
                                //Intersection Point Method between BeamParallelLine and Lnew
                                XYZ IntersectionPoint = Helper.FindIntersection(BeamPrallelLine, Lnew2);
                                Dim2.LeaderEndPosition = IntersectionPoint;


                            }
                            else
                            {

                                Line BeamPrallelLine = Helper.GetBeamParallelLine(B.Beam, 1);
                                //Intersection Point Method between BeamParallelLine and Lnew
                                XYZ IntersectionPoint = Helper.FindIntersection(BeamPrallelLine, Lnew2);
                                Dim2.LeaderEndPosition = IntersectionPoint;

                            }


                            if (Math.Abs(Dim.Value.Value - B.BeamWidth) < 0.0001 || Dim.Value.Value < 0.0001)
                            {
                                Doc.Delete(Dim.Id);
                            }

                            if (Math.Abs(Dim2.Value.Value - B.BeamWidth) < 0.0001 || Dim2.Value.Value < 0.0001)
                            {
                                Doc.Delete(Dim2.Id);
                            }

                        }

                        trans.Commit();
                    }



                }
                catch
                {

                }

            }
            MessageBox.Show("Dimensions Created", "Success");





        }

        public void CloseBTN(object parameter)
        {
            Command.BV.Close();
        }

        public void CreateSectionCommand(object parameter)
        {

            Document Doc = Command.Doc;
            var grds = new FilteredElementCollector(Doc).OfClass(typeof(Grid)).ToList();

            List<Grid> AllGridsInProject = new List<Grid>();

            foreach (Element ele in grds)
            {
                AllGridsInProject.Add(ele as Grid);
            }



            if (Elements?.Count > 0)
            {

                int i = 0;
                string SectionName = BeamsView.VM.SectionName;
                string SectionName1 = BeamsView.VM.SectionName1;
                string SectionName2 = BeamsView.VM.SectionName2;
                List<string> SectionNames = Helper.ListOfSectionNames(SectionName, SectionName1, SectionName2, Elements.Count);
                Element SpotElevationType = BeamsView.VM.SelectedSpotElevation;
                DimensionType D_Type = BeamsView.VM.SelectedType_Sec as DimensionType;
                Family BeamTag = BeamsView.VM.SelectedBeamTag;
                Family FloorTag = BeamsView.VM.SelectedFloorTag;
                bool IsScale1_20 = BeamsView.VM.Is1_20;
                FamilySymbol BreakLineFamily = BeamsView.VM.SelectedBreakLine;

                foreach (Element MainBeam in Elements)
                {

                    List<Element> Floors = Helper.GetAllFloors(Doc);
                    FO_Beam FOBeam = new FO_Beam(MainBeam, AllGridsInProject);







                    double BeamHeight = Helper.GetHeight(MainBeam);
                    double BeamWidth = Helper.GetWidth(MainBeam);
                    Autodesk.Revit.DB.View NewView;


                    FilteredElementCollector collector = new FilteredElementCollector(Doc)
                    .OfClass(typeof(ViewFamilyType));

                    // Filter out only section view family types
                    var sectionViewFamilyTypes = collector.Cast<ViewFamilyType>()
                        .Where(vft => vft.ViewFamily == ViewFamily.Section).ToList();

                    var sectionViewFamilyType = sectionViewFamilyTypes.FirstOrDefault();

                    var SectionBox = Helper.GetSectionViewperpendicularToBeams(MainBeam as FamilyInstance);

                    using (Transaction transaction = new Transaction(Doc, "Create Section View"))
                    {
                        transaction.Start();

                        #region Create SectionView
                        ViewSection sectionView = ViewSection.CreateSection(Doc, sectionViewFamilyType.Id, SectionBox);
                        Parameter farClipActiveParameter = sectionView.get_Parameter(BuiltInParameter.VIEWER_BOUND_FAR_CLIPPING);
                        Parameter detailLevelParam = sectionView.get_Parameter(BuiltInParameter.VIEW_DETAIL_LEVEL);
                        if (detailLevelParam != null)
                        {
                            // Set the Detail Level to Fine
                            ElementId detailLevelId = new ElementId((int)ViewDetailLevel.Fine);
                            detailLevelParam.Set(detailLevelId);
                        }

                        // Check if the parameter exists and is read-write
                        if (farClipActiveParameter != null && farClipActiveParameter.IsReadOnly == false)
                        {

                            farClipActiveParameter.Set(1);
                            Doc.Regenerate();

                        }
                        try
                        {
                            sectionView.Name = SectionNames[i];
                        }
                        catch
                        {
                            Random rnd = new Random();
                            int num = rnd.Next(10000);
                            sectionView.Name = SectionName + "_" + num.ToString();

                        }
                        if (IsScale1_20)
                        {
                            sectionView.Scale = 20;

                        }
                        else
                        {
                            sectionView.Scale = 25;
                        }
                        #endregion
                        NewView = sectionView;
                        transaction.Commit();
                    }
                    using (Transaction transaction = new Transaction(Doc, "Create Section View"))
                    {
                        transaction.Start();

                        #region Create B Dimensions
                        if (FOBeam.RightAndLeftFaces.Count > 0)
                        {
                            double ExtraDistance = 0;
                            if (FOBeam.CenterGrid != null)
                            {
                                ExtraDistance = 1.2;
                            }
                            else
                            {
                                ExtraDistance = 0.6;
                            }

                            ReferenceArray Reff = new ReferenceArray();
                            Reff.Append(FOBeam.RightAndLeftFaces[0].Reference);
                            Reff.Append(FOBeam.RightAndLeftFaces[1].Reference);

                            Line NewLine = Helper.MoveLine(FOBeam.BeamDimensionLine, new XYZ(0, 0, -BeamHeight - ExtraDistance));

                            Dimension WidthDimention = Doc.Create.NewDimension(NewView, NewLine, Reff, D_Type);


                        }
                        if (FOBeam.RightAndLeftFaces.Count >= 2 && FOBeam.TopAndBottomFaces.Count >= 2 && FOBeam.CenterGrid != null)
                        {
                            Face Face_1 = FOBeam.RightAndLeftFaces[0];

                            Face Face_2 = FOBeam.RightAndLeftFaces[1];
                            Grid MidGrid = FOBeam.CenterGrid;
                            Reference Grid_Reff = new Reference(MidGrid);

                            ReferenceArray ArrayForDimWithinBeam = new ReferenceArray();

                            Reference Face1_Reff = Face_1.Reference;
                            ArrayForDimWithinBeam.Append(Face1_Reff);
                            ArrayForDimWithinBeam.Append(Grid_Reff);
                            Line L = FOBeam.GridBeamDimensionLine;
                            Line LNew = Helper.MoveLine(L, new XYZ(0, 0, -BeamHeight - 0.75));

                            Dimension Dim = Doc.Create.NewDimension(NewView, LNew, ArrayForDimWithinBeam, D_Type);


                            ReferenceArray ArrayForDimWithinBeam2 = new ReferenceArray();

                            Reference Face2_Reff = Face_2.Reference;
                            ArrayForDimWithinBeam2.Append(Face2_Reff);
                            ArrayForDimWithinBeam2.Append(Grid_Reff);
                            Line L2 = FOBeam.GridBeamDimensionLine;
                            Line LNew2 = Helper.MoveLine(L2, new XYZ(0, 0, -BeamHeight - 0.75));


                            Dimension Dim2 = Doc.Create.NewDimension(NewView, LNew2, ArrayForDimWithinBeam2, D_Type);


                            if (Math.Abs(Dim.Value.Value - FOBeam.BeamWidth) < 0.0001 || Dim.Value.Value < 0.0001)
                            {
                                Doc.Delete(Dim.Id);
                            }

                            if (Math.Abs(Dim2.Value.Value - FOBeam.BeamWidth) < 0.0001 || Dim2.Value.Value < 0.0001)
                            {
                                Doc.Delete(Dim2.Id);
                            }

                        }
                        #endregion


                        XYZ HeadPoint = Helper.GetSectionHeadPosition(NewView as ViewSection);

                        transaction.Commit();
                    }

                    using (Transaction transaction = new Transaction(Doc, "Create Depth Dimension"))
                    {
                        transaction.Start();

                        if (FOBeam.TopAndBottomFaces.Count > 0)
                        {
                            double ExtraDistance = (BeamWidth / 2) + 0.5;



                            List<Element> FloorsInView = new FilteredElementCollector(Doc, NewView.Id).OfCategory(BuiltInCategory.OST_Floors).ToElements().ToList();

                            if (FloorsInView.Count == 0)
                            {
                                ReferenceArray Reff = new ReferenceArray();
                                Reff.Append(FOBeam.TopAndBottomFaces[0].Reference);
                                Reff.Append(FOBeam.TopAndBottomFaces[1].Reference);

                                XYZ BeamStartPoint = (MainBeam.Location as LocationCurve).Curve.GetEndPoint(0);
                                XYZ BeamEndPoint = (MainBeam.Location as LocationCurve).Curve.GetEndPoint(0);
                                double BeamSlope = Helper.CalculateSlope(BeamStartPoint, BeamEndPoint);
                                double TraverseSlope = Helper.PerpendicularSlope(BeamSlope);

                                double Xnew = BeamStartPoint.X - 5;
                                double Ynew = TraverseSlope * (Xnew - BeamStartPoint.X) + BeamStartPoint.Y;
                                double Znew = BeamStartPoint.Z;

                                XYZ DistnationPoint = new XYZ(Xnew, Ynew, Znew);



                                XYZ Point_1 = new XYZ(BeamStartPoint.X, BeamStartPoint.Y, BeamStartPoint.Z);
                                XYZ Point_2 = new XYZ(Point_1.X, Point_1.Y, Point_1.Z + 1);

                                Line TempLine = Line.CreateBound(Point_1, Point_2);

                                //Line NewLine = Methods.MoveLine(TempLine, DistnationPoint - Point_1);

                                Dimension DepthDimention = Doc.Create.NewDimension(NewView, TempLine, Reff, D_Type);

                                Line BeamLine = ((FOBeam.Beam).Location as LocationCurve).Curve as Line;

                                Line PrallelLine;
                                List<Line> PLines = Helper.OffsetLine(BeamLine, 1.5);

                                XYZ L1_CenterPoint = (PLines[0].GetEndPoint(0) + PLines[0].GetEndPoint(1)) / 2;
                                XYZ L2_CenterPoint = (PLines[1].GetEndPoint(0) + PLines[1].GetEndPoint(1)) / 2;

                                PrallelLine = PLines[Helper.GetParallelBeamIndex(FOBeam.Beam, PLines[0], PLines[1])];

                                XYZ BeamMidPoint = Helper.GetBeamCenterPoint(FOBeam.Beam);
                                XYZ DistnationBoint = (PrallelLine.GetEndPoint(0) + PrallelLine.GetEndPoint(1)) / 2;

                                ElementTransformUtils.MoveElement(Doc, DepthDimention.Id, (DistnationBoint - BeamMidPoint));


                            }
                            else
                            {
                                Element FloorInView = FloorsInView.FirstOrDefault();
                                CurveArray Array = new CurveArray();
                                foreach (Curve c in Helper.GetFloorCurves(FloorInView))
                                {
                                    Array.Append(c);
                                }

                                bool IsFloorVisable = Helper.IsPointInPolygon(Helper.GetBeamCenterPoint(FOBeam.Beam), Array);


                                if (!IsFloorVisable)
                                {
                                    ReferenceArray Reff = new ReferenceArray();
                                    Reff.Append(FOBeam.TopAndBottomFaces[0].Reference);
                                    Reff.Append(FOBeam.TopAndBottomFaces[1].Reference);

                                    XYZ BeamStartPoint = (MainBeam.Location as LocationCurve).Curve.GetEndPoint(0);
                                    XYZ BeamEndPoint = (MainBeam.Location as LocationCurve).Curve.GetEndPoint(0);
                                    double BeamSlope = Helper.CalculateSlope(BeamStartPoint, BeamEndPoint);
                                    double TraverseSlope = Helper.PerpendicularSlope(BeamSlope);

                                    double Xnew = BeamStartPoint.X - 5;
                                    double Ynew = TraverseSlope * (Xnew - BeamStartPoint.X) + BeamStartPoint.Y;
                                    double Znew = BeamStartPoint.Z;

                                    XYZ DistnationPoint = new XYZ(Xnew, Ynew, Znew);



                                    XYZ Point_1 = new XYZ(BeamStartPoint.X, BeamStartPoint.Y, BeamStartPoint.Z);
                                    XYZ Point_2 = new XYZ(Point_1.X, Point_1.Y, Point_1.Z + 1);

                                    Line TempLine = Line.CreateBound(Point_1, Point_2);

                                    //Line NewLine = Methods.MoveLine(TempLine, DistnationPoint - Point_1);

                                    Dimension DepthDimention = Doc.Create.NewDimension(NewView, TempLine, Reff, D_Type);

                                    Line BeamLine = ((FOBeam.Beam).Location as LocationCurve).Curve as Line;

                                    Line PrallelLine;
                                    List<Line> PLines = Helper.OffsetLine(BeamLine, 1.5);

                                    XYZ L1_CenterPoint = (PLines[0].GetEndPoint(0) + PLines[0].GetEndPoint(1)) / 2;
                                    XYZ L2_CenterPoint = (PLines[1].GetEndPoint(0) + PLines[1].GetEndPoint(1)) / 2;

                                    PrallelLine = PLines[Helper.GetParallelBeamIndex(FOBeam.Beam, PLines[0], PLines[1])];

                                    XYZ BeamMidPoint = Helper.GetBeamCenterPoint(FOBeam.Beam);
                                    XYZ DistnationBoint = (PrallelLine.GetEndPoint(0) + PrallelLine.GetEndPoint(1)) / 2;

                                    ElementTransformUtils.MoveElement(Doc, DepthDimention.Id, (DistnationBoint - BeamMidPoint));

                                }
                                else
                                {
                                    try
                                    {
                                        #region Add Dimension Total D
                                        {  // Add total d
                                            ReferenceArray Reff = new ReferenceArray();

                                            Reff.Append(FOBeam.TopAndBottomFaces.OrderBy(e => (e as PlanarFace).Origin.Z).FirstOrDefault().Reference);
                                            Solid Sf = Helper.GetSolidForFloor(FloorInView);
                                            FaceArray FaceArr = Sf.Faces;
                                            List<Face> FloorFaces = Helper.FaceArrayToList(FaceArr);
                                            List<PlanarFace> PlannerFacesOrigins = new List<PlanarFace>();
                                            foreach (PlanarFace pf in FloorFaces)
                                            {
                                                PlannerFacesOrigins.Add(pf);
                                            }

                                            var OrderedFaces = PlannerFacesOrigins.OrderByDescending(e => e.Origin.Z);
                                            Face FloorTopFace = OrderedFaces.FirstOrDefault();
                                            Reff.Append((FloorTopFace as PlanarFace).Reference);


                                            XYZ BeamStartPoint = (MainBeam.Location as LocationCurve).Curve.GetEndPoint(0);
                                            XYZ BeamEndPoint = (MainBeam.Location as LocationCurve).Curve.GetEndPoint(0);
                                            double BeamSlope = Helper.CalculateSlope(BeamStartPoint, BeamEndPoint);
                                            double TraverseSlope = Helper.PerpendicularSlope(BeamSlope);

                                            double Xnew = BeamStartPoint.X - 5;
                                            double Ynew = TraverseSlope * (Xnew - BeamStartPoint.X) + BeamStartPoint.Y;
                                            double Znew = BeamStartPoint.Z;

                                            XYZ DistnationPoint = new XYZ(Xnew, Ynew, Znew);



                                            XYZ Point_1 = new XYZ(BeamStartPoint.X, BeamStartPoint.Y, BeamStartPoint.Z);
                                            XYZ Point_2 = new XYZ(Point_1.X, Point_1.Y, Point_1.Z + 1);

                                            Line TempLine = Line.CreateBound(Point_1, Point_2);


                                            Dimension DepthDimention = Doc.Create.NewDimension(NewView, TempLine, Reff, D_Type);

                                            Line BeamLine = ((FOBeam.Beam).Location as LocationCurve).Curve as Line;

                                            Line PrallelLine;
                                            List<Line> PLines = Helper.OffsetLine(BeamLine, 1.5);

                                            XYZ L1_CenterPoint = (PLines[0].GetEndPoint(0) + PLines[0].GetEndPoint(1)) / 2;
                                            XYZ L2_CenterPoint = (PLines[1].GetEndPoint(0) + PLines[1].GetEndPoint(1)) / 2;

                                            PrallelLine = PLines[Helper.GetParallelBeamIndex(FOBeam.Beam, PLines[0], PLines[1])];


                                            XYZ BeamMidPoint = Helper.GetBeamCenterPoint(FOBeam.Beam);
                                            XYZ DistnationBoint = (PrallelLine.GetEndPoint(0) + PrallelLine.GetEndPoint(1)) / 2;

                                            ElementTransformUtils.MoveElement(Doc, DepthDimention.Id, (DistnationBoint - BeamMidPoint));



                                        }

                                        #endregion
                                    }
                                    catch { }


                                    {

                                        //Add Dimensions for slab thickness
                                        try
                                        {
                                            #region DimensionForSlabs

                                            ReferenceArray Reff = new ReferenceArray();
                                            FloorInView = FloorsInView.FirstOrDefault();
                                            Solid Sf = Helper.GetSolidForFloor(FloorInView);
                                            FaceArray FaceArr = Sf.Faces;
                                            List<Face> FloorFaces = Helper.FaceArrayToList(FaceArr);
                                            List<PlanarFace> PlannerFacesOrigins = new List<PlanarFace>();
                                            foreach (PlanarFace pf in FloorFaces)
                                            {
                                                PlannerFacesOrigins.Add(pf);
                                            }

                                            Face FloorTopFace = PlannerFacesOrigins.OrderByDescending(e => e.Origin.Z).FirstOrDefault();
                                            Face FloorBotFace = PlannerFacesOrigins.OrderBy(e => e.Origin.Z).FirstOrDefault();

                                            Reff.Append((FloorTopFace as PlanarFace).Reference);
                                            Reff.Append((FloorBotFace as PlanarFace).Reference);

                                            XYZ BeamStartPoint = (MainBeam.Location as LocationCurve).Curve.GetEndPoint(0);
                                            XYZ BeamEndPoint = (MainBeam.Location as LocationCurve).Curve.GetEndPoint(0);
                                            double BeamSlope = Helper.CalculateSlope(BeamStartPoint, BeamEndPoint);
                                            double TraverseSlope = Helper.PerpendicularSlope(BeamSlope);

                                            double Xnew = BeamStartPoint.X - 5;
                                            double Ynew = TraverseSlope * (Xnew - BeamStartPoint.X) + BeamStartPoint.Y;
                                            double Znew = BeamStartPoint.Z;

                                            XYZ DistnationPoint = new XYZ(Xnew, Ynew, Znew);



                                            XYZ Point_1 = new XYZ(BeamStartPoint.X, BeamStartPoint.Y, BeamStartPoint.Z);
                                            XYZ Point_2 = new XYZ(Point_1.X, Point_1.Y, Point_1.Z + 1);

                                            Line TempLine = Line.CreateBound(Point_1, Point_2);

                                            //Line NewLine = Methods.MoveLine(TempLine, DistnationPoint - Point_1);

                                            Dimension DepthDimention = Doc.Create.NewDimension(NewView, TempLine, Reff, D_Type);

                                            Line BeamLine = ((FOBeam.Beam).Location as LocationCurve).Curve as Line;

                                            Line PrallelLine;
                                            List<Line> PLines = Helper.OffsetLine(BeamLine, 1);

                                            XYZ L1_CenterPoint = (PLines[0].GetEndPoint(0) + PLines[0].GetEndPoint(1)) / 2;
                                            XYZ L2_CenterPoint = (PLines[1].GetEndPoint(0) + PLines[1].GetEndPoint(1)) / 2;


                                            PrallelLine = PLines[Helper.GetParallelBeamIndex(FOBeam.Beam, PLines[0], PLines[1])];


                                            XYZ BeamMidPoint = Helper.GetBeamCenterPoint(FOBeam.Beam);
                                            XYZ DistnationBoint = (PrallelLine.GetEndPoint(0) + PrallelLine.GetEndPoint(1)) / 2;
                                            ElementTransformUtils.MoveElement(Doc, DepthDimention.Id, (DistnationBoint - BeamMidPoint));

                                            Reff = new ReferenceArray();
                                            Reff.Append((FloorBotFace as PlanarFace).Reference);
                                            Reff.Append(FOBeam.TopAndBottomFaces.OrderBy(e => (e as PlanarFace).Origin.Z).FirstOrDefault().Reference);
                                            Dimension DepthDimention2 = Doc.Create.NewDimension(NewView, TempLine, Reff, D_Type);
                                            ElementTransformUtils.MoveElement(Doc, DepthDimention2.Id, (DistnationBoint - BeamMidPoint));
                                            #endregion
                                        }
                                        catch { }

                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                        ///
                                        try
                                        {
                                            #region Add BreakLines
                                            //Add Break Lines

                                            Line BeamLine = ((FOBeam.Beam).Location as LocationCurve).Curve as Line;
                                            List<Line> PLines2 = Helper.OffsetLine(BeamLine, 3);
                                            Line PrallelLine2;
                                            XYZ L1_CenterPoint2 = (PLines2[0].GetEndPoint(0) + PLines2[0].GetEndPoint(1)) / 2;
                                            XYZ L2_CenterPoint2 = (PLines2[1].GetEndPoint(0) + PLines2[1].GetEndPoint(1)) / 2;

                                            PrallelLine2 = PLines2[Helper.GetParallelBeamIndex(FOBeam.Beam, PLines2[0], PLines2[1])];
                                            XYZ DistnationPoint2 = (PrallelLine2.GetEndPoint(0) + PrallelLine2.GetEndPoint(1)) / 2;

                                            Line L_BreakLine = Line.CreateBound(new XYZ(DistnationPoint2.X, DistnationPoint2.Y, DistnationPoint2.Z - 2), new XYZ(DistnationPoint2.X, DistnationPoint2.Y, DistnationPoint2.Z + 0.5));

                                            if (BreakLineFamily != null)
                                                BreakLineFamily.Activate();

                                            var _familyInstance = Doc.Create.NewFamilyInstance(L_BreakLine, BreakLineFamily, NewView);
                                            Parameter _MaskingLengthParameter = _familyInstance.LookupParameter("MaskingLength");
                                            _MaskingLengthParameter.Set(1200 * 0.00328084);



                                            if (Helper.GetParallelBeamIndex(FOBeam.Beam, PLines2[0], PLines2[1]) == 1)
                                            {
                                                PrallelLine2 = PLines2[0];
                                            }
                                            else
                                            {
                                                PrallelLine2 = PLines2[1];
                                            }
                                            XYZ DistnationPoint3 = (PrallelLine2.GetEndPoint(0) + PrallelLine2.GetEndPoint(1)) / 2;

                                            Line L_BreakLine2 = Line.CreateBound(new XYZ(DistnationPoint3.X, DistnationPoint3.Y, DistnationPoint3.Z + 0.5), new XYZ(DistnationPoint3.X, DistnationPoint3.Y, DistnationPoint3.Z - 2));

                                            BreakLineFamily.Activate();

                                            var familyInstance = Doc.Create.NewFamilyInstance(L_BreakLine2, BreakLineFamily, NewView);

                                            Parameter MaskingLengthParameter = familyInstance.LookupParameter("MaskingLength");
                                            MaskingLengthParameter.Set(1200 * 0.00328084);


                                            #endregion
                                        }
                                        catch { }
                                    }
                                }
                            }


                        }

                        transaction.Commit();
                    }

                    using (Transaction transaction = new Transaction(Doc, "Hide and Spot Level"))
                    {
                        transaction.Start();

                        var levels = Helper.GetLevels(Doc);
                        var lvl = levels.FirstOrDefault();
                        NewView.SetCategoryHidden(lvl.Category.Id, true);

                        if (FOBeam.TopAndBottomFaces.Count > 0)
                        {
                            List<Element> FloorsInView = new FilteredElementCollector(Doc, NewView.Id).OfCategory(BuiltInCategory.OST_Floors).ToElements().ToList();


                            if (FloorsInView.Count > 0)
                            {
                                Element FloorInView = FloorsInView.FirstOrDefault();
                                CurveArray Array = new CurveArray();
                                foreach (Curve c in Helper.GetFloorCurves(FloorInView))
                                {
                                    Array.Append(c);
                                }
                                bool IsFloorVisable = Helper.IsPointInPolygon(Helper.GetBeamCenterPoint(FOBeam.Beam), Array);
                                if (!IsFloorVisable)
                                {
                                    List<PlanarFace> PFaces = new List<PlanarFace>();

                                    PFaces.Add(FOBeam.TopAndBottomFaces[0] as PlanarFace);
                                    PFaces.Add(FOBeam.TopAndBottomFaces[1] as PlanarFace);

                                    var TopFace = PFaces.OrderByDescending(e => e.Origin.Z).FirstOrDefault();
                                    XYZ Origin = Helper.GetBeamCenterPoint(FOBeam.Beam);
                                    XYZ BentPoint = new XYZ(Origin.X, Origin.Y, Origin.Z + 0.5);
                                    XYZ EndPoint = new XYZ(BentPoint.X + 0.6, BentPoint.Y, BentPoint.Z);



                                    SpotDimension spotElevation = Doc.Create.NewSpotElevation(NewView, TopFace.Reference, Origin, BentPoint, EndPoint, EndPoint, true);
                                    spotElevation.DimensionType = SpotElevationType as SpotDimensionType;
                                    //((spotElevation as Element) as FamilyInstance).Symbol = SpotElevationType as FamilySymbol;
                                    //BuiltInParameter para_symbol = BuiltInParameter.SPOT_ELEV_SYMBOL;
                                    //ElementId P = spotElevation.GetType().get_Parameter(para_symbol).AsElementId();
                                    //P.Set(SpotElevationType.Id);
                                    var ss = SpotElevationType;

                                }
                                else
                                {
                                    #region Get the Top Floor Face
                                    FloorInView = FloorsInView.FirstOrDefault();
                                    Solid Sf = Helper.GetSolidForFloor(FloorInView);
                                    FaceArray FaceArr = Sf.Faces;
                                    List<Face> FloorFaces = Helper.FaceArrayToList(FaceArr);
                                    List<PlanarFace> PlannerFacesOrigins = new List<PlanarFace>();
                                    foreach (PlanarFace pf in FloorFaces)
                                    {
                                        PlannerFacesOrigins.Add(pf);
                                    }

                                    PlanarFace FloorTopFace = PlannerFacesOrigins.OrderByDescending(p => p.Origin.Z).FirstOrDefault();
                                    #endregion

                                    List<PlanarFace> PFaces = new List<PlanarFace>();

                                    PFaces.Add(FOBeam.TopAndBottomFaces[0] as PlanarFace);
                                    PFaces.Add(FOBeam.TopAndBottomFaces[1] as PlanarFace);
                                    var BotFace = PFaces.OrderBy(e => e.Origin.Z).FirstOrDefault();

                                    XYZ BeamMidPoint = Helper.GetBeamCenterPoint(FOBeam.Beam);




                                    XYZ NewLocaion = new XYZ(BeamMidPoint.X, BeamMidPoint.Y, FloorTopFace.Origin.Z);
                                    //XYZ NewLocationMoves = Methods.MovePointInZeroDirection(NewLocaion, FOBeam.BeamWidth);



                                    XYZ Origin = FloorTopFace.Origin;
                                    XYZ BentPoint = new XYZ(Origin.X, Origin.Y, Origin.Z + 0.5);
                                    XYZ EndPoint = new XYZ(BentPoint.X + 0.6, BentPoint.Y, BentPoint.Z);

                                    XYZ MoveVector = new XYZ((NewLocaion.X - Origin.X), (NewLocaion.Y - Origin.Y), 0);

                                    SpotDimension spotElevation = Doc.Create.NewSpotElevation(NewView, FloorTopFace.Reference, Origin, BentPoint, EndPoint, EndPoint, true);
                                    spotElevation.DimensionType = SpotElevationType as SpotDimensionType;
                                    //BuiltInParameter para_symbol = BuiltInParameter.SPOT_ELEV_SYMBOL;
                                    //Parameter P = spotElevation.get_Parameter(para_symbol);
                                    //P.Set(SpotElevationType.Id);

                                    var ss = SpotElevationType;
                                    ElementTransformUtils.MoveElement(Doc, spotElevation.Id, MoveVector);

                                }


                            }
                            else
                            {
                                List<PlanarFace> PFaces = new List<PlanarFace>();

                                PFaces.Add(FOBeam.TopAndBottomFaces[0] as PlanarFace);
                                PFaces.Add(FOBeam.TopAndBottomFaces[1] as PlanarFace);

                                var TopFace = PFaces.OrderByDescending(e => e.Origin.Z).FirstOrDefault();
                                XYZ Origin = Helper.GetBeamCenterPoint(FOBeam.Beam);
                                XYZ BentPoint = new XYZ(Origin.X, Origin.Y, Origin.Z + 0.5);
                                XYZ EndPoint = new XYZ(BentPoint.X + 0.6, BentPoint.Y, BentPoint.Z);



                                SpotDimension spotElevation = Doc.Create.NewSpotElevation(NewView, TopFace.Reference, Origin, BentPoint, EndPoint, EndPoint, true);
                                spotElevation.DimensionType = SpotElevationType as SpotDimensionType;

                                //((spotElevation as Element) as FamilyInstance).Symbol = SpotElevationType as FamilySymbol;
                                //BuiltInParameter para_symbol = BuiltInParameter.SPOT_ELEV_SYMBOL;
                                //ElementId P = spotElevation.GetType().get_Parameter(para_symbol).AsElementId();
                                //P.Set(SpotElevationType.Id);
                                var ss = SpotElevationType;

                            }

                        }
                        transaction.Commit();
                    }



                    i++;
                }

            }

            MessageBox.Show("Sections Created Successfully", "FirstOption");

        }


        public void SelectBeamsCommand(object parameter)
        {
            Command.BV.Hide();
            UIDocument UiDoc = Command.UiDoc;
            Document Doc = Command.Doc;
            Elements = new List<Element>();
            try
            {

                BeamSelectionFilter beamFilter = new BeamSelectionFilter();
                var Reff = UiDoc.Selection.PickObjects(ObjectType.Element, beamFilter, "Select Beams");
                foreach (Reference reff in Reff)
                {
                    Elements.Add(Doc.GetElement(reff));
                }

            }
            catch
            {
                Elements.Clear();
            }

            if (Elements.Count > 0)
            {
                IsBeam = true;
                BeamsView.VM.ButtonBackgroundColor = System.Windows.Media.Brushes.Green;

            }
            else
            {
                IsBeam = false;
                BeamsView.VM.ButtonBackgroundColor = System.Windows.Media.Brushes.Red;
            }

            Command.BV.ShowDialog();
        }
        #endregion

    }
}
