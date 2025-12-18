using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ModelHealthChecker.ExternalEventHandlers;
using ModelHealthChecker.MVVM.View;
using ModelHealthChecker.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelHealthChecker
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public static UIDocument UiDoc;
        public static Document Doc;
        public static CheckerView frm;
        public static ExternalEvent WarningEventHandler;
        public static ExternalEvent FamilyElementsEventHandeler;
        public static ExternalEvent DeleteEventHandeler;
        public static ExternalEvent ShowDupEventHandeler;
        public static ExternalEvent DelDupEventHandeler;
        public static ExternalEvent IsolateLinkEventHandeler;
        public static ExternalEvent DeleteLinksEventHandeler;
        public static CheckerViewModel VM;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UiDoc = commandData.Application.ActiveUIDocument;
            Doc = UiDoc.Document;

            IExternalEventHandler ShowWarningEventHandeler = new ShowWarningsEventHandler();
            WarningEventHandler = ExternalEvent.Create(ShowWarningEventHandeler);

            IExternalEventHandler ShowElementsEventHandeler = new ShowFamilyElementsEventHandeler();
            FamilyElementsEventHandeler = ExternalEvent.Create(ShowElementsEventHandeler);

            IExternalEventHandler deleteFamilyEventHandeler = new DeleteFamilyEventHandeler();
            DeleteEventHandeler = ExternalEvent.Create(deleteFamilyEventHandeler);

            IExternalEventHandler ShowDupElementsEH = new ShowDuplicatedElementsEventHandeler();
            ShowDupEventHandeler = ExternalEvent.Create(ShowDupElementsEH);

            IExternalEventHandler DelDupElementsEH = new DeleteDupEventHandeler();
            DelDupEventHandeler = ExternalEvent.Create(DelDupElementsEH);

            IExternalEventHandler IsolateLinksEH = new IsolateLinkEventHandeler();
            IsolateLinkEventHandeler = ExternalEvent.Create(IsolateLinksEH);

            IExternalEventHandler DeleteLinksEH = new DeleteLinksEventHandeler();
            DeleteLinksEventHandeler = ExternalEvent.Create(DeleteLinksEH);



            VM = new CheckerViewModel(Doc);

            frm = CheckerView.CreateInstance(VM);
            frm.Show();

            return Result.Succeeded;
        }
    }
}
