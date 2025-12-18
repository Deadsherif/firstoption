using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelHealthChecker.ExternalEventHandlers
{
    public class IsolateLinkEventHandeler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document Doc = uidoc.Document;

            Element Link = Command.VM.SelectedRevitLink;

            int LinkId = Link.Id.IntegerValue;

            Element e = Doc.GetElement(new ElementId(LinkId));



            //Reference reff = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

            //Element ele = Doc.GetElement(reff);

            Helper.IsolateElementsIn3DView(uidoc, e as Element);



        }

        public string GetName()
        {
            return "a";
        }
    }
}
