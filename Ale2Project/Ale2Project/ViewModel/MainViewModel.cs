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
        private readonly IDfaService _dfaService;
        private readonly ILanguageCheckService _languageCheckService;
        private readonly IRegularExpressionParserService _regularExpressionParserService;

        //Models/Properties
        private ObservableCollection<string> _fileLines;
        private ObservableCollection<string> _regularExpressionLines;
        private ObservableCollection<string> _words;
        private GraphVizFileModel _file;
        private GraphVizFileModel _fileRe;
        private AutomatonModel _automaton;
        private AutomatonModel _automatonRe;
        private AutomatonModel _convertedDfa;


        #region Commands
        //Commands
        private RelayCommand _parseFileCommand;
        private RelayCommand _openFileCommand;
        private RelayCommand _showAutomatonCommand;
        private RelayCommand _verifyStringCommand;
        private RelayCommand _parseRegularExpressionCommand;
        private RelayCommand _copyReLinesCommand;
        private RelayCommand _showAllWordsCommand;
        private RelayCommand _convertToDfaCommand;
        private RelayCommand _copyConvertLinesCommand;
        private GraphVizFileModel _fileConvertedDfa;
        private ObservableCollection<string> _convertedDfaLines;
        private bool _isPda;

        public RelayCommand CopyConvertLinesCommand
        {
            get { return _copyConvertLinesCommand; }
            set { _copyConvertLinesCommand = value; RaisePropertyChanged(); }
        }

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

        public RelayCommand ShowAllWordsCommand
        {
            get { return _showAllWordsCommand; }
            set { _showAllWordsCommand = value; RaisePropertyChanged(); }
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
            return Automaton != null;
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

        public RelayCommand ConvertToDfaCommand
        {
            get { return _convertToDfaCommand; }
            set { _convertToDfaCommand = value; RaisePropertyChanged(); }
        }

        public bool ConvertToDfaCanExecute()
        {
            if (_automaton != null &&
                !_isDfa) return true;
            return false;
        }
        #endregion

        #region Properties
        public GraphVizFileModel File
        {
            get { return _file; }
            set { _file = value; RaisePropertyChanged(); }
        }
        public bool IsPda
        {
            get { return _isPda; }
            set { _isPda = value; RaisePropertyChanged(); }
        }

        public GraphVizFileModel FileConvertedDfa
        {
            get { return _fileConvertedDfa; }
            set { _fileConvertedDfa = value; RaisePropertyChanged(); }
        }
        public ObservableCollection<string> ConvertedDfaLines
        {
            get { return _convertedDfaLines; }
            set { _convertedDfaLines = value; RaisePropertyChanged(); }
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

        public AutomatonModel ConvertedDfa
        {
            get { return _convertedDfa; }
            set { _convertedDfa = value; RaisePropertyChanged(); }
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

        public ObservableCollection<string> Words
        {
            get { return _words; }
            set { _words = value; RaisePropertyChanged(); }
        }


        #endregion

        public MainViewModel(IFileService fileService,
            IGraphVizService graphVizService,
            IDfaService dfaService,
            ILanguageCheckService languageCheckService,
            IRegularExpressionParserService regularExpressionParserService)
        {
            _fileService = fileService;
            _graphVizService = graphVizService;
            _dfaService = dfaService;
            _languageCheckService = languageCheckService;
            _regularExpressionParserService = regularExpressionParserService;

            _words = new ObservableCollection<string>();
            _file = new GraphVizFileModel();

            OpenFileCommand = new RelayCommand(OpenFile, () => true);
            ParseFileCommand = new RelayCommand(ParseFile, ParseFileCanExecute);
            ShowAutomatonCommand = new RelayCommand(ShowAutomaton, ShowAutomatonCanExecute);
            VerifyStringCommmand = new RelayCommand(VerifyString, VerifyStringCanExecute);
            ParseRegularExpressionCommand = new RelayCommand(ParseRegularExpression, ParseRegularExpressionCanExecute);
            CopyRELinesCommand = new RelayCommand(CopyRELines, () => true);
            ShowAllWordsCommand = new RelayCommand(ShowAllWords, ShowAutomatonCanExecute);
            ConvertToDfaCommand = new RelayCommand(ConvertToDfa, ConvertToDfaCanExecute);
            CopyConvertLinesCommand = new RelayCommand(CopyConvertLines, () => true);

            RegularExpressionInput = "|(a,*b)";
            //|(.(a,*(b)),*(c))
            //|(*(a),.(b,c))
            //"*(|(*(.(a,b)),c))"
            //|(a,*b)
        }

        private void ConvertToDfa()
        {
            ConvertedDfa = _dfaService.ConvertNdfaToNfa(Automaton);
            FileConvertedDfa = _graphVizService.ConvertToGraphVizFile(ConvertedDfa);
            ConvertedDfaLines = new ObservableCollection<string>(_fileConvertedDfa.Lines);
        }

        private void ParseRegularExpression()
        {
            Automaton = _regularExpressionParserService.GetAutomaton(_regularExpressionInput);
            FileRE = _graphVizService.ConvertToGraphVizFile(Automaton);
            RegularExpressionLines = new ObservableCollection<string>(_fileRe.Lines);
        }

        private void ParseFile()
        {
            Automaton = _fileService.ParseGraphVizFile(File);
            if (_automaton.IsPda)
            {
                IsPda = _automaton.IsPda;
            }
            else
            {
                _automaton.IsDfa = _dfaService.IsAutomatonDfa(_automaton);
                IsDfa = _automaton.IsDfa;
            }
            ShowAutomatonCommand.RaiseCanExecuteChanged();
            ShowAllWordsCommand.RaiseCanExecuteChanged();
            ConvertToDfaCommand.RaiseCanExecuteChanged();
            VerifyStringCommmand.RaiseCanExecuteChanged();
        }

      
        private void OpenFile()
        {
            File = _fileService.ReadFile();
            FileLines = new ObservableCollection<string>(File.Lines);
            ParseFileCommand.RaiseCanExecuteChanged();
        }

        private void ShowAutomaton()
        {
            //_fileService.WriteGraphVizFileToDotFile(File.Lines);
            var tempFile = _graphVizService.ConvertToGraphVizFile(_automaton);
            _fileService.WriteGraphVizFileToDotFile(tempFile.Lines);
            _graphVizService.DisplayAutomaton();
        }

        private void VerifyString()
        {
            if (Automaton.IsPda) IsStringAccepted = _languageCheckService.IsAcceptedStringByPda(_verifyStringInput, Automaton);
            else IsStringAccepted = _languageCheckService.IsAcceptedString(_verifyStringInput, Automaton);
        }

        private void CopyRELines()
        {
            if (_fileRe == null) return;
            var text = "";

            foreach (var fileReLine in _fileRe.Lines)
            {
                text += fileReLine + Environment.NewLine;
            }
            Clipboard.SetText(text);
        }

        private void CopyConvertLines()
        {
            if (_convertedDfaLines == null) return;
            var text = "";

            foreach (var line in _fileConvertedDfa.Lines)
            {
                text += line + Environment.NewLine;
            }
            Clipboard.SetText(text);
        }

        private void ShowAllWords()
        {
            var words = _languageCheckService.GetAllWords(_automaton);
            Words = new ObservableCollection<string>(words);
        }
    }
}