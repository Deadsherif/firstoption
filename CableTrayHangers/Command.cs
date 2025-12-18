using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CableTrayHangers.MVVM.View;
using Autodesk.Revit.DB.Electrical;
using Firebase.Auth.Wpf.Sample;
using System.Windows.Controls;

namespace CableTrayHangers
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public static UIDocument UiDoc;
        public static Document Doc;
        public static List<ElementId> reffEle;
        public static List<Element> FinalPipes;
        //public static PipeSupportView frm;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UiDoc = commandData.Application.ActiveUIDocument;
            Doc = commandData.Application.ActiveUIDocument.Document;

            CreatHangersExteranlEventHandler creatHangersExteranlEventHandler = new CreatHangersExteranlEventHandler();
            ExternalEvent externalEvent = ExternalEvent.Create(creatHangersExteranlEventHandler);

            reffEle = new List<ElementId>();
            FinalPipes = new List<Element>();

            reffEle = UiDoc.Selection.GetElementIds().ToList();

            if (reffEle.Count > 0)
            {
                foreach (ElementId ele in reffEle)
                {
                    Element e = Doc.GetElement(ele);
                    if (e is CableTray)
                    {
                        Curve crv = (e.Location as LocationCurve).Curve;
                        if (crv is Line)
                        {
                            FinalPipes.Add(e);
                        }
                    }
                }

                if (FinalPipes != null && FinalPipes.Count > 0)
                {
                    using (Transaction trans = new Transaction(Doc, $"Create Hanger"))
                    {
                        trans.Start();
                        MainWindow frm = MainWindow.CreateInstance(creatHangersExteranlEventHandler, externalEvent);
                        frm.Show();
                        trans.Commit();
                    }
                }
                else
                {
                    MessageBox.Show("No Linear pipes where found in the selected pipes");
                }
            }
            else
            {
                MessageBox.Show("No Elements Selected");

            }
            return Result.Succeeded;
        }





    }
}
