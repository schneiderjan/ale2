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
        //Services
        private readonly IFileService _fileService;

        //Models
        private GraphVizFileModel _file;

        //Commands
        private RelayCommand _parseFileCommand;
        private RelayCommand _openFileCommand;

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

        public GraphVizFileModel File
        {
            get { return _file; }
            set { _file = value; RaisePropertyChanged(); }
        }

        public MainViewModel(IFileService fileService)
        {
            _fileService = fileService;
            OpenFileCommand = new RelayCommand(() => OpenFile(), () => true);
            ParseFileCommand = new RelayCommand( ()=> ParseFile(), () => true );
        }

        private void ParseFile()
        {
            if (File == null) return;
            _fileService.ParseGraphVizFile(File);

        }

        private void OpenFile()
        {
            File = _fileService.ReadFile();
        }
    }
}