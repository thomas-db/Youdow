using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.ComponentModel;
using Windows.Storage.Pickers;

namespace Youdow_ok
{
    public class Track : INotifyPropertyChanged
    {

        // BEGIN Changer la propriété d'un itemControl : tracks_DownloadProgressChanged -> exemple utilisation
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        // END Changer la propriété d'un itemControl

        // TODO mettre temps sur les tracks à droite
        public string title { set; get; }
        public string artist { set; get; }
        public string image { set; get; }
        public string link { set; get; }
        public string bg { set; get; }
        public int id { set; get; }
        public int progressValue { set; get; }

        public Track()
        {

        }

        public void SelectTrack(ref List<int> selectedTracks)
        {
            int found = -1;

            for (int i = 0; i != selectedTracks.Count; i++)
            {
                if (selectedTracks[i] == id)
                    found = i;
            }
            if (found != -1)
            {
                selectedTracks.RemoveAt(found);
                if (id % 2 == 0)
                    bg = "#141414";
                else
                    bg = "#080808";
                OnPropertyChanged("bg");
            }
            else
            {
                selectedTracks.Add(id);
                if (id % 2 == 0)
                    bg = "#b43c10";
                else
                    bg = "#a52d10";
                OnPropertyChanged("bg");
            }
            ColorAndTitleDownloadButton();
        }
        
        public void Save()
        {
            try
            {
                WebClient codeSampleReq = new WebClient();
                codeSampleReq.DownloadStringCompleted += getHtmlCodeYoutubeMp3AndDownload;
                codeSampleReq.DownloadStringAsync(new Uri("http://youtubeinmp3.com/download/?url=" + link));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur on search:\n" + ex.Message);
            }
        }

        private void getHtmlCodeYoutubeMp3AndDownload(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.OptionFixNestedTags = true;
                htmlDoc.LoadHtml(e.Result);
                //Create a webclient that'll handle your download
                WebClient client = new WebClient();
                //Run function when resource-read (OpenRead) operation is completed
                client.OpenReadCompleted += client_OpenReadCompleted;
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(tracks_DownloadProgressChanged);
                //Start download / open stream
                client.OpenReadAsync(new Uri("http://youtubeinmp3.com" + new ParsCode().attributValue(new ParsCode().getByAttName(e.Result, "download", "id")[0], "href")));//Use the url to your file instead, this is just a test file (pdf)
                RemoveListAndChangeColorTrack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("En raison d'une durée musical trop longue \"" + title + "\" ne peut pas être téléchargé");
                //MessageBox.Show("Erreur:\n" + ex.Message);
            }
        }

        void RemoveListAndChangeColorTrack()
        {
            if (id % 2 == 0)
                bg = "#141414";
            else
                bg = "#080808";
            OnPropertyChanged("bg");
            MainPage.selectedTracks.Remove(id);
            ColorAndTitleDownloadButton();
        }

        void ColorAndTitleDownloadButton()
        {
            MainPage.main.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                if (MainPage.selectedTracks.Count > 1)
                    MainPage.main.NbTitle.Text = MainPage.selectedTracks.Count.ToString() + " titres";
                else
                    MainPage.main.NbTitle.Text = MainPage.selectedTracks.Count.ToString() + " titre";
                if (MainPage.selectedTracks.Count == 1)
                    MainPage.main.DownloadButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 202, 80, 16));
                else if (MainPage.selectedTracks.Count == 0)
                    MainPage.main.DownloadButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 64, 64, 64));
            }));
        }

        void tracks_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressValue = e.ProgressPercentage;
            OnPropertyChanged("progressValue");
            //trackResult[0] = test;
            /*
            MainPage.main.Dispatcher.BeginInvoke(new Action(delegate ()
            {
                MainPage.main.ProgressTrack.Value = e.ProgressPercentage;
            }));
            */
        }

        private async void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                // TODO changer l'emplacement de sauvergarde et nettoyer le code
                byte[] buffer = new byte[e.Result.Length];
                //Store bytes in buffer, so it can be saved later on
                await e.Result.ReadAsync(buffer, 0, buffer.Length);
                //Store to isolatedstorage
                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                
                    //Create file
                    using (IsolatedStorageFileStream stream = file.OpenFile(title + ".mp3", FileMode.Create))
                    {
                        //Write content stream to file
                        await stream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    // Access the file.
                    StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                    StorageFile storedFile = await local.GetFileAsync(title + ".mp3");

                Microsoft.Xna.Framework.Media.PhoneExtensions.SongMetadata metaData = new Microsoft.Xna.Framework.Media.PhoneExtensions.SongMetadata();
                // enregistre musique dans le dossier choisi
                MainPage.main.Dispatcher.BeginInvoke(new Action(delegate ()
                {
                    if (MainPage.main.FolderName.Text != "")
                        metaData.AlbumName = MainPage.main.FolderName.Text;
                    else
                        metaData.AlbumName = "YoudowSave";
                    metaData.ArtistName = "Youdow";
                    metaData.GenreName = "";
                    if (artist != "Artiste inconnu")
                        metaData.Name = artist + " - " + title;
                    else
                        metaData.Name = artist + " - " + title;
                    var ml = new MediaLibrary();
                    Uri songUri = new Uri(title + ".mp3", UriKind.RelativeOrAbsolute);
                    var song = Microsoft.Xna.Framework.Media.PhoneExtensions.MediaLibraryExtensions.SaveSong(ml, songUri, metaData, Microsoft.Xna.Framework.Media.PhoneExtensions.SaveSongOperation.MoveToLibrary);
                    MessageBox.Show("Téléchargement de \"" + title + "\" terminé !");
                    file.DeleteFile(title + ".mp3");
                }));                
            }
            catch (Exception ex)
            {
                MessageBox.Show("We could not finish your download. Error message: " + ex.Message);
            }
        }
    }
}
