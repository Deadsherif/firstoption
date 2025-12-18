using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FamilyPlacer
{
    /// <summary>
    /// Interaction logic for UI.xaml
    /// </summary>
    public partial class UI : Page
    {
        Document doc = null;
        ExternalEvent _ev;
        ExternalEvent _ev2;
        ExternalEvent _ev3;
        ExternalEvent _ev4;
        ExternalEvent _ev5;

        public UI(Document _doc, ExternalEvent ev, ExternalEvent ev2, ExternalEvent ev3, ExternalEvent ev4, ExternalEvent ev5)
        {
            InitializeComponent();
            cadbox.ItemsSource = DB.Cads;
            revitbox.ItemsSource = DB.RevitLinks;
            familybox.ItemsSource = DB.FamilyTypes;
            floorlvlbox.ItemsSource = DB.Levels;
            ceillvlbox.ItemsSource = DB.Levels;
            walllvllbox.ItemsSource = DB.Levels;
            walltypesbox.ItemsSource = DB.WallTypes;
            pipetypebox.ItemsSource = DB.PipeTypes;
            pipeLevelbox.ItemsSource = DB.Levels;
            cabletraytypebox.ItemsSource = DB.CabletraysTypes;
            cabletrayLevelbox.ItemsSource = DB.Levels;
            conduittypebox.ItemsSource = DB.ConduitTypes;
            conduitLevelbox.ItemsSource = DB.Levels;
            ducttypebox.ItemsSource = DB.DuctTypes;
            ductLevelbox.ItemsSource = DB.Levels;
            categorybox.ItemsSource = DB.Categories;
            nonhostedfamilybox.ItemsSource = DB.FamilyTypes;
            familylvlbox.ItemsSource = DB.Levels;
            groupbox.ItemsSource = DB.Groups;

            doc = _doc;
            _ev = ev;
            _ev2 = ev2;
            _ev3 = ev3;
            _ev4 = ev4;
            _ev5 = ev5;

        }


        private void refresh_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var dwg = DB.SelectedCadLink = cadbox.SelectedItem as ImportInstance;
                DB.SelectedRevitLink = revitbox.SelectedItem as RevitLinkInstance;
                //Hide();

                blockbox.ItemsSource = Command.GetLayerNames(dwg, doc);
            }
            catch (Exception)
            {


            }




        }
        //private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    this.DragMove();
        //}

        //private void btnMinimize_Click(object sender, RoutedEventArgs e)
        //{
        //    this.WindowState = WindowState.Minimized;
        //}

        //private void btnMaximize_Click(object sender, RoutedEventArgs e)
        //{
        //    switch (this.WindowState)
        //    {
        //        case WindowState.Normal:
        //            this.WindowState = WindowState.Maximized;
        //            break;
        //        case WindowState.Maximized:
        //            this.WindowState = WindowState.Normal;
        //            break;
        //    }
        //}

        //private void btnClose_Click(object sender, RoutedEventArgs e)
        //{
        //    this.Close();
        //}

        private void Apply_Click(object sender, RoutedEventArgs e)
        {


            DB.SelectedPipeType = pipetypebox.SelectedItem as PipeType;
            DB.SelectedPipeLevel = pipeLevelbox.SelectedItem as Level;
            DB.SelectedDuctType = ducttypebox.SelectedItem as DuctType;
            DB.SelectedDuctLevel = ductLevelbox.SelectedItem as Level;
            DB.SelectedCabletrayType = cabletraytypebox.SelectedItem as CableTrayType;
            DB.SelectedCabletrayLevel = cabletrayLevelbox.SelectedItem as Level;
            DB.SelectedConduitType = conduittypebox.SelectedItem as ConduitType;
            DB.SelectedConduitLevel = conduitLevelbox.SelectedItem as Level;
            DB.SelectedCategory = categorybox.SelectedItem as string;
            DB.SelectedGroup = groupbox.SelectedItem as GroupType;
            DB.SelectedWallType = walltypesbox.SelectedItem as WallType;



            // duct data                                                                 
            DB.DuctHeight = ductheight.Text;
            DB.DuctHeight = ductheight.Text;
            DB.DuctBottomElevation = ductbottomelevation.Text;

            //pipe data                                                                  

            DB.PipeDiameterValue = pipediameter.Text;
            DB.PipeOffset = pipeoffset.Text;


            //conduit data                                                               
            DB.ConduitDiameterValue = conduitdiameter.Text;
            DB.ConduitMiddleElevation = ConduitOffset.Text;

            //cable tray data                                                            

            DB.CableTrayHeight = cabletrayheighttxt.Text;
            DB.CableTrayWidth = cabletraywidthtxt.Text;
            DB.CableTrayBottomElevation = cabletraybottomelevation.Text;







            DB.SelectedBlock = blockbox.SelectedItem as string;


            if (categorybox.Text == "Hosted")
            {
                DB.SelectedFloorLevel = (floorlvlbox.SelectedItem as Level);
                DB.SelectedCeilingLevel = (ceillvlbox.SelectedItem as Level);
                if (wallcheck.IsChecked == true)
                {
                    DB.SelectedWallLevel = (walllvllbox.SelectedItem as Level);
                    DB.SelectedWallType = (walltypesbox.SelectedItem as WallType);

                }

                DB.SelectedType = (familybox.SelectedItem as ElementType).Name;


                if (wallcheck.IsChecked == true)
                {
                    _ev.Raise();
                    
                }
                else if (Floorcheck.IsChecked == true)
                {
                    _ev2.Raise();
                  

                }
            }
            else if (categorybox.Text == "Non Hosted")
            {
                DB.SelectedType = (nonhostedfamilybox.SelectedItem as ElementType).Name;
                _ev3.Raise();
                

            }
            else if (categorybox.Text == "Groups")
            {
                _ev5.Raise();
               
            }
            else
            {
                _ev4.Raise();
               
            }








        }
        //private void OK_Click(object sender, RoutedEventArgs e)
        //{
           

        //}

        private void Floorcheck_Checked(object sender, RoutedEventArgs e)
        {

            walllvllbox.IsEnabled = false;
            floorlvlbox.IsEnabled = true;
            ceillvlbox.IsEnabled = true;

            wallcheck.IsChecked = false;
            //NonHostcheck.IsChecked = false;
            //Linebasedcheck.IsChecked = false;
            categorybox.IsEnabled = false;



        }

        private void wallcheck_Checked(object sender, RoutedEventArgs e)
        {
            walllvllbox.IsEnabled = true;

            Floorcheck.IsChecked = false;
            //NonHostcheck.IsChecked = false;
            //Linebasedcheck.IsChecked = false;
            categorybox.IsEnabled = false;



        }

        private void NonHostcheck_Checked(object sender, RoutedEventArgs e)
        {
            walllvllbox.IsEnabled = false;
            floorlvlbox.IsEnabled = false;
            ceillvlbox.IsEnabled = false;

            Floorcheck.IsChecked = false;
            wallcheck.IsChecked = false;
            //Linebasedcheck.IsChecked = false;
            categorybox.IsEnabled = false;


        }

        private void Linebasedcheck_Checked(object sender, RoutedEventArgs e)
        {

            Floorcheck.IsChecked = false;
            //NonHostcheck.IsChecked = false;
            wallcheck.IsChecked = false;
            categorybox.IsEnabled = true;

        }

        private void categorybox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (categorybox.SelectedItem as string == "Hosted")
            {
                Hostedtab.IsEnabled = true;
                Hostedtab.Focus();


                NonHostedtab.IsEnabled = false;
                Ductstab.IsEnabled = false;
                Pipestab.IsEnabled = false;
                Conduitstab.IsEnabled = false;
                CableTraystab.IsEnabled = false;
            }
            if (categorybox.SelectedItem as string == "Non Hosted")
            {
                NonHostedtab.IsEnabled = true;
                NonHostedtab.Focus();

                Hostedtab.IsEnabled = false;
                Ductstab.IsEnabled = false;
                Pipestab.IsEnabled = false;
                Conduitstab.IsEnabled = false;
                CableTraystab.IsEnabled = false;
            }
            if (categorybox.SelectedItem as string == "Ducts")
            {
                Ductstab.IsEnabled = true;
                Ductstab.Focus();

                Hostedtab.IsEnabled = false;
                NonHostedtab.IsEnabled = false;
                Pipestab.IsEnabled = false;
                Conduitstab.IsEnabled = false;
                CableTraystab.IsEnabled = false;
            }
            else if (categorybox.SelectedItem as string == "Pipes")
            {
                Pipestab.IsEnabled = true;
                Pipestab.Focus();

                Hostedtab.IsEnabled = false;
                NonHostedtab.IsEnabled = false;
                Ductstab.IsEnabled = false;
                Conduitstab.IsEnabled = false;
                CableTraystab.IsEnabled = false;
            }
            else if (categorybox.SelectedItem as string == "CableTrays")
            {
                CableTraystab.IsEnabled = true;
                CableTraystab.Focus();

                Hostedtab.IsEnabled = false;
                NonHostedtab.IsEnabled = false;
                Pipestab.IsEnabled = false;
                Conduitstab.IsEnabled = false;
                Ductstab.IsEnabled = false;

            }
            else if (categorybox.SelectedItem as string == "Conduits")
            {
                Conduitstab.IsEnabled = true;
                Conduitstab.Focus();

                Hostedtab.IsEnabled = false;
                NonHostedtab.IsEnabled = false;
                Pipestab.IsEnabled = false;
                Ductstab.IsEnabled = false;
                CableTraystab.IsEnabled = false;
            }
        }
     
     

    }
}
