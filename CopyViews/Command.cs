using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CopyViews.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CopyViews
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public static UIDocument UiDoc;
        public static Document Doc;
        public static ObservableCollection<View> SelectedViews;
        public static ObservableCollection<RevitViews> CopiedViews;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UiDoc = commandData.Application.ActiveUIDocument;
            Doc = UiDoc.Document;

            FilteredElementCollector collector = new FilteredElementCollector(Doc);
            List<Element> levelCollection = collector.OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().ToElements().ToList();

            List<ElementId> selectedElementIds = UiDoc.Selection.GetElementIds().ToList();

            // Filter the selected elements to only include sheets
            SelectedViews = new ObservableCollection<View>();
            if (selectedElementIds != null && selectedElementIds.Count > 0)
            {
                foreach (ElementId elementId in selectedElementIds)
                {
                    Element element = Doc.GetElement(elementId);

                    if (element is View view)
                    {
                        SelectedViews.Add(view);

                    }
                }
                if (SelectedViews != null && SelectedViews.Count > 0)
                {
                    CopiedViews = new ObservableCollection<RevitViews>();
                    foreach (View view in SelectedViews)
                    {
                        if (view.ViewType == ViewType.FloorPlan || view.ViewType == ViewType.CeilingPlan || view.ViewType == ViewType.EngineeringPlan || view.ViewType == ViewType.AreaPlan)
                        {
                            string AssosiatedLevelName = view.get_Parameter(BuiltInParameter.PLAN_VIEW_LEVEL).AsString();
                            ElementId Assosiated_LevelId = levelCollection.Where(e => e.Name == AssosiatedLevelName).FirstOrDefault().Id;
                            RevitViews newView = new RevitViews(Doc, view.Name, Assosiated_LevelId, view.ViewType);
                            CopiedViews.Add(newView);
                        }

                    }

                    MessageBox.Show($"{CopiedViews.Count} Views have been copied.", "First Option", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    MessageBox.Show("You haven't selected any views, Please Select Views to Copy", "First Option", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                }
            }
            else
            {
                MessageBox.Show("You haven't selected any views, Please Select Views to Copy", "First Option", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }



            return Result.Succeeded;
        }

    }
}
