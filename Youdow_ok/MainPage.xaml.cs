using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Youdow_ok.Resources;
using System.Collections.ObjectModel;
using HtmlAgilityPack;
using System.IO.IsolatedStorage;
using System.IO;
using System.Text;
using Windows.Storage;
using Windows.Storage.Pickers;
using System.Windows.Media;
using Windows.UI;
using Microsoft.Xna.Framework.Input;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Windows.ApplicationModel.Activation;

namespace Youdow_ok
{
    // TODO le grid DownloadGrid peux être remonté pour voir l'avancement des téléchargements
    // TODO afficher les résultat avec anitmation un par un
    // TODO pouvoir continuer à téléharger même en changeant d'appli
    // TODO restructurer le code avec le dispacher
    // TODO Mezigue n'arrive pas à être téléchargé
    // TODO mettre le focus en haut quand on fait une nouvelle recherche
     
    public partial class MainPage : PhoneApplicationPage
    {
        public static MainPage main;
        Tracks tracksC = new Tracks();
        public static List<int> selectedTracks = new List<int>();
        public static ObservableCollection<Track> trackResult = new ObservableCollection<Track>();

        public MainPage()
        {
            InitializeComponent();
            main = this;
            ThemeManager.ToDarkTheme();
        }

        // Begin search
        private void ButtonSearchGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                WebClient codeSampleReq = new WebClient();
                codeSampleReq.DownloadStringCompleted += getHtmlCodeYoutube;
                codeSampleReq.DownloadStringAsync(new Uri("http://bandeau2015-mail.cerfrance.fr/parse/player/index.php?name=" + SearchInput.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur on search:\n" + ex.Message);
            }
        }

        private void getHtmlCodeYoutube(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.OptionFixNestedTags = true;
                htmlDoc.LoadHtml(e.Result);
                tracksC.SetTrackList(e.Result);
                affResultTracks(tracksC.getList());
                selectedTracks.Clear();
                DownloadButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 64, 64, 64));
                NbTitle.Text = "0 titre";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur:\n" + ex.Message);
            }
        }

        private void affResultTracks(List<Track> tracks)
        {
            trackResult.Clear();
            AffTrack.ItemsSource = trackResult;
            for (int i = 0; i != tracks.Count && i != Int32.Parse(NbResult.Text); i++)
            {
                trackResult.Add(tracks[i]);
            }
        }

        // Select Track
        private void TrackGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            tracksC.getList()[(int)((Grid)sender).Tag].SelectTrack(ref selectedTracks);
        }

        // Save Track
        private void DownloadButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            for (int i = 0; i != selectedTracks.Count; i++)
            {
                tracksC.getList()[selectedTracks[i]].Save();
                /*Track test = new Track();
                test.progressValue = 50;
                test.title = "gros gros gros !!!!";
                trackResult[0] = test;*/
                //trackResult.FirstOrDefault(iTrack => iTrack.id == 1).title = "coucou";
                //trackResult.Insert(1, tracksC.getList()[selectedTracks[0]]);
            }
        }
        // End search

        // Begin other event

        private void SearchInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var empty = new System.Windows.Input.GestureEventArgs();
            if (e.Key.ToString().Equals("Enter"))
            {
                this.Focus();
                ButtonSearchGrid_Tap(ButtonSearchGrid, empty);
            }
        }

        private void SearchInput_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchInput.Select(0, SearchInput.Text.Length);
            SearchInput.Focus();
        }

        private void DownloadButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (selectedTracks.Count != 0)
                DownloadButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 180, 70, 12));
        }

        private void DownloadButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (selectedTracks.Count != 0)
                DownloadButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 202, 80, 16));
        }

        private void ResultGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Int32.Parse(NbResult.Text) >= 20)
                NbResult.Text = "5";
            else
                NbResult.Text = (Int32.Parse(NbResult.Text) + 5).ToString();
        }

        private static void IsFolderNameOk(object sender)
        {
            var textBox = sender as TextBox;
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("^[a-zA-Z0-9]*$");
            for (int i = 0; i != textBox.Text.Length; i++)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(textBox.Text.ToCharArray()[0].ToString(), "^[a-zA-Z]"))
                {
                }
            }
        }

        // TODO finir nom du dossier
        private void FolderName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            
            /*
            if (!System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, "^[a-zA-Z]"))
            {
                textBox.SelectionStart = 3;
                textBox.SelectionLength = 3;
                textBox.Focus();
            }*/
        }

        private void FolderName_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            //textBox.Text = textBox.Text.Replace('1', ' ');
            Regex regex = new Regex(@"[^0-9a-zA-Z *]");
            MatchCollection matches = regex.Matches(textBox.Text);
            if (matches.Count > 0)
            {
                textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
                textBox.SelectionStart = textBox.Text.Length;
                textBox.Focus();
                MessageBox.Show("Un nom de dossier peut contenir uniquement des lettres et des chiffres");
                //tell the user
            }
        }

        private void FolderName_GotFocus(object sender, RoutedEventArgs e)
        {
            FolderName.Select(0, SearchInput.Text.Length);
            FolderName.Focus();
        }

        // End other event
    }
}