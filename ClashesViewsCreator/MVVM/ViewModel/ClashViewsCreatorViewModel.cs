using Autodesk.Revit.DB;
using ClashesViewsCreator.Commands;
using ClashesViewsCreator.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClashesViewsCreator.MVVM.ViewModel
{
    public class ClashViewsCreatorViewModel : INotifyPropertyChanged
    {
        #region Imp
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion


        #region Fields
        bool isHTML { get; set; }
        bool isAll { get; set; }

        ObservableCollection<FO_Num> clashedIds;
        ObservableCollection<FO_Num> selectedIds;
        ObservableCollection<Element> types;
        Element selectedType;


        #endregion

        #region Properties
        public bool IsHTML
        {
            get
            {
                return isHTML;
            }
            set
            {
                isHTML = value;
                OnPropertyChanged();
            }
        }
        public bool IsAll
        {
            get
            {
                return isAll;
            }
            set
            {
                isAll = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateBTN { get; set; }
        public ICommand selectBTN { get; set; }

        public ICommand createFilterBTN { get; set; }

        public ICommand saveToExcelBTN { get; set; }

        public ICommand simulationBTN { get; set; }

        public ICommand ImportBTN { get; set; }

        public ObservableCollection<FO_Num> ClashedIds
        {
            get
            {
                return clashedIds;
            }
            set
            {
                clashedIds = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<FO_Num> SelectedIds
        {
            get
            {
                return selectedIds;
            }
            set
            {
                selectedIds = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Element> Types
        {
            get
            {
                return types;
            }
            set
            {
                types = value;
                OnPropertyChanged();
            }
        }

        public Element SelectedType
        {
            get
            {
                return selectedType;
            }
            set
            {
                selectedType = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructor
        public ClashViewsCreatorViewModel()
        {
            IsHTML = false;
            IsAll = true;
            CreateBTN = new RelayCommand(P => CreateButtonCommand(P));
            selectBTN = new RelayCommand(P => SelectCommand(P));
            createFilterBTN = new RelayCommand(P => CreateFilterCommand(P));
            saveToExcelBTN = new RelayCommand(P => SaveToExcelCommand(P));
            simulationBTN = new RelayCommand(P => SimulationCommand(P));
            ImportBTN = new RelayCommand(P => ImportCommand(P));
            List<Element> massTypes = new FilteredElementCollector(Command.Doc)
            .OfCategory(BuiltInCategory.OST_Mass)
            .WhereElementIsElementType()
            .ToElements().ToList();

            List<Element> GerenricTypes = new FilteredElementCollector(Command.Doc)
          .OfCategory(BuiltInCategory.OST_GenericModel)
          .WhereElementIsElementType()
          .ToElements().ToList();

            Types = new ObservableCollection<Element>(massTypes);
            foreach (Element element in GerenricTypes)
            {
                Types.Add(element);
            }
        }

        #endregion


        #region Button Actions
        public void CreateButtonCommand(object parameter)
        {
            Command.createAll_Event.Raise();
        }
        public void SelectCommand(object parameter)
        {
            Command.select_Event.Raise();
        }
        public void CreateFilterCommand(object parameter)
        {
            Command.CreateFilter_Event.Raise();
        }
        public void SaveToExcelCommand(object parameter)
        {
            Command.ExportExcel_Event.Raise();
        }
        public void SimulationCommand(object parameter)
        {
            Command.createSimulation_Event.Raise();
        }
        public void ImportCommand(object parameter)
        {
            Command.Import_Event.Raise();
        }
        #endregion
    }
}
