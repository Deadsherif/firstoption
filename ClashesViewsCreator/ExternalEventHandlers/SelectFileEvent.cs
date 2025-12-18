using Autodesk.Revit.UI;
using ClashesViewsCreator.MVVM.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClashesViewsCreator.ExternalEventHandlers
{
    public class SelectFileEvent : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            Command.AllIds = new List<int>();
            string ExcelFilePath;


            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "HTML Files|*.html;*.htm",
                Title = "Select HTML File"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                ExcelFilePath = openFileDialog.FileName;

                StringBuilder sb = new StringBuilder();

                if (Command.frm.VM.IsHTML)
                {
                    Command.Elements_Id_1 = ExtractNumbersFromHtml(ExcelFilePath, 1);
                    Command.Elements_Id_2 = ExtractNumbersFromHtml(ExcelFilePath, 2);
                }
                else
                {
                    Command.Elements_Id_1 = ExtractNumbersFromIterferenceReport(ExcelFilePath)[0];
                    Command.Elements_Id_2 = ExtractNumbersFromIterferenceReport(ExcelFilePath)[1];
                }


                foreach (int num in Command.Elements_Id_1)
                {
                    Command.AllIds.Add(num);
                }

                foreach (int num in Command.Elements_Id_2)
                {
                    Command.AllIds.Add(num);
                }

                Command.frm.VM.ClashedIds = new ObservableCollection<FO_Num>(GetFO_NumList(Command.AllIds)
                    .OrderByDescending(e => e.NumOfDuplication).ToList());
            }
        }

        public string GetName()
        {
            return "Mostafa";
        }
        static List<FO_Num> GetFO_NumList(List<int> originalList)
        {
            var groupedNumbers = originalList.GroupBy(x => x);

            List<FO_Num> result = new List<FO_Num>();

            foreach (var group in groupedNumbers)
            {
                FO_Num foNum = new FO_Num
                {
                    Number = group.Key,

                    NumOfDuplication = group.Count()
                };

                result.Add(foNum);
            }

            return result;
        }

        static List<int> ExtractNumbersFromHtml(string filePath, int ItemNum)
        {
            List<int> numbers = new List<int>();

            if (ItemNum == 1)
            {
                try
                {
                    // Read the entire content of the HTML file
                    string htmlContent = File.ReadAllText(filePath);

                    // Define a regular expression pattern to match lines containing numbers
                    //string pattern = @"<i>Element ID</i>";
                    string pattern = @"<td class=""item1Content""><i>Element ID</i>\s*:\s*(\d+)</td>";

                    // Match the pattern in the entire HTML content
                    MatchCollection matches = Regex.Matches(htmlContent, pattern);

                    // Process each match and extract the number
                    foreach (Match match in matches)
                    {
                        var ss = match.Value.Split('D');
                        int x = ExtractNumber(ss[1]);

                        numbers.Add(x);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            else if (ItemNum == 2)
            {
                try
                {
                    // Read the entire content of the HTML file
                    string htmlContent = File.ReadAllText(filePath);

                    // Define a regular expression pattern to match lines containing numbers
                    //string pattern = @"<i>Element ID</i>";
                    string pattern = @"<td class=""item2Content""><i>Element ID</i>\s*:\s*(\d+)</td>";

                    // Match the pattern in the entire HTML content
                    MatchCollection matches = Regex.Matches(htmlContent, pattern);

                    // Process each match and extract the number
                    foreach (Match match in matches)
                    {
                        var ss = match.Value.Split('D');
                        int x = ExtractNumber(ss[1]);

                        numbers.Add(x);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }


            return numbers;
        }


        static List<List<int>> ExtractNumbersFromIterferenceReport(string filePath)
        {
            List<int> numbers1 = new List<int>();
            List<int> numbers2 = new List<int>();


            try
            {
                // Read the entire content of the HTML file
                string[] lines = File.ReadAllLines(filePath);
                // Process each match and extract the number
                foreach (string LineText in lines)
                {
                    var ss = GetIds(LineText);
                    if (ss.Count == 2)
                    {
                        numbers1.Add(ss[0]);
                        numbers2.Add(ss[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }


            return new List<List<int>> { numbers1, numbers2 };

        }
        static int ExtractNumber(string input)
        {
            // Define a regular expression pattern to match the number
            string pattern = @"\d+";

            // Use Regex.Match to find the first match in the input string
            Match match = Regex.Match(input, pattern);

            // Check if a match was found
            if (match.Success)
            {
                // Get the matched value and convert it to an integer
                if (int.TryParse(match.Value, out int result))
                {
                    return result;
                }
            }

            // Return -1 if no match or parsing fails
            return -1;
        }
        static List<int> GetIds(string input)
        {
            List<int> ids = new List<int>();

            // Define the regular expression pattern
            string pattern = @"id\s+(\d+)";

            // Create a regex object
            Regex regex = new Regex(pattern);

            // Match the pattern in the input string
            MatchCollection matches = regex.Matches(input);

            // Extract and add the numbers to the list
            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    string idStr = match.Groups[1].Value;
                    if (int.TryParse(idStr, out int id))
                    {
                        ids.Add(id);
                    }
                }
            }

            return ids;
        }
    }
}
