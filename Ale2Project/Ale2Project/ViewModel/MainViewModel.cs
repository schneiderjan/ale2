using System;
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

        //Services
        private readonly IFileService _fileService;
        private readonly IGraphVizService _graphVizService;
        private readonly IDfaCheckService _ndaCheckService;
        private readonly IAcceptedStringCheckService _acceptedStringCheckService;

        //Models
        private GraphVizFileModel _file;
        private AutomatonModel _automaton;

        #region Commands
        //Commands
        private RelayCommand _parseFileCommand;
        private RelayCommand _openFileCommand;
        private RelayCommand _showAutomatonCommand;
        private RelayCommand _verifyStringCommand;
        private string _verifyStringInput;

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
        #endregion

        #region Properties
        public GraphVizFileModel File
        {
            get { return _file; }
            set { _file = value; RaisePropertyChanged(); }
        }

        public AutomatonModel Automaton
        {
            get { return _automaton; }
            set { _automaton = value; VerifyStringCommmand.RaiseCanExecuteChanged(); }
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

        #endregion

        public MainViewModel(IFileService fileService, IGraphVizService graphVizService, IDfaCheckService ndaCheckService, IAcceptedStringCheckService acceptedStringCheckService)
        {
            _fileService = fileService;
            _graphVizService = graphVizService;
            _ndaCheckService = ndaCheckService;
            _acceptedStringCheckService = acceptedStringCheckService;

            OpenFileCommand = new RelayCommand(OpenFile, () => true);
            ParseFileCommand = new RelayCommand(ParseFile, ParseFileCanExecute);
            ShowAutomatonCommand = new RelayCommand(ShowAutomaton, ShowAutomatonCanExecute);
            VerifyStringCommmand = new RelayCommand(VerifyString, VerifyStringCanExecute);
        }

        private void ParseFile()
        {
            Automaton = _fileService.ParseGraphVizFile(File);

            _automaton.IsNda = _ndaCheckService.IsAutomatonDfa(_automaton);
            IsDfa = _automaton.IsNda;

            ShowAutomatonCommand.RaiseCanExecuteChanged();
        }

        private void OpenFile()
        {
            File = _fileService.ReadFile();
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
    }
}