using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using ParserLibrary;
using ParserLibrary.Entities;

namespace ServerApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MySqlConnection dbConnection = new MySqlConnection(GeneralData.ConnectionString);
                Server httpServer = new Server(GeneralData.ServerPrefixUrl, dbConnection);

                Console.WriteLine("Server is start.");
                httpServer.Start(false);
                Console.WriteLine("Server is stop.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server is stop for exception:{ex.Message}");
            }

            Console.Read();
        }
    }
}
