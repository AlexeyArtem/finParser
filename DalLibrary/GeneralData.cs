using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ParserLibrary
{
    public static class GeneralData
    {
        /// <summary>
        /// Префикс, который прослушивает сервер
        /// </summary>
        public static readonly string ServerPrefixUrl = "http://+:8999/finParserServer/";

        /// <summary>
        /// Строка для подключения к БД
        /// </summary>
        public static readonly string ConnectionString = "server=localhost;port=3306;user=root;database=fin_parser;password=root";

        /// <summary>
        /// Путь к директории, в который хранятся результаты парсинга
        /// </summary>
        public static readonly string ParserDataPath = @"C:\FinParserData\";

        /// <summary>
        /// Клиент для отправки запросов и получения ответов
        /// </summary>
        public static readonly HttpClient HttpClient = new HttpClient();
    }
}
