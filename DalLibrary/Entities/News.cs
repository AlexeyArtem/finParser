using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParserLibrary.Entities
{
    /// <summary>
    /// Класс, описывающий новость
    /// </summary>
    [Table("news")]
    [Serializable]
    public class News : Entity
    {
        /// <summary>
        /// Идентификатор ресурса новости
        /// </summary>
        [Column("IdResource")]
        public int IdResource { get; set; }

        /// <summary>
        /// URL новости
        /// </summary>
        [Column("URL")]
        public string URL { get; set; }

        /// <summary>
        /// Дата публикации новости
        /// </summary>
        [Column("Date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Заголовок новости
        /// </summary>
        [Column("Title")]
        public string Title { get; set; }

        /// <summary>
        /// Содержание новости
        /// </summary>
        [Column("Content")]
        public string Content { get; set; }
    }
}
