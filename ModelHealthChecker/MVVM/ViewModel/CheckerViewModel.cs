using Autodesk.Revit.DB;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using ModelHealthChecker.MVVM.Model;
using System.Windows.Input;
using ModelHealthChecker.Commands;
using ModelHealthChecker.MVVM.View;

namespace ModelHealthChecker.MVVM.ViewModel
{
    public class CheckerViewModel : INotifyPropertyChanged
    {
        #region Imp
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion


        #region Fields
        double fileSize;
        int numOfWarnings;
        int numOfRevitLinks;
        ObservableCollection<Element> importedLinks;
        Element selectedRevitLink;


        ObservableCollection<Element> importedCAD;
        Element selectedCADLink;
        int numOfCADLinks;
        ObservableCollection<RevitWarning> warnings;
        ObservableCollection<RevitFamiy> loadedFamilies;
        ObservableCollection<RevitFamiy> selectedFamilies;
        RevitWarning selectedWarning;
        int numOfDuplicates;
        SeriesCollection columnSelectionSeries;
        ObservableCollection<FailureMessage> duplicatedWarnings;
        ObservableCollection<RevitDuplication> duplications;
        bool selectAll;
        RevitDuplication selectedDuplication;

        #endregion

        #region Poperties
        public double FileSize
        {
            get { return fileSize; }
            set
            {
                fileSize = value;
                OnPropertyChanged();
            }
        }
        public int NumOfWarnings
        {
            get { return numOfWarnings; }
            set
            {
                numOfWarnings = value;
                OnPropertyChanged();
            }
        }
        public int NumOfRevitLinks
        {
            get { return numOfRevitLinks; }
            set
            {
                numOfRevitLinks = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Element> ImportedLinks
        {
            get
            {
                return importedLinks;
            }
            set
            {
                importedLinks = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Element> ImportedCAD
        {
            get
            {
                return importedCAD;
            }
            set
            {
                importedCAD = value;
                OnPropertyChanged();
            }
        }
        public int NumOfCADLinks
        {
            get { return numOfCADLinks; }
            set
            {
                numOfCADLinks = value;
                OnPropertyChanged();
            }
        }
        public int NumOfDuplicate
        {
            get
            {
                return numOfDuplicates;
            }
            set
            {
                numOfDuplicates = value;
                OnPropertyChanged();
            }
        }
        public ICommand WarningBtn { get; set; }
        public ICommand ShowElementsBTN { get; set; }
        public ICommand DeleterBTN { get; set; }
        public ICommand ShowDuplicatedElementsBTN { get; set; }

        public ICommand IsolateLinkBTN { get; set; }
        public ICommand DelLinkBTN { get; set; }

        public ICommand DeleteDupElementsBTN { get; set; }
        public SeriesCollection ColumnSelectionSeries
        {
            get
            {
                return columnSelectionSeries;
            }
            set
            {
                columnSelectionSeries = value;
                OnPropertyChanged();
            }

        }
        public Axis Axis_X { get; set; }
        public Axis Axis_Y { get; set; }
        public ObservableCollection<RevitWarning> Warnings
        {
            get
            {
                return warnings;
            }
            set
            {
                warnings = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<RevitFamiy> LoadedFamilies
        {
            get
            {
                return loadedFamilies;
            }
            set
            {
                loadedFamilies = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<RevitFamiy> SelectedFamilies
        {
            get
            {
                return selectedFamilies;
            }
            set
            {
                selectedFamilies = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<RevitDuplication> Duplications
        {
            get
            {
                return duplications;
            }
            set
            {
                duplications = value;
                OnPropertyChanged();
            }
        }

        public RevitWarning SeletedWarning
        {
            get
            {
                return selectedWarning;
            }
            set
            {
                selectedWarning = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowWarningsButton { get; set; }

        public ObservableCollection<FailureMessage> DuplicatedWarnings
        {
            get
            {
                return duplicatedWarnings;
            }
            set
            {
                duplicatedWarnings = value;
                OnPropertyChanged();
            }

        }

        public bool SelectAll
        {
            get { return selectAll; }
            set
            {
                selectAll = value;
                if (selectAll)
                {
                    foreach (var dup in Duplications)
                    {
                        dup.IsSelected = true;

                    }
                }
                else
                {
                    foreach (var dup in Duplications)
                    {
                        dup.IsSelected = false;

                    }
                }
                OnPropertyChanged();
            }
        }

        public RevitDuplication SelectedDuplication
        {
            get
            {
                return selectedDuplication;
            }
            set
            {
                selectedDuplication = value;
                OnPropertyChanged();
            }
        }

        public ICommand LinksBTN { get; set; }

        public Element SelectedRevitLink
        {
            get
            {
                return selectedRevitLink;
            }
            set
            {
                selectedRevitLink = value;
                OnPropertyChanged();
            }
        }


        public Element SelectedCADLink
        {
            get
            {
                return selectedCADLink;
            }
            set
            {
                selectedCADLink = value;
                OnPropertyChanged();
            }
        }


        #endregion

        #region Constructors
        public CheckerViewModel(Autodesk.Revit.DB.Document Doc)
        {
            WarningBtn = new RelayCommand (p=> WarningCommand(p));
            ShowWarningsButton = new RelayCommand(p => ShowWarningElementCommand(p));
            ShowElementsBTN = new RelayCommand(p => ShowFamilyElementsCommand(p));
            DeleterBTN = new RelayCommand(p => DeleteFamilyCommand(p));
            ShowDuplicatedElementsBTN = new RelayCommand(p => ShowDuplicatedElementsCommand(p));
            DeleteDupElementsBTN = new RelayCommand(p => DeleteDupElementsCommand(p));
            LinksBTN = new RelayCommand(p => ImportsDetails(p));
            IsolateLinkBTN = new RelayCommand(p => IsolateLinkCommand(p));
            DelLinkBTN = new RelayCommand(p => DeleteLinksCommand(p));

            #region Work

            // Get file size in megabytes
            string filePath = Doc.PathName;

            if (filePath == null || filePath == "")
            {
                MessageBox.Show("This project is not saved so it is not possible to get the file size!", "First Option", MessageBoxButton.OK, MessageBoxImage.Information);
                FileSize = 0;
            }
            else
            {
                FileInfo fileInfo = new FileInfo(filePath);
                FileSize = Math.Round(fileInfo.Length / (1024.0 * 1024.0), 2);
            }


            ////////////////////////// Get Warnings ////////////////////////

            List<FailureMessage> warningSet = Doc.GetWarnings().ToList();
            Warnings = new ObservableCollection<RevitWarning>();
            int i = 1;
            foreach (FailureMessage warning in warningSet)
            {
                var ss = warning.GetType();
                RevitWarning wr = new RevitWarning
                {
                    Id = i,
                    Description = warning.GetDescriptionText(),
                    FailureElements = warning.GetFailingElements().ToList()
                };

                Warnings.Add(wr);
                i++;
            }
            NumOfWarnings = warningSet.Count;



            /////////////////////////////Get Imported Linkes ///////////////////////////

            List<Element> TempList = new FilteredElementCollector(Doc)
           .OfClass(typeof(RevitLinkType)).ToList();

            ImportedLinks = new ObservableCollection<Element>();

            foreach (var s in TempList)
            {
                ImportedLinks.Add(s);

            }
            NumOfRevitLinks = ImportedLinks.Count;

            ImportedCAD = new ObservableCollection<Element>();


            var Collection = new FilteredElementCollector(Doc)
                  .OfClass(typeof(ImportInstance)).ToList();

            foreach (Element ele in Collection)
            {
                ElementId ele_Id = ele.GetTypeId();
                Element typeOfElement = Doc.GetElement(ele_Id);
                if (typeOfElement is CADLinkType)
                {
                    ImportedCAD.Add(typeOfElement);
                }
            }
            NumOfCADLinks = ImportedCAD.Count;



            ColumnSelectionSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Revit Links",
                    Values = new ChartValues<double> { NumOfRevitLinks},
                     Fill = new SolidColorBrush(Colors.Blue),
                     Foreground = new SolidColorBrush(Colors.Black),
                     ColumnPadding = 25,
                     MaxColumnWidth = 150,

                },
                 new ColumnSeries
                {
                    Title = "AutoCAD Links",
                    Values = new ChartValues<double> {  NumOfCADLinks },
                     Fill = new SolidColorBrush(Colors.Red),
                     Foreground = new SolidColorBrush(Colors.Black),
                         ColumnPadding = 25,
                       MaxColumnWidth = 150,


                },


            };




            /////////////////////////////////// Loaded Families ///////////////////////////

            LoadedFamilies = new ObservableCollection<RevitFamiy>();

            FilteredElementCollector collector = new FilteredElementCollector(Doc);
            ICollection<Element> allFamilies = collector.OfClass(typeof(Family)).ToElements();

            foreach (Element familyElement in allFamilies)
            {
                // Do something with each family element
                Family family = familyElement as Family;

                List<Element> FamilyElements = Helper.GetAllElementsFromAFamily(Doc, family);

                RevitFamiy rf = new RevitFamiy();

                if (FamilyElements.Count == 0)
                {
                    rf.family = family;
                    rf.IsUsed = "No";

                }
                else
                {
                    rf.family = family;
                    rf.IsUsed = "Yes";
                }
                LoadedFamilies.Add(rf);
            }
            /////////////////////////////////// Get Duplicates
            //////////////////////////////
            ///
            ///

            DuplicatedWarnings = new ObservableCollection<FailureMessage>();
            Duplications = new ObservableCollection<RevitDuplication>();
            foreach (FailureMessage warning in warningSet)
            {
                string txt = warning.GetDescriptionText();
                if (txt == "There are identical instances in the same place. This will result in double counting in schedules.")
                {
                    DuplicatedWarnings.Add(warning);
                    RevitDuplication duplication = new RevitDuplication(warning, Doc);
                    Duplications.Add(duplication);
                }
            }
            NumOfDuplicate = DuplicatedWarnings.Count;








            #endregion

        }


        #endregion

        #region Button Actions
        public void WarningCommand(object parameter)
        {
            WarningsView warningsView = WarningsView.CreateInstance(Command.VM);
            warningsView.Show();
        }
        public void ShowWarningElementCommand(object parameter)
        {
            Command.WarningEventHandler.Raise();

        }
        public void ShowFamilyElementsCommand(object parameter)
        {
            Command.FamilyElementsEventHandeler.Raise();

        }
        public void DeleteFamilyCommand(object parameter)
        {
            Command.DeleteEventHandeler.Raise();
        }
        public void ShowDuplicatedElementsCommand(object parameter)
        {
            Command.ShowDupEventHandeler.Raise();

        }
        public void DeleteDupElementsCommand(object parameter)
        {
            Command.DelDupEventHandeler.Raise();
        }
        public void ImportsDetails(object parameter)
        {
            ImportedLinksView vm = new ImportedLinksView();
            vm.Show();

        }
        public void IsolateLinkCommand(object parameter)
        {
            Command.IsolateLinkEventHandeler.Raise();

        }
        public void DeleteLinksCommand(object parameter)
        {
            Command.DeleteLinksEventHandeler.Raise();

        }
   



        #endregion


    }
}
