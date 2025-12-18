using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using SplitWalls.SelectionFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SplitWalls
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument UiDoc = commandData.Application.ActiveUIDocument;
            Document Doc = commandData.Application.ActiveUIDocument.Document;
            bool AnyWall = false;

            #region MyRegion
            TaskDialog.Show("First Option", "Select all walls you want to split their layers then press finish.", TaskDialogCommonButtons.Ok);
            WallsSelectionFilter wallFilter = new WallsSelectionFilter();
            List<Reference> reffEle = new List<Reference>();
            try
            {
                reffEle = UiDoc.Selection.PickObjects(ObjectType.Element, wallFilter).ToList();
            }
            catch
            { }

            foreach (var item in reffEle)
            {

                try
                {

                    List<WallType> NewWallTypesCreated = new List<WallType>(); // Storage List for later
                    List<Wall> NewWallsCreated = new List<Wall>();  // Storage List for later

                    Element e = Doc.GetElement(item);

                    Wall wall = e as Wall;


                    NewWallsCreated.Add(wall);

                    string OriginalWallTypeName = wall.WallType.Name;

                    List<CompoundStructureLayer> WallLayers = GetWallLayers(wall);

                    // Only split walls with more than one layer.
                    if (WallLayers.Count <= 1)
                    {
                        continue;
                    }

                    foreach (CompoundStructureLayer Layer in WallLayers)
                    {
                        List<CompoundStructureLayer> LayersToKeep = new List<CompoundStructureLayer>();

                        if (Layer.Width > 0.001)
                        {
                            LayersToKeep.Add(Layer);
                        }
                        else
                        {
                            LayersToKeep.Add(Layer);    // If a layer width is 0 I want it to be included with the next layer
                            continue;
                        }

                        MakeWallLocationLineToFinishExterior(Doc, wall);
                        WallType newWallType = null;

                        // Now we Create a new WallType if it does not exist
                        try
                        {
                            newWallType = CreateNewWallType(Doc, wall, LayersToKeep, $"{OriginalWallTypeName}-{Doc.GetElement(Layer.MaterialId).Name}-{Math.Round(Layer.Width / 0.00328084, 0)}mm");
                            NewWallTypesCreated.Add(newWallType);
                        }
                        catch
                        {
                            newWallType = new FilteredElementCollector(Doc).OfClass(typeof(WallType))
                           .FirstOrDefault(type => type.Name == $"{OriginalWallTypeName}-{Doc.GetElement(Layer.MaterialId).Name}-{Math.Round(Layer.Width / 0.00328084, 0)}mm") as WallType;

                            NewWallTypesCreated.Add(newWallType);
                        }

                        //For the first loop only , a walltype has been created for The exterior layer , So we assign the Walltype of the original wall to it.
                        if (WallLayers.IndexOf(Layer) == 0)
                        {
                            using (Transaction trans = new Transaction(Doc, "Apply new Wall Type to original wall"))
                            {
                                trans.Start();
                                wall.WallType = NewWallTypesCreated.FirstOrDefault();
                                trans.Commit();
                            }
                        }

                    }

                    MakeWallLocationLineToFinishInterior(Doc, wall);

                    double DistanceToOffset = NewWallTypesCreated.FirstOrDefault().Width / 2;

                    Curve MainWallCurve = (wall.Location as LocationCurve).Curve;

                    Curve NewWallCurve = null;

                    NewWallTypesCreated.RemoveAt(0);

                    // Create a wall for each created walltype
                    foreach (WallType Type in NewWallTypesCreated)
                    {
                        DistanceToOffset = DistanceToOffset + Type.Width / 2;
                        if (wall.Flipped)
                        {

                            NewWallCurve = MainWallCurve.CreateOffset(DistanceToOffset, -XYZ.BasisZ);
                        }
                        else
                        {
                            NewWallCurve = MainWallCurve.CreateOffset(DistanceToOffset, XYZ.BasisZ);

                        }
                        Element NewWall;
                        using (Transaction trans = new Transaction(Doc, "Create New Wall"))
                        {
                            trans.Start();
                            NewWall = Wall.Create(Doc, NewWallCurve, wall.LevelId, false);
                            NewWallsCreated.Add(NewWall as Wall);
                            (NewWall as Wall).WallType = Type;


                            trans.Commit();
                        }

                        MakeWallLocationLineToFinishInterior(Doc, NewWall as Wall);

                        MainWallCurve = (NewWall.Location as LocationCurve).Curve;
                        DistanceToOffset = Type.Width / 2;
                        AdjustWallConstraints(wall, NewWall as Wall, Doc);



                    }

                    // Join the layers 
                    for (int i = 0; i < NewWallsCreated.Count - 1; i++)
                    {
                        Wall w1 = NewWallsCreated[i];
                        Wall w2 = NewWallsCreated[i + 1];

                        using (Transaction transaction = new Transaction(Doc, "Join Elements"))
                        {
                            transaction.Start();

                            // Attempt to join the elements
                            JoinGeometryUtils.JoinGeometry(Doc, w1, w2);

                            // Commit the transaction
                            transaction.Commit();
                        }
                    }

                    AnyWall = true;
                }
                catch
                {


                }

            }
            #endregion


            if (AnyWall)
                MessageBox.Show("Selected Walls have been seperated", "FirstOption", MessageBoxButton.OK, MessageBoxImage.Information);

            else
            {
                MessageBox.Show("No Walls have been seperated", "FirstOption", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }

            return Result.Succeeded;

        }



        private static void MakeWallLocationLineToFinishExterior(Document doc, Wall wall)
        {
            try
            {
                using (Transaction trans = new Transaction(doc, "Change"))
                {
                    trans.Start();


                    BuiltInParameter parameterName = BuiltInParameter.WALL_KEY_REF_PARAM;

                    Parameter LocationLineParameter = wall.get_Parameter(parameterName);

                    if (LocationLineParameter.AsInteger() != 2)
                    {
                        LocationLineParameter.Set(2);
                    }


                    trans.Commit();

                }
            }
            catch
            {

                throw;
            }

        }

        private static void MakeWallLocationLineToFinishInterior(Document doc, Wall wall)
        {
            try
            {
                using (Transaction trans = new Transaction(doc, "Change"))
                {
                    trans.Start();


                    BuiltInParameter parameterName = BuiltInParameter.WALL_KEY_REF_PARAM;

                    Parameter LocationLineParameter = wall.get_Parameter(parameterName);

                    if (LocationLineParameter.AsInteger() != 3)
                    {
                        LocationLineParameter.Set(3);
                    }


                    trans.Commit();

                }
            }
            catch
            {

                throw;
            }

        }

        private static WallType CreateNewWallType(Document Doc, Wall wall, List<CompoundStructureLayer> LayersToKeep, string NameOfNewWallType)
        {
            WallType oldWallType = wall.WallType;
            WallType newWallType = default;

            using (Transaction trans = new Transaction(Doc, "CreatingWallType"))
            {
                try
                {
                    trans.Start();
                    newWallType = oldWallType.Duplicate(NameOfNewWallType) as WallType;
                    CompoundStructure compoundStructure = newWallType.GetCompoundStructure();
                    IList<CompoundStructureLayer> NewLayers = compoundStructure.GetLayers();
                    int NumOfLayers = NewLayers.Count;
                    compoundStructure.SetLayers(LayersToKeep);
                    var NewLayerOfDuplicatedWall = compoundStructure.GetLayers();
                    compoundStructure.SetLayers(NewLayerOfDuplicatedWall);
                    newWallType.SetCompoundStructure(compoundStructure);
                    trans.Commit();
                }
                catch (Exception)
                {

                    throw;
                }
            }

            return newWallType;



        }

        private static void AdjustWallConstraints(Wall MainWall, Wall WallToAdjust, Document Doc)
        {
            ElementId BaseConstraint = MainWall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId();
            ElementId TopConstraint = MainWall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId();
            double TopUnconeccted = MainWall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble();

            double BaseOffset = MainWall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble();
            double TopOffset = MainWall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).AsDouble();


            int Structural = MainWall.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).AsInteger();

            using (Transaction transaction = new Transaction(Doc, "Adjust New Wall Parameters"))
            {
                transaction.Start();
                try
                {

                    WallToAdjust.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).Set(BaseConstraint);
                    WallToAdjust.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(BaseOffset);

                    if (TopConstraint.IntegerValue > 0)
                    {
                        WallToAdjust.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(TopConstraint);
                        WallToAdjust.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).Set(TopOffset);
                    }
                    else
                    {
                        WallToAdjust.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(TopConstraint);
                        WallToAdjust.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).Set(TopUnconeccted);

                    }
                    WallToAdjust.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).Set(Structural);
                }
                catch
                {
                    throw;
                }

                transaction.Commit();
            }


        }

        private static List<CompoundStructureLayer> GetWallLayers(Wall wall)
        {
            CompoundStructure compoundStructure = wall.WallType.GetCompoundStructure();
            return compoundStructure.GetLayers().ToList();

        }
    }

}
