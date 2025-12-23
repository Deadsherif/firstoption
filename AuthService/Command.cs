using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AuthService.MVVM.View;
using AuthService.Services;
using System;

namespace AuthService
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Check local cache before showing the UI
                if (LocalAuthCache.TryGetValidEntry())
                {
                    SubscriptionService.Status = SubscriptionService.SubscriptionStatus.Valid;
                    return Result.Succeeded;
                }

                SubscriptionService.Status = SubscriptionService.SubscriptionStatus.NotFound;

                AuthWindow window = new AuthWindow();
                window.ShowDialog();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
