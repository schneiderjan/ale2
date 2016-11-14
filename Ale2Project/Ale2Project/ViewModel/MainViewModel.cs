using Ale2Project.Model;
using Ale2Project.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

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
        IFileReaderService _fileReaderService;

        private RelayCommand openFileCommand;
        public RelayCommand OpenFileCommand { get { return openFileCommand; } set { openFileCommand = value; RaisePropertyChanged(); } }

        private GraphVizFile _file;
        public GraphVizFile File
        {
            get { return _file; }
            set { _file = value; RaisePropertyChanged(); }
        }

        public MainViewModel(IFileReaderService fileReaderService)
        {
            _fileReaderService = fileReaderService;
            OpenFileCommand = new RelayCommand(() => OpenFile(), () => true);

        }

        public void OpenFile()
        {
            File = _fileReaderService.ReadFile();
        }
    }
}