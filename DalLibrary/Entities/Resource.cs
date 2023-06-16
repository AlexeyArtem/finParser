using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ParserLibrary.Entities
{
    /// <summary>
    /// Класс, описывающий ресурс для парсинга
    /// </summary>
    [Table("resources")]
    public class Resource : Entity
    {
        [Column("ID")]
        public override int ID
        {
            get => base.ID;
            set
            {
                base.ID = value;
                if (Enum.IsDefined(typeof(ResourceId), value)) 
                {
                    ResourceId = (ResourceId)value;
                }
            }
        }

        public ResourceId ResourceId { get; private set; } = ResourceId.None;

        /// <summary>
        /// Название ресурса
        /// </summary>
        [Column("Name")]
        public string Name { get; set; }

        /// <summary>
        /// URL главной страницы ресурса с новостями
        /// </summary>
        [Column("URL")]
        public string URL { get; set; }
    }
}
