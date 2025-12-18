#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using COT.MVVM.Model;
#endregion

namespace COT.ExternalEventHandelers
{
    [Transaction(TransactionMode.Manual)]
    internal class RunMultibleTraysHandeler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            COT_Controller controller = new COT_Controller();
            Transaction tr = new Transaction(doc);
            if (ApplicationStatic_DB.ConduitsData == null || ApplicationStatic_DB.ConduitsData.Count == 0)
            {
                ApplicationStatic_DB.MainForm.Show();
                return;
            }

            foreach (Element t in ApplicationStatic_DB.conduitTypes)
            {
                if (t.Name == ApplicationStatic_DB.conduitTypeName)
                {
                    ApplicationStatic_DB.conduitType = t;
                    break;
                }
            }
            if (ApplicationStatic_DB.conduitType == null)
            {
                ApplicationStatic_DB.MainForm.Show();
                return;
            }


            tr.Start("COT");
            double lastOffset = 0;
            double D_Current = 0;
            double D_Last = 0;
            for (int i = 0; i < ApplicationStatic_DB.ConduitsData.Count; i++)
            {
                D_Current = ApplicationStatic_DB.ConduitsData[i];
                double _lastOffset = controller.DrawConduits(doc, ApplicationStatic_DB.SortedTrays, ApplicationStatic_DB.conduitType.Id, ApplicationStatic_DB.SortedTrays[0].curveHost.ReferenceLevel.Id, ApplicationStatic_DB.TrayThickness, D_Current, D_Last, lastOffset, ApplicationStatic_DB.firstTraySpacingCalculation, ApplicationStatic_DB.shiftToTrayBottom, ApplicationStatic_DB.withFittings, ApplicationStatic_DB.justifyFittings);
                D_Last = ApplicationStatic_DB.ConduitsData[i];
                lastOffset = _lastOffset;
            }
            tr.Commit();
            ApplicationStatic_DB.MainForm.Show();
        }

        public string GetName()
        {
            return "RunMultibleTraysHandeler";
        }
    }
}
