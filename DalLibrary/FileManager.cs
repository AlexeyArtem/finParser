using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ParserLibrary
{
    /// <summary>
    /// Менеджер для сохранения данных в файл
    /// </summary>
    public class FileManager
    {
        private string path;

        public FileManager() 
        {
            Directory.CreateDirectory(GeneralData.ParserDataPath);
            path = GeneralData.ParserDataPath + "Data started with " + DateTime.Today.ToString("dd/MM/yyyy") + ".txt";
        }

        /// <summary>
        /// Сохранить объект в файл json
        /// </summary>
        /// <param name="value"></param>
        public void SaveInJson(object value)
        {
            string json = JsonConvert.SerializeObject(value);
            File.AppendAllText(path, "\n" + json);
        }
    }
}
