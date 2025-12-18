using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipesSupports.MVVM.Model
{
    public class FO_PipeLine
    {

        public Line MainLine { get; private set; }

        public List<XYZ> SupportsPointsLocation { get; private set; }

        public double Length { get; private set; }
        public double Spacing { get; private set; }

        public double Diameter { get; private set; }

        public FO_PipeLine(Line pipeLine, double spacing = 1000 / 0.00328084, double diameter = 200 / 0.00328084)
        {
            MainLine = pipeLine;
            if (spacing <= 0)
            {
                Spacing = 1000 / 0.00328084;
            }
            else
            {
                Spacing = spacing;
            }

            Length = pipeLine.Length;

            SupportsPointsLocation = GetLinePoints(pipeLine, Spacing);
            Diameter = diameter;
        }

        private List<XYZ> GetLinePoints(Line pipeLine, double Spacing)
        {
            double length = pipeLine.Length;
            double Comulativelength = 0;
            List<XYZ> Result = new List<XYZ>();



            XYZ Direction = GetLineDirectionRatios(pipeLine);

            XYZ StartPoint = pipeLine.GetEndPoint(0);

            while (length - Comulativelength > Spacing * 0.00328084)
            {
                StartPoint = TranslateXYZ(StartPoint, Direction, Spacing * 0.00328084);
                Comulativelength += Spacing * 0.00328084;
                Result.Add(StartPoint);
            }

            return Result;



        }
        private XYZ TranslateXYZ(XYZ startPoint, XYZ direction, double distance)
        {
            // Normalize the direction vector
            XYZ normalizedDirection = direction.Normalize();

            // Calculate the displacement vector
            XYZ displacement = normalizedDirection * distance;

            // Calculate the new point after translation
            XYZ newPoint = startPoint + displacement;

            return newPoint;
        }
        private XYZ GetLineDirectionRatios(Line line)
        {
            // Assuming line is a valid Revit.DB.Line
            XYZ directionVector = line.Direction;

            // Extract individual components
            double a = directionVector.X;
            double b = directionVector.Y;
            double c = directionVector.Z;

            // Now, (a, b, c) are the direction ratios of the line
            return new XYZ(a, b, c);
        }

    }
}
