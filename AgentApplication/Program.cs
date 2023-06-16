using MySql.Data.MySqlClient;
using ParserLibrary;
using ParserLibrary.Entities;
using ParserLibrary.Parsers;
using System;
using System.Net;

namespace AgentApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Write("Enter the ip address of the server: ");
                string ip = Console.ReadLine(); // Добавить IPAddress.TryParse() для валидации введенного ip
                string serverUrl = GeneralData.ServerPrefixUrl.Replace("+", ip);

                Console.WriteLine("Agent is starting, wait...");

                MySqlConnection dbConnection = new MySqlConnection(GeneralData.ConnectionString);
                DbManager dbManager = new DbManager(dbConnection);
                ParserFactory parserFactory = new ParserFactory(dbManager.GetEntities<Resource>());
                Agent agent = new Agent(serverUrl, parserFactory);
                agent.Start();

                Console.WriteLine("Agent is work...");
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Agent is stop for exception:{ex.Message}");
            }

            Console.ReadLine();
        }
    }
}
