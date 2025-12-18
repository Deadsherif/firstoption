using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteViews
{
    public class Helper
    {
        public static ElementId GetViewFamilyTypeId(Document doc, ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.FloorPlan:
                    return GetViewFamilyTypeIdByViewFamily(doc, ViewFamily.FloorPlan);
                case ViewType.CeilingPlan:
                    return GetViewFamilyTypeIdByViewFamily(doc, ViewFamily.CeilingPlan);
                case ViewType.EngineeringPlan:
                    return GetViewFamilyTypeIdByViewFamily(doc, ViewFamily.StructuralPlan);
                case ViewType.AreaPlan:
                    return GetViewFamilyTypeIdByViewFamily(doc, ViewFamily.AreaPlan);

                default:
                    return ElementId.InvalidElementId; // Handle unknown ViewType
            }
        }
        public static ElementId GetViewFamilyTypeIdByViewFamily(Document doc, ViewFamily viewFamily)
        {
            ViewFamilyType viewFamilyType = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault(x => x.ViewFamily == viewFamily);

            return viewFamilyType?.Id ?? ElementId.InvalidElementId;
        }
        public static bool HasLevelInDocument(Document doc, Level LevelToCheck)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            List<Element> levelCollection = collector.OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().ToElements().ToList();

            Level lvl = levelCollection.Where(e => e.Name == LevelToCheck.Name).FirstOrDefault() as Level;
            if (lvl == null)
            {
                return false;
            }
            if (LevelToCheck.Name == lvl.Name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static Level GetLevelByName(Document doc, string Name)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            List<Element> levelCollection = collector.OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().ToElements().ToList();

            foreach (Level level in levelCollection)
            {
                if (level.Name == Name)
                {
                    return level;
                }
            }
            return null;
        }
    }
}
