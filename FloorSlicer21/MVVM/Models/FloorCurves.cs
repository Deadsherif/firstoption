using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloorSlicer21.MVVM.Models
{
    public class FloorCurves
    {
        public CurveArray MainFloorArray { get; set; }
        public List<CurveArray> OpeningsArray { get; set; }



    }
}
