using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlignCenter
{
    [Transaction(TransactionMode.Manual)]

    internal class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument activeUiDocument = commandData.Application.ActiveUIDocument;
            Document document = activeUiDocument.Document;
            Reference reference1 = activeUiDocument.Selection.PickObject(ObjectType.Element);
            Line centerline = Helper.GetCenterline(document.GetElement(reference1));
            IList<Reference> referenceList = activeUiDocument.Selection.PickObjects(ObjectType.Element);
            List<Element> elementList = new List<Element>();
            foreach (Reference reference2 in (IEnumerable<Reference>)referenceList)
                elementList.Add(document.GetElement(reference2));
            Transaction transaction = new Transaction(document, "Avalanche");
            transaction.Start();
            foreach (Element pipe in elementList)
            {
                Curve curve = Helper.GetCenterline(pipe).Clone();
                curve.MakeUnbound();
                IntersectionResult intersectionResult = curve.Project(centerline.Origin);
                if (intersectionResult != null)
                {
                    XYZ xyzPoint = intersectionResult.XYZPoint;
                    XYZ xyz = -xyzPoint + centerline.Origin;
                    ElementTransformUtils.MoveElement(document, pipe.Id, xyz);
                }
            }
            transaction.Commit();
            return Result.Succeeded;
        }

      
    }
}
