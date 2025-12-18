using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelAlign
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument activeUiDocument = commandData.Application.ActiveUIDocument;
            Document document = activeUiDocument.Document;
            List<Element> elementList = new List<Element>();
            Reference reference = activeUiDocument.Selection.PickObject(ObjectType.Element);
            Element element1 = document.GetElement(reference);
            foreach (Reference pickObject in (IEnumerable<Reference>)activeUiDocument.Selection.PickObjects(ObjectType.Element))
            {
                Element element2 = document.GetElement(pickObject.ElementId);
                elementList.Add(element2);
            }
            Transaction transaction = new Transaction(document, "Rukia");
            transaction.Start();
            ElementId elementId = element1.LookupParameter("Reference Level").AsElementId();
            double num = element1.LookupParameter("Middle Elevation").AsDouble();
            foreach (Element element3 in elementList)
            {
                element3.LookupParameter("Reference Level").Set(elementId);
                element3.LookupParameter("Middle Elevation").Set(num);
            }
            transaction.Commit();
            return (Result)0;
        }

       
    }
}
