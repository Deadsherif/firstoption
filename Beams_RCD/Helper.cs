using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beams_RCD
{
    public class Helper
    {
        public static ObservableCollection<Family> GetFramingTags(Document Doc)
        {
            FilteredElementCollector FamilyTypesFromRevit = new FilteredElementCollector(Doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_StructuralFramingTags);

            ObservableCollection<Family> Result = new ObservableCollection<Family>();

            foreach (Element Type in FamilyTypesFromRevit)
            {
                bool has = false;
                FamilySymbol S = Type as FamilySymbol;
                foreach (Family f in Result)
                {
                    if (S.Family.Id == f.Id)
                    {
                        has = true;
                        break;

                    }
                }

                if (!has)
                {

                    Result.Add(S.Family);
                }


            }

            return Result;
        }


        public static ObservableCollection<Family> GetSlabsTags(Document Doc)
        {
            FilteredElementCollector FamilyTypesFromRevit = new FilteredElementCollector(Doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_FloorTags);

            ObservableCollection<Family> Result = new ObservableCollection<Family>();

            foreach (Element Type in FamilyTypesFromRevit)
            {
                bool has = false;
                FamilySymbol S = Type as FamilySymbol;
                foreach (Family f in Result)
                {
                    if (S.Family.Id == f.Id)
                    {
                        has = true;
                        break;

                    }
                }

                if (!has)
                {

                    Result.Add(S.Family);
                }


            }

            return Result;
        }


        public static XYZ GetLeaderEndPosition(Element _Beam, int LeftOrtRight)
        {
            Line BeamLine = (_Beam.Location as LocationCurve).Curve as Line;
            int Index;

            List<Line> ParallelBeamLine = OffsetLine(BeamLine, 2);
            if (LeftOrtRight == 0)
            {
                Index = GetParallelBeamIndex(_Beam, ParallelBeamLine[0], ParallelBeamLine[1]);
            }
            else if (LeftOrtRight == 1)
            {
                Index = GetParallelBeamIndex(_Beam, ParallelBeamLine[0], ParallelBeamLine[1]);
                if (Index == 0)
                {
                    Index = 1;
                }
                else
                {
                    Index = 0;
                }
            }
            else
            {
                Index = 0;
            }
            Line CorrectLine = ParallelBeamLine[Index];

            XYZ P1 = CorrectLine.GetEndPoint(0);
            XYZ P2 = CorrectLine.GetEndPoint(1);

            return (P1 + P2) / 2;


        }

        public static int GetParallelBeamIndex(Element Beam, Line L1, Line L2)
        {
            int Result = 0;
            int Case;
            Line BeamLine = (Beam.Location as LocationCurve).Curve as Line;
            XYZ P1 = BeamLine.GetEndPoint(0);
            XYZ P2 = BeamLine.GetEndPoint(1);
            XYZ Center = (P1 + P2) / 2;
            int Quarter = 0;

            if (Math.Abs(P1.X - P2.X) <= Math.Abs(P1.Y - P2.Y))
            {
                Case = 1;
            }
            else
            {
                Case = 2;
            }

            XYZ Center1 = (L1.GetEndPoint(0) + L1.GetEndPoint(1)) / 2;
            XYZ Center2 = (L2.GetEndPoint(0) + L2.GetEndPoint(1)) / 2;



            /////////

            if (Center.X > 0 && Center.Y > 0)
            {
                Quarter = 1;
            }
            else if (Center.X < 0 && Center.Y > 0)
            {
                Quarter = 2;
            }
            else if (Center.X < 0 && Center.Y < 0)
            {
                Quarter = 3;
            }
            else
            {
                Quarter = 4;
            }


            ///////
            if (Case == 1)
            {
                if (Quarter == 1)
                {

                    if (Center1.DistanceTo(XYZ.Zero) <= Center2.DistanceTo(XYZ.Zero))
                    {
                        Result = 0;
                    }
                    else
                    {
                        Result = 1;
                    }
                }
                else if (Quarter == 2)
                {
                    if (Center1.DistanceTo(XYZ.Zero) <= Center2.DistanceTo(XYZ.Zero))
                    {
                        Result = 1;
                    }
                    else
                    {
                        Result = 0;
                    }
                }
                else if (Quarter == 3)
                {
                    if (Center1.DistanceTo(XYZ.Zero) <= Center2.DistanceTo(XYZ.Zero))
                    {
                        Result = 1;
                    }
                    else
                    {
                        Result = 0;
                    }
                }
                else if (Quarter == 4)
                {
                    if (Center1.DistanceTo(XYZ.Zero) <= Center2.DistanceTo(XYZ.Zero))
                    {
                        Result = 0;
                    }
                    else
                    {
                        Result = 1;
                    }
                }
            }
            else if (Case == 2)
            {
                if (Quarter == 1)
                {
                    if (Center1.DistanceTo(XYZ.Zero) <= Center2.DistanceTo(XYZ.Zero))
                    {
                        Result = 0;
                    }
                    else
                    {
                        Result = 1;
                    }
                }
                else if (Quarter == 2)
                {
                    if (Center1.DistanceTo(XYZ.Zero) <= Center2.DistanceTo(XYZ.Zero))
                    {
                        Result = 0;
                    }
                    else
                    {
                        Result = 1;
                    }
                }
                else if (Quarter == 3)
                {
                    if (Center1.DistanceTo(XYZ.Zero) <= Center2.DistanceTo(XYZ.Zero))
                    {
                        Result = 1;
                    }
                    else
                    {
                        Result = 0;
                    }
                }
                else if (Quarter == 4)
                {
                    if (Center1.DistanceTo(XYZ.Zero) <= Center2.DistanceTo(XYZ.Zero))
                    {
                        Result = 1;
                    }
                    else
                    {
                        Result = 0;
                    }
                }
            }

            return Result;


        }

        public static Line GetBeamParallelLine(Element _Beam, int LeftOrtRight)
        {
            Line BeamLine = (_Beam.Location as LocationCurve).Curve as Line;
            int Index;

            List<Line> ParallelBeamLine = OffsetLine(BeamLine, 2);
            if (LeftOrtRight == 0)
            {
                Index = GetParallelBeamIndex(_Beam, ParallelBeamLine[0], ParallelBeamLine[1]);
            }
            else if (LeftOrtRight == 1)
            {
                Index = GetParallelBeamIndex(_Beam, ParallelBeamLine[0], ParallelBeamLine[1]);
                if (Index == 0)
                {
                    Index = 1;
                }
                else
                {
                    Index = 0;
                }
            }
            else
            {
                Index = 0;
            }
            return ParallelBeamLine[Index];



        }

        public static List<Line> OffsetLine(Line originalLine, double offsetDistance)
        {
            List<Line> offsetLines = new List<Line>();

            // Get the direction vector of the original line
            XYZ direction = (originalLine.GetEndPoint(1) - originalLine.GetEndPoint(0)).Normalize();

            // Create an offset vector in one direction
            XYZ offsetVector1 = direction.CrossProduct(XYZ.BasisZ).Normalize() * offsetDistance;

            // Create an offset vector in the other direction
            XYZ offsetVector2 = -offsetVector1;

            // Create new points for the offset lines
            XYZ startPoint1 = originalLine.GetEndPoint(0) + offsetVector1;
            XYZ endPoint1 = originalLine.GetEndPoint(1) + offsetVector1;

            XYZ startPoint2 = originalLine.GetEndPoint(0) + offsetVector2;
            XYZ endPoint2 = originalLine.GetEndPoint(1) + offsetVector2;

            // Create the offset lines
            Line offsetLine1 = Line.CreateBound(startPoint1, endPoint1);
            Line offsetLine2 = Line.CreateBound(startPoint2, endPoint2);

            // Add the offset lines to the list
            offsetLines.Add(offsetLine1);
            offsetLines.Add(offsetLine2);

            return offsetLines;
        }
        public static ObservableCollection<Element> GetElevationDimensionStyle(Document Doc)
        {
            ObservableCollection<Element> Result = new ObservableCollection<Element>();

            FilteredElementCollector collector = new FilteredElementCollector(Doc);
            collector.OfClass(typeof(DimensionType));

            // Get a list of all dimension styles
            List<Element> dimensionStyles = collector.ToElements().ToList();

            ObservableCollection<Element> ListOfDimensionStyles = new ObservableCollection<Element>(dimensionStyles);
            foreach (Element element in ListOfDimensionStyles)
            {
                if ((element as DimensionType).FamilyName == "Spot Elevations")
                {
                    Result.Add(element);
                }

            }

            return Result;

        }

        public static ObservableCollection<FamilySymbol> GetAllDetailItems(Document Doc)
        {
            FilteredElementCollector FamilyTypesFromRevit = new FilteredElementCollector(Doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_DetailComponents);

            ObservableCollection<FamilySymbol> Result = new ObservableCollection<FamilySymbol>();

            foreach (Element Type in FamilyTypesFromRevit)
            {
                bool has = false;
                FamilySymbol S = Type as FamilySymbol;
                foreach (FamilySymbol f in Result)
                {
                    if (S.Family.Id == f.Id)
                    {
                        has = true;
                        break;

                    }
                }

                if (!has)
                {

                    Result.Add(S);
                }


            }

            return Result;

        }

        public static ObservableCollection<Element> GetPlanDimensionStyle(Document Doc)
        {
            ObservableCollection<Element> Result = new ObservableCollection<Element>();

            FilteredElementCollector collector = new FilteredElementCollector(Doc);
            collector.OfClass(typeof(DimensionType));

            // Get a list of all dimension styles
            List<Element> dimensionStyles = collector.ToElements().ToList();

            ObservableCollection<Element> ListOfDimensionStyles = new ObservableCollection<Element>(dimensionStyles);
            foreach (Element element in ListOfDimensionStyles)
            {
                if ((element as DimensionType).FamilyName == "Linear Dimension Style")
                {
                    Result.Add(element);
                }

            }

            return Result;

        }

        public static ObservableCollection<Element> GetAllSpotElevations(Document Doc)
        {
            var spotElevations = new FilteredElementCollector(Doc).OfClass(typeof(SpotDimensionType));

            List<Element> Result = new List<Element>();

            foreach (var spotElevation in spotElevations)
            {

                Result.Add(spotElevation);

            }


            return new ObservableCollection<Element>(Result);

        }

        public static XYZ FindIntersection(Line line1, Line line2)
        {
            double x1 = line1.GetEndPoint(0).X;
            double y1 = line1.GetEndPoint(0).Y;
            double z1 = line1.GetEndPoint(0).Z;

            double x2 = line1.GetEndPoint(1).X;
            double y2 = line1.GetEndPoint(1).Y;
            double z2 = line1.GetEndPoint(1).Z;

            double x3 = line2.GetEndPoint(0).X;
            double y3 = line2.GetEndPoint(0).Y;
            double z3 = line2.GetEndPoint(0).Z;

            double x4 = line2.GetEndPoint(1).X;
            double y4 = line2.GetEndPoint(1).Y;
            double z4 = line2.GetEndPoint(1).Z;

            double denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            if (denominator == 0)
            {
                // Lines are parallel, no intersection
                return null;
            }

            double t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / denominator;

            double intersectionX = x1 + t * (x2 - x1);
            double intersectionY = y1 + t * (y2 - y1);
            double intersectionZ = z1 + t * (z2 - z1);

            return new XYZ(intersectionX, intersectionY, intersectionZ);
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
        public static List<Face> FaceArrayToList(FaceArray Faces)
        {
            List<Face> FacesList = new List<Face>();
            foreach (Face Face in Faces)
            {
                FacesList.Add(Face);
            }

            return FacesList;
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
        public static double CalculateSlope(XYZ StartPoint, XYZ EndPoint)
        {
            if (Math.Abs(StartPoint.X - EndPoint.X) < 0.0001)
            {
                return 1000000;
            }

            return (EndPoint.Y - StartPoint.Y) / (EndPoint.X - StartPoint.X);
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



        //////////////////////////////////////////////
        ///



        public static double GetWidth(Element _Beam)
        {
            double Width = 0;
            Curve BeamCurve = (_Beam.Location as LocationCurve).Curve;
            try
            {
                Width = Command.Doc.GetElement(_Beam.GetTypeId()).LookupParameter("b").AsDouble();

            }
            catch
            {
                Width = 0;
            }
            return Width;

        }
        public static double GetHeight(Element _Beam)
        {
            double Width = 0;
            Curve BeamCurve = (_Beam.Location as LocationCurve).Curve;
            try
            {
                Width = Command.Doc.GetElement(_Beam.GetTypeId()).LookupParameter("h").AsDouble();

            }
            catch
            {
                Width = 0;
            }
            return Width;

        }
        public static BoundingBoxXYZ GetSectionViewperpendicularToBeams(FamilyInstance familyInstance)
        {
            LocationCurve lc = familyInstance.Location as LocationCurve;

            Curve curve = lc.Curve;
            XYZ BeamStartPoint = curve.GetEndPoint(0);
            XYZ BeamEndPoint = curve.GetEndPoint(1);
            XYZ BeamVector;
            if (Math.Abs(BeamStartPoint.Y - BeamEndPoint.Y) > Math.Abs(BeamStartPoint.X - BeamEndPoint.X))
            {
                if (BeamStartPoint.Y > BeamEndPoint.Y)
                {
                    BeamVector = BeamEndPoint - BeamStartPoint;
                }
                else

                {
                    BeamVector = -BeamEndPoint + BeamStartPoint;
                }

            }
            else
            {
                if (BeamStartPoint.X > BeamEndPoint.X)
                {
                    BeamVector = -BeamEndPoint + BeamStartPoint;
                }
                else

                {
                    BeamVector = BeamEndPoint - BeamStartPoint;
                }
            }


            BoundingBoxXYZ BeamBoundungBox = familyInstance.get_BoundingBox(null);
            double minZ = BeamBoundungBox.Min.Z + 1;
            double maxZ = BeamBoundungBox.Max.Z + 1;

            double VectorLength = BeamVector.GetLength();
            double offsetmax = 0.05 * VectorLength;
            double offsetmin = 0.05 * VectorLength;
            double height = maxZ - minZ;

            XYZ min = new XYZ(-VectorLength * 0.35, -offsetmin, 0);//---
            XYZ max = new XYZ(VectorLength * 0.35, offsetmax, 0.1);
            // section view dotted line in center of wall
            //XYZ max = new XYZ(w, maxZ + offset, offset); // section view dotted line offset from center of wall
            XYZ midpoint;
            if (Math.Abs(BeamStartPoint.Y - BeamEndPoint.Y) > Math.Abs(BeamStartPoint.X - BeamEndPoint.X))
            {
                if (BeamStartPoint.Y > BeamEndPoint.Y)
                {
                    midpoint = BeamStartPoint + 0.5 * BeamVector;

                }
                else

                {
                    midpoint = BeamStartPoint - 0.5 * BeamVector;

                }

            }
            else
            {
                if (BeamStartPoint.X > BeamEndPoint.X)
                {
                    midpoint = BeamStartPoint - 0.5 * BeamVector;
                }
                else

                {
                    midpoint = BeamStartPoint + 0.5 * BeamVector;

                }
            }

            XYZ Beamdir = BeamVector.Normalize();
            XYZ up = XYZ.BasisZ;
            XYZ viewdir = Beamdir.CrossProduct(up);
            Transform t = Transform.Identity;
            t.Origin = midpoint;
            t.BasisX = viewdir;
            t.BasisY = up;
            t.BasisZ = t.BasisX.CrossProduct(t.BasisY);    // DONE 

            BoundingBoxXYZ sectionBox = new BoundingBoxXYZ();
            sectionBox.Transform = t;

            double H = GetHeight(familyInstance as Element);
            sectionBox.Min = min + new XYZ(0, -H * 1.5, 0);
            sectionBox.Max = max + new XYZ(0, H / 1.5, 0);

            return sectionBox;
        }
        public static List<Element> GetAllFloors(Document Doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(Doc);
            return collector.OfClass(typeof(Floor)).ToElements().ToList();

        }

        public static XYZ GetSectionHeadPosition(ViewSection sectionView)
        {
            // Get the location curve of the section
            LocationCurve locationCurve = sectionView.Location as LocationCurve;

            if (locationCurve != null)
            {
                // Get the curve of the section
                Line curve = (locationCurve.Curve) as Line;

                // Get the start and end points of the curve
                XYZ startPoint = curve.GetEndPoint(0);
                XYZ endPoint = curve.GetEndPoint(1);

                // Determine the head position based on the orientation of the section
                XYZ headPosition = curve.Direction.Normalize();

                // Adjust the head position based on the curve direction
                if (curve.Direction.IsAlmostEqualTo(XYZ.BasisX) || curve.Direction.IsAlmostEqualTo(-XYZ.BasisX))
                {
                    headPosition = endPoint;
                }
                else if (curve.Direction.IsAlmostEqualTo(XYZ.BasisY) || curve.Direction.IsAlmostEqualTo(-XYZ.BasisY))
                {
                    headPosition = endPoint;
                }
                else if (curve.Direction.IsAlmostEqualTo(XYZ.BasisZ) || curve.Direction.IsAlmostEqualTo(-XYZ.BasisZ))
                {
                    headPosition = endPoint;
                }
                else
                {
                    // Handle other cases as needed
                    // You may need to analyze the curve direction further based on your specific requirements
                }

                return headPosition;
            }

            return null;
        }
        public static double PerpendicularSlope(double TempSlope)
        {

            double MainSlope;
            if (TempSlope == 1000000)
            {
                MainSlope = 0;

            }
            else if (Math.Abs(TempSlope) < 0.00001)
            {
                MainSlope = 1000000;
            }
            else
            {
                MainSlope = -1 / TempSlope;
            }

            return MainSlope;
        }
        public static ObservableCollection<Level> GetLevels(Document Doc)
        {

            FilteredElementCollector LevelsFromRevit = new FilteredElementCollector(Doc).OfClass(typeof(Level)).OfCategory(BuiltInCategory.OST_Levels);
            ObservableCollection<Level> Result = new ObservableCollection<Level>();

            foreach (Element item in LevelsFromRevit)
            {
                if (item is Level)
                    Result.Add(item as Level);
            }

            Result.OrderBy(x => x.Elevation);

            return Result;
        }
        private static double IsLeft(XYZ startPoint, XYZ endPoint, XYZ testPoint)
        {
            return ((endPoint.X - startPoint.X) * (testPoint.Y - startPoint.Y)) -
                   ((testPoint.X - startPoint.X) * (endPoint.Y - startPoint.Y));
        }
        public static bool IsPointInPolygon(XYZ testPoint, CurveArray polygonEdges)
        {
            int windingNumber = 0;

            for (int i = 0; i < polygonEdges.Size; i++)
            {
                Curve edge = polygonEdges.get_Item(i);

                XYZ startPoint = edge.GetEndPoint(0);
                XYZ endPoint = edge.GetEndPoint(1);

                // Check if the test point is above the edge
                if (startPoint.Y <= testPoint.Y)
                {
                    if (endPoint.Y > testPoint.Y && IsLeft(startPoint, endPoint, testPoint) > 0)
                    {
                        windingNumber++;
                    }
                }
                else
                {
                    if (endPoint.Y <= testPoint.Y && IsLeft(startPoint, endPoint, testPoint) < 0)
                    {
                        windingNumber--;
                    }
                }
            }

            return windingNumber != 0;
        }

        public static Solid GetSolidForFloor(Element floor)
        {
            Options options = new Options();
            options.ComputeReferences = true;
            options.DetailLevel = ViewDetailLevel.Fine;

            GeometryElement columnGeometry = floor.get_Geometry(options);
            Solid solid = default;

            foreach (GeometryObject geomObj in columnGeometry)
            {
                if ((geomObj as Solid) != null && (geomObj as Solid).Volume != 0)
                {
                    solid = geomObj as Solid;
                    break;
                }

            }


            return solid;
        }
        public static XYZ GetBeamCenterPoint(Element _Beam)
        {
            Curve BeamCurve = (_Beam.Location as LocationCurve).Curve;
            return (BeamCurve.GetEndPoint(0) + BeamCurve.GetEndPoint(1)) / 2;
        }
        public static List<string> ListOfSectionNames(string StartName, string num1, string num2, int NumberOfSheets)
        {
            List<string> Result = new List<string>();

            Result.Add(FixString(StartName, num1, num2));

            for (int i = 1; i < NumberOfSheets; i++)
            {
                try
                {
                    int n1 = Convert.ToInt32(num1);
                    int n2 = Convert.ToInt32(num2);

                    n1++;
                    n2++;

                    if (n1 <= 9)
                    {
                        num1 = $"0{n1}";

                    }
                    else
                    {
                        num1 = n1.ToString();
                    }
                    if (n2 <= 9)
                    {
                        num2 = $"0{n2}";
                    }
                    else
                    {
                        num2 = n2.ToString();
                    }
                }
                catch { }

                Result.Add(FixString(StartName, num1, num2));
            }
            return Result;
        }

        public static string FixString(string StartName, string num1, string num2)
        {
            string Result = $"{StartName}({num1}-{num2})";
            return Result;
        }

     


        public static List<Curve> GetFloorCurves(Element Floor)
        {
            Floor floorObj = Floor as Floor;
            GeometryElement geometryElement = floorObj.get_Geometry(new Options());
            Solid FloorSolid = GetSolidForFloor(Floor);
            FaceArray FaceArr = FloorSolid.Faces;
            Face MaxFace = FaceArrayToList(FaceArr).OrderByDescending(e => e.Area).FirstOrDefault();
            CurveLoop curveLoop = MaxFace.GetEdgesAsCurveLoops().FirstOrDefault();
            List<Curve> Result = new List<Curve>();
            foreach (Curve cr in curveLoop)
            {
                Result.Add(cr);
            }
            return Result;


        }

    }
}
