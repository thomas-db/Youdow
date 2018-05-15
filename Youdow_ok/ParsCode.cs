using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Youdow_ok
{
    class ParsCode
    {
        public ParsCode()
        {

        }

        //<balise>VALUE</balise>
        public string baliseValue(string str)
        {
            string balise;
            int i = 0;
            int a = 0;

            while (str[i] != '<' && i != 0)
                i--;
            a = 0;
            while (str[a] != ' ')
                a++;
            balise = str.Substring(i + 1, a - i - 1);
            a = str.IndexOf("</" + balise + ">");
            while (str[i] != '>')
                i++;
            if (a != -1)
                return (str.Substring(i + 1, a - i - 1));
            return (null);
        }

        //<balise attribut="VALUE"></balise>
        public string attributValue(string str, string balis)
        {
            int beg = 0;
            int end = 0;

            balis = balis + "=\"";
            if (str.IndexOf(balis) != -1)
            {
                beg = str.IndexOf(balis) + balis.Length;
                end = beg;
                while (str[end] != '"' && end != str.Length)
                {
                    end++;
                }
                string word = str.Substring(beg, end - beg);
                return (word);
            }
            return (null);
        }

        private int findGoodAttName(string content, string name, int i, string att)
        {
            int a = 0;

            while (a == 0 && i != -1)
            {
                i = content.IndexOf(name, i);
                if (i != -1)
                {
                    a = i;
                    while (content[a] != '"' && a > 0)
                    {
                        a--;
                    }
                    if (a - att.Length > 0)
                    {
                        if (content.Substring(a - (att.Length + 1), att.Length) == att)
                        {
                            //on a trouver le bon
                            a = 1;
                            return (i);
                        }
                    }
                    //on continu
                    a = 0;
                }
                if (i != -1)
                    i++;
            }
            return (-1);
        }

        private int closeBalise(string content, string balise)
        {
            if ((cb = content.IndexOf("</" + balise + ">", cb)) != -1)
            {
                cb++;
                return (1);
            }
            return (0);
        }

        private int openBalise(string content, string balise)
        {
            if ((ob = content.IndexOf("<" + balise + " ", ob)) != -1)
            {
                ob++;
                return (1);
            }
            return (0);
        }

        // Récupère balise par rapport a att | content: htmlcode -- name: nameof att -- att: id, class...
        public List<string> getByAttName(string content, string name, string att)
        {
            int begin = 0;
            int saveBegin = 0;
            int a = 0;
            int pos1 = 0;
            int pos2 = 0;
            string balise;
            List<string> tab = new List<string>();

            //position du mot rechercher <div id="name"
            while ((begin = findGoodAttName(content, name, begin, att)) != -1)
            {
                saveBegin = begin;
                while (content[begin] != '<' && begin != 0)
                    begin--;
                //on recupère le nom de la balise
                a = begin;
                while (content[a] != ' ' && content[a] != '\0')
                {
                    a++;
                }
                balise = content.Substring(begin + 1, a - begin - 1);
                cb = begin + 1;
                ob = begin + 1;
                while ((pos1 = openBalise(content, balise)) == 1 && (pos2 = closeBalise(content, balise)) == 1 && cb > ob)
                { }
                if (pos1 != 1 || pos2 != 1)
                    tab.Add(content.Substring(begin, content.Length - begin));
                else
                    tab.Add(content.Substring(begin, balise.Length + 2 + cb - begin));
                begin = saveBegin + 1;
            }
            //MessageBox.Show("Une erreur interne c'est produite, votre recherche n'a pas pu aboutir");
            return (tab);
        }

        // remplace les caractères spéciaux
        public string replaceEntite(string content)
        {
            content = content.Replace("&#39;", "'");
            content = content.Replace("&quot;", "\"");
            content = content.Replace("&laquo;", "«");
            content = content.Replace("&raquo;", "»");
            content = content.Replace("&lsaquo;", "<");
            content = content.Replace("&rsaquo;", ">");
            content = content.Replace("&ldquo;", "“");
            content = content.Replace("&rdquo;", "”");
            content = content.Replace("&bdquo;", "„");
            content = content.Replace("&apos;", "'");
            content = content.Replace("&lsquo;", "‘");
            content = content.Replace("&rsquo;", "’");
            content = content.Replace("&sbquo;", "‚");
            content = content.Replace("&hellip;", "…");
            content = content.Replace("&iexcl;", "¡");
            content = content.Replace("&iquest;", "¿");
            content = content.Replace("&uml;", "¨");
            content = content.Replace("&acute;", "´");
            content = content.Replace("&circ;", "ˆ");
            content = content.Replace("&tilde;", "˜");
            content = content.Replace("&cedil;", "¸");
            content = content.Replace("&middot;", "·");
            content = content.Replace("&bull;", "•");
            content = content.Replace("&macr;", "¯");
            content = content.Replace("&oline;", "‾");
            content = content.Replace("&ndash;", "–");
            content = content.Replace("&mdash;", "—");
            content = content.Replace("&brvbar;", "¦");
            content = content.Replace("&dagger;", "†");
            content = content.Replace("&Dagger;", "‡");
            content = content.Replace("&sect;", "§");
            content = content.Replace("&para;", "¶");
            content = content.Replace("&copy;", "©");
            content = content.Replace("&reg;", "®");
            content = content.Replace("&trade;", "™");
            content = content.Replace("&amp;", "&");
            content = content.Replace("&loz;", "◊");
            content = content.Replace("&spades;", "♠");
            content = content.Replace("&clubs;", "♣");
            content = content.Replace("&clubs;", "♥");
            content = content.Replace("&clubs;", "♦");
            content = content.Replace("&clubs;", "←");
            content = content.Replace("&clubs;", "↑");
            content = content.Replace("&clubs;", "→");
            content = content.Replace("&clubs;", "↓");
            content = content.Replace("&clubs;", "↔");
            return (content);
        }

        protected int ob = 0;
        protected int cb = 0;
        protected static bool con = true;
    }
}
