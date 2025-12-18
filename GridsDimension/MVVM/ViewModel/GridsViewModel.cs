using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Input;
using GridsDimension.Commands;
using System.Runtime.Remoting.Contexts;
using GridsDimension.MVVM.Model;

namespace GridsDimension.MVVM.ViewModel
{
    public class GridsViewModel : INotifyPropertyChanged
    {
        #region Imp
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Fields
        private string bubbleDistanceText;
        private bool doGrids;
        private bool currentView;
        private bool isListEnabled;
        private ObservableCollection<Autodesk.Revit.DB.View> planViews;
        private ObservableCollection<Autodesk.Revit.DB.View> selectedViews;
        private SolidColorBrush backgroundColor;

        /// /////////////////

        double DistanceFromBubble;
        double DistanceFromGrid;
        bool DrawInActiveView = true;
        ObservableCollection<Autodesk.Revit.DB.View> FinalSelectedView;
        #endregion



        #region Properties

        public ICommand CloseBTN { get; set; }
        public ICommand DrawBTN { get; set; }
        public SolidColorBrush BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; OnPropertyChanged(); }
        }

        public string BubbleDistanceText
        {
            get { return bubbleDistanceText; }
            set
            {

                try
                {
                    bubbleDistanceText = value;
                    double S = Convert.ToDouble(value);
                    DistanceFromBubble = S * 0.00328084;
                    OnPropertyChanged();
                }
                catch
                {
                    bubbleDistanceText = value;
                    DistanceFromBubble = 0;
                    OnPropertyChanged();
                }
            }
        }

        public bool CurrentView
        {
            get
            {
                return currentView;
            }
            set
            {
                DrawInActiveView = value;
                currentView = value;
                OnPropertyChanged();
                if (currentView)
                {
                    BackgroundColor = new SolidColorBrush(Colors.Gray);
                    IsListEnabled = false;
                }
                else
                {
                    BackgroundColor = new SolidColorBrush(Colors.White);
                    IsListEnabled = true;
                }
            }

        }
        public bool IsListEnabled
        {
            get
            {
                return isListEnabled;
            }
            set
            {
                isListEnabled = value;
                OnPropertyChanged();
            }

        }




        public bool DoGrids
        {
            get
            {
                return doGrids;
            }
            set
            {
                doGrids = value;
                OnPropertyChanged();
            }

        }

        public ObservableCollection<Autodesk.Revit.DB.View> PlanViews
        {
            get { return planViews; }
            set { planViews = value; OnPropertyChanged(); }
        }
        public ObservableCollection<Autodesk.Revit.DB.View> SelectedViews
        {
            get
            {
                return selectedViews;
            }
            set
            {
                selectedViews = value;
               FinalSelectedView = value;
                OnPropertyChanged();
            }
        }


        public ICommand cmd { get; set; }

        #endregion


        #region Constructor
        public GridsViewModel(ObservableCollection<Autodesk.Revit.DB.View> Views)
        {
            DoGrids = true;
            PlanViews = Views;
            cmd = new RelayCommand(p => ListBoxCommand(p));
            CloseBTN = new RelayCommand(p => CloseCommand(p));
            DrawBTN = new RelayCommand(p => DrawCommand(p));
            CurrentView = true;
            BackgroundColor = new SolidColorBrush(Colors.Gray);
            BubbleDistanceText = "1000";
        }

        #endregion



        #region ButtonActions
        public void CloseCommand(object parameter)
        {
            Command.ViewFrm.Close();
        }
        public void ListBoxCommand(object parameter)
        {
            List<Autodesk.Revit.DB.View> list = (List<Autodesk.Revit.DB.View>)(parameter);

            ObservableCollection<Autodesk.Revit.DB.View> views = new ObservableCollection<Autodesk.Revit.DB.View>();

            foreach (Autodesk.Revit.DB.View V in list)
            {
                views.Add(V);
            }

            try
            {
                Command.VMD.SelectedViews = views;
            }
            catch { }
        }

        public void DrawCommand(object parameter)
        {
            UIDocument uidoc = Command.UiDoc;
            Document Doc = uidoc.Document;

            List<List<FO_Grids>> ListsOfGridGroups = new List<List<FO_Grids>>();
            List<List<Grid>> FinalListsOfGridGroups = new List<List<Grid>>();
            Options Op = new Options();
            Op.DetailLevel = ViewDetailLevel.Fine;
            Op.ComputeReferences = true;


            var grds = new FilteredElementCollector(Doc).OfClass(typeof(Grid)).ToList();

            if (grds.Count > 0)
            {
                #region Setting the List of GridGroups

                List<FO_Grids> Grids = new List<FO_Grids>();

                foreach (Grid gr in grds)
                {
                    Grids.Add(new FO_Grids(gr));
                }

                int n = Grids.Count;

               Helper.GroupLinesBySlope(Grids); // Setting The groupId for grids based on the slope

                var NewGrids = Grids.OrderBy(g => g.GroupId).ToList();

                int NumberOfGroups = Helper.GetNumberOFGroups(NewGrids);  // getting the number of the groups
                int LargestGroupId = NewGrids[NewGrids.Count - 1].GroupId;

                for (int i = 1; i <= LargestGroupId; i++)
                {
                    List<FO_Grids> G = new List<FO_Grids>();

                    for (int j = 0; j < n; j++)
                    {
                        if (NewGrids[j].GroupId == i)
                        {
                            G.Add(NewGrids[j]);
                        }

                    }
                    if (G.Count != 0)
                    {
                        ListsOfGridGroups.Add(G);
                    }
                }

                #endregion


                #region ReAssign RevitGrids to Grids and Order the Lists
                // Now We have to create the exact same List of Lists but for grids not RevitGrids
                foreach (List<FO_Grids> Lst in ListsOfGridGroups)
                {
                    if (Lst[0].GroupId != 1)
                    {
                        var OrdLst = Lst.OrderBy(e => e.CenterPoint.Y).ToList();
                        List<Grid> _L = new List<Grid>();
                        foreach (FO_Grids g in OrdLst)
                        {
                            foreach (Grid OG in grds)
                            {
                                if (OG.Id.IntegerValue == g.Id.IntegerValue)
                                {
                                    _L.Add(OG); break;
                                }
                            }
                        }
                        FinalListsOfGridGroups.Add(_L);

                    }
                    else
                    {
                        var OrdLst = Lst.OrderBy(e => e.CenterPoint.X).ToList();
                        List<Grid> _L = new List<Grid>();
                        foreach (FO_Grids g in OrdLst)
                        {
                            foreach (Grid OG in grds)
                            {
                                if (OG.Id.IntegerValue == g.Id.IntegerValue)
                                {
                                    _L.Add(OG); break;
                                }
                            }
                        }
                        FinalListsOfGridGroups.Add(_L);
                    }
                }

                #endregion


                #region Get List of Views To Draw in

                //Now We Decide which views to draw in 

                if (DrawInActiveView)
                {
                    FinalSelectedView = new ObservableCollection<Autodesk.Revit.DB.View>();
                    FinalSelectedView.Add(Doc.ActiveView);
                }

                #endregion


                #region Draw Grids


                foreach (Autodesk.Revit.DB.View VV in FinalSelectedView)
                {
                    using (Transaction trans = new Transaction(Doc, "Create Dimensions"))
                    {
                        trans.Start();

                        foreach (List<Grid> G in FinalListsOfGridGroups)
                        {
                            double Addition = 0;
                            if (G.Count > 2)
                            {
                                Addition = 1000 * 0.00328084;
                            }
                            else
                            {
                                Addition = 0;
                            }

                            for (int u = 0; u < G.Count - 1; u++)
                            {
                                ReferenceArray Arr = new ReferenceArray();

                                XYZ P1 = Helper.GetPointOnLine(G[u].Curve.GetEndPoint(0), G[u].Curve.GetEndPoint(1), DistanceFromBubble + Addition);
                                XYZ P11 = Helper.GetPointOnLine(G[u].Curve.GetEndPoint(1), G[u].Curve.GetEndPoint(0), DistanceFromBubble + Addition);


                                XYZ P2 = Helper.GetPointOnLine(G[u + 1].Curve.GetEndPoint(0), G[u + 1].Curve.GetEndPoint(1), DistanceFromBubble + Addition);
                                XYZ P22 = Helper.GetPointOnLine(G[u + 1].Curve.GetEndPoint(1), G[u + 1].Curve.GetEndPoint(0), DistanceFromBubble + Addition);

                                Line L1 = Line.CreateBound(P1, P2);
                                Line L2 = Line.CreateBound(P11, P22);

                                Reference R1 = new Reference(G[u]);
                                Reference R2 = new Reference(G[u + 1]);

                                Arr.Append(R1);
                                Arr.Append(R2);
                                Doc.Create.NewDimension(VV, L1, Arr);
                                Doc.Create.NewDimension(VV, L2, Arr);

                            }

                            if (G.Count > 2)
                            {
                                XYZ Pa = Helper.GetPointOnLine(G[0].Curve.GetEndPoint(0), G[0].Curve.GetEndPoint(1), DistanceFromBubble);
                                XYZ Paa = Helper.GetPointOnLine(G[0].Curve.GetEndPoint(1), G[0].Curve.GetEndPoint(0), DistanceFromBubble);

                                XYZ Pb = Helper.GetPointOnLine(G[G.Count - 1].Curve.GetEndPoint(0), G[G.Count - 1].Curve.GetEndPoint(1), DistanceFromBubble);
                                XYZ Pbb = Helper.GetPointOnLine(G[G.Count - 1].Curve.GetEndPoint(1), G[G.Count - 1].Curve.GetEndPoint(0), DistanceFromBubble);

                                Line La = Line.CreateBound(Pa, Pb);
                                Line Lb = Line.CreateBound(Paa, Pbb);

                                Reference R11 = new Reference(G[0]);
                                Reference R22 = new Reference(G[G.Count - 1]);
                                ReferenceArray Arr1 = new ReferenceArray();
                                Arr1.Append(R11);
                                Arr1.Append(R22);
                                Doc.Create.NewDimension(VV, La, Arr1);
                                Doc.Create.NewDimension(VV, Lb, Arr1);
                            }

                        }
                        trans.Commit();

                    }

                }

                #endregion


            }
        }
        #endregion
    }
}
