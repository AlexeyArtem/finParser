using System.Collections.Generic;

namespace ParserLibrary.Parsers.Core
{
    /// <summary>
    /// Парсер главной страницы ресурса
    /// </summary>
    public interface IParserMainPage
    {
        /// <summary>
        /// Сменить главную страницу ресурса
        /// </summary>
        void SetNextMainPage();

        /// <summary>
        /// Парсинг ссылок на новости с главной страницы ресурса
        /// </summary>
        /// <returns></returns>
        List<string> ParseCurrentMainPage();
        
        /// <summary>
        /// Завершен ли парсинг всех новостных ссылок с ресурса
        /// </summary>
        bool IsCompleteParsed { get; }
    }
}
