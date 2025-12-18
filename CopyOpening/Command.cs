using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CopyOpening.External_Event_Handlers;
using CopyOpening.MVVM.View;
using CopyOpening.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyOpening
{
    [Transaction(TransactionMode.Manual)]
    internal class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RunCopyOpeningExternalEventHandler placeCopyOpeningExternalEventHandler = new RunCopyOpeningExternalEventHandler();
            var ev = ExternalEvent.Create(placeCopyOpeningExternalEventHandler);

           var document = commandData.Application.ActiveUIDocument.Document;

            MainWindowViewModel viewModel = new MainWindowViewModel(document);
            var ui = MainWindow.CreateInstance(viewModel);



            placeCopyOpeningExternalEventHandler.MainviewModel = viewModel;
     
            viewModel.Ev = ev;
            
  
          
            ui.DataContext = viewModel;
            ui.ViewModel = viewModel;
            placeCopyOpeningExternalEventHandler.Mainview= ui;
         
            




            ui.Show();
            return Result.Succeeded;
        }
    }
}
