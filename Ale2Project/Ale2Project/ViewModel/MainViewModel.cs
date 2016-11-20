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
        private bool _isNda;

        //Services
        private readonly IFileService _fileService;
        private readonly IGraphVizService _graphVizService;
        private readonly IDfaCheckService _ndaCheckService;

        //Models
        private GraphVizFileModel _file;
        private AutomatonModel _automaton;

        //Commands
        private RelayCommand _parseFileCommand;
        private RelayCommand _openFileCommand;
        private RelayCommand _showAutomatonCommand;

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
            set { _showAutomatonCommand = value; }
        }

        public GraphVizFileModel File
        {
            get { return _file; }
            set { _file = value; RaisePropertyChanged(); }
        }

        public AutomatonModel Automaton
        {
            get { return _automaton; }
            set { _automaton = value; }
        }

        public bool IsNda
        {
            get { return _isNda; }
            set { _isNda = value; RaisePropertyChanged(); }
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


        public MainViewModel(IFileService fileService, IGraphVizService graphVizService, IDfaCheckService ndaCheckService)
        {
            _fileService = fileService;
            _graphVizService = graphVizService;
            _ndaCheckService = ndaCheckService;

            OpenFileCommand = new RelayCommand(() => OpenFile(), () => true);
            ParseFileCommand = new RelayCommand(() => ParseFile(), () => ParseFileCanExecute());
            ShowAutomatonCommand = new RelayCommand(() => ShowAutomaton(), () => ShowAutomatonCanExecute());
        }

        private void ParseFile()
        {
            Automaton = _fileService.ParseGraphVizFile(File);

            _automaton.IsNda = _ndaCheckService.IsAutomatonNda(_automaton);
            IsNda = _automaton.IsNda;

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
    }
}