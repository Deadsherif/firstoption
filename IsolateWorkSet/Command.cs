using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IsolateWorkSet
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIDocument activeUiDocument = commandData.Application.ActiveUIDocument;
                Document document = activeUiDocument.Document;
                List<WorksetId> worksetIdList = new List<WorksetId>();

                List<Reference> pickedObjects = new List<Reference>();
                try
                {
                    pickedObjects = activeUiDocument.Selection.PickObjects(ObjectType.Element).ToList();
                }
                catch
                {

                }
                if (pickedObjects != null && pickedObjects.Count != 0)
                {
                    foreach (Reference pickObject in pickedObjects)
                    {
                        WorksetId worksetId = document.GetElement(pickObject).WorksetId;
                        worksetIdList.Add(worksetId);
                    }
                    using (Transaction transaction = new Transaction(document))
                    {
                        transaction.Start("Isolate Workset");
                        foreach (Workset workset in (IEnumerable<Workset>)new FilteredWorksetCollector(document).OfKind((WorksetKind)4).ToWorksets())
                            document.ActiveView.SetWorksetVisibility(((WorksetPreview)workset).Id, (WorksetVisibility)1);
                        foreach (WorksetId worksetId in worksetIdList)
                            document.ActiveView.SetWorksetVisibility(worksetId, (WorksetVisibility)0);
                        transaction.Commit();
                    }

                    MessageBox.Show("Isolation Completed", "First Option", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "First Option", MessageBoxButton.OK, MessageBoxImage.Error);
                return Result.Failed;
            }
        }
    }
}
