using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeDimensionColors
{
    public class Helper
    {
        public static bool HasFractions(double value)
        {
            // Check if the fractional part is greater than zero
            return Math.Abs(value - Math.Floor(value)) > double.Epsilon;
        }
        public static List<double> GetDimensionValues(Dimension dimension)
        {
            List<double> dimensionValues = new List<double>();

            if (dimension != null)
            {
                foreach (DimensionSegment segment in dimension.Segments)
                {
                    // Get the value of each segment
                    string segmentstring = segment.Value.ToString();
                    double segementValue;
                    if (double.TryParse(segmentstring, out segementValue))
                    {
                        dimensionValues.Add(segementValue);
                    }
                }
            }

            return dimensionValues;
        }
    }
}
