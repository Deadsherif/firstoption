using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlignTags
{
   public class Helper
   {
      public static double GetTagTextHeight(View view, Element tag)
      {

         if (tag != null && tag is IndependentTag)
         {
            if (view is ViewPlan)
            {

               XYZ MaxPoint = null;
               if (Math.Abs(tag.get_BoundingBox(view).Max.Y - (tag as IndependentTag).TagHeadPosition.Y) < Math.Abs(tag.get_BoundingBox(view).Min.Y - (tag as IndependentTag).TagHeadPosition.Y))
               {
                  MaxPoint = tag.get_BoundingBox(view).Max;

               }
               else
               {
                  MaxPoint = tag.get_BoundingBox(view).Min;

               }

               XYZ MidPoint = (tag as IndependentTag).TagHeadPosition;


               return (Math.Abs(MaxPoint.Y - MidPoint.Y)) * 2;

            }
            else if (view is ViewSection)
            {
               XYZ MaxPoint = null;
               if (Math.Abs(tag.get_BoundingBox(view).Max.Z - (tag as IndependentTag).TagHeadPosition.Z) < Math.Abs(tag.get_BoundingBox(view).Min.Z - (tag as IndependentTag).TagHeadPosition.Z))
               {
                  MaxPoint = tag.get_BoundingBox(view).Max;

               }
               else
               {
                  MaxPoint = tag.get_BoundingBox(view).Min;

               }

               XYZ MidPoint = (tag as IndependentTag).TagHeadPosition;


               return (Math.Abs(MaxPoint.Z - MidPoint.Z)) * 2;

            }
         }
         return 0;
      }
      public static int CountLines(string text)
      {
         if (string.IsNullOrEmpty(text))
         {
            return 0;
         }

         // Split the text into lines using newline characters as separators
         string[] lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

         // Return the number of lines
         return lines.Length;
      }

      public static List<Element> OrderPointsFromLeftToRight(View view, List<Element> elements, bool IsLeftToRight)
      {
         // Get the view direction and right direction
         XYZ viewDirection = view.ViewDirection;
         XYZ rightDirection = view.RightDirection;

         // Create a dictionary to store points and their corresponding distances along the right direction
         Dictionary<Element, double> pointDistances = new Dictionary<Element, double>();

         // Calculate the distance of each point along the right direction from the origin
         foreach (Element element in elements)
         {
            var tag = (element as IndependentTag);

#if (R2020 || R2021)
            var reference = tag.GetTaggedReference();

            XYZ point = tag.LeaderEnd;

#else
            var references = tag.GetTaggedReferences();
            var reference = references.FirstOrDefault();
            XYZ point = tag.GetLeaderEnd(reference);
#endif

            XYZ vectorToPoint = point - view.Origin;
            double distance = vectorToPoint.DotProduct(rightDirection);
            pointDistances.Add(element, distance);
         }
         List<Element> resut = new List<Element>();
         // Sort the points based on their distances along the right direction
         if (IsLeftToRight)
         {
            resut = pointDistances.OrderBy(p => p.Value).Select(p => p.Key).ToList();

         }
         else
         {
            resut = pointDistances.OrderByDescending(p => p.Value).Select(p => p.Key).ToList();
         }

         return resut;
      }
      public static List<Element> OrderPointsFromTopToBot(View view, List<Element> elements, bool IsTopToBot)
      {
         // Get the view direction and right direction
         XYZ viewDirection = view.ViewDirection;
         XYZ upDirection = view.UpDirection;

         // Create a dictionary to store points and their corresponding distances along the right direction
         Dictionary<Element, double> pointDistances = new Dictionary<Element, double>();

         // Calculate the distance of each point along the right direction from the origin
         foreach (Element element in elements)
         {
            var tag = element as IndependentTag;
#if (R2020 || R2021)
            var reference = tag.GetTaggedReference();

            XYZ point = tag.LeaderEnd;

#else
            var references = tag.GetTaggedReferences();
            var reference = references.FirstOrDefault();
            XYZ point = tag.GetLeaderEnd(reference);
#endif
            XYZ vectorToPoint = point - view.Origin;
            double distance = vectorToPoint.DotProduct(upDirection);
            pointDistances.Add(element, distance);
         }
         List<Element> resut = new List<Element>();
         // Sort the points based on their distances along the right direction
         if (IsTopToBot)
         {
            resut = pointDistances.OrderByDescending(p => p.Value).Select(p => p.Key).ToList();

         }
         else
         {
            resut = pointDistances.OrderBy(p => p.Value).Select(p => p.Key).ToList();
         }


         return resut;
      }

      public static Line GetLeaderLine(View view, XYZ startPoint, double angleWithRightDirection, double length)
      {
         // Get the view's right direction
         XYZ rightDirection = view.RightDirection.Normalize();
         XYZ MovementDirection = RotateDirectionAroundAxis(rightDirection, view.ViewDirection, angleWithRightDirection);
         XYZ endPoint = MovePoint(startPoint, MovementDirection, 1000);

         // Create the line
         Line line = Line.CreateBound(startPoint, endPoint);

         return line;
      }
      public static XYZ RotateDirectionAroundAxis(XYZ direction, XYZ axis, double angleDegrees)
      {
         // Convert angle to radians
         double angleRadians = angleDegrees * (Math.PI / 180);

         // Calculate rotation matrix components
         double cosAngle = Math.Cos(angleRadians);
         double sinAngle = Math.Sin(angleRadians);
         double oneMinusCos = 1 - cosAngle;

         // Calculate components of the rotated direction vector
         double newX = (cosAngle + axis.X * axis.X * oneMinusCos) * direction.X +
                       (axis.X * axis.Y * oneMinusCos - axis.Z * sinAngle) * direction.Y +
                       (axis.X * axis.Z * oneMinusCos + axis.Y * sinAngle) * direction.Z;

         double newY = (axis.Y * axis.X * oneMinusCos + axis.Z * sinAngle) * direction.X +
                       (cosAngle + axis.Y * axis.Y * oneMinusCos) * direction.Y +
                       (axis.Y * axis.Z * oneMinusCos - axis.X * sinAngle) * direction.Z;

         double newZ = (axis.Z * axis.X * oneMinusCos - axis.Y * sinAngle) * direction.X +
                       (axis.Z * axis.Y * oneMinusCos + axis.X * sinAngle) * direction.Y +
                       (cosAngle + axis.Z * axis.Z * oneMinusCos) * direction.Z;

         // Create the new direction vector
         XYZ newDirection = new XYZ(newX, newY, newZ);

         return newDirection;
      }

      public static XYZ GetElbowPoint(View view, XYZ LeaderHeadPoint, Line LeaderLine)
      {
         XYZ StartPoint = MovePoint(LeaderHeadPoint, view.RightDirection, 100);
         XYZ EndPoint = MovePoint(LeaderHeadPoint, -view.RightDirection, 100);

         Line LineAtHead = Line.CreateBound(StartPoint, EndPoint);

         XYZ dir1 = LeaderLine.GetEndPoint(1) - LeaderLine.GetEndPoint(0);
         XYZ dir2 = LineAtHead.GetEndPoint(1) - LineAtHead.GetEndPoint(0);

         // Calculate vectors relative to one endpoint of each line
         XYZ p1 = LeaderLine.GetEndPoint(0);
         XYZ p2 = LineAtHead.GetEndPoint(0);

         // Calculate the determinant of the direction vectors
         XYZ vector = dir1.CrossProduct(dir2);
         double determinant = vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;

         // Check if the lines are parallel
         if (Math.Abs(determinant) < 1e-9)
         {
            // Lines are parallel, return null or handle accordingly
            return null;
         }

         // Calculate the parameters for the intersection point
         double s = ((p2 - p1).CrossProduct(dir2)).DotProduct(dir1.CrossProduct(dir2)) / determinant;
         double t = ((p2 - p1).CrossProduct(dir1)).DotProduct(dir1.CrossProduct(dir2)) / determinant;

         // Calculate the intersection point
         XYZ intersectionPoint = p1 + s * dir1;

         return intersectionPoint;
      }
      public static XYZ MovePoint(XYZ originalPoint, double slope, double distance)
      {
         // Calculate the new coordinates
         double deltaX = distance / Math.Sqrt(1 + Math.Pow(slope, 2));
         double deltaY = slope * deltaX;

         double x1 = originalPoint.X + deltaX;
         double y1 = originalPoint.Y + deltaY;

         // Return the new point
         return new XYZ(x1, y1, originalPoint.Z);
      }
      public static XYZ MovePoint(XYZ point, XYZ direction, double distance)
      {
         // Normalize the direction vector (make it a unit vector)
         XYZ normalizedDirection = direction.Normalize();

         // Calculate the movement vector by scaling the normalized direction vector by the distance
         XYZ movementVector = normalizedDirection * distance;

         // Calculate the new point by adding the movement vector to the original point
         XYZ newPoint = point + movementVector;

         return newPoint;
      }


   }
}
