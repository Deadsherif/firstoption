using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using CableTrayHangers.MVVM.Model;
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
using CableTrayHangers.Commands;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Electrical;
using CableTrayHangers.MVVM.View;

namespace CableTrayHangers.MVVM.ViewModel
{
    public class TraySupportViewModel : INotifyPropertyChanged
    {
        #region Imp
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion


        #region Fields
        int numOfPipes;
        string singleLength;
        double totalLength;
        ObservableCollection<FamilySymbol> pipesFamilyTypes;
        ObservableCollection<Line> pipeLines;
        double spacing;
        string anchorParaName;
        string centerElevationParaName;
        FamilySymbol selectedType;
        int floorId;
        double tolarance;
        double widthtolarance;
        string widthParaName;
        string heightParaName;

        #endregion



        #region Properties
        public int NumOfPipes
        {
            get { return numOfPipes; }
            set
            {
                numOfPipes = value;
                OnPropertyChanged();
            }
        }
        public string SingleLength
        {
            get
            {
                return singleLength;
            }
            set
            {
                singleLength = value;
                OnPropertyChanged();
            }
        }
        public double TotalLength
        {
            get
            {
                return totalLength;
            }
            set
            {
                totalLength = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FamilySymbol> PipesFamilyTypes
        {
            get
            {
                return pipesFamilyTypes;
            }
            set
            {
                pipesFamilyTypes = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Line> PipeLines
        {
            get
            {
                return pipeLines;
            }
            set
            {
                pipeLines = value;
                OnPropertyChanged();
            }
        }

        public double Spacing
        {
            get
            {
                return spacing;
            }
            set
            {
                spacing = value;
                OnPropertyChanged();
            }
        }

        public string AnchorParaName
        {
            get
            {
                return anchorParaName;
            }
            set
            {
                anchorParaName = value;
                OnPropertyChanged();
            }
        }
        public string CenterElevationParaName
        {
            get
            {
                return centerElevationParaName;
            }
            set
            {
                centerElevationParaName = value;
                OnPropertyChanged();
            }
        }

        public FamilySymbol SelectedType
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

        public ICommand createBTN { get; set; }

        public ICommand selectCeilingBTN { get; set; }

        public int FloorId
        {
            get
            {
                return floorId;
            }
            set
            {
                floorId = value;
                OnPropertyChanged();
            }
        }

        public double floorButtom_Z { get; set; }

        public double Tolarance
        {
            get
            {
                return tolarance;
            }
            set
            {
                tolarance = value;
                OnPropertyChanged();
            }
        }
        public double WidthTolarance
        {
            get
            {
                return widthtolarance;
            }
            set
            {
                widthtolarance = value;
                OnPropertyChanged();
            }
        }

        public string WidthParaName
        {
            get
            {
                return widthParaName;
            }
            set
            {
                widthParaName = value;
                OnPropertyChanged();
            }
        }
        public string HeightParaName
        {
            get
            {
                return heightParaName;
            }
            set
            {
                heightParaName = value;
                OnPropertyChanged();
            }
        }
        #endregion
        private PipeSupportView frm;
        private ExternalEvent _externalEvent;

        #region Constructor
        public TraySupportViewModel(PipeSupportView _frm,ExternalEvent externalEvent)
        {
            _externalEvent= externalEvent;
            AnchorParaName = "AnchorElevation";
            WidthParaName = "Width";
            HeightParaName = "Height";
            Tolarance = 5;
            WidthTolarance = 5;
            CenterElevationParaName = "CenterElevation";
            Spacing = 1000;
            FloorId = -1;
            PipeLines = new ObservableCollection<Line>();

            numOfPipes = Command.FinalPipes.Count;
            if (numOfPipes > 1)
            {
                SingleLength = "var";
            }
            else
            {
                Element pipe = Command.FinalPipes.FirstOrDefault();
                if (pipe != null)
                {
                    SingleLength = Math.Round(((pipe.Location as LocationCurve).Curve.Length / 0.00328084), 0).ToString();
                }
                else
                {
                    SingleLength = "null";
                }
            }

            TotalLength = 0;
            foreach (Element pipe in Command.FinalPipes)
            {
                Curve crv = (pipe.Location as LocationCurve).Curve;
                PipeLines.Add(crv as Line);
                TotalLength += crv.Length;
            }

            TotalLength = Math.Round(TotalLength / 0.00328084, 0);

            PipesFamilyTypes = Helper.GetAllMechanicalEquipmentFamilySymbols(Command.Doc);
            createBTN = new RelayCommand(P => CreatSupportCommand(P));
            selectCeilingBTN = new RelayCommand(P => SelectCeilingCommand(P));
        }

        #endregion

        #region Buttons Actions
        public void CreatSupportCommand(object parameter)
        {
            _externalEvent.Raise();
        }
        public void SelectCeilingCommand(object parameter)
        {
            Document doc = Command.Doc;
            UIDocument uiDoc = Command.UiDoc;
           
            Reference reff = null;
            try
            {
                reff = uiDoc.Selection.PickObject(ObjectType.Face, "Select Face");
            }
            catch
            {
                reff = null;
            }
            if (reff != null)
            {
                Element ele = doc.GetElement(reff);

                double HangerLoc = 0;
                try
                {

                    HangerLoc = reff.GlobalPoint.Z;


                    this.FloorId = ele.Id.IntegerValue;
                    this.floorButtom_Z = HangerLoc;

                }
                catch
                {

                    MessageBox.Show("Selected Face can't be selected!");
                    this.FloorId = -1;

                }
            }

           
        }
        #endregion


    }
}
