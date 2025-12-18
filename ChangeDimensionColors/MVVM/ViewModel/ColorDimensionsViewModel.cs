using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using ChangeDimensionColors.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace ChangeDimensionColors.MVVM.ViewModel
{
    public class ColorDimensionsViewModel : INotifyPropertyChanged
    {
        #region Imp
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Fields
        int roundNum;
        int selectedDims;
        Color selectedColor;
        Autodesk.Revit.DB.Color revitColor;
        #endregion


        #region Properties
        public int RoundNum
        {
            get
            { return roundNum; }
            set
            {
                roundNum = value;
                OnPropertyChanged();
            }
        }
        public int SelectedDims
        {
            get
            { return selectedDims; }
            set
            {
                selectedDims = value;
                OnPropertyChanged();
            }
        }
        public Autodesk.Revit.DB.Color RevitColor

        {
            get { return revitColor; }
            set
            {
                revitColor = value;
                OnPropertyChanged();
            }
        }
        public Color SelectedColor
        {
            get
            {
                return selectedColor;
            }
            set
            {
                selectedColor = value;
                OnPropertyChanged();
                RevitColor = new Autodesk.Revit.DB.Color(selectedColor.R, selectedColor.G, selectedColor.B);
            }
        }
        public ICommand selectBTN { get; set; }
        public ICommand DetectBTN { get; set; }

        #endregion


        #region Constructor
        public ColorDimensionsViewModel()
        {
            RoundNum = 3;
            SelectedDims = 0;
            RevitColor = new Autodesk.Revit.DB.Color(212, 0, 205);
            SelectedColor = System.Windows.Media.Color.FromRgb(212, 0, 205);
            selectBTN = new RelayCommand(p=> SelectCommand(p));
            DetectBTN = new RelayCommand(p => DetectCommand(p));
        }
        #endregion

        #region Buttons Actions
        public void SelectCommand(object parameter)
        {
            UIDocument uiDoc = Command.UiDoc;
            SelectDims dimFilter = Command.dimFilter;
            Command.frm.Hide();

            try
            {
                Command.reffEle = uiDoc.Selection.PickObjects(ObjectType.Element, dimFilter, "Select Dimensions").ToList();
            }
            catch
            {

            }
            Command.frm.VM.SelectedDims = Command.reffEle?.Count ?? 0;
            Command.frm.ShowDialog();
        }
        public void DetectCommand(object parameter)
        {
            Document Doc = Command.Doc;
            List<Reference> reffEle = Command.reffEle;

            if (reffEle != null && reffEle.Count > 0)
            {
                try
                {
                    using (Transaction trans = new Transaction(Doc, "Change Dim Color"))
                    {
                        trans.Start();

                        foreach (var ele in reffEle)
                        {
                            Dimension dim = Doc.GetElement(ele) as Dimension;
                            double? dimValue = dim.Value;
                            double dimFinalValue;
                            if (double.TryParse(dimValue.ToString(), out dimFinalValue)) // Single Dimension
                            {
                                if (Helper.HasFractions(Math.Round(UnitUtils.Convert(dimFinalValue, UnitTypeId.Feet, UnitTypeId.Millimeters), 3)))
                                {

                                    OverrideGraphicSettings overrideSettings = new OverrideGraphicSettings();

                                    // Set the dimension color to red (change it to any other color as needed)
                                    overrideSettings.SetProjectionLineColor(Command.frm.VM.RevitColor); // RGB values
                                    overrideSettings.SetProjectionLineWeight(5);

                                    // Apply the override settings to the view
                                    Doc.ActiveView.SetElementOverrides(dim.Id, overrideSettings);

                                    // Refresh the view to see the changes
                                    Doc.ActiveView.Document.Regenerate();
                                }

                            }
                            else
                            {
                                List<double> DimSegements = Helper.GetDimensionValues(dim);
                                foreach (double DimSegement in DimSegements)
                                {

                                    if (Helper.HasFractions(Math.Round(UnitUtils.Convert(DimSegement, UnitTypeId.Feet, UnitTypeId.Millimeters), 3)) || DimSegement < 0.00001)
                                    {

                                        OverrideGraphicSettings overrideSettings = new OverrideGraphicSettings();

                                        // Set the dimension color to red (change it to any other color as needed)
                                        overrideSettings.SetProjectionLineColor(Command.frm.VM.RevitColor); // RGB values
                                        overrideSettings.SetProjectionLineWeight(5);

                                        // Apply the override settings to the view
                                        Doc.ActiveView.SetElementOverrides(dim.Id, overrideSettings);

                                        // Refresh the view to see the changes
                                        Doc.ActiveView.Document.Regenerate();
                                        break;

                                    }
                                }
                            }


                        }
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {

                    MessageBox.Show(e.Message);
                }

            }
            else
            {
                MessageBox.Show("There are no dimensions selected", "FirstOption", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }
        #endregion


    }
}

