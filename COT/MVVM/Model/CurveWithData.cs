using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COT.MVVM.Model
{
    public class CurveWithData
    {
        public Curve curve;
        public XYZ startPoint;
        public XYZ endPoint;
        public XYZ normalVector;
        public XYZ _normalVector;
        public MEPCurve curveHost;


        public CurveWithData(MEPCurve e, Curve c, XYZ sp)
        {
            curveHost = e;
            curve = c;
            startPoint = sp;

            if (c.GetEndPoint(0) == sp)
            {
                endPoint = c.GetEndPoint(1);
            }
            else
            {
                endPoint = c.GetEndPoint(0);
            }

            XYZ _endPoint = new XYZ(endPoint.X, endPoint.Y, endPoint.Z - 1);
            XYZ mainVector = endPoint - startPoint;
            XYZ cloneVector = _endPoint - startPoint;
            normalVector = mainVector.CrossProduct(cloneVector);
        }
        public CurveWithData(MEPCurve e, Curve c, XYZ sp, XYZ ep)
        {
            curveHost = e;
            curve = c;
            startPoint = sp;
            endPoint = ep;

            XYZ _endPoint = new XYZ(endPoint.X, endPoint.Y, endPoint.Z - 1);
            XYZ mainVector = endPoint - startPoint;
            XYZ cloneVector = _endPoint - startPoint;
            normalVector = mainVector.CrossProduct(cloneVector);
        }
        public CurveWithData(MEPCurve e, XYZ sp)
        {
            curveHost = e;
            LocationCurve lc = e.Location as LocationCurve;
            Curve c = lc.Curve;
            curve = c;
            startPoint = sp;

            if (c.GetEndPoint(0) == sp)
            {
                endPoint = c.GetEndPoint(1);
            }
            else
            {
                endPoint = c.GetEndPoint(0);
            }

            XYZ _endPoint = new XYZ(endPoint.X, endPoint.Y, endPoint.Z - 1);
            XYZ mainVector = endPoint - startPoint;
            XYZ cloneVector = _endPoint - startPoint;
            normalVector = mainVector.CrossProduct(cloneVector);
        }
        public CurveWithData(MEPCurve e, XYZ sp, XYZ ep)
        {
            curveHost = e;
            LocationCurve lc = e.Location as LocationCurve;
            Curve c = lc.Curve;
            curve = c;
            startPoint = sp;
            endPoint = ep;

            XYZ mainVector = endPoint - startPoint;
            XYZ cloneVector = new XYZ();



            double dX = startPoint.X - endPoint.X;
            double dY = startPoint.Y - endPoint.Y;
            if (dX < 0)
            {
                dX = dX * -1;
            }
            if (dY < 0)
            {
                dY = dY * -1;
            }
            if (dX < 0.0001 && dY < 0.0001)
            {
                CableTray ct = curveHost as CableTray;
                XYZ trayNormal = ct.CurveNormal;
                XYZ _trayNormal = trayNormal.Negate();

                XYZ X = new XYZ(1, 0, 0);
                XYZ Y = new XYZ(0, 1, 0);
                XYZ Z = new XYZ(0, 0, 1);


                double th1 = _trayNormal.AngleTo(X);
                double th2 = _trayNormal.AngleTo(Y);
                double th3 = _trayNormal.AngleTo(Z);


                XYZ _endPoint = new XYZ((endPoint.X + (1 * Math.Cos(th1))), (endPoint.Y + (1 * Math.Cos(th2))), (endPoint.Z + (1 * Math.Cos(th3))));
                cloneVector = _endPoint - startPoint;
            }
            else
            {

                XYZ _endPoint = new XYZ(endPoint.X, endPoint.Y, endPoint.Z - 1);

                cloneVector = _endPoint - startPoint;

            }


            //normalVector = mainVector.CrossProduct(cloneVector);

            double i = (mainVector.Y * cloneVector.Z) - (mainVector.Z * cloneVector.Y);
            double j = (mainVector.Z * cloneVector.X) - (mainVector.X * cloneVector.Z);
            double k = (mainVector.X * cloneVector.Y) - (mainVector.Y * cloneVector.X);

            _normalVector = new XYZ(i, j, k);



        }

        public List<XYZ> GetShiftedCurvePoints(double offset)
        {
            List<XYZ> output = new List<XYZ>();

            XYZ X = new XYZ(1, 0, 0);
            XYZ Y = new XYZ(0, 1, 0);
            XYZ Z = new XYZ(0, 0, 1);


            double th1 = _normalVector.AngleTo(X);
            double th2 = _normalVector.AngleTo(Y);
            double th3 = _normalVector.AngleTo(Z);

            XYZ point1 = new XYZ((startPoint.X + (offset * Math.Cos(th1))), (startPoint.Y + (offset * Math.Cos(th2))), (startPoint.Z + (offset * Math.Cos(th3))));
            XYZ point2 = new XYZ((endPoint.X + (offset * Math.Cos(th1))), (endPoint.Y + (offset * Math.Cos(th2))), (endPoint.Z + (offset * Math.Cos(th3))));

            output.Add(point1);
            output.Add(point2);



            return output;
        }

    }
}

