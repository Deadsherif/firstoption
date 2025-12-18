using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelHealthChecker.MVVM.Model
{
    public class RevitFamiy
    {
        public Family family { get; set; }
        public string IsUsed { get; set; }

    }
}
