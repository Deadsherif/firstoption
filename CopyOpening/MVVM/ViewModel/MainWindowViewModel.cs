using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using CopyOpening.External_Event_Handlers;

using CopyOpening.MVVM.View;
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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;


namespace CopyOpening.MVVM.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Attributes
        private ExternalEvent ev;
        private List<RevitLinkInstance> revitLinkInstances;
        private RevitLinkInstance selectedLink;



        #endregion
        #region Properties
        public Document document { get; set; }


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



        public ExternalEvent Ev
        {
            get { return ev; }
            set { ev = value; OnPropertyChanged(nameof(Ev)); }
        }


        #endregion
        #region Functions
        public ICommand CopyOpeningCommand { get; }
        public ICommand NavigateCommand { get; }
        public MainWindowViewModel(Document document)
        {
            CopyOpeningCommand = new RelayCommand(P => RunExporter(P));
            revitLinkInstances =  new FilteredElementCollector(document)
                .OfClass(typeof(RevitLinkInstance)).Cast<RevitLinkInstance>().ToList();
        }

        public void RunExporter(object parameter)
        {
            Ev.Raise();

        }




        #endregion


    }


}
