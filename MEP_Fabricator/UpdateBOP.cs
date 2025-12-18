// Decompiled with JetBrains decompiler
// Type: MEP_Fabricator.UpdateBOP
// Assembly: MEP_Fabricator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8CFBF883-28B0-455B-8DCF-FF413BFD7A7C
// Assembly location: \\fs\Temp-Share\03 R&D\omar amen\New folder (4)\2020\MEP_Fabricator.dll

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

 
namespace MEP_Fabricator
{
  [Transaction(TransactionMode.Manual)]
  internal class UpdateBOP : IExternalCommand
  {
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
      Document document = commandData.Application.ActiveUIDocument.Document;
      Category category = document.Settings.Categories.get_Item((BuiltInCategory)(-2008044));
      CategorySet categorySet = commandData.Application.Application.Create.NewCategorySet();
      categorySet.Insert(category);
      DefinitionFile definitionFile;
      try
      {
        definitionFile = commandData.Application.Application.OpenSharedParameterFile();
      }
      catch (Exception ex)
      {
        TaskDialog.Show("Error", "not valid shared parameter file!");
        return (Result)(-1);
      }
      if (definitionFile != null)
      {
        if (!EV.Get_TXT_SearchResult(definitionFile.Filename, "49994AD6-E279-4114-A09B-946894590BCC"))
        {
          EV.AddNewLine(definitionFile.Filename, definitionFile.Groups.Size, "49994AD6-E279-4114-A09B-946894590BCC", "BOP (ft)", "ButtomOfPipeFT", "TEXT", "the offset from level to the pipe buttom");
          definitionFile = commandData.Application.Application.OpenSharedParameterFile();
        }
        if (!EV.Get_TXT_SearchResult(definitionFile.Filename, "49994AD6-E279-4114-A09B-946894590B8D"))
        {
          EV.AddNewLine(definitionFile.Filename, definitionFile.Groups.Size, "49994AD6-E279-4114-A09B-946894590B8D", "BOP (m)", "ButtomOfPipeM", "TEXT", "the offset from level to the pipe buttom");
          definitionFile = commandData.Application.Application.OpenSharedParameterFile();
        }
        if (!EV.Get_TXT_SearchResult(definitionFile.Filename, "49994AD6-E279-4114-A09B-946894590B8E"))
        {
          EV.AddNewLine(definitionFile.Filename, definitionFile.Groups.Size, "49994AD6-E279-4114-A09B-946894590B8E", "BOP (mm)", "ButtomOfPipeMM", "TEXT", "the offset from level to the pipe buttom");
          definitionFile = commandData.Application.Application.OpenSharedParameterFile();
        }
      }
      else
        TaskDialog.Show("not found", "cant find the shared parameter file");
      foreach (DefinitionGroup group in definitionFile.Groups)
      {
        string name = group.Name;
        if (group.Name == "ButtomOfPipeFT")
        {
          IEnumerable<ExternalDefinition> externalDefinitions = ((IEnumerable) group.Definitions).Cast<ExternalDefinition>().Select<ExternalDefinition, ExternalDefinition>((Func<ExternalDefinition, ExternalDefinition>) (d => d));
          using (Transaction transaction = new Transaction(document))
          {
            transaction.Start("Add BOP Shared Parameter");
            foreach (ExternalDefinition externalDefinition in externalDefinitions)
            {
              if (((Definition) externalDefinition).Name == "BOP (ft)")
              {
                InstanceBinding instanceBinding = commandData.Application.Application.Create.NewInstanceBinding(categorySet);
                document.ParameterBindings.Insert((Definition) externalDefinition, (Binding) instanceBinding, (BuiltInParameterGroup)(-5000123));
                break;
              }
            }
            transaction.Commit();
          }
        }
        if (group.Name == "ButtomOfPipeM")
        {
          IEnumerable<ExternalDefinition> externalDefinitions = ((IEnumerable) group.Definitions).Cast<ExternalDefinition>().Select<ExternalDefinition, ExternalDefinition>((Func<ExternalDefinition, ExternalDefinition>) (d => d));
          using (Transaction transaction = new Transaction(document))
          {
            transaction.Start("Add BOP Shared Parameter");
            foreach (ExternalDefinition externalDefinition in externalDefinitions)
            {
              if (((Definition) externalDefinition).Name == "BOP (m)")
              {
                InstanceBinding instanceBinding = commandData.Application.Application.Create.NewInstanceBinding(categorySet);
                document.ParameterBindings.Insert((Definition) externalDefinition, (Binding) instanceBinding, (BuiltInParameterGroup)(-5000123));
                break;
              }
            }
            transaction.Commit();
          }
        }
        if (group.Name == "ButtomOfPipeMM")
        {
          IEnumerable<ExternalDefinition> externalDefinitions = ((IEnumerable) group.Definitions).Cast<ExternalDefinition>().Select<ExternalDefinition, ExternalDefinition>((Func<ExternalDefinition, ExternalDefinition>) (d => d));
          using (Transaction transaction = new Transaction(document))
          {
            transaction.Start("Add BOP Shared Parameter");
            foreach (ExternalDefinition externalDefinition in externalDefinitions)
            {
              if (((Definition) externalDefinition).Name == "BOP (mm)")
              {
                InstanceBinding instanceBinding = commandData.Application.Application.Create.NewInstanceBinding(categorySet);
                document.ParameterBindings.Insert((Definition) externalDefinition, (Binding) instanceBinding, (BuiltInParameterGroup)(-5000123));
                break;
              }
            }
            transaction.Commit();
          }
        }
      }
      IList<Element> elements1 = new FilteredElementCollector(document).WherePasses((ElementFilter) new ElementCategoryFilter((BuiltInCategory)(-2008044))).ToElements();
      using (Transaction transaction = new Transaction(document))
      {
        transaction.Start("Update BOP");
        foreach (Element element in (IEnumerable<Element>) elements1)
        {
          try
          {
            Parameter parameter1 = element.LookupParameter("Diameter");
            Parameter parameter2 = element.LookupParameter("Middle Elevation");
            double num1 = parameter1.AsDouble();
            double num2 = parameter2.AsDouble();
            Parameter parameter3 = element.LookupParameter("BOP (ft)");
            Parameter parameter4 = element.LookupParameter("BOP (m)");
            Parameter parameter5 = element.LookupParameter("BOP (mm)");
            double num3 = num2 - num1 / 2.0;
            parameter3.Set(num3.ToString());
            parameter4.Set((num3 * 0.3048).ToString());
            parameter5.Set((num3 * 0.3048 * 1000.0).ToString());
          }
          catch (Exception ex)
          {
          }
        }
        transaction.Commit();
      }
      return (Result) 0;
    }
  }
}
