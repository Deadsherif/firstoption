using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Beams_RCD.MVVM.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beams_RCD
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public static UIDocument UiDoc;
        public static Document Doc;
        public static BeamsView BV;
        public static ObservableCollection<Element> FinalListOfDimensionStyles;
        public static ObservableCollection<Element> FinalListOfSecDimensionStyles;
        public static ObservableCollection<FamilySymbol> AllDetailComponents;
        public static ObservableCollection<Element> AllSpotElevations;
        public static ObservableCollection<Family> AllFramingTags;
        public static ObservableCollection<Family> AllFlooringTags;



        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UiDoc = commandData.Application.ActiveUIDocument;
            Doc = commandData.Application.ActiveUIDocument.Document;


            FinalListOfDimensionStyles = Helper.GetPlanDimensionStyle(Doc);
            FinalListOfSecDimensionStyles = Helper.GetElevationDimensionStyle(Doc);

            AllFramingTags = Helper.GetFramingTags(Doc);
            AllFlooringTags = Helper.GetSlabsTags(Doc);
            AllDetailComponents = Helper.GetAllDetailItems(Doc);
            var tMP = Helper.GetAllSpotElevations(Doc).Where(x => (x as SpotDimensionType).StyleType == DimensionStyleType.SpotElevation).ToList();
            AllSpotElevations = new ObservableCollection<Element>(tMP);
            BV = new BeamsView();
            BV.ShowDialog();


            return Result.Succeeded;
        }
  

    }

}
