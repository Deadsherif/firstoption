// Decompiled with JetBrains decompiler
// Type: ViewsTransfer.DB
// Assembly: ViewsTransfer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 097D957F-A3C8-4D58-A2D2-2E99F75DDB65
// Assembly location: C:\Users\ahmed\Downloads\ViewsTransfer\ViewsTransfer\bin\Debug\ViewsTransfer.dll

using Autodesk.Revit.DB;
using System.Collections.Generic;


namespace ViewsTransfer
{
  internal class DB
  {
    public static Document DB_Doc = (Document) null;
    public static View DB_SrcView = (View) null;
    public static List<ElementId> DB_ElesId = new List<ElementId>();
    public static List<ElementId> DB_View = new List<ElementId>();
    public static List<ElementId> DB_ViewData = new List<ElementId>();
    public static List<ElementId> DB_SheetViews = new List<ElementId>();
    public static List<XYZ> DB_SheetViewsPoint = new List<XYZ>();
    public static List<ElementId> DBALL_Views = new List<ElementId>();
    public static List<ElementId> DBALL_TitleBlocks = new List<ElementId>();
    public static List<List<ElementId>> DBALL_View = new List<List<ElementId>>();
    public static List<List<ElementId>> DBALL_TitleBlock = new List<List<ElementId>>();
    public static List<List<ElementId>> DBALL_SheetViews = new List<List<ElementId>>();
    public static List<List<XYZ>> DBALL_SheetViewsPoint = new List<List<XYZ>>();
  }
}
