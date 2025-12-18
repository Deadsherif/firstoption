using Autodesk.Revit.DB;
using CopyViews.MVVM.Model;
using PasteViews.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PasteViews.MVVM.ViewModel
{
    public class PasteViewModel : INotifyPropertyChanged
    {
        #region Imp
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Fields
        ObservableCollection<RevitViews> viewsToPaste;
        bool createLevels;

        #endregion


        #region Properties
        public ObservableCollection<RevitViews> ViewsToPaste
        {
            get { return viewsToPaste; }
            set
            {
                viewsToPaste = value;
                OnPropertyChanged();
            }
        }
        public bool CreateLevels
        {
            get
            {
                return createLevels;
            }
            set
            {
                createLevels = value;
                OnPropertyChanged();
            }
        }
        public ICommand pasteBTN { get; set; }
        public ICommand clearBTN { get; set; }


        #endregion


        #region Constructor
        public PasteViewModel()
        {
            ViewsToPaste = Command.CopiedViews;
            CreateLevels = true;
            pasteBTN = new RelayCommand(P => pasteCommand(P));
            clearBTN = new RelayCommand(P => clearCommand(P));
        }

        #endregion

        #region Buttons Actions
        public void pasteCommand(object parameter)
        {
            Document doc = Command.Doc;

            ObservableCollection<RevitViews> views = Command.frm.VM.ViewsToPaste;
            if (views != null && views.Count > 0)
            {

                if (!Command.frm.VM.CreateLevels)
                {
                    int ViewCounter = 0;
                    using (Transaction trans = new Transaction(doc, "Create View"))
                    {
                        trans.Start();

                        foreach (RevitViews r_view in views)
                        {
                            if (Helper.GetLevelByName(doc, r_view.Level.Name) != null)
                            {
                                ViewPlan newView = ViewPlan.Create(doc, Helper.GetViewFamilyTypeId(doc, r_view.viewType), Helper.GetLevelByName(doc, r_view.Level.Name).Id);
                                ViewCounter++;
                                try
                                {
                                    newView.Name = r_view.Name;
                                }
                                catch
                                {
                                    Random r = new Random();
                                    newView.Name = $"{r_view.Name}_ (RNDM{r.Next(100, 999).ToString()})";
                                }
                            }
                        }
                        trans.Commit();
                    }

                    MessageBox.Show($"\nViews Created: {ViewCounter}", "First Option", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (Command.frm.VM.CreateLevels)
                {
                    List<Level> levelsCreated = new List<Level>();
                    int MainViewCounter = 0;
                    int ViewCounter = 0;
                    using (Transaction trans = new Transaction(doc, "Create Levels"))
                    {
                        trans.Start();
                        foreach (RevitViews r_view in views)
                        {
                            if (!Helper.HasLevelInDocument(doc, r_view.Level))
                            {
                                if (!levelsCreated.Contains(r_view.Level))
                                {
                                    Level l = Level.Create(doc, r_view.Level.Elevation);
                                    l.Name = r_view.Level.Name;
                                    ViewPlan newView = ViewPlan.Create(doc, Helper.GetViewFamilyTypeIdByViewFamily(doc, ViewFamily.FloorPlan), l.Id);
                                    MainViewCounter++;
                                    levelsCreated.Add(l);
                                    try
                                    {
                                        newView.Name = l.Name;
                                    }
                                    catch
                                    {
                                        Random r = new Random();
                                        newView.Name = $"{l.Name}_ (RNDM{r.Next(100, 999).ToString()})";
                                    }
                                }

                            }
                        }
                        trans.Commit();
                    }
                    using (Transaction trans = new Transaction(doc, "Create View"))
                    {
                        trans.Start();
                        foreach (RevitViews r_view in views)
                        {
                            ViewPlan newView = ViewPlan.Create(doc, Helper.GetViewFamilyTypeId(doc, r_view.viewType), Helper.GetLevelByName(doc, r_view.Level.Name).Id);
                            ViewCounter++;
                            try
                            {
                                newView.Name = r_view.Name;
                            }
                            catch
                            {
                                Random r = new Random();
                                newView.Name = $"{r_view.Name}_ (RNDM{r.Next(100, 999).ToString()})";
                            }
                        }
                        trans.Commit();
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Levels Created: ");
                    if (levelsCreated.Count <= 0)
                    {
                        sb.AppendLine("None");
                    }
                    else
                    {
                        foreach (Level lvl in levelsCreated)
                        {
                            sb.AppendLine($"Name: {lvl.Name}....Elevation: {Math.Round(lvl.Elevation / 0.00328084, 0)} mm");
                        }
                    }

                    sb.AppendLine($"\nDefault Views Created: {MainViewCounter}");
                    sb.AppendLine($"\nViews Created: {ViewCounter}");

                    MessageBox.Show(sb.ToString(), "First Option", MessageBoxButton.OK, MessageBoxImage.Information);


                }


            }

            else
            {
                MessageBox.Show("There is no views to paste, Please copy views first using the 'Copy Views' button!", "First Option", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }
        public void clearCommand(object parameter)
        {
            MessageBoxResult MSG = MessageBox.Show("Are you sure you want to clear copied views?", "FirstOption", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (MSG == MessageBoxResult.Yes)
            {
                CopyViews.Command.CopiedViews.Clear();
                Command.CopiedViews.Clear();
                Command.frm.VM.ViewsToPaste.Clear();
                MessageBox.Show("Copied Views Cleared from clipboard", "FirstOption", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        #endregion


    }
}
