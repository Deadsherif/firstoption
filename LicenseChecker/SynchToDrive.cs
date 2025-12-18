using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using System.Windows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using Color = Google.Apis.Sheets.v4.Data.Color;
using Border = Google.Apis.Sheets.v4.Data.Border;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace DrafterPerformance
{
    internal class SynchToDrive
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "LicesneFO";
        public static String spreadsheetId = "1LcR9qp3R0JDEvKfpXt8lKNywr6efxebHJznW7ZkmSnk";
        public static String spreadsheetName = "Trial";
        static SheetsService service;
        static GoogleCredential credential;
        public static Exception Ex;
        static List<string> sheetList = new List<string>();


        public SynchToDrive()
        {
            if (service != null)
                return;
            JsonCredentialParameters jsonCredentialParameters = new JsonCredentialParameters();
            jsonCredentialParameters.Type = "service_account";
            jsonCredentialParameters.ProjectId = "licensefo";
            jsonCredentialParameters.PrivateKeyId = "3eaeb4af8486acebe1aae172ab5d3ade5403f55b";
            jsonCredentialParameters.PrivateKey =
                "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDnbgWge7PXSVN3\nHODAUKHI7mgleyzab1E3KlaS6bOGpLrNa99vG3OK9cE1U6JmpSx1pRczQG5bekdS\nvvzsRq8bo9Z1hwZaE2PAZe+PoghGaWSO7grTpXcLky1ImYeSzln6FVxz7pyv48Ix\nPBMJzE0azJmrhHxUiTEJ45sGDEJ6TXfdbXUYsXiHvrdlcND/rUGH0Qy2iENEubPs\nhEsgYEM3bVjs9DRPe9Rj3X4WQDR5+nvYd1G7C4NYDn/JmqWe0Az5dWJyEAIbkJy6\ncru18vn6898NwM8w1gfYg7U/iwUxzmyGAZtXWWVZWXTEw0ZQv98F6I5uBdM/Y4mL\nhyD+81trAgMBAAECggEAIFIXQGsbmgIdlqcBJG/zsvIeDWNUckqAQdS3M9v7kd7z\npQ4JraSx0EMIxqp1eyGvlzEv5PJ8ob3utA8EQk3k1VaznsT1Ys25F/z50J9vycBH\npkZ3BfPihPVQVQahY86kdkQBc2zBk+GaafHaiXsaJuyoEXIhQv/In3maruTgwTTz\nJ/12w6OGpr0MdnF9R6NJmsUeyeUmQUeAyGpiBL9nfcVk4bVka/e//pywiEwf5tEX\nOZ4DDZMwOc12eCzZdHBQBcHmUZ7gSe+omzL7RaxRuIoiiFWjwYJSlxPeKboXZKg8\nZosJew/FxDA8V8cQPo0CH6QX0KV2MChFJMtMwS10UQKBgQD28hSWfQT1xhq68suh\nH9haMlsC+B/tCzACsXN2+5FeGPnOoEz8Cx4jtZOLcLl4UBvcrpm7Ov+NS1R6W5Z4\nCRqtjFwIMKGm5d3a9Wm2eI7RyWUP0G2sdS6+rtlF18ZmwjhNrPkbecC91lrVcmLr\nKwhu4uTkcZxJvUpW1TG9UWDTjwKBgQDv6k3iXM7QAItNOIGd7sU5bvfUxwrudnaY\nrOO/YIfrS6BGIjlxeWkyOkmZTS2/YbnUqN7kn2KP0dSfVK0Iry+zWlLzMLV6KL2E\n/q7/9pBim7LcX1ATOWK85pS246sBHJm6HYnB6Mla2zsFza9JaBBKBgNAFu3sege+\n58Wnpn/cZQKBgBH5OKpjWxMQYi2tMkj96l9WPu2OZbR5Fft8Mu08Di6NHtF9NV06\n6eyrcJu7jqRFIGIFi0bnWmZMT0/AjskZ0IlFKAWolHtzS5x+ND2FtM9sGyD1iKX+\nuXJDg6jjtZsRE8wDhPYM/IxUp7PVaYG4VhrMBODjq/5p+pkrVz1ySNvdAoGBAN7f\n2W6aLP4KI16fSbo/4DQUpGKkbcFNh6sZoZFdyaaKYOAGVzrVQSaKA2lIy2DpFks6\nfaYBcvyMP2usq/pPVI7XMAv/ifdr9XOhbU0X7tXMMoKKgMb7HoDB7BFiuq+TtDOz\n0Q+0g6nF07T2eQpGhCtgHpDhPDHcw2lTwsiV4l4JAoGAa3q4jbXGwXqVMDXyBh4p\nfHF+E0v87FgCC+8lURgYryL/YV825SYwb/Xtv5XDCqGhXVtNuaO1Z0ctwN29AaHf\nNPE9bdy24a/wFk9VH73pzNI8ktQbZ0X8FPqO02mOONptisjdRm/qqmpOsbi0H2IR\nwXO33cLjFxm5KK+P5kY7O+A=\n-----END PRIVATE KEY-----\n";

            jsonCredentialParameters.ClientEmail = "licensefo@licensefo.iam.gserviceaccount.com";
            jsonCredentialParameters.ClientId = "110871079473768138871";
            jsonCredentialParameters.TokenUrl = "https://oauth2.googleapis.com/token";


            credential = GoogleCredential.FromJsonParameters(jsonCredentialParameters).CreateScoped(Scopes);

            // Create Google Sheets API service.
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }


        public string GetCodeByMachineId(string MachineId)
        {

            String range = $"{spreadsheetName}!A2:E";

            SpreadsheetsResource.ValuesResource.GetRequest request =
              service.Spreadsheets.Values.Get(spreadsheetId, range);

            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null)
            {
                var targetRow = values.Where(X => X.Contains(MachineId)).FirstOrDefault();
                if (targetRow != null)
                { return targetRow.LastOrDefault().ToString(); }
                else
                    return "";
            }
            return "";
        }

    }
}
