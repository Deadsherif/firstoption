using Autodesk.Revit.UI;
using ClashesViewsCreator.MVVM.Model;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace ClashesViewsCreator.ExternalEventHandlers
{
    public class ExportToExcelEvent : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            ObservableCollection<FO_Num> ToExcel = Command.frm.VM.ClashedIds;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files|*.xlsx";
            saveFileDialog.Title = "Save Excel File";
            saveFileDialog.FileName = "output";
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            if (saveFileDialog.ShowDialog() == true)
            {
                CreateExcelFile(ToExcel, saveFileDialog.FileName);
            }
        }

        public string GetName()
        {
            return "Mostafa";
        }

        static void CreateExcelFile(ObservableCollection<FO_Num> foNumList, string filePath)
        {
            try
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("FO_Num Data");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Number";
                    worksheet.Cells[1, 2].Value = "Number Of Duplication";

                    // Add data
                    for (int i = 0; i < foNumList.Count; i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = foNumList[i].Number;
                        worksheet.Cells[i + 2, 2].Value = foNumList[i].NumOfDuplication;
                    }

                    // Save the Excel package to a file
                    FileInfo fileInfo = new FileInfo(filePath);
                    package.SaveAs(fileInfo);
                }

                MessageBox.Show($"Excel file '{filePath}' created successfully.", "FirstOption", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
            }
        }
    }
}
