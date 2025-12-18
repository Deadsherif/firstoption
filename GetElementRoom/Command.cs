using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using GetElementRoom.External_Event_Handlers;
using GetElementRoom.MVVM.View;
using GetElementRoom.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetElementRoom
{
    [Transaction(TransactionMode.Manual)]
    internal class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //export stl 
            RunGetElementRoomExternalEventHandler GetElementRoomExternalEventHandler = new RunGetElementRoomExternalEventHandler();
            var ev = ExternalEvent.Create(GetElementRoomExternalEventHandler);

            var document = commandData.Application.ActiveUIDocument.Document;
            MainWindowViewModel viewModel = new MainWindowViewModel( document);
            var ui = MainWindow.CreateInstance(viewModel); 

            GetElementRoomExternalEventHandler.MainviewModel = viewModel;
     
            viewModel.Ev = ev;
         
  
          
            ui.DataContext = viewModel;
            ui.ViewModel = viewModel;
            GetElementRoomExternalEventHandler.Mainview= ui;
         
            




            ui.Show();
            return Result.Succeeded;
        }
    }
}
