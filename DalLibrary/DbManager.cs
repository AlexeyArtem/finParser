using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using ParserLibrary.Entities;
using MySql.Data.MySqlClient;
using ParserLibrary;

namespace ParserLibrary
{
    /// <summary>
    /// Менеджер для взаимодействия с базой данных
    /// </summary>
    public class DbManager
    {
        private readonly IDbConnection connection;

        private readonly ProcedureManager procedureManager;
        public ProcedureManager ProcedureManager { get => procedureManager; }

        public DbManager(IDbConnection connection) 
        {
            if (connection is null)
                throw new ArgumentNullException(nameof(connection));

            this.connection = connection;
            procedureManager = new ProcedureManager(connection);
        }
        ~DbManager() 
        {
            connection.Close();
        }

        /// <summary>
        /// Получить сущности из таблицы БД по типу
        /// </summary>
        /// <returns></returns>
        public List<T> GetEntities<T>() where T: Entity, new()
        {
            try
            {
                List<T> entities = new List<T>();
                Type typeItem = typeof(T);

                if (typeItem.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() is TableAttribute tableAttr)
                {
                    string sql = $"SELECT * FROM {tableAttr.Name}";
                    IDbCommand sqlCommand = connection.CreateCommand();
                    sqlCommand.CommandText = sql;

                    DataTable dt = new DataTable();
                    connection.Open();
                    using (IDataReader rdr = sqlCommand.ExecuteReader())
                    {
                        dt.Load(rdr);
                    }

                    var columnAttrs = new Dictionary<PropertyInfo, ColumnAttribute>();
                    foreach (var prop in typeItem.GetProperties())
                    {
                        var attr = prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault() as ColumnAttribute;
                        if (attr != null)
                            columnAttrs.Add(prop, attr);
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        T item = new T();
                        foreach (var prop in columnAttrs.Keys)
                        {
                            var value = row[columnAttrs[prop].Name];
                            prop.SetValue(item, value);
                        }
                        entities.Add(item);
                    }
                }

                return entities;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Добавить сущность в базу данных
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        //public bool Add<T>(T entity) where T : Entity
        //{
        //    bool isAdded = false;
        //    try
        //    {
        //        Type typeEntity = typeof(T);
        //        if (typeEntity.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() is TableAttribute tableAttr)
        //        {
        //            // Подготовка аргументов для Sql-команды
        //            var arguments = new Dictionary<string, object>();
        //            foreach (var prop in typeEntity.GetProperties())
        //            {
        //                var attr = prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault() as ColumnAttribute;
        //                if (attr != null)
        //                    arguments.Add(attr.Name, prop.GetValue(entity));
        //            }

        //            // Создание Sql-команды
        //            StringBuilder sqlColumns = new StringBuilder($"INSERT {tableAttr.Name}(");
        //            StringBuilder sqlValues = new StringBuilder("output INSERTED.ID VALUES (");
        //            foreach (var arg in arguments)
        //            {
        //                sqlColumns.Append($"{arg.Key},");
        //                sqlValues.Append($"@{arg.Key},");
        //            }
        //            sqlColumns.Remove(sqlColumns.Length - 1, 1);
        //            sqlColumns.Append(")");
        //            sqlValues.Remove(sqlValues.Length - 1, 1);
        //            sqlValues.Append(")");
        //            string sql = sqlColumns.Append(" " + sqlValues.ToString()).ToString();

        //            // Выполнение Sql-команды
        //            connection.Open();
        //            IDbCommand sqlCommand = connection.CreateCommand();
        //            sqlCommand.CommandText = sql.ToString();
        //            foreach (var arg in arguments) 
        //            {
        //                IDbDataParameter parameter = sqlCommand.CreateParameter();
        //                parameter.ParameterName = arg.Key;
        //                parameter.Value = arg.Value;
        //            }
                    
        //            object result = sqlCommand.ExecuteScalar();
        //            entity.ID = (int)result;

        //            isAdded = true;
        //        }
        //        return isAdded;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }
        //}

        /// <summary>
        /// Обновить сущность в базе данных
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        //public void Update<T>(T entity) where T: Entity
        //{
        //    Type typeEntity = typeof(T);
        //    try
        //    {
        //        if (typeEntity.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() is TableAttribute tableAttr)
        //        {
        //            // Подготовка аргументов для Sql-команды
        //            var arguments = new Dictionary<string, object>();
        //            //var columnAttrs = new Dictionary<PropertyInfo, ColumnAttribute>();
        //            foreach (var prop in typeEntity.GetProperties())
        //            {
        //                var attr = prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault() as ColumnAttribute;
        //                if (attr != null)
        //                    arguments.Add(attr.Name, prop.GetValue(entity));
        //                //columnAttrs.Add(prop, attr);
        //            }

        //            // Создание Sql-команды
        //            StringBuilder sql = new StringBuilder($"UPDATE {tableAttr.Name} SET ");
        //            foreach (var arg in arguments)
        //            {
        //                sql.Append($"{arg.Key} = {arg.Value},");
        //            }
        //            sql.Remove(sql.Length - 1, 1);
        //            //sql.Append($" WHERE ID = {}");

        //            // Выполнение Sql-команды
        //            connection.Open();
        //            MySqlCommand sqlCommand = new MySqlCommand(sql.ToString(), dbConnection);
        //            //sqlCommand.Ex();

        //            DataTable dt = new DataTable();

        //            using (MySqlDataReader reader = sqlCommand.ExecuteReader())
        //            {
        //                // Получить ID новой записи в БД
        //                // entity.ID // Присвоение ID
        //                dt.Load(reader);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }
        //}
    }
}
