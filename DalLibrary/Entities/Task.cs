using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ParserLibrary.Entities
{
    /// <summary>
    /// Класс, описывающий задачу для агента
    /// </summary>
    [Table("tasks")]
    [Serializable]
    public class Task : Entity
    {
        /// <summary>
        /// URL новости для парсинга
        /// </summary>
        [Column("URL")]
        public string URL { get; set; }

        /// <summary>
        /// Текущий статус задачи
        /// </summary>
        [Column("IdStatus")]
        public TaskStatus Status { get; set; }

        /// <summary>
        /// Идентификатор ресурса новости
        /// </summary>
        [Column("IdResource")]
        public int IdResource { get; set; }
    }
}
