

namespace ParserLibrary.Parsers.Core
{
    /// <summary>
    /// Фабрика создания и получения парсеров
    /// </summary>
    public interface IParserFactory
    {
        /// <summary>
        /// Получить парсер главной страницы ресурса по идентифкатору
        /// </summary>
        /// <param name="idResource"></param>
        /// <returns></returns>
        IParserMainPage GetParserMainPage(int idResource);

        /// <summary>
        /// Получить парсер новостной страницы ресурса
        /// </summary>
        /// <param name="idResource"></param>
        /// <returns></returns>
        IParserNewsPage GetParserNewsPage(int idResource);
    }
}
