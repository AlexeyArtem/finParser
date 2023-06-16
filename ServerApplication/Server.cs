using ParserLibrary;
using ParserLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ServerApplication
{
    /// <summary>
    /// Сервер, к которому обращаются агенты для получения задачи или отправки результата задачи
    /// </summary>
    public class Server
    {
        /// <summary>
        /// URL, который прослушивает сервер
        /// </summary>
        private readonly string url;

        private HttpListener listener;
        private Planner planner;
        private DbManager dbManager;
        private FileManager fileManager;

        public Server(string serverUrl, IDbConnection dbConnection) 
        {
            url = serverUrl;
            listener = new HttpListener();
            listener.Prefixes.Add(url);

            dbManager = new DbManager(dbConnection);
            planner = new Planner(dbManager);
            fileManager = new FileManager();
        }
        ~Server() 
        {
            Stop();
        }

        /// <summary>
        /// Возвращает значение, показывающее, был ли запущен сервер
        /// </summary>
        public bool IsStarted { get; private set; } = false;

        /// <summary>
        /// Вызов события отправки задач агенту
        /// </summary>
        /// <param name="tasks"></param>
        private void OnTasksSent(List<Task> tasks) => TasksSent?.Invoke(this, new TasksEventArgs(tasks));

        /// <summary>
        /// Вызов события получения результатов выполнения задач от агента
        /// </summary>
        /// <param name="tasks"></param>
        private void OnTasksResultsReceived(List<Task> tasks) => TasksResultsReceived?.Invoke(this, new TasksEventArgs(tasks));

        /// <summary>
        /// Происходит после отправки задач агенту
        /// </summary>
        public event TasksEventHandler TasksSent;

        /// <summary>
        /// Происходит после получения результатов выполнения задач от агента
        /// </summary>
        public event TasksEventHandler TasksResultsReceived;

        /// <summary>
        /// Запустить сервер
        /// </summary>
        public void Start(bool isStartFormingTasks = true) 
        {
            if (IsStarted) return;

            TasksSent += planner.ProcessSendingTasks;
            TasksResultsReceived += planner.ProcessReceiptTaskResults;
            listener.Start();
            planner.Start(isStartFormingTasks);
            IsStarted = true;

            while (true) 
            {
                var context = listener.GetContext();
                string httpMethod = context.Request.HttpMethod;

                if (httpMethod == "GET") // Запрос агента на получение задач 
                {
                    // Получение и подготовка задач для отправки
                    var tasks = planner.GetTasksForAgent();
                    if (tasks.Count > 0) 
                    {
                        string jsonTasks = JsonSerializer.Serialize(tasks);
                        byte[] data = Encoding.UTF8.GetBytes(jsonTasks);

                        // Подготовка заголовков ответа
                        var response = context.Response;
                        response.ContentType = "application/json";
                        response.ContentEncoding = Encoding.UTF8;
                        response.ContentLength64 = data.LongLength;

                        // Отправка результата
                        response.OutputStream.Write(data, 0, data.Length);
                        response.Close();
                        OnTasksSent(tasks);
                    }
                }
                else if (httpMethod == "POST") // Запрос агента на отправку результатов выполнения задач
                {
                    var request = context.Request;
                    string jsonResults;
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        jsonResults = reader.ReadToEnd();
                    }

                    var results = JsonSerializer.Deserialize<List<TaskResult>>(jsonResults);
                    var tasks = new List<Task>();
                    var news = new List<News>();
                    foreach (var result in results) 
                    {
                        tasks.Add(result.Task);
                        news.Add(result.News);
                        //dbManager.Add(result.News); // Сохранение результата в БД
                        dbManager.ProcedureManager.AddNewsProcedure(result.News.IdResource, result.News.Date, result.News.Title, result.News.Content);
                    }
                    fileManager.SaveInJson(news); // Сохранение результата в файл Json

                    OnTasksResultsReceived(tasks);
                }
            }
        }

        /// <summary>
        /// Остановить сервер
        /// </summary>
        public void Stop() 
        {
            if(listener.IsListening)
                listener.Stop();

            planner.Stop();
            TasksResultsReceived = null;
            TasksSent = null;
            IsStarted = false;
        }
    }
}
