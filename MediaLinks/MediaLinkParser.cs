using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Reflection;
using System.Resources;
using System.Globalization;

namespace MediaLinks
{
    public class MediaLinkParser
    {
        private List<string> _words;
        private string _imdbid;
        private string _url;

        private CultureInfo _cultureInfo;
        private ResourceManager _resourceManager;

        public MediaLinkParser()
        {
            _cultureInfo = CultureInfo.CreateSpecificCulture("de");
            _resourceManager = new ResourceManager("MediaLinks.Resources", Assembly.Load("MediaLinks"));
        }

        public List<string> Words
        {
            get { return _words; }
        }

        public string ImdbId
        {
            get { return _imdbid; }
        }

        public string Url
        {
            get { return _url; }
        }

        public void Parse(Key key, string fileName)
        {
            _words = new List<string>();
            string extension = null;

            string[] musicExtensions = {"mp3", "mpc", "flac", "ogg", "m4a"};
            string[] movieExtensions = {"avi", "mkv", "mp4", "mpg", "mpeg", "ogm", "divx"};
            string[] imageExtensions = {"iso", "img", "bin", "cue", "nrg"};
            var allExtensions = musicExtensions.Concat(movieExtensions).Concat(imageExtensions).ToArray();

            // remove strings within brackets
            var name = fileName;
            var matches = Regex.Matches(name, @"\(([^)]*)\)");

            foreach (Match match in matches)
            {
                name = name.Replace(match.Groups[0].Value, string.Empty);
            }

            // replace special characters
            name = name.Replace("&", "%26");

            // split name in parts
            var parts = new List<string>(name.Split(new[] {' ', '.', '_', '+' },
                StringSplitOptions.RemoveEmptyEntries));

            // remove extension
            var lastPart = parts.LastOrDefault();
            if (lastPart != null && allExtensions.Contains(lastPart.ToLower()))
            {
                extension = lastPart.ToLower();
                parts.Remove(lastPart);
            }
            
            // add parts to words if requirements fulfilled
            foreach (var part in parts)
            {
                // parse imdb id
                if (part.StartsWith("[tt") && part.EndsWith("]"))
                {
                    _imdbid = part.Substring(1, part.Length - 2);
                }
                else if (musicExtensions.Contains(extension))
                {
                    // if file is an audio file, remove cd or track number or dash
                    if (!(part.Length == 2 && int.TryParse(part, out int number)) && part != "-")
                        _words.Add(part);
                }
                else if (part == "-")
                {
                    //replace small dash with big dash if not an audio file
                    _words.Add("–");
                }
                else
                {
                    _words.Add(part);
                }
            }

            CreateUrl(key);
        }

        public void OpenUrl()
        {
            if (!string.IsNullOrWhiteSpace(_url))
            {
                var processName = "firefox";
                var processRunning = Process.GetProcessesByName(processName).Length != 0;

                Process.Start(processName, _url);

                if (!processRunning)
                {
                    Thread.Sleep(3000);
                }
            }
        }

        private void CreateUrl(Key key)
        {
            if (_words.Any())
            {
                
                switch (key)
                {
                    case Key.GoogleSearch:
                        CreateUrl(GetString("GoogleSearchPrefix"), _words, "+");
                        break;
                    case Key.GoogleSearchEpisodes:
                        CreateUrl(GetString("GoogleSearchPrefix"), _words, "+", 
                            GetString("GoogleSearchEpisodesSuffix"));
                        break;
                    case Key.GoogleSearchFeelingLucky:
                        CreateUrl(GetString("GoogleSearchPrefix"), _words, "+", "&btnI");
                        break;
                    case Key.GoogleSearchEpisodesFeelingLucky:
                        CreateUrl(GetString("GoogleSearchPrefix"), _words, "+",
                            GetString("GoogleSearchEpisodesSuffix")+ "&btnI");
                        break;
                    case Key.Wikipedia:
                        CreateUrl(GetString("WikipediaPrefix"), _words, "_");
                        break;
                    case Key.WikipediaEpisodes:
                        CreateUrl(GetString("WikipediaPrefix"), _words, "_", 
                            GetString("WikipediaEpisodesSuffix"));
                        break;
                    case Key.WikipediaSearch:
                        CreateUrl(GetString("WikipediaSearchPrefix"), _words, "+");
                        break;
                    case Key.WikipediaEpisodesSearch:
                        CreateUrl(GetString("WikipediaSearchPrefix"), _words, "+", 
                            GetString("WikipediaEpisodesSearchSuffix"));
                        break;
                    case Key.Imdb:
                        CreateUrl(GetString("ImdbPrefix"), _imdbid, "/");
                        break;
                    case Key.ImdbSearch:
                        CreateUrl(GetString("ImdbSearchPrefix"), _words, "+");
                        break;
                    case Key.ThetvdbSearch:
                        CreateUrl(GetString("ThetvdbSearchPrefix"), _words, "+",
                            GetString("ThetvdbSearchSuffx"));
                        break;
                    case Key.Serienjunkies:
                        CreateUrl(GetString("SerienjunkiesPrefix"), _words, "-", "/");
                        break;
                    case Key.SerienjunkiesSearch:
                        CreateUrl(GetString("SerienjunkiesSearchPrefix"), _words, "+", "/");
                        break;
                    case Key.YoutubeSearch:
                        CreateUrl(GetString("YoutubeSearchPrefix"), _words, "+");
                        break;
                    case Key.YoutubeSearchTrailer:
                        CreateUrl(GetString("YoutubeSearchPrefix"), _words, "+", 
                            GetString("YoutubeSearchTrailerSuffix"));
                        break;
                    case Key.Fernsehserien:
                        CreateUrl(GetString("FernsehserienPrefix"), _words, "-");
                        break;
                    case Key.FernsehserienSearch:
                        CreateUrl(GetString("FernsehserienSearchPrefix"), _words, "%20");
                        break;
                    case Key.MetacriticSearch:
                        CreateUrl(GetString("MetacriticSearchPrefix"), _words, "+", 
                            GetString("MetacriticSearchSuffix"));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(GetString("UnkownEnumType") + ": " + key);
                }
            }
        }

        private void CreateUrl(string urlFirstPart, string name, string urlLastPart = "")
        {
            CreateUrl(urlFirstPart, new List<string> {name}, string.Empty, urlLastPart);
        }

        private void CreateUrl(string urlFirstPart, ICollection<string> words, string separator,
            string urlLastPart = "")
        {
            if (words.Any())
            {
                _url = urlFirstPart + string.Join(separator, words) + urlLastPart;
            }
        }

        private string GetString(string resourceName)
        {
            return _resourceManager.GetString(resourceName, _cultureInfo);
        }
    }
}