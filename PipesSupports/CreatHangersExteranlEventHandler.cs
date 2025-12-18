using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PipesSupports.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PipesSupports.MVVM.ViewModel;
using Autodesk.Revit.DB.Plumbing;

namespace PipesSupports
{
    public class CreatHangersExteranlEventHandler : IExternalEventHandler
    {
        public PipeSupportViewModel VM { get;  set; }

        public void Execute(UIApplication app)
        {
            if (VM.FloorId != -1)
            {
                try
                {
                    Document doc = Command.Doc;
                    List<Element> Element = Command.FinalPipes;
                    double spacing = VM.Spacing;
                    FamilySymbol familytype = VM.SelectedType;
                    foreach (Element ele in Element)
                    {
                        FO_PipeLine pipeLine = new FO_PipeLine((ele.Location as LocationCurve).Curve as Line, spacing, (ele as Pipe).get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER).AsDouble());


                        using (Transaction trans = new Transaction(doc, $"Create {familytype.Name}"))
                        {
                            trans.Start();
                            familytype.Activate();

                            foreach (XYZ point in pipeLine.SupportsPointsLocation)
                            {
                                try
                                {
                                    Element Support = doc.Create.NewFamilyInstance(point, familytype, StructuralType.NonStructural);
                                    XYZ P1 = point;
                                    XYZ P2 = new XYZ(point.X, point.Y, point.Z + 10);
                                    Line rotateLine = Line.CreateBound(P1, P2);

                                    double angle = Helper.GetAngleWithYAxis((ele.Location as LocationCurve).Curve as Line);
                                    ElementTransformUtils.RotateElement(doc, Support.Id, rotateLine, angle);
                                    Support.LookupParameter("Offset from Host")?.Set(0);
                                    Support.LookupParameter("Elevation from Level")?.Set(0);
                                    Support.LookupParameter(VM.CenterElevationParaName)?.Set(point.Z);
                                    Support.LookupParameter(VM.AnchorParaName).Set(VM.floorButtom_Z);
                                    Support.LookupParameter(VM.DiameterParaName)?.Set(pipeLine.Diameter + 2 * VM.Tolarance * 0.00328084);
                                }
                                catch
                                {
                                }

                            }
                            trans.Commit();
                        }

                    }
                }
                catch (Exception ex)
                {

                    TaskDialog.Show("Error", ex.Message);
                }
            }
            else
            {
                TaskDialog.Show("Error","Please select a valid floor or ceiling");
            }
        }

        public string GetName()
        => "Hanger Creation";
    }
}
