using Autodesk.Revit.DB;
using COT.MVVM.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COT.MVVM.Model
{
   public class ApplicationStatic_DB
    {
        public static List<double> ConduitsData;
        public static string conduitTypeName;
        public static Element conduitType;
        public static List<Element> conduitTypes;
        public static double TrayThickness;
        public static bool firstTraySpacingCalculation;
        public static bool shiftToTrayBottom;
        public static bool withFittings;
        public static bool justifyFittings;
        public static COT_Form_Main MainForm;
        public static List<CurveWithData> SortedTrays;
        public static MEPCurve cutomTray;
        public static Curve customReferrenceAnchor;
        public static List<MEPCurve> cutomConds;
        public static List<XYZ> points = new List<XYZ>();

    }
}
