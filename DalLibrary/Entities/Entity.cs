using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ParserLibrary.Entities
{
    /// <summary>
    /// Базовый класс сущности
    /// </summary>
    public abstract class Entity
    {
        [Column("ID")]
        public virtual int ID { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Entity entity) 
                return entity.ID == ID;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ID;
        }
    }
}
