// Decompiled with JetBrains decompiler
// Type: MEP_Fabricator.MEP_NavisReportViewer
// Assembly: MEP_Fabricator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8CFBF883-28B0-455B-8DCF-FF413BFD7A7C
// Assembly location: \\fs\Temp-Share\03 R&D\omar amen\New folder (4)\2020\MEP_Fabricator.dll

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

 
namespace MEP_Fabricator
{
  [Transaction(TransactionMode.Manual)]
  internal class MEP_NavisReportViewer : IExternalCommand
  {
    public static int A_ID = 971009;
    public static int B_ID = 970917;
    public static int old1 = 0;
    public static int old2 = 0;

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
      Document document = commandData.Application.ActiveUIDocument.Document;
      List<int> a = new List<int>();
      List<int> b = new List<int>();
      bool flag = true;
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Filter = "Text|*.txt|All|*.*";
      openFileDialog.FilterIndex = 1;
      openFileDialog.Multiselect = false;
      if (openFileDialog.ShowDialog() != DialogResult.OK)
        return (Result)(-1);
      foreach (string readAllLine in File.ReadAllLines(openFileDialog.FileName))
      {
        if (readAllLine.Contains("Element ID:"))
        {
          if (flag)
          {
            a.Add(int.Parse(readAllLine.Remove(0, 12)));
            flag = false;
          }
          else
          {
            b.Add(int.Parse(readAllLine.Remove(0, 12)));
            flag = true;
          }
        }
      }
      ExternalEvent e = ExternalEvent.Create((IExternalEventHandler) new MEP_NavisReportViewer_EventHandler());
      new ClashManager(a.Count.ToString(), a, b, e).Show();
      return (Result) 0;
    }
  }
}
