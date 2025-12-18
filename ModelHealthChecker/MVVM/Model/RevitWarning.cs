using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelHealthChecker.MVVM.Model
{
    public class RevitWarning
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public List<ElementId> FailureElements { get; set; }

    }
}
