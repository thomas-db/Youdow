using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Youdow_ok
{
    class Tracks
    {
        public List<Track> list = new List<Track>();

        public Tracks()
        {

        }

        public int SetTrackList(string htmlCode)
        {
            try
            {
                ParsCode pars = new ParsCode();
                htmlCode = pars.replaceEntite(htmlCode);
                List<string> title_list;
                List<string> time_list;
                List<string> bigBloc = pars.getByAttName(htmlCode, "yt-lockup-video", "class");
                Track tracky = new Track();
                int posTrackList = 0;
                list.Clear();

                if (pars.getByAttName(htmlCode, "search-message", "class").Count != 0)
                    return 1;
                for (int i = 0; i != bigBloc.Count; i++)
                {
                    if (pars.getByAttName(bigBloc[i], "http", "data-url").Count == 0
                        && (title_list = pars.getByAttName(bigBloc[i], "spf-prefetch", "rel")).Count != 0
                        && (time_list = pars.getByAttName(bigBloc[i], "video-time", "class")).Count != 0)
                    {
                        Track track = new Track();
                        string name = pars.baliseValue(title_list[0]).Replace('\\', ' ').Replace('/', ' ').Replace(':', ' ').Replace('*', ' ').Replace('?', ' ').Replace('"', ' ').Replace('>', ' ').Replace('<', ' ').Replace('|', ' ');
                        setTitleAndArtist(name.Split(new string[] { " - " }, StringSplitOptions.None), track);
                        track.link = "https://www.youtube.com/watch?v=" + pars.attributValue(bigBloc[i], "href").Replace("/watch?v=", "");
                        track.image = "https://i.ytimg.com/vi/" + pars.attributValue(bigBloc[i], "href").Replace("/watch?v=", "") + "/default.jpg";
                        if (i % 2 == 0)
                            track.bg = "#FF141414";
                        else
                            track.bg = "#FF080808";
                        track.id = posTrackList;
                        track.progressValue = 0;
                        posTrackList++;
                        list.Add(track);
                        //MessageBox.Show(pars.baliseValue(time_list[0]).Trim(new Char[] { ' ', 'D', 'u', 'r', 'é', 'e', '-', '.' }));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur:\n" + ex.Message);
            }
            return 0;
        }

        private void setTitleAndArtist(string[] name, Track track)
        {
            if (name.Length > 1)
            {
                track.artist = name[0];
                for (int i = 1; i != name.Length; i++)
                {
                    track.title += name[i];
                }                
            }
            else
            {
                track.artist = "Artiste inconnu";
                track.title = name[0];
            }            
        }

        public List<Track> getList() { return list; }
    }
}
