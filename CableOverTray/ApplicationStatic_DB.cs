// Decompiled with JetBrains decompiler
// Type: CableOverTray.ApplicationStatic_DB
// Assembly: COT, Version=2021.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 774E24BD-3C4D-44CA-9437-58E3900DE92B
// Assembly location: C:\Users\ahmed\AppData\Roaming\Autodesk\Revit\Addins\2023\CableOverTray.dll

using Autodesk.Revit.DB;
using System.Collections.Generic;

 
namespace CableOverTray
{
  internal class ApplicationStatic_DB
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
  }
}
