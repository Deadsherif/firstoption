// Decompiled with JetBrains decompiler
// Type: CableOverTray.CurveWithData
// Assembly: COT, Version=2021.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 774E24BD-3C4D-44CA-9437-58E3900DE92B
// Assembly location: C:\Users\ahmed\AppData\Roaming\Autodesk\Revit\Addins\2023\CableOverTray.dll

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using System;
using System.Collections.Generic;

 
namespace CableOverTray
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
      this.curveHost = e;
      this.curve = c;
      this.startPoint = sp;
      this.endPoint = c.GetEndPoint(0) != sp ? c.GetEndPoint(0) : c.GetEndPoint(1);
      this.normalVector = (this.endPoint - this.startPoint).CrossProduct(new XYZ(this.endPoint.X, this.endPoint.Y, this.endPoint.Z - 1.0) - this.startPoint);
    }

    public CurveWithData(MEPCurve e, Curve c, XYZ sp, XYZ ep)
    {
      this.curveHost = e;
      this.curve = c;
      this.startPoint = sp;
      this.endPoint = ep;
      this.normalVector = (this.endPoint - this.startPoint).CrossProduct(new XYZ(this.endPoint.X, this.endPoint.Y, this.endPoint.Z - 1.0) - this.startPoint);
    }

    public CurveWithData(MEPCurve e, XYZ sp)
    {
      this.curveHost = e;
      Curve curve = (e.Location as LocationCurve).Curve;
      this.curve = curve;
      this.startPoint = sp;
      this.endPoint = curve.GetEndPoint(0) != sp ? curve.GetEndPoint(0) : curve.GetEndPoint(1);
      this.normalVector = (this.endPoint - this.startPoint).CrossProduct(new XYZ(this.endPoint.X, this.endPoint.Y, this.endPoint.Z - 1.0) - this.startPoint);
    }

    public CurveWithData(MEPCurve e, XYZ sp, XYZ ep)
    {
      this.curveHost = e;
      this.curve = (e.Location as LocationCurve).Curve;
      this.startPoint = sp;
      this.endPoint = ep;
      XYZ xyz1 = this.endPoint - this.startPoint;
      XYZ xyz2 = new XYZ();
      double num1 = this.startPoint.X - this.endPoint.X;
      double num2 = this.startPoint.Y - this.endPoint.Y;
      if (num1 < 0.0)
        num1 *= -1.0;
      if (num2 < 0.0)
        num2 *= -1.0;
      XYZ xyz3;
      if (num1 < 0.0001 && num2 < 0.0001)
      {
        XYZ xyz4 = (this.curveHost as CableTray).CurveNormal.Negate();
        XYZ source1 = new XYZ(1.0, 0.0, 0.0);
        XYZ source2 = new XYZ(0.0, 1.0, 0.0);
        XYZ source3 = new XYZ(0.0, 0.0, 1.0);
        double d1 = xyz4.AngleTo(source1);
        double d2 = xyz4.AngleTo(source2);
        double d3 = xyz4.AngleTo(source3);
        xyz3 = new XYZ(this.endPoint.X + 1.0 * Math.Cos(d1), this.endPoint.Y + 1.0 * Math.Cos(d2), this.endPoint.Z + 1.0 * Math.Cos(d3)) - this.startPoint;
      }
      else
        xyz3 = new XYZ(this.endPoint.X, this.endPoint.Y, this.endPoint.Z - 1.0) - this.startPoint;
      this._normalVector = new XYZ(xyz1.Y * xyz3.Z - xyz1.Z * xyz3.Y, xyz1.Z * xyz3.X - xyz1.X * xyz3.Z, xyz1.X * xyz3.Y - xyz1.Y * xyz3.X);
    }

    public List<XYZ> GetShiftedCurvePoints(double offset)
    {
      List<XYZ> shiftedCurvePoints = new List<XYZ>();
      XYZ source1 = new XYZ(1.0, 0.0, 0.0);
      XYZ source2 = new XYZ(0.0, 1.0, 0.0);
      XYZ source3 = new XYZ(0.0, 0.0, 1.0);
      double d1 = this._normalVector.AngleTo(source1);
      double d2 = this._normalVector.AngleTo(source2);
      double d3 = this._normalVector.AngleTo(source3);
      XYZ xyz1 = new XYZ(this.startPoint.X + offset * Math.Cos(d1), this.startPoint.Y + offset * Math.Cos(d2), this.startPoint.Z + offset * Math.Cos(d3));
      XYZ xyz2 = new XYZ(this.endPoint.X + offset * Math.Cos(d1), this.endPoint.Y + offset * Math.Cos(d2), this.endPoint.Z + offset * Math.Cos(d3));
      shiftedCurvePoints.Add(xyz1);
      shiftedCurvePoints.Add(xyz2);
      return shiftedCurvePoints;
    }
  }
}
