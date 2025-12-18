using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOSP.MVVM.Model
{
    public class Fabricator
    {
        public static void AddNewLine(
          string pathToFile,
          int size,
          string paramID,
          string paramName,
          string groupName,
          string paramType,
          string paramDiscription)
        {
            string[] strArray = File.ReadAllLines(pathToFile);
            string str1 = pathToFile;
            string str2 = str1.Remove(str1.Length - 4, 4) + "n.txt";
            File.Create(str2).Dispose();
            string str3 = "";
            for (int index = 0; index < strArray.Length; ++index)
                str3 = str3 + strArray[index] + Environment.NewLine;
            string contents = str3 + string.Format("GROUP\t{0}\t{1}", (object)(size + 1), (object)groupName) + Environment.NewLine + string.Format("PARAM\t{0}\t{1}\t{2}\t\t{3}\t1\t{4}\t1\t0", (object)paramID, (object)paramName, (object)paramType, (object)(size + 1), (object)paramDiscription);
            File.AppendAllText(str2, contents);
            File.Delete(pathToFile);
            File.Move(str2, pathToFile);
        }
        public static bool Get_TXT_SearchResult(string pathToFile, string paramID)
        {
            bool txtSearchResult = false;
            foreach (string readAllLine in File.ReadAllLines(pathToFile))
            {
                if (readAllLine.Contains(paramID))
                {
                    txtSearchResult = true;
                    break;
                }
            }
            return txtSearchResult;
        }


    }
}
