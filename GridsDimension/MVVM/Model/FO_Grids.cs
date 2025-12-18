using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridsDimension.MVVM.Model
{
    public class FO_Grids
    {
        public ElementId Id { get; set; }
        public Line Line { get; set; }
        public XYZ StartPoint { get; }
        public XYZ EndPoint { get; }
        public XYZ CenterPoint { get; }
        public int GroupId { get; set; }
        public double Slope { get; set; }

        public FO_Grids(Grid Grd)
        {
            Line = Grd.Curve as Line;
            StartPoint = Grd.Curve.GetEndPoint(0);
            EndPoint = Grd.Curve.GetEndPoint(1);
            Slope = CalculateSlope(StartPoint, EndPoint);
            CenterPoint = (StartPoint + EndPoint) / 2;
            Id = Grd.Id;
        }
        public static double CalculateSlope(XYZ StartPoint, XYZ EndPoint)
        {
            if (StartPoint.X == EndPoint.X)
            {
                return 1000000;
            }

            return (EndPoint.Y - StartPoint.Y) / (EndPoint.X - StartPoint.X);
        }





    }
}
