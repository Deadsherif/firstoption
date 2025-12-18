using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CableTrayHangers.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CableTrayHangers.MVVM.ViewModel;

namespace CableTrayHangers
{
    public class CreatHangersExteranlEventHandler : IExternalEventHandler
    {
        public TraySupportViewModel VM { get;  set; }

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
                    Transaction tr = new Transaction(doc, "Create Hangers");
                    tr.Start();
                    foreach (Element ele in Element)
                    {

                        double depth = (ele as CableTray).get_Parameter(BuiltInParameter.RBS_CABLETRAY_HEIGHT_PARAM)?.AsDouble() ?? 200;
                        double width = (ele as CableTray).get_Parameter(BuiltInParameter.RBS_CABLETRAY_WIDTH_PARAM)?.AsDouble() ?? 200;

                        FO_TrayLine trayLine = new FO_TrayLine((ele.Location as LocationCurve).Curve as Line, spacing, width, depth);


                        familytype.Activate();
                        foreach (XYZ point in trayLine.SupportsPointsLocation)
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
                                Support.LookupParameter(VM.WidthParaName)?.Set(trayLine.Width - UnitUtils.ConvertToInternalUnits(100, UnitTypeId.Millimeters) + 2 * VM.WidthTolarance * 0.00328084);
                                Support.LookupParameter(VM.HeightParaName)?.Set(trayLine.Depth + 2 * VM.Tolarance * 0.00328084);
                            }
                            catch
                            {
                            }

                        }


                    }
                    tr.Commit();
                }
                catch (Exception ex)
                {

                    TaskDialog.Show("Error",ex.Message);
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
