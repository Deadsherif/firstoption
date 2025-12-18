using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyPlacer
{
    internal class DB
    {
        internal static List<Wall> walls;

        public static List<Element> Cads { get; internal set; }
        public static List<Element> RevitLinks { get; internal set; }
        public static IEnumerable Blocks { get; internal set; }
        public static ImportInstance SelectedCadLink { get; internal set; }
        public static RevitLinkInstance SelectedRevitLink { get; internal set; }
        public static string SelectedBlock { get; internal set; }
        public static IEnumerable FamilyTypes { get; internal set; }
        public static string SelectedType { get; internal set; }
        public static IEnumerable<Ceiling> Ceilings { get; internal set; }
        public static IEnumerable<Floor> Floors { get; internal set; }
        public static IEnumerable<Ceiling> FilteredCeiling { get; internal set; }
        public static List<Level> Levels { get; internal set; }
        public static Level SelectedCeilingLevel { get; internal set; }
        public static Level SelectedFloorLevel { get; internal set; }
        public static Level SelectedWallLevel { get; internal set; }
        public static Level SelectedPipeLevel { get; internal set; }
        public static List<PipeType> PipeTypes { get; internal set; }
        public static PipeType SelectedPipeType { get; internal set; }
        public static Level SelectedConduitLevel { get; internal set; }
        public static List<ConduitType> ConduitTypes { get; internal set; }
        public static ConduitType SelectedConduitType { get; internal set; }
        public static Level SelectedCabletrayLevel { get; internal set; }
        public static CableTrayType SelectedCabletrayType { get; internal set; }
        public static List<CableTrayType> CabletraysTypes { get; internal set; }
        public static List<DuctType> DuctTypes { get; internal set; }
        public static DuctType SelectedDuctType { get; internal set; }
        public static Level SelectedDuctLevel { get; internal set; }
        public static string[] Categories { get; internal set; }
        public static string SelectedCategory { get; internal set; }
        public static string ConduitDiameterValue { get; internal set; }
        public static string DuctWidth { get; internal set; }
        public static string DuctHeight { get; internal set; }
        public static string DuctBottomElevation { get; internal set; }
        public static string CableTrayHeight { get; internal set; }
        public static string CableTrayWidth { get; internal set; }
        public static string CableTrayBottomElevation { get; internal set; }
        public static string ConduitMiddleElevation { get; internal set; }
        public static string PipeDiameterValue { get; internal set; }
        public static string PipeOffset { get; internal set; }
        public static List<Element> Groups { get; internal set; }
        public static GroupType SelectedGroup { get; internal set; }
        public static List<WallType> WallTypes { get; internal set; }
        public static WallType SelectedWallType { get; internal set; }
    }
}
