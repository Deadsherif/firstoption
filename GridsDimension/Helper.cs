using Autodesk.Revit.DB;
using GridsDimension.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridsDimension
{
    internal class Helper
    {
       
        public static void GroupLinesBySlope(List<FO_Grids> lines)
        {
            int currentGroupId = 1;

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].GroupId == 0 && lines[i].Slope >= 360 || (lines[i].GroupId == 0 && lines[i].Slope <= -360))
                {
                    lines[i].GroupId = currentGroupId;
                }

            }


            currentGroupId++;

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].GroupId == 0)
                {
                    lines[i].GroupId = currentGroupId;

                    for (int j = i + 1; j < lines.Count; j++)
                    {
                        if (lines[j].GroupId == 0 && Math.Abs(lines[j].Slope - lines[i].Slope) < 0.0001)
                        {
                            lines[j].GroupId = currentGroupId;
                        }
                    }


                    currentGroupId++;
                }
            }
        }
        public static int GetNumberOFGroups(List<FO_Grids> OrderedList)
        {
            int temp = OrderedList[0].GroupId;
            int count = 0;
            for (int i = 0; i < OrderedList.Count; i++)
            {
                foreach (FO_Grids g in OrderedList)
                {
                    if (g.GroupId == temp)
                    {
                        count++;
                        break;
                    }
                }
                temp++;
            }
            return count;
        }

        public static XYZ GetPointOnLine(XYZ startPoint, XYZ endPoint, double distanceFromStart)
        {
            double length = Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2));
            double ratio = distanceFromStart / length;

            double newX = startPoint.X + (endPoint.X - startPoint.X) * ratio;
            double newY = startPoint.Y + (endPoint.Y - startPoint.Y) * ratio;

            return new XYZ(newX, newY, startPoint.Z);
        }


        public static bool AreLinesPerpendicular(double slope1, double slope2)
        {
            // Check if either slope is vertical (infinite)
            if ((Math.Abs(slope1) > 1000000 && Math.Abs(slope2) < 0.000001) ||
                (Math.Abs(slope1) < 0.000001 && Math.Abs(slope2) > 1000000))
            {
                return true; // One line is vertical and the other is horizontal (perpendicular)
            }

            // Check if the product of slopes is equal to -1 (within a small tolerance)
            return Math.Abs(slope1 * slope2 + 1.0) < 0.00001;

        }
        public static bool AreLinesParallel(double slope1, double slope2)
        {
            if (Math.Abs(slope1) > 1000000 && Math.Abs(slope2) > 1000000)
            {
                return true; // Both lines are vertical and parallel
            }

            // Check if both slopes are horizontal (zero)
            if (Math.Abs(slope1) < 0.00001 && Math.Abs(slope2) < 0.00001)
            {
                return true; // Both lines are horizontal and parallel
            }

            // Check if the slopes are equal (within a small tolerance)
            return Math.Abs(slope1 - slope2) < 0.00001;
        }

        public static Line MoveLine(Line originalLine, XYZ translationVector)
        {
            // Get the original start and end points of the line
            XYZ startPoint = originalLine.GetEndPoint(0);
            XYZ endPoint = originalLine.GetEndPoint(1);

            // Translate the points by the given vector
            XYZ newStartPoint = startPoint + translationVector;
            XYZ newEndPoint = endPoint + translationVector;

            // Create a new Line with the translated points
            Line newLine = Line.CreateBound(newStartPoint, newEndPoint);

            return newLine;
        }

        public static double CalculateSlope(XYZ StartPoint, XYZ EndPoint)
        {
            if (StartPoint.X == EndPoint.X)
            {
                return 1000000;
            }

            return (EndPoint.Y - StartPoint.Y) / (EndPoint.X - StartPoint.X);
        }

        public static Line GetTransitionedLine(Line Line, double TransiotionValue)
        {
            XYZ P1 = Line.GetEndPoint(0);
            XYZ P2 = Line.GetEndPoint(1);
            XYZ TransVector = null;

            double Slope = CalculateSlope(P1, P2);
            if (Math.Abs(Slope) > 100)
            {
                TransVector = new XYZ(-TransiotionValue, 0, 0);
            }

            else if (Math.Abs(Slope) < 0.0001)
            {
                TransVector = new XYZ(0, TransiotionValue, 0);
            }

            else
            {
                double Alpha = Math.Atan(Slope);
                TransVector = new XYZ(-TransiotionValue * Math.Sin(Alpha), TransiotionValue * Math.Cos(Alpha), 0);
            }


            Line L = MoveLine(Line, TransVector);
            return L;

        }

        public static Grid GetPerfectGrid_CircularColumn(XYZ CenterPoint, List<Grid> grids)
        {
            Grid PerfectGrid = null;
            double BestSpacing = double.MaxValue;
            foreach (Grid grid in grids)
            {
                Line LG = grid.Curve as Line;
                double TempSpacing = GetDistanceBetweenPointAndLine(CenterPoint, LG);
                if (TempSpacing > 0.00001)
                {
                    if (TempSpacing <= BestSpacing)
                    {
                        PerfectGrid = grid;
                        BestSpacing = TempSpacing;
                    }
                }
                else
                {
                    return PerfectGrid = null;
                }
            }

            return PerfectGrid;

        }
        public static double GetDistanceBetweenPointAndLine(XYZ Point, Line L)
        {
            XYZ P1 = L.GetEndPoint(0);
            XYZ P2 = L.GetEndPoint(1);
            double Result = default;

            double Slope = CalculateSlope(P1, P2);

            if (Slope > 100)
            {
                Result = Math.Abs(Point.X - P1.X);
            }
            else if (Math.Abs(Slope) < 0.00001)
            {
                if (Point.Y < 0 && P1.Y < 0)
                {
                    Result = Math.Abs(-1 * Point.Y - -1 * P1.Y);
                }
                else
                {
                    Result = Math.Abs(Point.Y - P1.Y);
                }
            }
            else
            {
                double A = Slope;
                double B = -1;
                double C = -1 * Slope * P1.X + P1.Y;

                Result = (Math.Abs(A * Point.X + B * Point.Y + C)) / (Math.Sqrt(A * A + B * B));

            }


            return Result;




        }
        public static Solid GetColumnSolid(Element Column, Options OP)
        {
            GeometryElement columnGeometry = Column.get_Geometry(OP);
            Solid solid = default;

            try
            {
                foreach (GeometryObject geomObj in columnGeometry)
                {
                    GeometryInstance GeoIns = geomObj as GeometryInstance;

                    var GeoInstance = GeoIns.GetSymbolGeometry();
                    foreach (var item in GeoInstance)
                    {
                        if ((item as Solid).Volume != 0)
                        {
                            solid = item as Solid;
                            break;
                        }
                    }
                }
            }
            catch
            {
                foreach (GeometryObject geomObj in columnGeometry)
                {
                    if ((geomObj as Solid) != null && (geomObj as Solid).Volume != 0)
                    {
                        solid = geomObj as Solid;
                        break;
                    }

                }
            }

            return solid;
        }

   
    }
}
