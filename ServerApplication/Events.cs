using ParserLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApplication
{
    /// <summary>
    /// Представляет метод, который обрабатывает события, связанные с задачами
    /// </summary>
    public delegate void TasksEventHandler(object sender, TasksEventArgs args);

    /// <summary>
    /// Предоставляет данные о событиях, связанныех с задачами
    /// </summary>
    public class TasksEventArgs : EventArgs
    {
        public TasksEventArgs(List<Task> tasks) : base()
        {
            Tasks = tasks;
        }

        public List<Task> Tasks { get; }
    }
}
