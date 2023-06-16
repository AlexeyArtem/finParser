using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLibrary
{
    /// <summary>
    /// Статус задачи
    /// </summary>
    public enum TaskStatus
    {
        New = 1,
        InWork = 2,
        Completed = 3,
        Sended = -1 // Сеансовый статус, который не заносится в БД
    }

    /// <summary>
    /// Идентификатор ресурса
    /// </summary>
    public enum ResourceId 
    {
        None = 0,
        Mfd = 1
    }
}
