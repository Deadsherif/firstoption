using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlignCenter
{
    public class Helper
    {
        public static Line GetCenterline(Element pipe)
        {
            #region MyRegion

            Options options = new Options();
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = true;
            if (pipe.Document.ActiveView != null)
            {
                options.View = pipe.Document.ActiveView;
            }
            else
            {
                options.DetailLevel = (ViewDetailLevel)3;
            }
            foreach (GeometryObject geometryObject in pipe.get_Geometry(options))
            {
                Line centerline = geometryObject as Line;
                if (centerline != null)
                    return centerline;
            }
            return (Line)null;
            #endregion


            //if (pipe != null && (pipe.Location as LocationCurve) != null)
            //{
            //    return (pipe.Location as LocationCurve).Curve as Line;
            //}
            //else
            //{
            //    return null;
            //}

        }
    }
}
