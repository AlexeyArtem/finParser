using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace ParserLibrary
{
    public static class Extensions
    {
        /// <summary>
        /// Удалить html-теги из строки
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StripTags(this string str) 
        {
            return Regex.Replace(str, "<.*?>", string.Empty);
        }

        /// <summary>
        /// Создать сущность по типу
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T CreateEntity<T>(this DataRow row) where T : class, new()
        {
            try
            {
                Type type = typeof(T);
                T entity = new T();
                foreach (var prop in type.GetProperties())
                {
                    ColumnAttribute columnAttr = prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault() as ColumnAttribute;
                    if (columnAttr != null)
                    {
                        object value = row[columnAttr.Name];
                        prop.SetValue(entity, value);
                    }
                }
                return entity;
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }
    }
}
