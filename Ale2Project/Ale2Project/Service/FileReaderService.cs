using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using Ale2Project.Model;
using Cursor = System.Windows.Input.Cursor;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Ale2Project.Service
{
    public class FileReaderService : IFileReaderService
    {
        public GraphVizFile ReadFile()
        {
            GraphVizFile file = new GraphVizFile();
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            // Display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                file.FilePath = filename;
                file.Lines = System.IO.File.ReadLines(filename).ToList();
            }
            return file;
        }
    }

}

