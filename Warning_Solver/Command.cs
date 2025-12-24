using AuthService;
using AuthService.Services;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warning_Solver.ExternalEvenHandelers;
using Warning_Solver.MVVM.Model;
using Warning_Solver.MVVM.View;


namespace Warning_Solver
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {

        public static WarningSolverView ProcessWin = null;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {            
            var valid = Auth.ValidateAuth("Warning_Solver","9");            
            if (valid == SubscriptionService.SubscriptionStatus.Valid)
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                commandData.Application.Application.FailuresProcessing += new EventHandler<FailuresProcessingEventArgs>(DoFailureProcessing);
                Document doc = uidoc.Document;
                var warnings = doc.GetWarnings();
                Database.Warnings = warnings;
                Transaction tr = new Transaction(doc, "Solve Warnings");
                tr.Start();
                //uidoc.Selection.PickObject(ObjectType.Element);
                Solve_EventHandeler solve = new Solve_EventHandeler();
                ExternalEvent ev = ExternalEvent.Create(solve);
                ProcessWin = new WarningSolverView(ev, commandData);
                ProcessWin.Show();
                tr.Commit();
            }
            return Result.Succeeded;
        }


        public void DoFailureProcessing(
        object sender,
        FailuresProcessingEventArgs args)
        {
            FailuresAccessor failureMes = args.GetFailuresAccessor();

            IList<FailureMessageAccessor> a
              = failureMes.GetFailureMessages();

            foreach (FailureMessageAccessor failure in a)
            {
                FailureSeverity fsav = failureMes.GetSeverity();


                if (fsav == FailureSeverity.Warning)
                {
                    failureMes.DeleteWarning(failure);

                }
                else
                {
                    failureMes.ResolveFailure(failure);
                }
                args.SetProcessingResult(FailureProcessingResult.ProceedWithCommit);
            }
        }
    }

}

