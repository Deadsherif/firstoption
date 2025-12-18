using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipesSupports
{
    public class Helper
    {
        public static ObservableCollection<FamilySymbol> GetAllMechanicalEquipmentFamilySymbols(Document Doc)
        {

            FilteredElementCollector PipesFromRevit = new FilteredElementCollector(Doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_MechanicalEquipment);
            ObservableCollection<FamilySymbol> Result = new ObservableCollection<FamilySymbol>();

            foreach (Element item in PipesFromRevit)
            {

                Result.Add(item as FamilySymbol);
            }

            Result.OrderBy(x => x.Name);

            return Result;
        }
        public static double GetAngleWithYAxis(Line line)
        {
            // Get the direction vector of the line


            // Define the Y-axis vector
            XYZ yAxis = new XYZ(0, 1, 0);

            // Calculate the dot product between the line direction and Y-axis vectors
            XYZ p1 = line.GetEndPoint(0);
            XYZ p2 = line.GetEndPoint(1);

            XYZ startPoint = new XYZ();
            XYZ endPoint = new XYZ();

            if (p1.X > p2.X)
            {
                startPoint = p1;
                endPoint = p2;
            }
            else if (p1.X < p2.X)
            {
                startPoint = p2;
                endPoint = p1;
            }
            else
            {
                return 0;
            }
            XYZ lineDirection = endPoint - startPoint;

            double dotProduct = lineDirection.DotProduct(yAxis);

            // Calculate the magnitude of the vectors
            double lineDirectionMagnitude = lineDirection.GetLength();
            double yAxisMagnitude = yAxis.GetLength();

            // Calculate the cosine of the angle between the line and Y-axis
            double cosineAngle = dotProduct / (lineDirectionMagnitude * yAxisMagnitude);

            // Calculate the angle in radians
            double angleInRadians = Math.Acos(cosineAngle);



            return angleInRadians;
        }
    }
}
