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
    public class HandleClient
    {
        TcpClient clientSocket;
        string clNo;
        Database database;

        public void startClient(TcpClient inClientSocket, string clineNo)
        {
            database = new Database("test");
            database.ConnectToDatabase();

            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }
        private void doChat()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;
            Byte[] sendBytes = null;
            requestCount = 0;

            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();

                    //CHECKS TO SEE IF THE CLIENT DISCONNECTS
                    if (networkStream.Read(bytesFrom, 0, bytesFrom.Length) != 0)
                    {
                        dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                        ServerCommands(dataFromClient, networkStream, sendBytes);
                    }
                    else
                    {

                        Console.Write(" >> Closing all connections.");

                        networkStream.Close();
                        this.clientSocket.Dispose();
                        this.clientSocket.Close();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(" >> " + e.ToString());

                    dataFromClient = string.Empty;

                    this.clientSocket.Dispose();
                    this.clientSocket.Close();

                    break;
                }
            }

        }

        private void ServerCommands(string dataFromClient, NetworkStream networkStream, Byte[] sendBytes)
        {
            if (dataFromClient.Contains("USERNAME;"))
            {
                string[] words = dataFromClient.Split(';');
                string getPassword = database.SearchUser(words[1]);

                Byte[] msg = System.Text.Encoding.ASCII.GetBytes(getPassword);

                Console.WriteLine(" >> Username: " + words[1]);
                Console.WriteLine(" >> Password: " + getPassword + "\n");

                networkStream.Write(msg, 0, msg.Length);
                networkStream.Flush();
            }
            else if (dataFromClient.Contains("PASSWORD;"))
            {
                string[] words = dataFromClient.Split(';');
                string getUserInfo = database.GetUserInfo(words[1]);

                Byte[] msg = System.Text.Encoding.ASCII.GetBytes(getUserInfo);
                networkStream.Write(msg, 0, msg.Length);
                networkStream.Flush();
            }
            else if (dataFromClient.Contains("CREATE_ACCOUNT;"))
            {
                string[] words = dataFromClient.Split(';');

                database.CreateAccount(words[1], words[2], words[3], words[4], words[5]);

                Console.WriteLine(" >> Created a new account.");
            }
            else if (dataFromClient.Contains("CHANNELS;"))
            {
                //GET ALL CHANNEL NAMES
                string allChannels = database.GetChannels();
                Byte[] msg = System.Text.Encoding.ASCII.GetBytes(allChannels);

                //SEND TO CLIENT 
                networkStream.Write(msg, 0, msg.Length);
                networkStream.Flush();
            }
            else if (dataFromClient.Contains("USERS;"))
            {
                //GET ALL USERS ID, NAME, LASTNAME, AND PICTURE
                string allUsers = database.GetAllUsers();

                Byte[] msg = System.Text.Encoding.ASCII.GetBytes(allUsers);

                networkStream.Write(msg, 0, msg.Length);
                networkStream.Flush();
            }
            else if (dataFromClient.Contains("EDIT;"))
            {
                string[] edit = dataFromClient.Split(';');

                for (int a = 1; a < edit.Length; ++a)
                {
                    Console.Write(edit[a] + " ");
                }

                Console.WriteLine("\n");

                database.EditUser(edit);
            }
            else if (dataFromClient.Contains("CHANNEL_MESSAGE;"))
            {

                string[] channelMessage = dataFromClient.Split(';');

                database.InsertChannelMessage(channelMessage);
            }
            else if (dataFromClient.Contains("GET_CHANNEL_MESSAGES;"))
            {
                string[] channelID = dataFromClient.Split(';');

                string s = database.GetAllChannelMessages(channelID[1]);
                Console.WriteLine(" >> " + s);

                Byte[] msg = System.Text.Encoding.ASCII.GetBytes(database.GetAllChannelMessages(channelID[1]));

                networkStream.Write(msg, 0, msg.Length);
                networkStream.Flush();
            }
            else if (dataFromClient.Contains("GET_DIRECT_MESSAGES;"))
            {
                Console.WriteLine(" >> Direct " + dataFromClient + "\n");

                string[] receiverID = dataFromClient.Split(';');

                Console.WriteLine(" >> Sender ID: " + receiverID[1] + " Receiver ID: " + receiverID[2] + "\n");

                string s = database.GetAllDirectMessages(Convert.ToInt32(receiverID[1]), Convert.ToInt32(receiverID[2]));

                Console.WriteLine(" >> " + s);

                Byte[] msg = System.Text.Encoding.ASCII.GetBytes(database.GetAllDirectMessages(Convert.ToInt32(receiverID[1]), Convert.ToInt32(receiverID[2])));

                networkStream.Write(msg, 0, msg.Length);
                networkStream.Flush();
            }
            else if (dataFromClient.Contains("DIRECT_MESSAGE;"))
            {
                string[] directMessage = dataFromClient.Split(';');

                if (directMessage[4].Contains("'"))
                {
                    directMessage[4] = directMessage[4].Replace("'", "''");

                    Console.WriteLine(" >> UPDATED MESSAGE: " + directMessage[4]);
                }

                Console.WriteLine("\n");

                database.InsertDirectMessage(directMessage);
                networkStream.Flush();

                dataFromClient = string.Empty;

                Console.WriteLine(" >> Inserting new channel message: " + dataFromClient);
            }
            else
            {
                sendBytes = System.Text.Encoding.ASCII.GetBytes(dataFromClient);

                networkStream.Write(sendBytes, 0, sendBytes.Length);
                networkStream.Flush();
            }
        }
    }
}
