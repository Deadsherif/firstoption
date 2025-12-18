
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using FamilyPlacer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Floor = Autodesk.Revit.DB.Floor;
using Wall = Autodesk.Revit.DB.Wall;


namespace Firebase.Auth.Wpf.Sample
{
    public partial class MainWindow : Window
    {
  
 
        public static MainWindow instance { get; set; }
        public bool IsClosed { get; private set; }
        private Document _doc;
        private ExternalEvent _ev;
        private ExternalEvent _ev2;
        private ExternalEvent _ev3;
        private ExternalEvent _ev4;
        private ExternalEvent _ev5;

        public MainWindow(Document doc, ExternalEvent ev, ExternalEvent ev2, ExternalEvent ev3, ExternalEvent ev4, ExternalEvent ev5)
        {
            InitializeComponent();
            _doc = doc;
            _ev = ev;
                _ev2 = ev2;
            _ev3 = ev3;
            _ev4 = ev4;
                _ev5 = ev5;

         //Hide();
         //CentralView.CreateInstance(_mainClass).Show() ;
         var view = new FamilyPlacer.UI(_doc, _ev, _ev2, _ev3, _ev4, _ev5);
         if (view.wallcheck.IsChecked == true)
         {
            // return walls 
            FilteredElementCollector fec0 = new FilteredElementCollector(_doc);
            var walls = fec0.OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().Cast<Wall>().Where(x => _doc.GetElement(x.LevelId).Name == DB.SelectedWallLevel.Name && x.WallType.Name == DB.SelectedWallType.Name).ToList();
            DB.walls = walls;

         }

       

            FilteredElementCollector fec = new FilteredElementCollector(_doc);
            var ceilings = fec.OfCategory(BuiltInCategory.OST_Ceilings).WhereElementIsNotElementType().Cast<Ceiling>().Where(x => _doc.GetElement(x.LevelId).Name == DB.SelectedCeilingLevel.Name).ToList();
            DB.Ceilings = ceilings;

            FilteredElementCollector fec3 = new FilteredElementCollector(_doc);
            var ceilingswithFloor = fec3.OfCategory(BuiltInCategory.OST_Ceilings).WhereElementIsNotElementType().Cast<Ceiling>().Where(y => _doc.GetElement(y.LevelId).Name == DB.SelectedFloorLevel.Name).ToList();

            /// this filter to get all ceiling close to Floor 
            var FilteredCeiling = ceilingswithFloor.Where(x => Math.Abs(x.get_Parameter(BuiltInParameter.CEILING_HEIGHTABOVELEVEL_PARAM).AsDouble()) < 1).ToList();
            DB.FilteredCeiling = FilteredCeiling;

            FilteredElementCollector fec2 = new FilteredElementCollector(_doc);
            List<Floor> Floors = fec2.OfCategory(BuiltInCategory.OST_Floors).WhereElementIsNotElementType().Cast<Floor>().ToList();
            DB.Floors = Floors;
         
         this.Frame.Navigate(view);
         this.Width = view.Width; this.Height = view.Height;
         this.WindowState = WindowState.Normal;

         //FirebaseUI.Instance.Client.AuthStateChanged += this.AuthStateChanged;

      }

    
        public static MainWindow CreateInstance(Document _doc, ExternalEvent ev, ExternalEvent ev2, ExternalEvent ev3, ExternalEvent ev4, ExternalEvent ev5)
        {
            if (instance == null || instance.IsClosed)
                instance = new MainWindow(_doc, ev, ev2, ev3, ev4, ev5);
            else
                instance.Activate();

            return instance;
        }
        protected override void OnClosed(EventArgs e) => IsClosed = true;

    }
}
