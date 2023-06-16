using ParserLibrary.Entities;
using ParserLibrary.Parsers.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParserLibrary.Parsers
{
    /// <summary>
    /// Класс для парсинга информации c сайта Mfd
    /// </summary>
    public class MfdParser : IParserMainPage, IParserNewsPage
    {
        private readonly static HttpClient httpClient = GeneralData.HttpClient;

        private bool isCompleteParsed = false;
        private Regex regexNewsPage;
        private Resource resource;
        private int startIdNews;

        public MfdParser(Resource resource)
        {   
            regexNewsPage = new Regex("selected_date=(\\d{1,2}\\.\\d{1,2}\\.\\d{2,4}).*?<h1.*?>(.*?)<.*?content\">(.*?)</div>", RegexOptions.Singleline);
            this.resource = resource;
            startIdNews = 1;
        }

        public bool IsCompleteParsed => isCompleteParsed;

        public News Parse(string url)
        {
            try
            {
                Task<HttpResponseMessage> response = httpClient.GetAsync(url);
                response.Wait();

                using (HttpContent content = response.Result.Content)
                {
                    Task<string> htmlTask = content.ReadAsStringAsync();
                    htmlTask.Wait();

                    string html = htmlTask.Result;
                    var matches = regexNewsPage.Matches(html);
                    if (matches.Count > 0)
                    {
                        Match math = matches[0];
                        if (math.Groups.Count > 2)
                        {
                            DateTime date = DateTime.Parse(math.Groups[1].Value);
                            string title = math.Groups[2].Value?.StripTags();
                            string text = math.Groups[3].Value?.StripTags();

                            News news = new News()
                            {
                                Date = date,
                                Title = title,
                                IdResource = resource.ID,
                                URL = url,
                                Content = text
                            };

                            return news;
                        }
                    }
                }
                return null;
            }
            finally 
            {
                httpClient.CancelPendingRequests();
            }
        }

        public List<string> ParseCurrentMainPage()
        {
            List<string> urls = new List<string>();

            int count = startIdNews + 100;

            for (int i = startIdNews; i < count; i++) 
            {
                urls.Add(resource.URL + "view/?id=" + i);
                startIdNews = i;
            }

            return urls;
        }

        public async void SetNextMainPage()
        {
            try
            {
                string url = resource.URL + "view/?id=" + startIdNews;
                HttpResponseMessage response = await httpClient.GetAsync(url);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    isCompleteParsed = false;
                else
                    startIdNews++;
            }
            finally 
            {
                httpClient.CancelPendingRequests();
            }

        }
    }
}
