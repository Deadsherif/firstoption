using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RenameSheets.MVVM.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RenameSheets
{
    [Transaction(TransactionMode.Manual)]

    public class Command : IExternalCommand
    {
        public static UIDocument UiDoc;
        public static Document Doc;
        public static ObservableCollection<View> SelectedViews;
        public static ObservableCollection<Parameter> SheetsParameters;
        public static List<ViewSheet> FinalViewSheets;
        public static RenameSheetsView frm;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UiDoc = commandData.Application.ActiveUIDocument;
            Doc = UiDoc.Document;

            List<ElementId> selectedSheetsId = UiDoc.Selection.GetElementIds().ToList();
            FinalViewSheets = new List<ViewSheet>();
            SheetsParameters = new ObservableCollection<Parameter>();
            if (selectedSheetsId.Count > 0)
            {
                foreach (ElementId sheetId in selectedSheetsId)
                {
                    if (Doc.GetElement(sheetId) is ViewSheet viewSheet)
                    {
                        FinalViewSheets.Add(viewSheet);
                    }
                }
                if (FinalViewSheets.Count > 0)
                {

                    foreach (ViewSheet vs in FinalViewSheets)
                    {
                        ObservableCollection<Parameter> Temp = GetInstanceParameterNames(vs);
                        foreach (Parameter p in Temp)
                        {
                            if (!DoesParamaterExist(SheetsParameters, p))
                            {
                                SheetsParameters.Add(p);
                            }

                        }

                    }

                    frm = new RenameSheetsView();
                    frm.ShowDialog();



                }
                else
                {
                    MessageBox.Show("There were no View Sheets selected in your selection", "First Option", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Please Select Sheets First!", "First Option", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            return Result.Succeeded;
        }

        public ObservableCollection<Parameter> GetInstanceParameterNames(Element element)
        {

            // Get the instance parameters of the Element
            ParameterSet parameters = element.Parameters;

            // Filter only instance parameters
            List<Parameter> instanceParameters = parameters.Cast<Parameter>().OrderBy(e => e.Definition.Name).ToList();

            ObservableCollection<Parameter> Result = new ObservableCollection<Parameter>();
            foreach (Parameter parameter in instanceParameters)
            {
                Result.Add(parameter);
            }
            return Result;
        }
        public bool DoesParamaterExist(ObservableCollection<Parameter> parameters, Parameter p)
        {
            foreach (Parameter parameter in parameters)
            {

                if (parameter.Definition.Name == p.Definition.Name)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
