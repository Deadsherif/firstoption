// Decompiled with JetBrains decompiler
// Type: MEP_Fabricator.Fabricator
// Assembly: MEP_Fabricator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8CFBF883-28B0-455B-8DCF-FF413BFD7A7C
// Assembly location: \\fs\Temp-Share\03 R&D\omar amen\New folder (4)\2020\MEP_Fabricator.dll

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Firebase.Auth.Wpf.Sample;
using System;
using System.Net.Http;


namespace MEP_Fabricator
{
    [Transaction(TransactionMode.Manual)]
    public class Fabricator : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Fabricator.exe(ExternalEvent.Create((IExternalEventHandler) new EV()));


            var xe = ExternalEvent.Create((IExternalEventHandler)new EV());
            var LicenseWindow = MainWindow.CreateInstance(xe);
            LicenseWindow.Show();
            return (Result)0;
        }

        //public static async void exe(ExternalEvent xe)
        //{
        //    HttpClient client = new HttpClient();
        //    try
        //    {
        //        string content = await client.GetStringAsync("https://revit-apis.herokuapp.com/api?id=619f6a6ff12ecbe54b609e13");
        //        if (content == "ok")
        //            xe.Raise();
        //        else
        //            TaskDialog.Show("Trial", "Trial has been expired, please contact TEAServ");
        //        content = (string)null;
        //        client = (HttpClient)null;
        //    }
        //    catch (Exception ex)
        //    {
        //        TaskDialog.Show("connection", "No Internet! please contact TEAServ");
        //        client = (HttpClient)null;
        //    }
        //}
    }
}
