using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FamilyPlacer
{
    internal class NonHostedEventHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {

            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;
            
            var dwgdata = Command.GetTuples_Points_Blocknames(DB.SelectedCadLink, doc);
            Transaction tr = new Transaction(doc, "Placing Families");
            tr.Start();

            #region Retrieve all Walls 
            //make family instance on ceiling as host


            foreach (var tuple in dwgdata)
            {
                if (tuple.Item1 == DB.SelectedBlock)
                {

                    var familysymbol = Command.GetFamilySymbole(doc, DB.SelectedType);
                    familysymbol.Activate();
                    try
                    {
                        var familyinstance = doc.Create.NewFamilyInstance(tuple.Item2, familysymbol, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);


                    }
                    catch (Exception)
                    {

                    }


                }

            }
            #endregion

            tr.Commit();
        }

        public string GetName()
        {
            return "ok";
        }
    }
}

