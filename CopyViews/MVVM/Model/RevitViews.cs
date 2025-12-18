using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyViews.MVVM.Model
{
    public class RevitViews
    {
        public string Name { get; set; }
        public Level Level { get; private set; }
        public ElementId LevelId { get; private set; }

        public double Elevation { get; private set; }

        public ViewType viewType { get; private set; }

        public RevitViews(Document doc, string name, ElementId level_Id, ViewType view_Type)
        {

            Level = (doc.GetElement(level_Id) as Level);
            LevelId = Level.Id;
            Elevation = Math.Round(Level.Elevation / 0.00328084, 0);
            Name = name;
            viewType = view_Type;


        }

    }
}
