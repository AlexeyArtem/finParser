using ParserLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParserLibrary
{
    /// <summary>
    /// Класс, описывающий результат выполнения задачи
    /// </summary>
    public class TaskResult
    {
        /// <summary>
        /// Задача, которая была выполнена
        /// </summary>
        public Task Task { get; set; }

        /// <summary>
        /// Результат выполненной задачи
        /// </summary>
        public News News { get; set; }
    }
}
