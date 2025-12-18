using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTrim
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument activeUiDocument = commandData.Application.ActiveUIDocument;
            Document document = activeUiDocument.Document;
            IList<Reference> referenceList1 = activeUiDocument.Selection.PickObjects(ObjectType.Element);
            List<Element> elementList1 = new List<Element>();
            foreach (Reference reference in (IEnumerable<Reference>)referenceList1)
                elementList1.Add(document.GetElement(reference));
            IList<Reference> referenceList2 = activeUiDocument.Selection.PickObjects(ObjectType.Element);
            List<Element> elementList2 = new List<Element>();
            foreach (Reference reference in (IEnumerable<Reference>)referenceList2)
                elementList2.Add(document.GetElement(reference));
            List<Tuple<Element, XYZ>> source1 = new List<Tuple<Element, XYZ>>();
            foreach (Element element in elementList1)
            {
                XYZ origin = ((element.Location as LocationCurve).Curve as Line).Origin;
                source1.Add(Tuple.Create<Element, XYZ>(element, new XYZ(origin.X, origin.Y, origin.Z)));
            }
            List<Tuple<Element, XYZ>> list = source1.OrderBy<Tuple<Element, XYZ>, double>((Func<Tuple<Element, XYZ>, double>)(x => x.Item2.Z)).ThenByDescending<Tuple<Element, XYZ>, double>((Func<Tuple<Element, XYZ>, double>)(x => x.Item2.X)).ThenBy<Tuple<Element, XYZ>, double>((Func<Tuple<Element, XYZ>, double>)(x => x.Item2.Y)).ToList<Tuple<Element, XYZ>>();
            Transaction transaction = new Transaction(document, "Zelus");
            transaction.Start();
            foreach (Tuple<Element, XYZ> tuple in list)
            {
                Element element1 = tuple.Item1;
                List<XYZ> source2 = new List<XYZ>();
                List<Tuple<Element, XYZ>> source3 = new List<Tuple<Element, XYZ>>();
                LocationCurve location1 = element1.Location as LocationCurve;
                Line curve1 = location1.Curve as Line;
                XYZ direction1 = curve1.Direction;
                XYZ origin1 = curve1.Origin;
                XYZ endPoint1 = ((Curve)curve1).GetEndPoint(0);
                XYZ endPoint2 = ((Curve)curve1).GetEndPoint(1);
                foreach (Element element2 in elementList2)
                {
                    XYZ origin2 = ((element2.Location as LocationCurve).Curve as Line).Origin;
                    source3.Add(Tuple.Create<Element, XYZ>(element2, new XYZ(Math.Abs(origin1.X - origin2.X), Math.Abs(origin1.Y - origin2.Y), Math.Abs(origin1.Z - origin2.Z))));
                }
                Element element3 = source3.OrderBy<Tuple<Element, XYZ>, double>((Func<Tuple<Element, XYZ>, double>)(x => x.Item2.Z)).ThenBy<Tuple<Element, XYZ>, double>((Func<Tuple<Element, XYZ>, double>)(y => y.Item2.X)).ThenBy<Tuple<Element, XYZ>, double>((Func<Tuple<Element, XYZ>, double>)(z => z.Item2.Y)).FirstOrDefault<Tuple<Element, XYZ>>().Item1;
                LocationCurve location2 = element3.Location as LocationCurve;
                Line curve2 = location2.Curve as Line;
                XYZ direction2 = curve2.Direction;
                XYZ origin3 = curve2.Origin;
                XYZ endPoint3 = ((Curve)curve2).GetEndPoint(0);
                XYZ endPoint4 = ((Curve)curve2).GetEndPoint(1);
                XYZ xyz1 = new XYZ(Math.Abs(endPoint1.X - endPoint3.X), Math.Abs(endPoint1.Y - endPoint3.Y), Math.Abs(endPoint1.Z - endPoint3.Z));
                XYZ xyz2 = new XYZ(Math.Abs(endPoint1.X - endPoint4.X), Math.Abs(endPoint1.Y - endPoint4.Y), Math.Abs(endPoint1.Z - endPoint4.Z));
                XYZ xyz3 = new XYZ(Math.Abs(endPoint2.X - endPoint3.X), Math.Abs(endPoint2.Y - endPoint3.Y), Math.Abs(endPoint2.Z - endPoint3.Z));
                XYZ xyz4 = new XYZ(Math.Abs(endPoint2.X - endPoint4.X), Math.Abs(endPoint2.Y - endPoint4.Y), Math.Abs(endPoint2.Z - endPoint4.Z));
                source2.Add(xyz1);
                source2.Add(xyz2);
                source2.Add(xyz3);
                source2.Add(xyz4);
                XYZ xyz5 = source2.OrderBy<XYZ, double>((Func<XYZ, double>)(x => x.X)).ThenBy<XYZ, double>((Func<XYZ, double>)(y => y.Y)).ThenBy<XYZ, double>((Func<XYZ, double>)(z => z.Z)).FirstOrDefault<XYZ>();
                if (xyz5 == xyz1)
                {
                    XYZ xyz6 = new XYZ(direction1.X * xyz1.X, direction1.Y * xyz1.Y, direction1.Z * xyz1.Z);
                    XYZ xyz7 = endPoint1.Add(xyz6);
                    Line bound1 = Line.CreateBound(endPoint2, xyz7);
                    location1.Curve = (Curve)bound1;
                    Line bound2 = Line.CreateBound(endPoint4, xyz7);
                    location2.Curve = (Curve)bound2;
                }
                if (xyz5 == xyz2)
                {
                    XYZ xyz8 = new XYZ(direction1.X * xyz2.X, direction1.Y * xyz2.Y, direction1.Z * xyz2.Z);
                    XYZ xyz9 = endPoint1.Add(xyz8);
                    Line bound3 = Line.CreateBound(endPoint2, xyz9);
                    location1.Curve = (Curve)bound3;
                    Line bound4 = Line.CreateBound(endPoint3, xyz9);
                    location2.Curve = (Curve)bound4;
                }
                if (xyz5 == xyz3)
                {
                    XYZ xyz10 = new XYZ(direction1.X * xyz3.X, direction1.Y * xyz3.Y, direction1.Z * xyz3.Z);
                    XYZ xyz11 = endPoint2.Add(xyz10);
                    Line bound5 = Line.CreateBound(endPoint1, xyz11);
                    location1.Curve = (Curve)bound5;
                    Line bound6 = Line.CreateBound(endPoint4, xyz11);
                    location2.Curve = (Curve)bound6;
                }
                if (xyz5 == xyz4)
                {
                    XYZ xyz12 = new XYZ(direction1.X * xyz4.X, direction1.Y * xyz4.Y, direction1.Z * xyz4.Z);
                    XYZ xyz13 = endPoint2.Add(xyz12);
                    Line bound7 = Line.CreateBound(endPoint1, xyz13);
                    location1.Curve = (Curve)bound7;
                    Line bound8 = Line.CreateBound(endPoint3, xyz13);
                    location2.Curve = (Curve)bound8;
                }
                Conduit conduit1 = tuple.Item1 as Conduit;
                ConnectorSet connectors1 = ((MEPCurve)conduit1).ConnectorManager.Connectors;
                Conduit conduit2 = element3 as Conduit;
                ConnectorSet connectors2 = ((MEPCurve)conduit2).ConnectorManager.Connectors;
                foreach (Connector connector1 in connectors1)
                {
                    foreach (Connector connector2 in connectors2)
                    {
                        try
                        {
                            connector1.ConnectTo(connector2);
                        }
                        catch
                        {
                        }
                    }
                }
                document.Create.NewElbowFitting(((MEPCurve)conduit1).ConnectorManager.Lookup(1), ((MEPCurve)conduit2).ConnectorManager.Lookup(1));
                elementList2.Remove(element3);
            }
            transaction.Commit();
            return Result.Succeeded;
        }
    }
}
