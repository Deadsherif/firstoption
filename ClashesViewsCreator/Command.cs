using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ClashesViewsCreator.ExternalEventHandlers;
using ClashesViewsCreator.MVVM.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashesViewsCreator
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public static UIDocument UiDoc;
        public static Document Doc;
        public static List<int> Elements_Id_1;
        public static List<int> Elements_Id_2;
        public static List<int> AllIds;
        public static ExternalEvent select_Event;
        public static ExternalEvent createAll_Event;
        public static ExternalEvent createSimulation_Event;
        public static ExternalEvent ExportExcel_Event;
        public static ExternalEvent CreateFilter_Event;
        public static ExternalEvent Import_Event;
        public static ClashesViewCreatorView frm;
        public static bool IsOpen = false;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UiDoc = commandData.Application.ActiveUIDocument;
            Doc = UiDoc.Document;

            IExternalEventHandler externalEvent = new SelectFileEvent();
            select_Event = ExternalEvent.Create(externalEvent);

            IExternalEventHandler AllexternalEvent = new CreateEventHandeler();
            createAll_Event = ExternalEvent.Create(AllexternalEvent);

            IExternalEventHandler sim_ev = new CreateSimulationEvent();
            createSimulation_Event = ExternalEvent.Create(sim_ev);


            IExternalEventHandler ex_ev = new ExportToExcelEvent();
            ExportExcel_Event = ExternalEvent.Create(ex_ev);

            IExternalEventHandler fl_ev = new CreateFilterEventHandeler();
            CreateFilter_Event = ExternalEvent.Create(fl_ev);

            IExternalEventHandler im_ev = new ImportFamilyEventHandeler();
            Import_Event = ExternalEvent.Create(im_ev);
            if (!IsOpen)
            {
                IsOpen = true;
                frm = new ClashesViewCreatorView();
                frm.Show();

            }

            return Result.Succeeded;

        }
       
    }
}
