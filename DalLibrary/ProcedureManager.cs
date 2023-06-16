using MySql.Data.MySqlClient;
using ParserLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace ParserLibrary
{
    /// <summary>
    /// Менеджер для вызова хранимых процедур в БД
    /// </summary>
    public class ProcedureManager
    {
        private static readonly string nameOutIdParam = "out_id";
        private IDbConnection connection;

        public ProcedureManager(IDbConnection connection)
        {
            if (connection is null)
                throw new ArgumentNullException(nameof(connection));

            this.connection = connection;
        }
        ~ProcedureManager()
        {
            connection.Dispose();
        }

        private DataTable CallProcedure(string nameProcedure, params Tuple<string, object, bool>[] prms) 
        {
            DataTable table = new DataTable();
            try
            {
                IDbCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = nameProcedure;
                foreach (var param in prms)
                {
                    IDbDataParameter dbDataParameter = command.CreateParameter();
                    dbDataParameter.ParameterName = param.Item1;
                    dbDataParameter.Value = param.Item2;
                    if (param.Item3) dbDataParameter.Direction = ParameterDirection.Output;
                    command.Parameters.Add(dbDataParameter);
                }
                
                connection.Open();
                using IDataReader rdr = command.ExecuteReader();
                table.Load(rdr);
            }
            finally
            {
                connection.Close();
            }

            return table;
        }

        private Tuple<string, object, bool> CreateParam(string name, object value, bool isOutput = false) 
        {
            return new Tuple<string, object, bool>(name, value, isOutput);
        }

        /// <summary>
        /// Вызвать хранимую процедуру добавления новой задачи
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public int? AddNewTaskProcedure(string url, int idResource) 
        {
            int? idNewTask = null;
            CallProcedure
                ("add_task",
                CreateParam("url", url),
                CreateParam("id_resource", idResource),
                CreateParam(nameOutIdParam, idNewTask, true));

            return idNewTask;
        }

        /// <summary>
        /// Вызвать хранимую процедуру смены статусов задач
        /// </summary>
        /// <param name="tasksId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public void UpdateTaskStatusProcedure(int tasksId, TaskStatus status)
        {
            CallProcedure("update_task_status", 
                CreateParam("id_task", tasksId), 
                CreateParam("task_status", status));
        }

        /// <summary>
        /// Вызвать хранимую процедуру получения задач по статусу
        /// </summary>
        /// <param name="taskStatus"></param>
        /// <returns></returns>
        public DataTable GetTasksByStatusProcedure(TaskStatus taskStatus) 
        {
            DataTable result = CallProcedure("get_tasks_by_status", CreateParam("task_status", taskStatus));
            return result;
        }

        /// <summary>
        /// Вызвать хранимую процедуру добавления новости
        /// </summary>
        /// <param name="idResource"></param>
        /// <param name="date"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public int? AddNewsProcedure(int idResource, DateTime date, string title, string content) 
        {
            int? id = null;
            CallProcedure
                ("add_news",
                CreateParam("id_resource", idResource),
                CreateParam("date_news", date),
                CreateParam("title", title),
                CreateParam("content", content),
                CreateParam(nameOutIdParam, id, true));

            return id;
        }
    }
}
