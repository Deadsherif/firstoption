using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SetElementWorkSet
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument activeUiDocument = commandData.Application.ActiveUIDocument;
            Document document = activeUiDocument.Document;
            ICollection<ElementId> elementIds = activeUiDocument.Selection.GetElementIds();
            if (elementIds != null && elementIds.Count != 0)
            {
                var allWorkSets = document.GetWorksetTable();
              
                int integerValue = document.GetWorksetTable().GetActiveWorksetId()?.IntegerValue??-1;
                if (integerValue != -1)
                {
                    Transaction transaction = new Transaction(document, "Change worksets of selected elements");
                    transaction.Start();
                    foreach (ElementId elementId in elementIds)
                    {
                        document.GetElement(elementId)
                            .get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM)?
                            .Set(integerValue);
                    }
                    transaction.Commit();
                    MessageBox.Show("Completed!", "First Option", MessageBoxButton.OK, MessageBoxImage.Information);

                    return Result.Succeeded; 
                }
                else
                {
                    MessageBox.Show("No Active Worksets found!", "First Option", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return Result.Succeeded;

                }
            }
            else
            {
                MessageBox.Show("No Elements Selected!", "First Option", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            return Result.Failed;
            }
        }
    }
}
