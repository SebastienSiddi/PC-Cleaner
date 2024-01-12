using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace PCCleaner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        string version = "1.0.0";
        public DirectoryInfo winTemp;
        public DirectoryInfo appTemp;
        #endregion

        #region Public methods
        public MainWindow()
        {
            InitializeComponent();
            winTemp = new DirectoryInfo(@"C:\Windows\Temp");
            appTemp = new DirectoryInfo(System.IO.Path.GetTempPath());
            CheckActu();            
            GetDate();
        }        

        /// <summary>
        /// Vérifie si une actu est disponible
        /// </summary>
        public void CheckActu()
        {
            string url = "http://localhost/siteweb/actu.txt";
            using (HttpClient client = new())
            {
                string actu = client.GetStringAsync(url).Result;

                if (actu != String.Empty)
                {
                    news.Content = actu;
                    news.Visibility = Visibility.Visible;
                    banner.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Vérifie si une nouvelle version est disponible
        /// </summary>
        public void CheckVersion()
        {
            string url = "http://localhost/siteweb/version.txt";
            using (HttpClient client = new())
            {
                string v = client.GetStringAsync(url).Result;

                if (this.version != v)
                {
                    MessageBox.Show("Une nouvelle version est disponible !", "Mise à jour", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Votre logiciel est à jour !", "Mise à jour", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// Récupère la taille totale des fichiers à traiter
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public long DirSize(DirectoryInfo dir)
        {
            return dir.GetFiles().Sum(files => files.Length) + dir.GetDirectories().Sum(directories => DirSize(directories));
        }

        /// <summary>
        /// Supprime les fichiers temporaires
        /// </summary>
        /// <param name="dir"></param>
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

        /// <summary>
        /// Fonction du bouton "Nettoyer"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            SaveDate();
        }

        /// <summary>
        /// Fonction du bouton "Historique"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Histo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TODO: Créer page historique", "Historique", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Fonction du bouton "Mise à jour"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MAJ_Click(object sender, RoutedEventArgs e)
        {
            CheckVersion();
        }

        /// <summary>
        /// Fonction du bouton "Site Web"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Fonction du bouton "Analyser"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Analyser_Click(object sender, RoutedEventArgs e)
        {
            AnalyseFolders();
        }

        /// <summary>
        /// Analyse et calcule la taille des dossiers à traiter
        /// </summary>
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
            dateAnalyse.Content = DateTime.Now.ToString("G");
            this.SaveDate();
        }        

        /// <summary>
        /// Stocke la date du dernier nettoyage dans un fichier
        /// </summary>
        public void SaveDate()
        {
            string date = DateTime.Now.ToString("G");
            File.WriteAllText("date.txt", date);
        }

        /// <summary>
        /// Retourne la date de la dernière analyse
        /// </summary>
        public void GetDate()
        {
            string date = "";

            try
            {
                date = File.ReadAllText("date.txt");
            }
            catch (Exception)
            {
                Console.WriteLine($"Erreur: Le fichier date.txt n'existe pas");
            }

            if(date != String.Empty)
            {
                dateAnalyse.Content = date;
            }            
        }
        #endregion
    }
}