using ParserLibrary.Entities;
using ParserLibrary;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using ParserLibrary.Parsers.Core;

namespace AgentApplication
{
    /// <summary>
    /// Класс, описывающий агента, который выполняет задачи от сервера
    /// </summary>
    public class Agent
    {
        private readonly static HttpClient httpClient = GeneralData.HttpClient;

        private bool isStarted;
        private string serverUrl;
        private Thread threadWork;
        IParserFactory parserFactory;

        public Agent(string serverUrl, IParserFactory parserFactory) 
        {
            if (string.IsNullOrEmpty(serverUrl))
                throw new ArgumentNullException(nameof(serverUrl));

            if (parserFactory is null)
                throw new ArgumentNullException(nameof(parserFactory));

            this.serverUrl = serverUrl;
            this.parserFactory = parserFactory;
        }
        ~Agent() 
        {
            Stop();
        }

        /// <summary>
        /// Выполнить задачи от сервера
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        private List<TaskResult> ExecuteTasks(IEnumerable<Task> tasks) 
        {
            List<TaskResult> result = new List<TaskResult>();
            foreach (Task task in tasks) 
            {
                IParserNewsPage parserNewsPage = parserFactory.GetParserNewsPage(task.IdResource);
                News news = parserNewsPage?.Parse(task.URL);
                if (news != null) 
                {
                    TaskResult taskResult = new TaskResult() { Task = task, News = news };
                    result.Add(taskResult);
                }
            }
            return result;
        }

        /// <summary>
        /// Выполнить работу агента
        /// </summary>
        private async void Work() 
        {
            while (true)
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(serverUrl);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string jsonTasks = await response.Content.ReadAsStringAsync();
                        List<Task> tasks = JsonSerializer.Deserialize<List<Task>>(jsonTasks);

                        List<TaskResult> result = ExecuteTasks(tasks);
                        if (result.Count > 0)
                        {
                            string jsonResult = JsonSerializer.Serialize(result);
                            var content = new StringContent(jsonResult, Encoding.UTF8, "application/json");
                            await httpClient.PostAsync(serverUrl, content);
                        }
                    }
                }
                finally 
                {
                    httpClient.CancelPendingRequests();
                }
                    
            }
        }

        /// <summary>
        /// Запустить агента
        /// </summary>
        public void Start() 
        {
            if (!isStarted) 
            {
                isStarted = true;
                threadWork = new Thread(Work);
                threadWork.Start();
            }
        }

        /// <summary>
        /// Остановить агента
        /// </summary>
        public void Stop() 
        {
            if (isStarted) 
            {
                threadWork?.Abort();
                httpClient.Dispose();
                isStarted = false;
            }
        }
    }
}
