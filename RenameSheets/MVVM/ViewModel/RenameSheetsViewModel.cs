using Autodesk.Revit.DB;
using RenameSheets.Commands;
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

namespace RenameSheets.MVVM.ViewModel
{
    public class RenameSheetsViewModel : INotifyPropertyChanged
    {
        #region Imp
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion


        #region Fields
        bool createNum;
        bool createName;
        Parameter selectedParameter;
        Parameter _selectedParameter;
        int selectedSheets;
        ObservableCollection<Parameter> sheetParameters;
        ObservableCollection<Parameter> builtParameters;
        ObservableCollection<Parameter> _builtParameters;
        ObservableCollection<string> prefexs;
        ObservableCollection<string> _prefexs;
        ObservableCollection<string> sufixes;
        ObservableCollection<string> _sufixes;
        string newName;
        string newNumber;
        string prefex;
        string _prefex;
        string sufix;
        string _sufix;

        #endregion

        #region Properties

        public bool CreateNum
        {
            get
            {
                return createNum;
            }
            set
            {
                createNum = value;
                OnPropertyChanged();
            }
        }
        public bool CreateName
        {
            get
            {
                return createName;
            }
            set
            {
                createName = value;
                OnPropertyChanged();
            }
        }
        public int SelectedSheets
        {
            get
            {
                return selectedSheets;
            }
            set
            {
                selectedSheets = value;
                OnPropertyChanged();
            }
        }
        public Parameter SelectedParameter
        {
            get
            {
                return selectedParameter;
            }
            set
            {
                selectedParameter = value;
                OnPropertyChanged();
            }
        }
        public Parameter _SelectedParameter
        {
            get
            {
                return _selectedParameter;
            }
            set
            {
                _selectedParameter = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Parameter> SheetParameters
        {
            get
            {
                return sheetParameters;
            }
            set
            {
                sheetParameters = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Parameter> BuiltParameters
        {
            get
            {
                return builtParameters;
            }
            set
            {
                builtParameters = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Parameter> _BuiltParameters
        {
            get
            {
                return _builtParameters;
            }
            set
            {
                _builtParameters = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Prefexs
        {
            get
            {
                return prefexs;
            }
            set
            {
                prefexs = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> _Prefexs
        {
            get
            {
                return _prefexs;
            }
            set
            {
                _prefexs = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> Sufixes
        {
            get
            {
                return sufixes;
            }
            set
            {
                sufixes = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> _Sufixes
        {
            get
            {
                return _sufixes;
            }
            set
            {
                _sufixes = value;
                OnPropertyChanged();
            }
        }

        public string NewName
        {
            get
            {
                return newName;
            }
            set
            {
                newName = value;
                OnPropertyChanged();
            }
        }
        public string NewNumber
        {
            get
            {
                return newNumber;
            }
            set
            {
                newNumber = value;
                OnPropertyChanged();
            }
        }
        public string Prefex
        {
            get
            {
                return prefex;
            }
            set
            {
                prefex = value;
                OnPropertyChanged();
            }
        }
        public string _Prefex
        {
            get
            {
                return _prefex;
            }
            set
            {
                _prefex = value;
                OnPropertyChanged();
            }
        }
        public string Sufix
        {
            get
            {
                return sufix;
            }
            set
            {
                sufix = value;
                OnPropertyChanged();
            }
        }

        public string _Sufix
        {
            get
            {
                return _sufix;
            }
            set
            {
                _sufix = value;
                OnPropertyChanged();
            }
        }


        public ICommand AddNameBTN { get; set; }
        public ICommand DeleteNameBTN { get; set; }

        public ICommand AddNumberBTN { get; set; }

        public ICommand DeleteNumberBTN { get; set; }

        public ICommand runBTN { get; set; }
        #endregion

        #region Constructor
        public RenameSheetsViewModel()
        {
            SheetParameters = Command.SheetsParameters;
            BuiltParameters = new ObservableCollection<Parameter>();
            _BuiltParameters = new ObservableCollection<Parameter>();
            Prefexs = new ObservableCollection<string>();
            _Prefexs = new ObservableCollection<string>();
            Sufixes = new ObservableCollection<string>();
            _Sufixes = new ObservableCollection<string>();
            AddNameBTN = new RelayCommand(p=>AddNameCommand(p));
            DeleteNameBTN = new RelayCommand(p => deleteNameCommand(p));
            AddNumberBTN = new RelayCommand(p => addNumberCommand(p));
            DeleteNumberBTN = new RelayCommand(p => deleteNumberCommand(p));
            runBTN = new RelayCommand(p => runCommand(p));
            SelectedSheets = Command.FinalViewSheets.Count;
            CreateNum = true;
            CreateName = true;
        }
        #endregion

        #region Buttons Actions
        public void AddNameCommand(object parameter)
        {
            if (Command.frm.VM.SelectedParameter != null)
            {
                Parameter SelectedPara = Command.frm.VM.SelectedParameter;
                string prefex = Command.frm.VM.Prefex;
                string sufix = Command.frm.VM.Sufix;
                string ParaName = SelectedPara.Definition.Name;

                Command.frm.VM.NewName += prefex + ParaName + sufix;
                Command.frm.VM.BuiltParameters.Add(SelectedPara);

                if (prefex != null)
                {
                    Command.frm.VM.Prefexs.Add(prefex);
                }
                else
                {
                    Command.frm.VM.Prefexs.Add("");

                }
                if (sufix != null)
                {
                    Command.frm.VM.Sufixes.Add(sufix);
                }
                else
                {
                    Command.frm.VM.Sufixes.Add("");

                }

            }
            else
            {
                MessageBox.Show("Can't build without selecting a parameter to build from");
            }
        }
        public void deleteNameCommand(object parameter)
        {
            if (Command.frm.VM.BuiltParameters.Count > 0)
            {
                string ToRemoveName = Command.frm.VM.BuiltParameters[Command.frm.VM.BuiltParameters.Count - 1].Definition.Name;
                string ToRemovePrefex = Command.frm.VM.Prefexs[Command.frm.VM.Prefexs.Count - 1];
                string ToRemoveSufix = Command.frm.VM.Sufixes[Command.frm.VM.Sufixes.Count - 1];

                int x = ToRemoveName?.Length ?? 0;
                int y = ToRemovePrefex?.Length ?? 0;
                int z = ToRemoveSufix?.Length ?? 0;

                int ToRemoveLenght = x + y + z;
                Command.frm.VM.NewName = Command.frm.VM.NewName.Substring(0, Command.frm.VM.NewName.Length - ToRemoveLenght);

                Command.frm.VM.BuiltParameters.RemoveAt(Command.frm.VM.BuiltParameters.Count - 1);
                Command.frm.VM.Prefexs.RemoveAt(Command.frm.VM.Prefexs.Count - 1);
                Command.frm.VM.Sufixes.RemoveAt(Command.frm.VM.Sufixes.Count - 1);
            }
        }
        public void addNumberCommand(object parameter)
        {
            if (Command.frm.VM._SelectedParameter != null)
            {
                Parameter SelectedPara = Command.frm.VM._SelectedParameter;
                string prefex = Command.frm.VM._Prefex;
                string sufix = Command.frm.VM._Sufix;
                string ParaName = SelectedPara.Definition.Name;


                Command.frm.VM.NewNumber += prefex + ParaName + sufix;
                Command.frm.VM._BuiltParameters.Add(SelectedPara);




                if (prefex != null)
                {
                    Command.frm.VM._Prefexs.Add(prefex);
                }
                else
                {
                    Command.frm.VM._Prefexs.Add("");

                }
                if (sufix != null)
                {
                    Command.frm.VM._Sufixes.Add(sufix);
                }
                else
                {
                    Command.frm.VM._Sufixes.Add("");

                }

            }
            else
            {
                MessageBox.Show("Can't build number without selecting a parameter to build from");
            }
        }
        public void deleteNumberCommand(object parameter)
        {
            if (Command.frm.VM._BuiltParameters.Count > 0)
            {
                string ToRemoveName = Command.frm.VM._BuiltParameters[Command.frm.VM._BuiltParameters.Count - 1].Definition.Name;
                string ToRemovePrefex = Command.frm.VM._Prefexs[Command.frm.VM._Prefexs.Count - 1];
                string ToRemoveSufix = Command.frm.VM._Sufixes[Command.frm.VM._Sufixes.Count - 1];

                int x = ToRemoveName?.Length ?? 0;
                int y = ToRemovePrefex?.Length ?? 0;
                int z = ToRemoveSufix?.Length ?? 0;

                int ToRemoveLenght = x + y + z;
                Command.frm.VM.NewNumber = Command.frm.VM.NewNumber.Substring(0, Command.frm.VM.NewNumber.Length - ToRemoveLenght);

                Command.frm.VM._BuiltParameters.RemoveAt(Command.frm.VM._BuiltParameters.Count - 1);
                Command.frm.VM._Prefexs.RemoveAt(Command.frm.VM._Prefexs.Count - 1);
                Command.frm.VM._Sufixes.RemoveAt(Command.frm.VM._Sufixes.Count - 1);
            }
        }
        public void runCommand(object parameter)
        {
            int numOfParameters = Command.frm.VM.BuiltParameters?.Count ?? 0;
            int _numOfParameters = Command.frm.VM._BuiltParameters?.Count ?? 0;
            Document doc = Command.Doc;
            if (_numOfParameters > 0 && Command.frm.VM.CreateNum)
            {
                List<ViewSheet> sheets = Command.FinalViewSheets;

                try
                {
                    int NumExists = 0;
                    using (Transaction trans2 = new Transaction(doc, "Change sheet Numbers"))
                    {
                        trans2.Start();
                        foreach (ViewSheet sheet in sheets)
                        {
                            StringBuilder NumberBuilder = new StringBuilder();

                            for (int i = 0; i < _numOfParameters; i++)
                            {
                                string PName = sheet.LookupParameter(Command.frm.VM._BuiltParameters[i].Definition.Name).AsString();
                                string Prefix = Command.frm.VM._Prefexs[i];
                                string Sufix = Command.frm.VM._Sufixes[i];

                                NumberBuilder.Append(Prefix + PName + Sufix);
                            }

                            string FinalNumber = NumberBuilder.ToString();


                            try
                            {
                                sheet.LookupParameter("Sheet Number").Set(FinalNumber);
                            }
                            catch
                            {
                                NumExists++;
                            }
                        }


                        if (NumExists > 0)
                        {

                            MessageBox.Show($"{NumExists} sheets could not set their Number due to duplication of numbers");

                        }
                        trans2.Commit();
                    }
                }
                catch (Exception e)
                {

                    MessageBox.Show(e.Message);
                }


            }
            if (numOfParameters > 0 && Command.frm.VM.CreateName)
            {
                List<ViewSheet> sheets = Command.FinalViewSheets;
                try
                {
                    using (Transaction trans = new Transaction(doc, "Change sheet Names"))
                    {
                        trans.Start();
                        foreach (ViewSheet sheet in sheets)
                        {
                            StringBuilder NameBuilder = new StringBuilder();

                            for (int i = 0; i < numOfParameters; i++)
                            {
                                string PName = sheet.LookupParameter(Command.frm.VM.BuiltParameters[i].Definition.Name).AsString();
                                string Prefix = Command.frm.VM.Prefexs[i];
                                string Sufix = Command.frm.VM.Sufixes[i];

                                NameBuilder.Append(Prefix + PName + Sufix);
                            }

                            string FinalName = NameBuilder.ToString();


                            sheet.Name = FinalName;
                        }


                        trans.Commit();
                    }
                }
                catch (Exception e)
                {

                    MessageBox.Show(e.Message);
                }

            }

            MessageBox.Show($"   Done    ", "First Options",MessageBoxButton.OK, MessageBoxImage.Information);



        }
        #endregion
    }
}
