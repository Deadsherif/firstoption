using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using GridsDimension.MVVM.View;
using GridsDimension.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridsDimension
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public static ExternalEvent exEvent;
        public static ObservableCollection<View> OrderedViews;
        public static GridsViewModel VMD;
        public static Grids ViewFrm;
        public static UIDocument UiDoc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {


            UiDoc = commandData.Application.ActiveUIDocument;
            Document Doc = UiDoc.Document;

            var V = new FilteredElementCollector(Doc).OfClass(typeof(View)).OfType<View>();

            ObservableCollection<View> Views = new ObservableCollection<View>();

            foreach (View view in V)
            {
                if ((view.ViewType == ViewType.FloorPlan && view.GenLevel != null) ||
                    (view.ViewType == ViewType.CeilingPlan && view.GenLevel != null) ||
                    (view.ViewType == ViewType.EngineeringPlan && view.GenLevel != null)
                    /*view.ViewType == ViewType.AreaPlan*/)
                { Views.Add(view); }

            }

            OrderedViews = new ObservableCollection<View>();

            var v = Views.OrderBy(view => view.ViewType).ToList();
            foreach (var e in v)
            {
                OrderedViews.Add(e);
            }

            VMD = new GridsViewModel(OrderedViews);
            ViewFrm = new Grids();

            ViewFrm.ShowDialog();

            return Result.Succeeded;

        }
    }
}
