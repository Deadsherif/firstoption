using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chamfer.MVVM.Model
{
    public static class LineExtensions
    {
        private const double a = 1E-09;

        public static bool IsVertical(this Line line)
        {
            XYZ direction = line.Direction;
            return direction.X == 0 && Math.Abs(direction.Y) == 1;
        }

        public static bool IsHorizontal(this Line line)
        {
            XYZ direction = line.Direction;
            return direction.Y == 0 && Math.Abs(direction.X) == 1;
        }

        public static bool IsParallel(this Line line1, Line line2)
        {
            // Get the direction vectors of both lines
            XYZ direction1 = line1.Direction.Normalize();
            XYZ direction2 = line2.Direction.Normalize();

            // Check if the direction vectors are equal or opposite
            return direction1.IsAlmostEqualTo(direction2) || direction1.IsAlmostEqualTo(-direction2);
        }

        public static bool IsParallel(this Line line, PlanarFace face)
        {
            return line.Direction.IsPerpendicular(face.FaceNormal);
        }

        public static bool IsPerpendicular(this Line line, Line other)
        {
            return line.Direction.IsPerpendicular(other.Direction);
        }

        public static bool IsPerpendicular(this Line line, PlanarFace face)
        {
            return line.Direction.IsParallel(face.FaceNormal);
        }

        public static XYZ Intersection(this Line line, Line otherLine)
        {
            IntersectionResultArray intersectionResultArray;
            return ((Curve)line).Intersect((Curve)otherLine, out intersectionResultArray) == 8 && intersectionResultArray.Size < 2 ? intersectionResultArray[0].XYZPoint : (XYZ)null;
        }

        public static bool IsOverlapping(this Line line, Line otherLine)
        {
            IntersectionResultArray intersectionResultArray;
            return ((Curve)line).Intersect((Curve)otherLine, ref intersectionResultArray) == 8 && intersectionResultArray.Size > 0;
        }

        public static Line FixLine(this Line line)
        {
            return line.Direction.IsReversed() ? Line.CreateBound(((Curve)line).GetEndPoint(1), ((Curve)line).GetEndPoint(0)) : line;
        }

        public static Line Merge(this Line line, Line otherLine)
        {
            Line line1 = line.FixLine();
            XYZ endPoint1 = ((Curve)line1).GetEndPoint(0);
            XYZ endPoint2 = ((Curve)line1).GetEndPoint(1);
            Line line2 = otherLine.FixLine();
            XYZ endPoint3 = ((Curve)line2).GetEndPoint(0);
            XYZ endPoint4 = ((Curve)line2).GetEndPoint(1);
            return Line.CreateBound(XYZ.op_Subtraction(endPoint1, endPoint3).IsReversed() ? endPoint1 : endPoint3, XYZ.op_Subtraction(endPoint4, endPoint2).IsReversed() ? endPoint2 : endPoint4);
        }

        public static XYZ UnboundIntersection(this Line line, Line otherLine)
        {
            IntersectionResultArray intersectionResultArray;
            return (((Curve)line).IsBound ? (Curve)Line.CreateUnbound(line.Origin, line.Direction) : (Curve)line).Intersect(((Curve)otherLine).IsBound ? (Curve)Line.CreateUnbound(otherLine.Origin, otherLine.Direction) : (Curve)otherLine, ref intersectionResultArray) == 8 && intersectionResultArray.Size < 2 ? intersectionResultArray[0].XYZPoint : (XYZ)null;
        }

        public static double HorizontalDistanceTo(this Line line, Line otherLine)
        {
            return line.HorizontalDistanceTo(otherLine.Origin);
        }

        public static double HorizontalDistanceTo(this Line line, XYZ point)
        {
            XYZ endPoint1 = ((Curve)line).GetEndPoint(0);
            XYZ endPoint2 = ((Curve)line).GetEndPoint(1);
            double x1 = point.X;
            double y1 = point.Y;
            double x2 = endPoint1.X;
            double y2 = endPoint1.Y;
            double x3 = endPoint2.X;
            double y3 = endPoint2.Y;
            return ((x3 - x2) * (y2 - y1) - (x2 - x1) * (y3 - y2)) / Math.Sqrt((x3 - x2) * (x3 - x2) + (y3 - y2) * (y3 - y2));
        }
    }
}
