using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Warning_Solver.MVVM.Model;
using Comparer = Warning_Solver.MVVM.Model.Comparer;

namespace Warning_Solver.MVVM.View
{
    /// <summary>
    /// Interaction logic for WarningSolverView.xaml
    /// </summary>
    public partial class WarningSolverView : Window
    {
        int _max = 0;
        ExternalEvent nev;
        public WarningSolverView(ExternalEvent ev, ExternalCommandData commandData)
        {
            nev = ev;
            InitializeComponent();
            setUIData();
        }
        public void setUIData()
        {
            numlbl.Content = Database.Warnings.Count().ToString();
            constxt.Text = "Pick type and press solve to start our transaction......";
            var warningtypes = Database.Warnings.Distinct(new Comparer()).ToList();
            foreach (var warning in warningtypes)
            {
                if (!warningbox.Items.Contains(warning.GetDescriptionText()))
                {
                warningbox.Items.Add(warning.GetDescriptionText());
                }
            }
        }

        private void warningbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Database.selecteditem = warningbox.SelectedItem;
            constxt.Text = $"The selected Type is\n....................\n{warningbox.SelectedItem}\n.....................\nIt has {Database.Warnings.Where(x => x.GetDescriptionText() == warningbox.SelectedItem.ToString()).Count()} Warnings\n.....................\n";
            if (warningbox.SelectedItem.ToString() == "Elements have duplicate \"Mark\" values.")
            {
                markbox.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                markbox.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        public void OnRaising(object sender, Autodesk.Revit.DB.Events.ProgressChangedEventArgs e)
        {

            constxt.Text = e.Caption;


        }
        private void solvebtn_Clicked(object sender, RoutedEventArgs e)
        {
            nev.Raise();
        }



        private void markbox_Checked(object sender, RoutedEventArgs e)
        {
            Database.ischecked = true;
        }
        private void markbox_UnChecked(object sender, RoutedEventArgs e)
        {
            Database.ischecked = false;
        }
        //set up a delegate    
        private delegate void ProgressBarDelegate();


        public void UpdateProgressBar(string msg)
        {
            constxt.Text += "\n" + msg;
            constxt.ScrollToEnd();
            progressBar.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgress), DispatcherPriority.Background);
        }
        public void UpdateProgressBar_commit()
        {
            for (int i = 0; i < ((int)(_max * 0.1)); i++)
            {
                try
                {
                    progressBar.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgress), DispatcherPriority.Background);
                }
                catch (Exception) { }
            }
        }
        public void SetWaitMsg()
        {

            constxt.Text += "\n" + "Finalizing...";
            constxt.ScrollToEnd();
            progressBar.Dispatcher.Invoke(new ProgressBarDelegate(DummyUpdate), DispatcherPriority.Background);

        }
        public void SetCompleteMsg()
        {

            constxt.Text += "\n" + "Completed successfully!";
            constxt.ScrollToEnd();
            progressBar.Dispatcher.Invoke(new ProgressBarDelegate(DummyUpdate), DispatcherPriority.Background);

        }


        public void SetProgressBarLimits(int max)
        {
            _max = max;
            progressBar.IsIndeterminate = false;
            progressBar.Maximum = max + ((int)(max * 0.1));
            progressBar.Minimum = 0;
        }
        //update the progress bar
        private void UpdateProgress()
        {

            progressBar.Value += 1;  //event here

        }
        private void DummyUpdate()
        {
            try
            {
                progressBar.Value += 0;  //event here

            }
            catch (Exception)
            {


            }

        }


        private void progressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
