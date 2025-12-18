using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ParameterWizard.MVVM.Model;
using ParameterWizard.MVVM.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParameterWizard 
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public static int ss;
        public static List<string> names;
        public static List<BuiltInCategory> ccc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument activeUiDocument = commandData.Application.ActiveUIDocument;
            Document document = activeUiDocument.Document;
            Application application = commandData.Application.Application;
            Autodesk.Revit.UI.Selection.Selection selection = activeUiDocument.Selection;
            Command.ccc = new List<BuiltInCategory>();
            Command.names = new List<string>();
            Reference reference;
            try
            {
                reference = selection.PickObject(ObjectType.Element);
            }
            catch 
            {
                return Result.Succeeded;
            }
            Element element = document.GetElement(reference.ElementId);
            bool instance = false;
            bool type = false;
            string ins = "";
            string ty = "";
            FamilyInstance familyInstance = (FamilyInstance)null;
            try
            {
                familyInstance = document.GetElement(reference.ElementId) as FamilyInstance;
            }
            catch
            {
            }
            bool shared;
            string sh;
            if (familyInstance != null)
            {
                shared = true;
                instance = true;
                type = true;
                sh = "For all " + ((Element)familyInstance).Category.Name + " category";
                ins = "For " + ((ElementType)familyInstance.Symbol).FamilyName;
                ty = "For " + ((ElementType)familyInstance.Symbol).FamilyName;
            }
            else
            {
                shared = true;
                sh = "For all " + element.Category.Name + " category";
            }
            int num = (int)new MainView(shared, sh, instance, ins, type, ty).ShowDialog();
            switch (Command.ss)
            {
                case 0:
                    using (List<string>.Enumerator enumerator = Command.names.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            string current = enumerator.Current;
                            this.AddFamilyParameter(document, reference.ElementId, current, true);
                         
                        }
                        break;
                    }
                case 1:
                    using (List<string>.Enumerator enumerator = Command.names.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            string current = enumerator.Current;
                            this.AddFamilyParameter(document, reference.ElementId, current, false);
                          
                        }
                        break;
                    }
                case 2:
                    using (List<string>.Enumerator enumerator = Command.names.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            string current = enumerator.Current;
                            this.AddSharedParameter(document, commandData, element.Category, current);
                        }
                        break;
                    }
            }
            return Result.Succeeded;
        }

        public bool AddSharedParameter(
          Document doc,
          ExternalCommandData commandData,
          Category cate,
          string Name)
        {
            Enum.GetValues(typeof(BuiltInCategory));
            commandData.Application.Application.Create.NewCategorySet();
            DefinitionFile definitionFile1;
            try
            {
                definitionFile1 = commandData.Application.Application.OpenSharedParameterFile();
            }
            catch (Exception ex)
            {
                TaskDialog.Show($"Not valid\n{ex.Message}", "not valid shared parameter file! try to create new one");
                return false;
            }
            if (definitionFile1 != null)
            {
                this.AddNewLine(definitionFile1.Filename, definitionFile1.Groups.Size, Name);
                DefinitionFile definitionFile2 = commandData.Application.Application.OpenSharedParameterFile();
                foreach (DefinitionGroup group in definitionFile2.Groups)
                {
                    string name = group.Name;
                    if (group.Name == string.Format("ParameterWizard{0}", (object)definitionFile2.Groups.Size))
                    {
                        IEnumerable<ExternalDefinition> externalDefinitions = (group.Definitions)
                            .Cast<ExternalDefinition>().
                            Select<ExternalDefinition, ExternalDefinition>((Func<ExternalDefinition, ExternalDefinition>)(d => d));
                        using (Transaction transaction = new Transaction(doc))
                        {
                            transaction.Start("Add Shared Parameter");
                            foreach (ExternalDefinition externalDefinition in externalDefinitions)
                            {
                                if (((Definition)externalDefinition).Name == Name)
                                {
                                    CategorySet categorySet = commandData.Application.Application.Create.NewCategorySet();
                                    try
                                    {
                                        Category category = cate;
                                        if (category != null)
                                            categorySet.Insert(category);
                                        InstanceBinding instanceBinding = commandData.Application.Application.Create.NewInstanceBinding(categorySet);
                                        doc.ParameterBindings.Insert((Definition)externalDefinition, (Binding)instanceBinding, BuiltInParameterGroup.PG_TEXT);
                                        break;
                                    }
                                    catch 
                                    {
                                        break;
                                    }
                                }
                            }
                            transaction.Commit();
                        }
                    }
                }
                return true;
            }
            TaskDialog.Show("not found", "cant find the shared parameter file");
            return false;
        }

        public void AddNewLine(string pathToFile, int size, string Name)
        {
            Random random = new Random();
            int num1 = random.Next(1, 9);
            int num2 = random.Next(1, 9);
            int num3 = random.Next(1, 9);
            int num4 = random.Next(1, 9);
            int num5 = random.Next(1, 9);
            string[] strArray = File.ReadAllLines(pathToFile);
            string str1 = pathToFile;
            string str2 = str1.Remove(str1.Length - 4, 4) + "n.txt";
            File.Create(str2).Dispose();
            string str3 = "";
            for (int index = 0; index < strArray.Length; ++index)
                str3 = str3 + strArray[index] + Environment.NewLine;
            string contents = str3 + string.Format("GROUP\t{0}\tParameterWizard{1}", (object)(size + 1), (object)(size + 1)) + Environment.NewLine + string.Format("PARAM\t8271EB25-C6F3-40F7-B962-DA8{0}{1}6{2}8{3}6E{4}\t{5}\tTEXT\t\t{6}\t1\tGenerated Parameter\t1\t0", (object)num1, (object)num2, (object)num3, (object)num4, (object)num5, (object)Name, (object)(size + 1));
            File.AppendAllText(str2, contents);
            File.Delete(pathToFile);
            File.Move(str2, pathToFile);
        }

        public bool Get_TXT_SearchResult(string pathToFile, string paramName)
        {
            bool txtSearchResult = false;
            foreach (string readAllLine in File.ReadAllLines(pathToFile))
            {
                if (readAllLine.Contains(paramName))
                {
                    txtSearchResult = true;
                    break;
                }
            }
            return txtSearchResult;
        }

        public void AddFamilyParameter(Document doc, ElementId ID, string Name, bool IsInstance)
        {
            Family family = (doc.GetElement(ID) as FamilyInstance).Symbol.Family;
            Document document = doc.EditFamily(family);
            FamilyManager familyManager = document.FamilyManager;
            Transaction transaction1 = new Transaction(document);
            transaction1.Start("_AddP");
            try
            {
                familyManager.AddParameter(Name, GroupTypeId.Text, SpecTypeId.String.Text, IsInstance);
            }
            catch 
            {
            }
            transaction1.Commit();
            Transaction transaction2 = new Transaction(document);
            transaction2.Start("_LIP");
            IFamilyLoadOptions ifamilyLoadOptions = (IFamilyLoadOptions)new familyLoadOptions();
            document.LoadFamily(doc, ifamilyLoadOptions);
            transaction2.Commit();
        }
    }
}
