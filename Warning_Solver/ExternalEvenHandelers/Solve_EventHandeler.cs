using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warning_Solver.MVVM.Model;

namespace Warning_Solver.ExternalEvenHandelers
{
    public class Solve_EventHandeler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            UIDocument uidoc = app.ActiveUIDocument;
            Document doc = uidoc.Document;

            switch (Database.selecteditem as string)
            {

                #region 01 Line in Sketch is slightly off axis and may cause inaccuracies.
                case "Line in Sketch is slightly off axis and may cause inaccuracies.":
                    var targetedwarnings = Database.Warnings.Where(x => x.GetDescriptionText() == "Line in Sketch is slightly off axis and may cause inaccuracies.");
                    Transaction tr = new Transaction(doc, "line Correction");
                    tr.Start();
                    Command.ProcessWin.SetProgressBarLimits(targetedwarnings.ToList<FailureMessage>().Count);


                    foreach (var warning in targetedwarnings)
                    {

                        Command.ProcessWin.UpdateProgressBar("shjfbdsfjkhbgf");
                        foreach (var elementId in warning.GetFailingElements())
                        {

                            var ele = doc.GetElement(elementId);
                            var modelline = ele as ModelLine;
                            if (ele != null && modelline!=null)
                            {
                                var locationcuvre = ele.Location as LocationCurve;
                                var line = locationcuvre.Curve as Line;
                                var dir = line.Direction.Normalize();
                                var vx = Math.Round(dir.X);
                                var vy = Math.Round(dir.Y);
                                var point1 = line.GetEndPoint(0);
                                var point2 = line.GetEndPoint(1);
                                var pr1 = line.GetEndParameter(0);
                                var pr2 = line.GetEndParameter(1);

                                var newline = Line.CreateUnbound(point1, new XYZ(vx, vy, 0));
                                newline.MakeBound(pr1, pr2);
                                locationcuvre.Curve = newline;
                            }
                        }
                    }
                    Command.ProcessWin.SetWaitMsg();
                    tr.Commit();

                    Command.ProcessWin.UpdateProgressBar_commit();
                    Command.ProcessWin.SetCompleteMsg();
                    break;
                #endregion

                #region 02 Wall in Sketch is slightly off axis and may cause inaccuracies.
                case "Wall is slightly off axis and may cause inaccuracies.":
                    var walltargetedwarnings = Database.Warnings.Where(x => x.GetDescriptionText() == "Wall is slightly off axis and may cause inaccuracies.");
                    Transaction wtr = new Transaction(doc, "wall Correction");
                    wtr.Start();
                    foreach (var warning in walltargetedwarnings)
                    {
                        foreach (var elementId in warning.GetFailingElements())
                        {
                            var ele = doc.GetElement(elementId);
                            var modelline = ele as ModelLine;
                            if (ele != null)
                            {
                                var locationcuvre = ele.Location as LocationCurve;
                                var line = locationcuvre.Curve as Line;
                                var dir = line.Direction.Normalize();
                                var vx = Math.Round(dir.X);
                                var vy = Math.Round(dir.Y);
                                var point1 = line.GetEndPoint(0);
                                var point2 = line.GetEndPoint(1);
                                var pr1 = line.GetEndParameter(0);
                                var pr2 = line.GetEndParameter(1);

                                var newline = Line.CreateUnbound(point1, new XYZ(vx, vy, 0));
                                newline.MakeBound(pr1, pr2);
                                locationcuvre.Curve = newline;
                            }
                        }
                    }
                    wtr.Commit();
                    break;
                #endregion
                #region 03 Ref Plane is slightly off axis and may cause inaccuracies.
                case "Ref Plane is slightly off axis and may cause inaccuracies.":
                    var Reftargetedwarnings = Database.Warnings.Where(x => x.GetDescriptionText() == "Ref Plane is slightly off axis and may cause inaccuracies.");
                    Transaction Rtr = new Transaction(doc, "wall Correction");
                    Rtr.Start();
                    foreach (var warning in Reftargetedwarnings)
                    {
                        foreach (var elementId in warning.GetFailingElements())
                        {
                            var ele = doc.GetElement(elementId) as ReferencePlane;
                            if (ele != null)
                            {
                                var dir1 = ele.Direction;
                                var vx = Math.Round(dir1.X);
                                var vy = Math.Round(dir1.Y);
                                ele.Direction = new XYZ(vx, vy, 0);
                            }
                            else
                            {
                                var dp = doc.GetElement(elementId) as Element;
                                // still have issue here that i cant set the correct location to the datum plan 
                            }
                        }
                    }
                    Rtr.Commit();
                    break;
                #endregion
                #region 04 There are identical instances in the same place. This will result in double counting in schedules...
                case "There are identical instances in the same place. This will result in double counting in schedules.":
                    var identicalelemetnswarning = Database.Warnings.Where(x => x.GetDescriptionText() == "There are identical instances in the same place. This will result in double counting in schedules.");
                    foreach (var warning in identicalelemetnswarning)
                    {
                        Transaction ddet = new Transaction(doc, "Delete Duplicated Elemtns.");
                        ddet.Start();
                        var elementids = warning.GetFailingElements().ToList();
                        elementids.RemoveAt(0);
                        foreach (var eleid in elementids)
                        {
                            var ele = doc.GetElement(eleid);
                            if (ele != null)
                            {
                                ele.Pinned = false;
                            }
                        }
                        try
                        {

                            doc.Delete(elementids);
                        }
                        catch (Exception)
                        {


                        }

                        ddet.Commit();
                    }

                    break;
                #endregion
                #region 05 Highlighted elements are joined but do not intersect.
                case "Highlighted elements are joined but do not intersect.":
                    var notintersectedelemetnswarning = Database.Warnings.Where(x => x.GetDescriptionText() == "Highlighted elements are joined but do not intersect.");
                    foreach (var warning in notintersectedelemetnswarning)
                    {
                        Transaction intr = new Transaction(doc, "Delete Duplicated Elemtns.");
                        intr.Start();
                        var elementids = warning.GetFailingElements().ToList();
                        JoinGeometryUtils.UnjoinGeometry(doc, doc.GetElement(elementids.ElementAt(0)), doc.GetElement(elementids.ElementAt(1)));
                        intr.Commit();
                    }
                    break;
                #endregion
                #region 06 Highlighted walls overlap. One of them may be ignored when Revit finds room boundaries. 
                case "Highlighted walls overlap. One of them may be ignored when Revit finds room boundaries. Use Cut Geometry to embed one wall within the other.":
                    // in this case i want to handle the task dialog message that appears when running code to solve the warning
                    var overlabwarning = Database.Warnings.Where(x => x.GetDescriptionText() == "Highlighted walls overlap. One of them may be ignored when Revit finds room boundaries. Use Cut Geometry to embed one wall within the other.");
                    foreach (var warning in overlabwarning)
                    {
                        var elementId = warning.GetFailingElements().FirstOrDefault();
                        var ele = doc.GetElement(elementId) as Wall;
                        Transaction otr = new Transaction(doc, "overlab sole");
                        otr.Start();
                        ele.CrossSection = WallCrossSection.SingleSlanted;
                        otr.Commit();
                    }
                    break;
                #endregion
                #region 07 Rectangular opening doesn't cut its host. 
                case "Rectangular opening doesn't cut its host.":


                    var rectwarings = Database.Warnings.Where(x => x.GetDescriptionText() == "Rectangular opening doesn't cut its host.");
                    foreach (var warning in rectwarings)
                    {
                        var elementId = warning.GetFailingElements().FirstOrDefault();

                        Transaction otr = new Transaction(doc, "overlab sole");
                        otr.Start();
                        doc.Delete(elementId);
                        otr.Commit();
                    }




                    break;
                #endregion
                #region 08 Elements have duplicate \"Number\" values.
                case "Elements have duplicate \"Number\" values.":

                    var duplicatednumwarnings = Database.Warnings.Where(x => x.GetDescriptionText().StartsWith("Elements have duplicate \"Number\" values."));
                    foreach (var warning in duplicatednumwarnings)
                    {

                        var elementIds = warning.GetFailingElements();
                        var zeroarerooms = elementIds.Where(x => (doc.GetElement(x) as Room).Area == 0);
                        var count = zeroarerooms.Count();
                        Transaction roomnumtr = new Transaction(doc, "room number error resolve");
                        roomnumtr.Start();
                        if (count == 0)
                        {
                            var room = doc.GetElement(elementIds.FirstOrDefault()) as Room;
                            room.Number = room.Number + "'";
                        }
                        else
                        {
                            foreach (var room in zeroarerooms)
                            {
                                doc.Delete(room);
                            }
                        }
                        roomnumtr.Commit();
                    }
                    break;
                #endregion
                #region 09 Elements have duplicate \"Type Mark\" values.
                case "Elements have duplicate \"Type Mark\" values.":
                    var duplicatedtypewarnings = Database.Warnings.Where(x => x.GetDescriptionText() == "Elements have duplicate \"Type Mark\" values.");
                    foreach (var warning in duplicatedtypewarnings)
                    {
                        var elementIds = warning.GetFailingElements();
                        var zerotypesids = elementIds.Where(x => (doc.GetElement(x) as ElementType).GetDependentElements(null).Count == 0);
                        // there are alot of types hase elemetns
                        var zerotypesnumbers = zerotypesids.Count();
                        Transaction xr = new Transaction(doc, "type mark error");
                        xr.Start();
                        if (zerotypesnumbers == 0)
                        {
                            foreach (var typeid in elementIds)
                            {
                                var type = doc.GetElement(typeid);
                                type.get_Parameter(BuiltInParameter.WINDOW_TYPE_ID).Set("");
                            }
                        }
                        else
                        {
                            var anytype = doc.GetElement(elementIds.ElementAt(0)).get_Parameter(BuiltInParameter.WINDOW_TYPE_ID).Set("");
                        }
                        xr.Commit();
                    }
                    break;
                #endregion
                #region 10 Elements have duplicate \"Mark\" values.
                case "Elements have duplicate \"Mark\" values.":
                    var duplicatedmarkwarnings = Database.Warnings.Where(x => x.GetDescriptionText() == "Elements have duplicate \"Mark\" values.");
                    if (Database.ischecked)
                    {
                        Transaction transaction = new Transaction(doc, "resolve mark error");
                        transaction.Start();
                        foreach (var warning in duplicatedmarkwarnings)
                        {
                            var elementIds = warning.GetFailingElements();
                            foreach (var elementId in elementIds)
                            {
                                var ele = doc.GetElement(elementId);
                                var markparameter = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);
                                try
                                {
                                    var ccc = markparameter.Set("");
                                }
                                catch (Exception)
                                {


                                }


                            }
                        }
                        transaction.Commit();
                    }

                    break;
                #endregion
                #region 11 Insert conflicts with joined Wall
                case "Insert conflicts with joined Wall.":
                    var ConglictedWalls = Database.Warnings.Where(x => x.GetDescriptionText() == "Insert conflicts with joined Wall.");
                    if (Database.ischecked)
                    {
                        Transaction transaction = new Transaction(doc, "resolve conflicts with joined Wall error");
                        transaction.Start();
                        foreach (var warning in ConglictedWalls)
                        {
                            var elementIds = warning.GetFailingElements();
                            foreach (var elementId in elementIds)
                            {
                                var ele = doc.GetElement(elementId);
                                var markparameter = ele.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);
                                try
                                {
                                    var ccc = markparameter.Set("");
                                }
                                catch (Exception)
                                {


                                }
                            }
                        }
                        transaction.Commit();
                    }

                    break;



                #endregion
                #region 12 Floors OverLap
                case "Highlighted floors overlap.":
                    // in this case i want to handle the task dialog message that appears when running code to solve the warning
                    var floorOverLapWarning = Database.Warnings.Where(x => x.GetDescriptionText() == "Highlighted floors overlap.");
                    foreach (var warning in floorOverLapWarning)
                    {
                        var elementId = warning.GetFailingElements().FirstOrDefault();
                        var ele = doc.GetElement(elementId) as Floor;
                        Transaction otr = new Transaction(doc, "overlab solved");
                        otr.Start();
                        ele.Pinned = false;
                        doc.Delete(ele.Id);
                        otr.Commit();
                    }
                    break;



                #endregion
                #region 13 Grid is slightly off axis and may cause inaccuracies.
                case "Grid is slightly off axis and may cause inaccuracies.":
                    var gridWarnings = Database.Warnings.Where(x => x.GetDescriptionText() == "Grid is slightly off axis and may cause inaccuracies.");
                    Transaction ts = new Transaction(doc, "line Correction");
                    ts.Start();
                    Command.ProcessWin.SetProgressBarLimits(gridWarnings.ToList<FailureMessage>().Count);


                    foreach (var warning in gridWarnings)
                    {

                        Command.ProcessWin.UpdateProgressBar("shjfbdsfjkhbgf");
                        foreach (var elementId in warning.GetFailingElements())
                        {

                            var ele = doc.GetElement(elementId) as Grid;
                           
                            if (ele != null )
                            {
                                AlignOffAxisGrid(ele);
                            }
                        }
                    }
                    Command.ProcessWin.SetWaitMsg();
                    ts.Commit();

                    Command.ProcessWin.UpdateProgressBar_commit();
                    Command.ProcessWin.SetCompleteMsg();
                    break;
                    #endregion

            }

            Command.ProcessWin.progressBar.Value = 0;
            Command.ProcessWin.Focus();
            var warnings = doc.GetWarnings();
            Database.Warnings = warnings;
 
            Warning_Solver.Command.ProcessWin.setUIData();
        }
        /// <summary>
        /// Align the given grid horizontally or vertically if it is very slightly off axis.
        /// </summary>
        void AlignOffAxisGrid(Grid grid)
        {
            //Grid grid = doc.GetElement(
            //  sel.GetElementIds().FirstOrDefault()) as Grid;
            Document doc = grid.Document;
            XYZ direction = grid.Curve
                .GetEndPoint(1)
                .Subtract(grid.Curve.GetEndPoint(0))
                .Normalize();

            double distance2hor = direction.DotProduct(XYZ.BasisY);
            double distance2vert = direction.DotProduct(XYZ.BasisX);
            double angle = 0;
            // Maybe use another criterium then <0.0001
            double max_distance = 0.0001;

            if (Math.Abs(distance2hor) < max_distance)
            {
                XYZ vector = direction.X < 0
                    ? direction.Negate()
                    : direction;

                angle = Math.Asin(-vector.Y);
            }

            if (Math.Abs(distance2vert) < max_distance)
            {
                XYZ vector = direction.Y < 0
                    ? direction.Negate()
                    : direction;

                angle = Math.Asin(vector.X);
            }

            if (angle.CompareTo(0) != 0)
            {
               
                    ElementTransformUtils.RotateElement(doc,
                        grid.Id,
                        Line.CreateBound(grid.Curve.GetEndPoint(0),
                        grid.Curve.GetEndPoint(0).Add(XYZ.BasisZ)),
                        angle);
                  
                
            }
        }
        public string GetName()
        {
            return "ok";
        }

    }
}
