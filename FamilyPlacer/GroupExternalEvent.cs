using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyPlacer
{
   internal class GroupExternalEvent : IExternalEventHandler
   {
      public void Execute(UIApplication app)
      {
         UIDocument uidoc = app.ActiveUIDocument;
         Document doc = uidoc.Document;
         var dwgdata = Command.GetTuples_Points_Blocknames(DB.SelectedCadLink, doc);
         Transaction tr = new Transaction(doc, "Placing Families");
         tr.Start();

         foreach (var tuple in dwgdata)
         {
            Transform cadTransform = Transform.Identity;
            if (DB.SelectedCadLink is ImportInstance importInstance)
               cadTransform = importInstance.GetTransform();

            XYZ worldPoint = cadTransform.OfPoint(tuple.Item2);
            if (tuple.Item1 == DB.SelectedBlock)
            {
               doc.Create.PlaceGroup(worldPoint, DB.SelectedGroup);
            }
         }

         tr.Commit();
      }

      public string GetName()
      {
         return "ok";
      }
   }
}
