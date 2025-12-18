using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warning_Solver.MVVM.Model
{
    public class Database
    {
        internal static object selecteditem;

        internal static bool ischecked;
        public static IList<FailureMessage> Warnings { get; internal set; }

    }
}
