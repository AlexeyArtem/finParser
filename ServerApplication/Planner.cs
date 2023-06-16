using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using ParserLibrary;
using ParserLibrary.Entities;
using ParserLibrary.Parsers;
using ParserLibrary.Parsers.Core;

namespace ServerApplication
{
    /// <summary>
    /// Планировщик задач для агентов
    /// </summary>
    public class Planner
    {
        private readonly object tasksLocker = new object();

        private bool isStarted;
        private List<Task> tasks;
        private List<Resource> resources;
        private DbManager dbManager;
        private ParserFactory parserFactory;
        /// <summary>
        /// Поток, в котором формируются новые задачи для парсинга
        /// </summary>
        private Thread threadFormingTask;

        public Planner(DbManager dbManager)
        {
            resources = new List<Resource>();
            tasks = new List<Task>();
            this.dbManager = dbManager;
        }
        ~Planner()
        {
            Stop();
        }

        /// <summary>
        /// Начать формирование новых задач
        /// </summary>
        private void StartFormingTasks()
        {
            System.Threading.Tasks.Parallel.ForEach(resources, resource =>
            {
                IParserMainPage parser = parserFactory.GetParserMainPage(resource.ID);
                while (!parser.IsCompleteParsed)
                {
                    List<string> urls = parser.ParseCurrentMainPage();
                    foreach (var url in urls)
                    {
                        lock (tasksLocker)
                        {
                            if (tasks.Find(t => t.URL == url) == null)
                            {
                                try
                                {
                                    int? taskId = dbManager.ProcedureManager.AddNewTaskProcedure(url, resource.ID);
                                    if (taskId != null)
                                    {
                                        Task task = new Task()
                                        {
                                            ID = taskId.Value,
                                            IdResource = resource.ID,
                                            Status = TaskStatus.New,
                                            URL = url
                                        };
                                        tasks.Add(task);
                                    }
                                }
                                catch(Exception ex) 
                                {
                                    if (!ex.Message.Contains("Duplicate"))
                                        throw ex;
                                }
                            }
                        }
                    }
                    parser.SetNextMainPage();
                }
            });
        }

        /// <summary>
        /// Изменить статусы задач
        /// </summary>
        private void СhangeTaskStatuses(List<Task> tasks, TaskStatus taskStatus) 
        {
            foreach (var task in tasks)
            {
                dbManager.ProcedureManager.UpdateTaskStatusProcedure(task.ID, taskStatus);
                task.Status = taskStatus;
            }
        }

        /// <summary>
        /// Загрузить данные из БД
        /// </summary>
        private void Load()
        {
            // Загрузка ресурсов
            var resources = dbManager.GetEntities<Resource>();
            this.resources.Clear();
            this.resources.AddRange(resources);

            // Загрузка задач
            var tasks = dbManager.ProcedureManager.GetTasksByStatusProcedure(TaskStatus.New).AsEnumerable().Select(r => r.CreateEntity<Task>());
            this.tasks.Clear();
            this.tasks.AddRange(tasks);
        }

        /// <summary>
        /// Запустить планировщик
        /// </summary>
        public void Start(bool isStartFormingTasks) 
        {
            try
            {
                if (isStarted) 
                    return;

                Load();
                parserFactory = new ParserFactory(resources);
                if (isStartFormingTasks)
                {
                    threadFormingTask = new Thread(StartFormingTasks);
                    threadFormingTask.Start();
                }

                isStarted = true;
            }
            catch (Exception ex) 
            {
                Stop();
                throw ex;
            }
        }

        /// <summary>
        /// Остановить планировщик задач
        /// </summary>
        public void Stop() 
        {
            threadFormingTask?.Abort();
            isStarted = false;
            ResetTasksInWork();
        }

        /// <summary>
        /// Выдать задачи для агента
        /// </summary>
        /// <returns></returns>
        public List<Task> GetTasksForAgent() 
        {
            //NOTE:
            // - Количество выдаваемых задача = количеству уникальных ресурсов

            var tasksForAgent = new List<Task>(resources.Count);
            foreach (var resource in resources)
            {
                var task = tasks.Find(t => t.IdResource == resource.ID && t.Status == TaskStatus.New);
                if (task != null)
                {
                    tasksForAgent.Add(task);
                    task.Status = TaskStatus.Sended; // Sended - сеансовый статус, который не заносится в БД
                }
            }

            return tasksForAgent;
        }

        /// <summary>
        /// Сбросить статусы задач "В обработке"
        /// </summary>
        public void ResetTasksInWork() 
        {
            lock (tasksLocker) 
            {
                var tasks = this.tasks.Where(t => t.Status == TaskStatus.InWork).ToList();
            }
            СhangeTaskStatuses(tasks, TaskStatus.New);
        }

        /// <summary>
        /// Обработчик события отправки задач агенту
        /// </summary>
        public void ProcessSendingTasks(object sencer, TasksEventArgs args) 
        {
            СhangeTaskStatuses(args.Tasks, TaskStatus.InWork);
        }

        /// <summary>
        /// Обработчик события получения задач от агента
        /// </summary>
        public void ProcessReceiptTaskResults(object sender, TasksEventArgs args) 
        {
            СhangeTaskStatuses(args.Tasks, TaskStatus.Completed);
            lock (tasksLocker)
            {
                foreach (Task task in args.Tasks) 
                {
                    Task foundTask = tasks.Find(t => t == task);
                    if (foundTask != null)
                        foundTask.Status = TaskStatus.Completed;
                    else
                        tasks.Add(task);
                }
            }
        }
    }
}
