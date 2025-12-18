using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace DivideFloors
{
    internal class ApplicationDB
    {
        public static  Reference reference;
        public static string Offset;
        public static string width;
        public static string GridX;
        public static string GridY;
        public static string length;
        public static string thickness;
        public static string rotation;
        public static string material;
    }
}
