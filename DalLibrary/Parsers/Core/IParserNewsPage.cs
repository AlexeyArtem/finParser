using ParserLibrary.Entities;

namespace ParserLibrary.Parsers.Core
{
    /// <summary>
    /// Парсер страницы с новостью
    /// </summary>
    public interface IParserNewsPage
    {
        /// <summary>
        /// Спарсить новость по ссылке
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        News Parse(string url);
    }
}
