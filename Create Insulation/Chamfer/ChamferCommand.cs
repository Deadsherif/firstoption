using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Chamfer
{
    [Transaction(TransactionMode.Manual)]
    public class ChamferCommand : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // ISSUE: object of a compiler-generated type is created
            // ISSUE: variable of a compiler-generated type
            ChamferCommand.a a = new ChamferCommand.a();
            UIDocument activeUiDocument = commandData.Application.ActiveUIDocument;
            // ISSUE: reference to a compiler-generated field
            a.a = commandData.Application.ActiveUIDocument.Document;
            // ISSUE: reference to a compiler-generated field
            Application application = a.a.Application;
            try
            {
                // ISSUE: reference to a compiler-generated method
                CurveElement[] array = activeUiDocument.Selection.GetElementIds().Select<ElementId, Element>(new Func<ElementId, Element>(a.b)).Where<Element>((Func<Element, bool>)(A_0 => A_0 is CurveElement)).Cast<CurveElement>().ToArray<CurveElement>();
                if (array.Length < 2)
                    throw new Exception("Select at least two lines.");
                double result;
                if (!double.TryParse(global::a.d().Value as string, out result))
                    throw new Exception("Size must be number.");
                // ISSUE: reference to a compiler-generated field
                DisplayUnitType displayUnits = a.a.GetUnits().GetFormatOptions((UnitType)0).DisplayUnits;
                double internalUnits = UnitUtils.ConvertToInternalUnits(result, displayUnits);
                SizeType A_3 = (SizeType)Enum.Parse(typeof(SizeType), ((RibbonItem)global::a.c().Current).Name);
                // ISSUE: reference to a compiler-generated field
                using (Transaction transaction = new Transaction(a.a, "Create Chamfer"))
                {
                    transaction.Start();
                    List<ElementId> second = c.a(array, b.a, internalUnits, A_3);
                    activeUiDocument.Selection.SetElementIds((ICollection<ElementId>)((IEnumerable<CurveElement>)array).Where<CurveElement>((Func<CurveElement, bool>)(A_0 => ((Element)A_0).IsValidObject)).Select<CurveElement, ElementId>((Func<CurveElement, ElementId>)(A_0 => ((Element)A_0).Id)).Concat<ElementId>((IEnumerable<ElementId>)second).ToArray<ElementId>());
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }
            return (Result)0;
        }
    }
}
