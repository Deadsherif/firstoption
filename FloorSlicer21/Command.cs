using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FloorSlicer21.MVVM.Models;
using FloorSlicer21.SelectionFilters;
using Autodesk.Revit.Attributes;

namespace FloorSlicer21
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument UiDoc = commandData.Application.ActiveUIDocument;
            Document Doc = commandData.Application.ActiveUIDocument.Document;
            FloorsSelectionFilter dimFilter = new FloorsSelectionFilter();
            List<Reference> reffEle = new List<Reference>();

            try
            {
                reffEle = UiDoc.Selection.PickObjects(ObjectType.Element, dimFilter, "Select Floors").ToList();
            }
            catch (Exception)
            {

                return Result.Succeeded;
            }
            List<Element> SelectedElements = new List<Element>();
            foreach (var reff in reffEle)
            {
                SelectedElements.Add(Doc.GetElement(reff));
            }

            int FloorCount = 0;
            foreach (Element floor in SelectedElements)
            {
                if (floor is Floor)
                {
                    try
                    {
                        List<CompoundStructureLayer> FloorLayers = Helper.GetFloorLayers(Doc, floor);

                        if (FloorLayers.Count > 1)
                        {
                            int WideLayersCounter = 0;
                            foreach (CompoundStructureLayer layer in FloorLayers)
                            {
                                if (layer.MaterialId.IntegerValue == -1)
                                {
                                    MessageBox.Show($"A Floor you selected has a layer that has no assigned material\n\nFloor Type :{(floor as Floor).FloorType.Name}", "FirstOption", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                    return Result.Succeeded;
                                }

                                if (layer.Width > 0.001)
                                {
                                    WideLayersCounter++;
                                }
                            }

                            if (WideLayersCounter <= 1)
                            {
                                continue;
                            }

                            string OriginalFloorTypeName = (floor as Floor).FloorType.Name;
                            List<FloorType> NewTypesCreated = new List<FloorType>();
                            List<Floor> NewFloors = new List<Floor>();

                            List<CompoundStructureLayer> LayersToKeep = new List<CompoundStructureLayer>();

                            foreach (CompoundStructureLayer Layer in FloorLayers)
                            {

                                if (Layer.Width > 0.001)
                                {
                                    LayersToKeep.Add(Layer);
                                }
                                else
                                {
                                    LayersToKeep.Add(Layer);    // If a layer width is 0 I want it to be included with the next layer
                                    continue;
                                }

                                FloorType NewFloorType = null;
                                try
                                {
                                    NewFloorType = Helper.CreateNewFloorType(Doc, floor, LayersToKeep, $"{OriginalFloorTypeName}-{Doc.GetElement(Layer.MaterialId).Name}-{Math.Round(Layer.Width / 0.00328084, 0)}mm");
                                    NewTypesCreated.Add(NewFloorType);
                                }
                                catch
                                {
                                    NewFloorType = new FilteredElementCollector(Doc).OfClass(typeof(FloorType))
                                           .FirstOrDefault(type => type.Name == $"{OriginalFloorTypeName}-{Doc.GetElement(Layer.MaterialId).Name}-{Math.Round(Layer.Width / 0.00328084, 0)}mm") as FloorType;
                                    NewTypesCreated.Add(NewFloorType);

                                }
                                LayersToKeep = new List<CompoundStructureLayer>();
                            }





                            #region Old
                            List<CurveArray> Loops = new List<CurveArray>(); // Turn in to List<List<CurveArray>>


                            //    //We create a new floor , it is created to the first loop in "Loops" which is the outter loop .
                            using (Transaction transaction = new Transaction(Doc, "Create New Floor"))
                            {
                                transaction.Start();
                                Loops = Helper.GetFloorCurveArray(Doc, floor);
                                CurveArray MainLoop = Loops.FirstOrDefault();
                                (floor as Floor).FloorType = NewTypesCreated.FirstOrDefault();
                                double FirstLayerThickness = Helper.FloorThickness(floor, Doc);
                                double HeightOffsetValue = floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).AsDouble();

                                //convert curve array to curve loop 
                                List<Curve> curves = new List<Curve>();
                                foreach (Curve curve in MainLoop)
                                {
                                    curves.Add(curve);
                                }

                               var curveLoop =  CurveLoop.Create(curves);
                                var listLoop = new List<CurveLoop>() { curveLoop };


                                double ComulativeWidth = 0;
                                for (int i = 1; i < NewTypesCreated.Count; i++)
                                {
                                   
                                    //Floor NewFloor = Doc.Create.NewFloor(MainLoop, NewTypesCreated[i], Doc.GetElement(floor.LevelId) as Level, false);
                                    Floor NewFloor = Floor.Create(Doc, listLoop, NewTypesCreated[i].Id, floor.LevelId);
                                    Parameter NewOffset = NewFloor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM);
                                    NewOffset.Set(-FirstLayerThickness + HeightOffsetValue - ComulativeWidth);
                                    ComulativeWidth = ComulativeWidth + Helper.FloorThickness(NewFloor, Doc);
                                    NewFloors.Add(NewFloor);
                                }

                                transaction.Commit();
                            }

                            //    //Now for the rest of loops we create openinigs in the previously created floor
                            foreach (Floor f in NewFloors)
                            {
                                using (Transaction transaction = new Transaction(Doc, "Create New Floor"))
                                {
                                    transaction.Start();


                                    for (int j = 1; j < Loops.Count; j++)
                                    {
                                        Doc.Create.NewOpening(f, Loops[j], true);
                                    }


                                    transaction.Commit();
                                }
                            }

                            //    //Join the layers
                            for (int i = 0; i < NewFloors.Count - 1; i++)
                            {

                                Floor w1 = NewFloors[i];
                                Floor w2 = NewFloors[i + 1];

                                using (Transaction transaction = new Transaction(Doc, "Join Elements"))
                                {
                                    transaction.Start();

                                    // Attempt to join the elements
                                    JoinGeometryUtils.JoinGeometry(Doc, w1, w2);

                                    // Commit the transaction
                                    transaction.Commit();
                                }
                            }
                            FloorCount++;

                            #endregion
                        }
                    }
                    catch (Exception e)
                    {

                        MessageBox.Show(e.Message);
                    }

                }
            }

            MessageBox.Show($"{FloorCount} Floors have been sliced", "FirstOption", MessageBoxButton.OK, MessageBoxImage.Information);


            return Result.Succeeded;
        }



    }
}
