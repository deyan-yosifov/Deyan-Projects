using System;
using System.IO;
using System.Windows;

namespace ImageRecognition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ImagesDatabaseFolderName = "ImagesDatabase";
        private const string DatabaseFileName = "database.json";

        public MainWindow()
        {
            InitializeComponent();

            this.EnsureDatabaseExists();
        }

        private void EnsureDatabaseExists()
        {
            string dir = Directory.GetCurrentDirectory();
            string databaseFolderPath = Path.Combine(dir, ImagesDatabaseFolderName);

            if (!Directory.Exists(databaseFolderPath))
            {
                Directory.CreateDirectory(databaseFolderPath);
            }

            string databaseFilePath = Path.Combine(databaseFolderPath, DatabaseFileName);
            if (!File.Exists(databaseFilePath))
            {
                File.Create(databaseFilePath);
            }
        }
    }
}
