using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using GetElementRoom.External_Event_Handlers;

using GetElementRoom.MVVM.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Xml.Linq;


namespace GetElementRoom.MVVM.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string tolerance="";

        public string Tolerance
        {
            get { return tolerance; }
            set { tolerance = value; }
        }

        #region Attributes
        private ExternalEvent ev;
        private ExternalEvent jsonev;
        private ExternalEvent evNavigae;
        private bool isChecked;
        private bool allElementChecked;
        private bool modelChecked = true;
        public double Progress { get; set; }
        private double bar;

        public IProgress<int> ProgressReporter { get; set; } = null;

        // public GridViewModel GridViewModel{ get; set; }


        #endregion
        #region Properties
        public bool AllElementChecked
        {
            get { return allElementChecked; }
            set
            {
                if (allElementChecked != value)
                {
                    allElementChecked = value;
                    //SelectedElementChecked = false;
                    OnPropertyChanged(nameof(AllElementChecked));
                }
            }
        }


        public double Bar
        {
            get { return bar; }
            set
            {

                bar = value;
                OnPropertyChanged(nameof(Bar));
            }
        }
        public bool ModelChecked
        {
            get { return modelChecked; }
            set
            {
                if (modelChecked != value)
                {
                    modelChecked = value;
                    //AllElementChecked = false;
                    OnPropertyChanged(nameof(ModelChecked));

                }
            }
        }

        public ExternalEvent Ev
        {
            get { return ev; }
            set { ev = value; OnPropertyChanged(nameof(Ev)); }
        }
        public ExternalEvent JsonEv
        {
            get { return jsonev; }
            set { jsonev = value; OnPropertyChanged(nameof(JsonEv)); }
        }
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                   
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }


        #endregion
        #region Functions
        public ICommand GetElementRoomCommand { get; }
        public ICommand NavigateCommand { get; }
        public MainWindowViewModel(Autodesk.Revit.DB.Document document)
        {
            
            GetElementRoomCommand = new RelayCommand(P => RunExporter(P));
            revitLinkInstances = new FilteredElementCollector(document)
                .OfClass(typeof(RevitLinkInstance)).Cast<RevitLinkInstance>().ToList();
        }
       
       public void RunExporter(object parameter)
        {
            Progress = 0;
            var progressReporter = new Progress<int>(progress =>
            {
                Progress = progress;
                OnPropertyChanged(nameof(Progress));
            });
            ProgressReporter = progressReporter;
            Ev.Raise();
          

        }



        #endregion
        private List<RevitLinkInstance> revitLinkInstances;
        private RevitLinkInstance selectedLink;
        public RevitLinkInstance SelectedLink
        {
            get { return selectedLink; }
            set { selectedLink = value; }
        }

        public List<RevitLinkInstance> RevitLinkInstances
        {
            get { return revitLinkInstances; }
            set { revitLinkInstances = value; }
        }

    }

   
}
