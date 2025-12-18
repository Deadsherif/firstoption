using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using GetElementRoom.MVVM.ViewModel;
using System.Windows;
using System.Linq.Expressions;
using System.Windows.Controls;
using System.Xml.Linq;
using Autodesk.Revit.DB.Mechanical;

using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Threading;
using GetElementRoom.MVVM.View;
using System.Threading;
using System.Windows.Documents;

using System.Data;
using Microsoft.Win32;
using System.IO.Packaging;
using System.Diagnostics;
using static GetElementRoom.Helper;
using System.Security.Cryptography;
using System.Windows.Media.Media3D;
using Autodesk.Revit.Creation;
using Document = Autodesk.Revit.DB.Document;
using Autodesk.Revit.DB.Architecture;
using System.Windows.Media;
using Autodesk.Revit.DB.Plumbing;

namespace GetElementRoom.External_Event_Handlers
{
    internal class RunGetElementRoomExternalEventHandler : IExternalEventHandler
    {

        public MainWindowViewModel MainviewModel { get; set; }
        public MainWindow Mainview { get; set; }
        public RunGetElementRoomExternalEventHandler()
        {

        }

        public void Execute(UIApplication app)
        {

            var doc = app.ActiveUIDocument.Document;

            var view = doc.ActiveView as View3D;
            //var revitLinkInstance = new FilteredElementCollector(doc)
            //     .OfClass(typeof(RevitLinkInstance)).Cast<RevitLinkInstance>().Where(X=>X.Name.Contains("ARC")).ToList().FirstOrDefault();
            var revitLinkInstance = MainviewModel.SelectedLink;
            if (revitLinkInstance == null) return;
            var linkDoc = revitLinkInstance.GetLinkDocument();
            if (view == null)
            {
                MessageBox.Show("Please Open 3d View");
                return;
            }
            var elements = GetModelElements(doc, view);
            Transaction tx = new Transaction(doc, "set cobie space name parameter");
            tx.Start();
            Parallel.ForEach(elements ,element=>
            {

                try
                {
                    var pointToCheck = element.Location as LocationPoint;
                    if (pointToCheck != null)
                    {
                        var conversion = double.TryParse(MainviewModel.Tolerance, out double Tolerance);
                        var room = CheckPointInRoomOrSpace(doc, revitLinkInstance, pointToCheck.Point, Tolerance);
                        if (room !=null)
                        {

                            var Name = room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();
                            var Number = room.get_Parameter(BuiltInParameter.ROOM_NUMBER).AsString();


                            //var CobieSpaceName = room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();
                            //var BuildingType = room.LookupParameter("Building Type").AsString();
                            //var LevelCode = room.LookupParameter("Level code").AsString(); 
                            //var CobieSpaceNumber = room.Number;



                            var AssetLocation = element.LookupParameter("ASSET_LOCATION");
                            if (AssetLocation != null)
                                AssetLocation.Set($"{Number}-{Name}");
                        }

                    }
                    

                    //var cobieParameterDesc = element.LookupParameter("COBie.Space.Description");
                    //var cobieParameterName = element.LookupParameter("COBie.Space.Name");
                    //if (cobieParameterDesc != null)
                    //    cobieParameterDesc.Set(CobieSpaceName);
                    //if (cobieParameterName != null)
                    //{ var x = cobieParameterName.Set($"{BuildingType}.{LevelCode}.{CobieSpaceNumber}."); }
                }
                catch (Exception)
                {


                }


            });
            tx.Commit();
        }
        public string GetName() => "Run Tool";

        public Room CheckPointInRoomOrSpace(Document doc, RevitLinkInstance revitLinkInstance, XYZ pointToCheck,double Tolerance)
        {
            var linkDoc = revitLinkInstance.GetLinkDocument();
            // Get all rooms
            FilteredElementCollector collector = new FilteredElementCollector(linkDoc);
            ICollection<Element> rooms = collector.OfClass(typeof(SpatialElement)).OfCategory(BuiltInCategory.OST_Rooms).ToElements();
            if (rooms.Count == 0) return null;
            foreach (SpatialElement room in rooms)
            {
                // Get the room's location point
                LocationPoint roomLocation = room.Location as LocationPoint;
                if (roomLocation != null)
                {
                    XYZ roomPoint = roomLocation.Point;

                    // Check if the point is inside the room
                    if (IsPointInsideRoom(doc, pointToCheck, revitLinkInstance, room, Tolerance))
                    {
                        return room as Room;

                    }
                }
            }



            return null;
        }

        private bool IsPointInsideRoom(Document doc, XYZ point,RevitLinkInstance revitLinkInstance, SpatialElement roomOrSpace,double Tolerance)
        {

            var trns = revitLinkInstance.GetTotalTransform();
            var linkDoc = revitLinkInstance.GetLinkDocument();

            var linkedProjectBasePoint = BasePoint.GetProjectBasePoint(linkDoc).Position;

            var actualBasePointLocation = trns.OfPoint(linkedProjectBasePoint);
            XYZ LinkedDocBasePoint = BasePoint.GetProjectBasePoint(linkDoc).Position;
            XYZ CurrentDocBasePoint = BasePoint.GetProjectBasePoint(doc).Position;
            var TransalationVector = CurrentDocBasePoint - trns.Origin;


            var bb = roomOrSpace.get_BoundingBox(doc.ActiveView);
            if (bb == null) return false;
            bb.Min = bb.Min + TransalationVector;
            bb.Max = bb.Max + TransalationVector;

            if (bb == null) return false;
            Outline outline = new Outline(bb.Min, new XYZ(bb.Max.X, bb.Max.Y, bb.Max.Z + (Tolerance* 0.00328084)));

            //BoundingBoxIntersectsFilter intersectsFilter = new BoundingBoxIntersectsFilter(outline);
            //intersectsFilter.
            bool isinside = false;
            var x = point.X;
            var y = point.Y;
            var z = point.Z;

            if (x <= bb.Max.X && x >= bb.Min.X && y <= bb.Max.Y && y >= bb.Min.Y && z <= bb.Max.Z + (Tolerance * 0.00328084) && z >= bb.Min.Z)
            {
                isinside = true;
            }
            return isinside;

        }


    }
}
