using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BOSP.MVVM.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOSP
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;
            Category category = document.Settings.Categories.get_Item(BuiltInCategory.OST_PipeCurves);
            CategorySet categorySet = commandData.Application.Application.Create.NewCategorySet();
            categorySet.Insert(category);
            DefinitionFile definitionFile;
            try
            {
                definitionFile = commandData.Application.Application.OpenSharedParameterFile();
            }
            catch (Exception ex)
            {
                TaskDialog.Show(ex.Message, "not valid shared parameter file!");
                return Result.Failed;
            }
            if (definitionFile != null)
            {
                if (!Fabricator.Get_TXT_SearchResult(definitionFile.Filename, "19994AD6-E279-4114-A09B-946894590BCC"))
                {
                    Fabricator.AddNewLine(definitionFile.Filename, definitionFile.Groups.Size, "19994AD6-E279-4114-A09B-946894590BCC", "SBOP (ft)", "StartButtomOfPipeFT", "TEXT", "the offset from level to the pipe buttom");
                    definitionFile = commandData.Application.Application.OpenSharedParameterFile();
                }
                if (!Fabricator.Get_TXT_SearchResult(definitionFile.Filename, "29994AD6-E279-4114-A09B-946894590B8D"))
                {
                    Fabricator.AddNewLine(definitionFile.Filename, definitionFile.Groups.Size, "29994AD6-E279-4114-A09B-946894590B8D", "SBOP (m)", "StartButtomOfPipeM", "TEXT", "the offset from level to the pipe buttom");
                    definitionFile = commandData.Application.Application.OpenSharedParameterFile();
                }
                if (!Fabricator.Get_TXT_SearchResult(definitionFile.Filename, "39994AD6-E279-4114-A09B-946894590B8E"))
                {
                    Fabricator.AddNewLine(definitionFile.Filename, definitionFile.Groups.Size, "39994AD6-E279-4114-A09B-946894590B8E", "SBOP (mm)", "StartButtomOfPipeMM", "TEXT", "the offset from level to the pipe buttom");
                    definitionFile = commandData.Application.Application.OpenSharedParameterFile();
                }
                if (!Fabricator.Get_TXT_SearchResult(definitionFile.Filename, "59994AD6-E279-4114-A09B-946894590B8E"))
                {
                    Fabricator.AddNewLine(definitionFile.Filename, definitionFile.Groups.Size, "59994AD6-E279-4114-A09B-946894590B8E", "EBOP (ft)", "EndButtomOfPipeFT", "TEXT", "the offset from level to the pipe buttom");
                    definitionFile = commandData.Application.Application.OpenSharedParameterFile();
                }
                if (!Fabricator.Get_TXT_SearchResult(definitionFile.Filename, "69994AD6-E279-4114-A09B-946894590B8E"))
                {
                    Fabricator.AddNewLine(definitionFile.Filename, definitionFile.Groups.Size, "69994AD6-E279-4114-A09B-946894590B8E", "EBOP (m)", "EndButtomOfPipeM", "TEXT", "the offset from level to the pipe buttom");
                    definitionFile = commandData.Application.Application.OpenSharedParameterFile();
                }
                if (!Fabricator.Get_TXT_SearchResult(definitionFile.Filename, "79994AD6-E279-4114-A09B-946894590B8E"))
                {
                    Fabricator.AddNewLine(definitionFile.Filename, definitionFile.Groups.Size, "79994AD6-E279-4114-A09B-946894590B8E", "EBOP (mm)", "EndButtomOfPipeMM", "TEXT", "the offset from level to the pipe buttom");
                    definitionFile = commandData.Application.Application.OpenSharedParameterFile();
                }
            }
            else
                TaskDialog.Show("not found", "cant find the shared parameter file");
            foreach (DefinitionGroup group in definitionFile.Groups)
            {
                string name = group.Name;
                if (group.Name == "StartButtomOfPipeFT")
                {
                    IEnumerable<ExternalDefinition> externalDefinitions = ((IEnumerable)group.Definitions).Cast<ExternalDefinition>().Select<ExternalDefinition, ExternalDefinition>((Func<ExternalDefinition, ExternalDefinition>)(d => d));
                    using (Transaction transaction = new Transaction(document))
                    {
                        transaction.Start("Add BOP Shared Parameter");
                        foreach (ExternalDefinition externalDefinition in externalDefinitions)
                        {
                            if (((Definition)externalDefinition).Name == "SBOP (ft)")
                            {
                                InstanceBinding instanceBinding = commandData.Application.Application.Create.NewInstanceBinding(categorySet);
                                document.ParameterBindings.Insert((Definition)externalDefinition, (Binding)instanceBinding, BuiltInParameterGroup.PG_TEXT);
                                break;
                            }
                        }
                        transaction.Commit();
                    }
                }
                if (group.Name == "StartButtomOfPipeM")
                {
                    IEnumerable<ExternalDefinition> externalDefinitions = ((IEnumerable)group.Definitions).Cast<ExternalDefinition>().Select<ExternalDefinition, ExternalDefinition>((Func<ExternalDefinition, ExternalDefinition>)(d => d));
                    using (Transaction transaction = new Transaction(document))
                    {
                        transaction.Start("Add BOP Shared Parameter");
                        foreach (ExternalDefinition externalDefinition in externalDefinitions)
                        {
                            if (((Definition)externalDefinition).Name == "SBOP (m)")
                            {
                                InstanceBinding instanceBinding = commandData.Application.Application.Create.NewInstanceBinding(categorySet);
                                document.ParameterBindings.Insert((Definition)externalDefinition, (Binding)instanceBinding, BuiltInParameterGroup.PG_TEXT);
                                break;
                            }
                        }
                        transaction.Commit();
                    }
                }
                if (group.Name == "StartButtomOfPipeMM")
                {
                    IEnumerable<ExternalDefinition> externalDefinitions = ((IEnumerable)group.Definitions).Cast<ExternalDefinition>().Select<ExternalDefinition, ExternalDefinition>((Func<ExternalDefinition, ExternalDefinition>)(d => d));
                    using (Transaction transaction = new Transaction(document))
                    {
                        transaction.Start("Add BOP Shared Parameter");
                        foreach (ExternalDefinition externalDefinition in externalDefinitions)
                        {
                            if (((Definition)externalDefinition).Name == "SBOP (mm)")
                            {
                                InstanceBinding instanceBinding = commandData.Application.Application.Create.NewInstanceBinding(categorySet);
                                document.ParameterBindings.Insert((Definition)externalDefinition, (Binding)instanceBinding, BuiltInParameterGroup.PG_TEXT);
                                break;
                            }
                        }
                        transaction.Commit();
                    }
                }
                if (group.Name == "EndButtomOfPipeFT")
                {
                    IEnumerable<ExternalDefinition> externalDefinitions = ((IEnumerable)group.Definitions).Cast<ExternalDefinition>().Select<ExternalDefinition, ExternalDefinition>((Func<ExternalDefinition, ExternalDefinition>)(d => d));
                    using (Transaction transaction = new Transaction(document))
                    {
                        transaction.Start("Add BOP Shared Parameter");
                        foreach (ExternalDefinition externalDefinition in externalDefinitions)
                        {
                            if (((Definition)externalDefinition).Name == "EBOP (ft)")
                            {
                                InstanceBinding instanceBinding = commandData.Application.Application.Create.NewInstanceBinding(categorySet);
                                document.ParameterBindings.Insert((Definition)externalDefinition, (Binding)instanceBinding, BuiltInParameterGroup.PG_TEXT);
                                break;
                            }
                        }
                        transaction.Commit();
                    }
                }
                if (group.Name == "EndButtomOfPipeM")
                {
                    IEnumerable<ExternalDefinition> externalDefinitions = ((IEnumerable)group.Definitions).Cast<ExternalDefinition>().Select<ExternalDefinition, ExternalDefinition>((Func<ExternalDefinition, ExternalDefinition>)(d => d));
                    using (Transaction transaction = new Transaction(document))
                    {
                        transaction.Start("Add BOP Shared Parameter");
                        foreach (ExternalDefinition externalDefinition in externalDefinitions)
                        {
                            if (((Definition)externalDefinition).Name == "EBOP (m)")
                            {
                                InstanceBinding instanceBinding = commandData.Application.Application.Create.NewInstanceBinding(categorySet);
                                document.ParameterBindings.Insert((Definition)externalDefinition, (Binding)instanceBinding, BuiltInParameterGroup.PG_TEXT);
                                break;
                            }
                        }
                        transaction.Commit();
                    }
                }
                if (group.Name == "EndButtomOfPipeMM")
                {
                    IEnumerable<ExternalDefinition> externalDefinitions = ((IEnumerable)group.Definitions).Cast<ExternalDefinition>().Select<ExternalDefinition, ExternalDefinition>((Func<ExternalDefinition, ExternalDefinition>)(d => d));
                    using (Transaction transaction = new Transaction(document))
                    {
                        transaction.Start("Add BOP Shared Parameter");
                        foreach (ExternalDefinition externalDefinition in externalDefinitions)
                        {
                            if (((Definition)externalDefinition).Name == "EBOP (mm)")
                            {
                                InstanceBinding instanceBinding = commandData.Application.Application.Create.NewInstanceBinding(categorySet);
                                document.ParameterBindings.Insert((Definition)externalDefinition, (Binding)instanceBinding, BuiltInParameterGroup.PG_TEXT);
                                break;
                            }
                        }
                        transaction.Commit();
                    }
                }
            }
            IList<Element> elements1 = new FilteredElementCollector(document).WherePasses((ElementFilter)new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves)).ToElements();
            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("Update BOP");
                foreach (Element element in (IEnumerable<Element>)elements1)
                {
                    try
                    {
                        Parameter parameter1 = element.LookupParameter("Diameter");
                        Parameter parameter2 = element.LookupParameter("Middle Elevation");
                        Parameter parameter3 = element.LookupParameter("Start Middle Elevation");
                        Parameter parameter4 = element.LookupParameter("End Middle Elevation");
                        double num1 = parameter1.AsDouble();
                        parameter2.AsDouble();
                        double num2 = parameter3.AsDouble();
                        double num3 = parameter4.AsDouble();
                        Parameter parameter5 = element.LookupParameter("SBOP (ft)");
                        Parameter parameter6 = element.LookupParameter("SBOP (m)");
                        Parameter parameter7 = element.LookupParameter("SBOP (mm)");
                        Parameter parameter8 = element.LookupParameter("EBOP (ft)");
                        Parameter parameter9 = element.LookupParameter("EBOP (m)");
                        Parameter parameter10 = element.LookupParameter("EBOP (mm)");
                        double num4 = num2 - num1 / 2.0;
                        double num5 = num3 - num1 / 2.0;
                        parameter5.Set(Math.Round(num4, 1).ToString());
                        parameter6.Set(Math.Round(num4 * 0.3048, 1).ToString());
                        parameter7.Set(Math.Round(num4 * 0.3048 * 1000.0, 1).ToString());
                        parameter8.Set(Math.Round(num5, 1).ToString());
                        parameter9.Set(Math.Round(num5 * 0.3048, 1).ToString());
                        parameter10.Set(Math.Round(num5 * 0.3048 * 1000.0, 1).ToString());
                    }
                    catch
                    {
                    }
                }
                transaction.Commit();
            }
            return Result.Succeeded;
        }
    }
}
