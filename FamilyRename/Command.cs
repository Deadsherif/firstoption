using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using FamilyRename.MVVM.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FamilyRename
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class Command : IExternalCommand
    {
        public static int flag;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;
            int num = (int)new Main().ShowDialog();
            switch (flag)
            {
                case 1:
                    this.F_Write(document);
                    flag = 0;
                    break;
                case 2:
                    this.F_Read(document);
                    flag = 0;
                    break;
            }
            return (Result)0;
        }
        public void F_Write(Document doc)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;
            string selectedPath = folderBrowserDialog.SelectedPath;
            string fileName = !(selectedPath[selectedPath.Length - 1].ToString() == "\\") ? selectedPath + "\\" + doc.Title + ".xls" : selectedPath + doc.Title + ".xls";
            
        }

        public void F_Read(Document doc)
        {
            Transaction transaction = new Transaction(doc);
            transaction.Start("Update Families Names");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                
            }
            transaction.Commit();
        }


    }
}
