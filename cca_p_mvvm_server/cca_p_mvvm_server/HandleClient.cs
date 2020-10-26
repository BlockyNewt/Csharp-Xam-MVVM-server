using System;
using System.Collections.Generic;
using System.IO;
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
        Database database;
        int clientID;

        public void startClient(TcpClient inClientSocket)
        {
            database = new Database("test");
            database.ConnectToDatabase();

            this.clientSocket = inClientSocket;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }
        private void doChat()
        {
            byte[] bytesFrom = new byte[10025];
            string dataFromClient = null;

            while ((true))
            {
                try
                {
                    NetworkStream networkStream = clientSocket.GetStream();

                    //CHECKS TO SEE IF THE CLIENT DISCONNECTS
                    if (networkStream.Read(bytesFrom, 0, bytesFrom.Length) != 0)
                    {
                        dataFromClient = System.Text.Encoding.UTF8.GetString(bytesFrom);
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                        //Console.WriteLine(dataFromClient);

                        ServerCommands(dataFromClient, networkStream);
                    }
                    else
                    {
                        Console.WriteLine(" >> Closing all connections.");

                        this.database.ChangeLoggedValue(this.clientID, 0);

                        this.database.CloseDatabaseConnection();

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

        private void ServerCommands(string dataFromClient, NetworkStream networkStream)
        {
            if (dataFromClient.Contains("USERNAME;"))
            {
                string[] words = dataFromClient.Split(';');
                string getPassword = database.SearchUser(words[1]);

                Byte[] msg = System.Text.Encoding.UTF8.GetBytes(getPassword);

                Console.WriteLine(" >> Username: " + words[1]);
                Console.WriteLine(" >> Password: " + getPassword + "\n");

                networkStream.Write(msg, 0, msg.Length);
                networkStream.Flush();
            }
            else if (dataFromClient.Contains("PASSWORD;"))
            {
                string[] words = dataFromClient.Split(';');
                string getUserInfo = database.GetUserInfo(words[1]);

                Byte[] msg = System.Text.Encoding.UTF8.GetBytes(getUserInfo);
                networkStream.Write(msg, 0, msg.Length);
                networkStream.Flush();
            }
            else if (dataFromClient.Contains("IS_LOGGED;"))
            {
                string[] words = dataFromClient.Split(';');
                string getUserId = words[1];

                int idConvert = Convert.ToInt32(getUserId);

                this.clientID = idConvert;

                string getLogged = this.database.CheckIfUserIsLogged(idConvert);

                Byte[] msg = System.Text.Encoding.UTF8.GetBytes(getLogged);

                networkStream.Write(msg, 0, msg.Length);
                networkStream.Flush();
            }
            else if (dataFromClient.Contains("CHANGE_LOGGED_VALUE;"))
            {
                string[] words = dataFromClient.Split(';');
                string getUserID = words[1];
                string getNewLoggedValue = words[2];

                int converUserId = Convert.ToInt32(getUserID);
                int convertLoggedValue = Convert.ToInt32(getNewLoggedValue);

                this.database.ChangeLoggedValue(converUserId, convertLoggedValue);
            }
            else if (dataFromClient.Contains("USER_ID;"))
            {
                string[] words = dataFromClient.Split(';');
                string userID = words[1];

                this.clientID = Convert.ToInt32(userID);
            }
            else if (dataFromClient.Contains("CREATE_ACCOUNT;"))    
            {
                string[] words = dataFromClient.Split(';');

                database.CreateAccount(words[1], words[2], words[3], words[4], words[5], words[6], words[7]);

                Console.WriteLine(" >> Created a new account.");
            }
            else if (dataFromClient.Contains("USERNAME_CHECK;"))
            {
                string[] words = dataFromClient.Split(';');

                Byte[] msg = System.Text.Encoding.UTF8.GetBytes(database.CheckIfUsernameIsTaken(words[1]));

                networkStream.Write(msg, 0, msg.Length);
                networkStream.Flush();
            }
            else if (dataFromClient.Contains("EMAIL_CHECK;"))
            {
                string[] words = dataFromClient.Split(';');

                Byte[] msg = System.Text.Encoding.UTF8.GetBytes(database.CheckIfEmailIsTaken(words[1]));

                networkStream.Write(msg, 0, msg.Length);
                networkStream.Flush();
            }
            else if (dataFromClient.Contains("CHANNELS;"))
            {
                //GET ALL CHANNEL NAMES
                string allChannels = database.GetChannels();

                Byte[] msg = System.Text.Encoding.UTF8.GetBytes(allChannels);

                //SEND TO CLIENT 
                networkStream.Write(msg, 0, msg.Length);
                networkStream.Flush();
            }
            else if (dataFromClient.Contains("USERS;"))
            {
                //GET ALL USERS ID, NAME, LASTNAME, AND PICTURE
                string allUsers = database.GetAllUsers();

                Byte[] msg = System.Text.Encoding.UTF8.GetBytes(allUsers);

                networkStream.Write(msg, 0, msg.Length);
                networkStream.Flush();
            }
            else if (dataFromClient.Contains("CHATS;"))
            {
                string[] words = dataFromClient.Split(';');

                //GET ALL USERS ID, NAME, LASTNAME, AND PICTURE
                string allUsers = database.GetAllChats(words[1]);

                Console.WriteLine(" >> ALL USERS:" + allUsers + "\n");

                Byte[] msg = System.Text.Encoding.UTF8.GetBytes(allUsers);

                networkStream.Write(msg, 0, msg.Length);
                networkStream.Flush();
            }
            else if (dataFromClient.Contains("ADD_CHAT;"))
            {
                string[] words = dataFromClient.Split(';');

                this.database.AddNewChat(words);

                Console.WriteLine(" >> Added new chat.");
            }
            else if (dataFromClient.Contains("REMOVE_CHAT;"))
            {
                string[] words = dataFromClient.Split(';');

                this.database.RemoveChat(words[1]);

                Console.WriteLine(" >> Removed chat.");
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

                networkStream.Flush();
            }
            else if (dataFromClient.Contains("GET_CHANNEL_MESSAGES;"))
            {
                string[] channelID = dataFromClient.Split(';');

                string s = database.GetAllChannelMessages(channelID[1]);

                Console.WriteLine(" >> " + s + "\n");

                Byte[] msg = System.Text.Encoding.UTF8.GetBytes(database.GetAllChannelMessages(channelID[1]));

                networkStream.Write(msg, 0, msg.Length);
                networkStream.Flush();
            }
            else if (dataFromClient.Contains("GET_DIRECT_MESSAGES;"))
            {
                string[] receiverID = dataFromClient.Split(';');

                string s = database.GetAllDirectMessages(Convert.ToInt32(receiverID[1]), Convert.ToInt32(receiverID[2]));

                Console.WriteLine(" >> " + s + "\n");

                Byte[] msg = System.Text.Encoding.UTF8.GetBytes(database.GetAllDirectMessages(Convert.ToInt32(receiverID[1]), Convert.ToInt32(receiverID[2])));

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

                Console.WriteLine(" >> " + dataFromClient);
            }
            else
            {
                Byte[] msg = System.Text.Encoding.UTF8.GetBytes(dataFromClient);

                networkStream.Write(msg, 0, msg.Length);
                networkStream.Flush();
            }
        }
    }
}
