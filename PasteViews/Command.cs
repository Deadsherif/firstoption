using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CopyViews.MVVM.Model;
using PasteViews.MVVM.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PasteViews
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public static UIDocument UiDoc;
        public static Document Doc;
        public static ObservableCollection<RevitViews> CopiedViews;
        public static PasteView frm;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UiDoc = commandData.Application.ActiveUIDocument;
            Doc = UiDoc.Document;
            if (CopyViews.Command.CopiedViews != null && (CopyViews.Command.CopiedViews.Count > 0))
            {
                CopiedViews = CopyViews.Command.CopiedViews;
                frm = new PasteView();
                frm.ShowDialog();

            }
            else
            {
                MessageBox.Show("There is no views to paste, Please copy views first using the 'Copy Views' button!", "First Option", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            }

            return Result.Succeeded;
        }
    }
}
