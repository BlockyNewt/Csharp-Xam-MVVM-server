using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cca_p_mvvm_server
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Any, 45000);
            TcpClient clientSocket = default(TcpClient);

            serverSocket.Start();
            Console.WriteLine(" >> " + "Server Started \n");

            Database database = new Database("test");

            database.ConnectToDatabase();

            if(database.ServerRestartOrBoot())
            {
                database.CloseDatabaseConnection();

                while (true)
                {
                    clientSocket = serverSocket.AcceptTcpClient();

                    HandleClient client = new HandleClient();
                    client.startClient(clientSocket);
                }

                clientSocket.Close();
                serverSocket.Stop();
                Console.WriteLine(" >> " + "exit");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine(" >> Problem with ServerRestartOrBoot() function.");

                Console.ReadLine();
            }
        }
    }
}   
