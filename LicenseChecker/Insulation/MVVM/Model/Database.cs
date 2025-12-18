using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insulation.MVVM.Model
{
    internal class Database
    {
        public static double thickness;
        public static bool booloffset { get; set; }
        public static double offsetfromface { get; set; }

        public static List<Material> materials { get; set; }
        public static string radioname { get; set; }
        public static List<Category> categories { get; set; }
        public static List<Element> levels { get; set; }
        public static Level selectedlevel { get; set; }
        public static ElementId selectedmaterial { get; set; }
        public static ElementId selectedcategory { get; set; }

    }
}
