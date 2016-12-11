using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Ale2Project.Model;
using Ale2Project.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Ale2Project.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        //Bindings
        private bool _isDfa;
        private bool _isStringAccepted;
        private string _verifyStringInput;
        private string _regularExpressionInput;

        //Services
        private readonly IFileService _fileService;
        private readonly IGraphVizService _graphVizService;
        private readonly IDfaCheckService _ndaCheckService;
        private readonly IAcceptedStringCheckService _acceptedStringCheckService;
        private readonly IRegularExpressionParserService _regularExpressionParserService;

        //Models/Properties
        private ObservableCollection<string> _fileLines;
        private ObservableCollection<string> _regularExpressionLines;
        private GraphVizFileModel _file;
        private GraphVizFileModel _fileRe;
        private AutomatonModel _automaton;
        private AutomatonModel _automatonRe;

        #region Commands
        //Commands
        private RelayCommand _parseFileCommand;
        private RelayCommand _openFileCommand;
        private RelayCommand _showAutomatonCommand;
        private RelayCommand _verifyStringCommand;
        private RelayCommand _parseRegularExpressionCommand;
        private RelayCommand _copyReLinesCommand;

        public RelayCommand VerifyStringCommmand
        {
            get { return _verifyStringCommand; }
            set { _verifyStringCommand = value; RaisePropertyChanged(); }
        }

        public RelayCommand OpenFileCommand
        {
            get { return _openFileCommand; }
            set { _openFileCommand = value; RaisePropertyChanged(); }
        }
        public RelayCommand ParseFileCommand
        {
            get { return _parseFileCommand; }
            set { _parseFileCommand = value; RaisePropertyChanged(); }
        }

        public RelayCommand ShowAutomatonCommand
        {
            get { return _showAutomatonCommand; }
            set { _showAutomatonCommand = value; RaisePropertyChanged(); }
        }

        private bool ShowAutomatonCanExecute()
        {
            if (Automaton != null) return true;
            else return false;
        }

        private bool ParseFileCanExecute()
        {
            if (File != null) return true;
            else return false;
        }

        public bool VerifyStringCanExecute()
        {
            if (!String.IsNullOrEmpty(_verifyStringInput) && Automaton != null) return true;
            else return false;
        }

        public RelayCommand ParseRegularExpressionCommand
        {
            get { return _parseRegularExpressionCommand; }
            set { _parseRegularExpressionCommand = value; }
        }


        public RelayCommand CopyRELinesCommand
        {
            get { return _copyReLinesCommand; }
            set { _copyReLinesCommand = value; RaisePropertyChanged(); }
        }


        public bool ParseRegularExpressionCanExecute()
        {
            if (!string.IsNullOrEmpty(_regularExpressionInput)) return true;
            return false;
        }
        #endregion

        #region Properties
        public GraphVizFileModel File
        {
            get { return _file; }
            set { _file = value; RaisePropertyChanged(); }
        }

        public GraphVizFileModel FileRE
        {
            get { return _fileRe; }
            set { _fileRe = value; RaisePropertyChanged(); }
        }

        public AutomatonModel Automaton
        {
            get { return _automaton; }
            set { _automaton = value; VerifyStringCommmand.RaiseCanExecuteChanged(); }
        }

        public AutomatonModel AutomatonRE
        {
            get { return _automatonRe; }
            private set { _automatonRe = value; RaisePropertyChanged(); }
        }

        public bool IsDfa
        {
            get { return _isDfa; }
            set { _isDfa = value; RaisePropertyChanged(); }
        }

        public bool IsStringAccepted
        {
            get { return _isStringAccepted; }
            set { _isStringAccepted = value; RaisePropertyChanged(); }
        }

        public string VerifyStringInputInput
        {
            get { return _verifyStringInput; }
            set
            {
                _verifyStringInput = value;
                RaisePropertyChanged();
                VerifyStringCommmand.RaiseCanExecuteChanged();
            }
        }

        public string RegularExpressionInput
        {
            get { return _regularExpressionInput; }
            set
            {
                _regularExpressionInput = value;
                RaisePropertyChanged();
                ParseRegularExpressionCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<string> FileLines
        {
            get { return _fileLines; }
            set { _fileLines = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<string> RegularExpressionLines
        {
            get { return _regularExpressionLines; }
            set { _regularExpressionLines = value; RaisePropertyChanged(); }
        }

        #endregion

        public MainViewModel(IFileService fileService,
            IGraphVizService graphVizService,
            IDfaCheckService ndaCheckService,
            IAcceptedStringCheckService acceptedStringCheckService,
            IRegularExpressionParserService regularExpressionParserService)
        {
            _fileService = fileService;
            _graphVizService = graphVizService;
            _ndaCheckService = ndaCheckService;
            _acceptedStringCheckService = acceptedStringCheckService;
            _regularExpressionParserService = regularExpressionParserService;

            _file = new GraphVizFileModel();

            OpenFileCommand = new RelayCommand(OpenFile, () => true);
            ParseFileCommand = new RelayCommand(ParseFile, ParseFileCanExecute);
            ShowAutomatonCommand = new RelayCommand(ShowAutomaton, ShowAutomatonCanExecute);
            VerifyStringCommmand = new RelayCommand(VerifyString, VerifyStringCanExecute);
            ParseRegularExpressionCommand = new RelayCommand(ParseRegularExpression, ParseRegularExpressionCanExecute);
            CopyRELinesCommand = new RelayCommand(CopyRELines, (() => true));


            RegularExpressionInput = "|(a,*b)";
            //|(.(a,*(b)),*(c))
            //|(*(a),.(b,c))
            //"*(|(*(.(a,b)),c))"
            //|(a,*b)
        }

        private void ParseRegularExpression()
        {
            AutomatonRE = _regularExpressionParserService.GetAutomaton(_regularExpressionInput);
            FileRE = _graphVizService.ConvertToGraphVizFile(AutomatonRE);
            RegularExpressionLines = new ObservableCollection<string>(_fileRe.Lines);
        }

        private void ParseFile()
        {
            Automaton = _fileService.ParseGraphVizFile(File);

            _automaton.IsDfa = _ndaCheckService.IsAutomatonDfa(_automaton);
            IsDfa = _automaton.IsDfa;

            ShowAutomatonCommand.RaiseCanExecuteChanged();
        }

        private void OpenFile()
        {
            File = _fileService.ReadFile();
            FileLines = new ObservableCollection<string>(File.Lines);
            ParseFileCommand.RaiseCanExecuteChanged();
        }

        private void ShowAutomaton()
        {
            _fileService.WriteGraphVizFileToDotFile(File.Lines);
            _graphVizService.DisplayAutomaton();
        }

        private void VerifyString()
        {
            IsStringAccepted = _acceptedStringCheckService.IsAcceptedString(_verifyStringInput, Automaton);
        }

        private void CopyRELines()
         {
            if (_fileRe == null) return;
            var text = "";

            foreach (var fileReLine in _fileRe.Lines)
            {
                text += fileReLine+Environment.NewLine;
            }
            Clipboard.SetText(text);
        }
    }
}