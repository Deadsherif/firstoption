using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ModelHealthChecker.MVVM.Model
{
    public class RevitDuplication : INotifyPropertyChanged
    {
        #region Imp
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        bool isSelected;
        public List<ElementId> ElementsCausingTheDuplication { get; private set; }
        public ElementId ElementId_2 { get; private set; }
        public ElementId ElementId_1 { get; private set; }

        public Element Element_1 { get; private set; }
        public Element Element_2 { get; private set; }

        public string Element_1_Dis { get; private set; }
        public string Element_2_Dis { get; private set; }

        public bool IsSelected
        {
            get
            {
                return isSelected;

            }
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        public RevitDuplication(FailureMessage warning, Document Doc)
        {
            ElementsCausingTheDuplication = warning.GetFailingElements().ToList();
            if (ElementsCausingTheDuplication.Count == 2)
            {
                ElementId_1 = ElementsCausingTheDuplication[0];
                ElementId_2 = ElementsCausingTheDuplication[1];
                Element_1 = Doc.GetElement(ElementId_1);
                Element_2 = Doc.GetElement(ElementId_2);

                Element_1_Dis = $"{Element_1.Category.Name}  [{ElementId_1}]";
                Element_2_Dis = $"{Element_2.Category.Name}  [{ElementId_2}]";

            }

            IsSelected = false;
        }


    }
}
