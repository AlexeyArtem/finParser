using ParserLibrary.Entities;
using System.Collections.Generic;
using ParserLibrary.Parsers.Core;

namespace ParserLibrary.Parsers
{
    /// <summary>
    /// Фабрика для получения объекта-парсера данных для нужного ресурса
    /// </summary>
    public class ParserFactory : IParserFactory
    {
        private Dictionary<int, IParserMainPage> parsersMainPage;
        private Dictionary<int, IParserNewsPage> parsersNewsPage;

        public ParserFactory(IEnumerable<Resource> resources) 
        {
            parsersMainPage = new Dictionary<int, IParserMainPage>();
            parsersNewsPage = new Dictionary<int, IParserNewsPage>();

            CreateParsers(resources);
        }

        private void CreateParsers(IEnumerable<Resource> resources) 
        {
            foreach (var resource in resources) 
            {
                switch (resource.ResourceId)
                {
                    case ResourceId.Mfd:
                        var mfd = new MfdParser(resource);
                        parsersMainPage.Add((int)resource.ResourceId, mfd);
                        parsersNewsPage.Add((int)resource.ResourceId, mfd);
                        break;
                }
            }
        }

        public IParserMainPage GetParserMainPage(int idResource)
        {
            if (parsersMainPage.TryGetValue(idResource, out IParserMainPage parser))
                return parser;

            return null;
        }

        public IParserNewsPage GetParserNewsPage(int idResource)
        {
            if (parsersNewsPage.TryGetValue(idResource, out IParserNewsPage parser))
                return parser;

            return null;
        }
    }
}
