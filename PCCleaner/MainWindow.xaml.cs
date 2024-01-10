using System.Diagnostics;
using System.IO;
using System.Windows;

namespace PCCleaner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DirectoryInfo winTemp;
        public DirectoryInfo appTemp;     

        public MainWindow()
        {
            InitializeComponent();
            winTemp = new DirectoryInfo(@"C:\Windows\Temp");
            appTemp = new DirectoryInfo(System.IO.Path.GetTempPath());
        }
        
        public long DirSize(DirectoryInfo dir)
        {
            return dir.GetFiles().Sum(files => files.Length) + dir.GetDirectories().Sum(directories => DirSize(directories));
        }

        public void ClearTempData(DirectoryInfo dir)
        {
            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {
                    file.Delete();
                    Console.WriteLine(file.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur : {ex.Message}");
                }
            }

            foreach (DirectoryInfo directory in dir.GetDirectories())
            {
                try
                {
                    directory.Delete(true);
                    Console.WriteLine(directory.FullName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur : {ex.Message}");
                }
            }
        }

        private void Button_Nettoyer_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Nettoyage en cours...");
            btnClean.Content = "\nNETTOYAGE EN COURS...";

            Clipboard.Clear();

            try
            {
                ClearTempData(winTemp);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }

            try
            {
                ClearTempData(appTemp);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }

            btnClean.Content = "\nNETTOYAGE TERMINÉ !";
            size.Content = "0 Mb";
        }

        private void Button_Histo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TODO: Créer page historique", "Historique", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_MAJ_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Votre logiciel est à jour !", "Mise à jour", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button_Web_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("http://anthony-cardinale.fr/pc-cleaner")
                {
                    UseShellExecute = true
                });
            } catch (Exception ex) 
            {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
        }

        private void Button_Analyser_Click(object sender, RoutedEventArgs e)
        {
            AnalyseFolders();
        }

        public void AnalyseFolders()
        {
            Console.WriteLine("Début de l'analyse");
            long totalSize = 0;

            try
            {
                totalSize += DirSize(winTemp) / 100000;
                totalSize += DirSize(appTemp) / 100000;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Impossible d'analyser les dossiers : {ex.Message}");
            }

            size.Content = $"{totalSize} Mb";
            title.Content = "Analyse effectuée !";
            date.Content = DateTime.Today;
        }
    }
}