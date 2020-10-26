﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace cca_p_mvvm_server
{
    public class Database
    {
        public Database(string databaseName)
        {
            this.conn_Str_ = "server=localhost;user=root;password=password;database=" + databaseName + ";port=3306";
        }

        private string conn_Str_;
        private MySqlConnection conn_;


        public void ConnectToDatabase()
        {
            this.conn_ = new MySqlConnection(this.conn_Str_);

            try
            {
                Console.WriteLine(" >> Connecting to MySQL...");

                this.conn_.Open();

                Console.Write(" >> Connected to Database.\n\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());
            }
        }

        public string SearchUser(string username)
        {
            string query = "Select Password_ from Users where Username_ = '" + username + "';";

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = sqlCommand.ExecuteReader();

                string passwordIs = string.Empty;

                while (rdr.Read())
                {
                    passwordIs = rdr[0].ToString();
                }

                rdr.Close();

                if(passwordIs == string.Empty)
                {
                    return "EMPTY";
                }
                else
                {
                    return passwordIs;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());

                return string.Empty;
            }
        }

        public void CreateAccount(string firstname, string lastname, string username, string password, string bio, string profilePicture, string logged)
        {
            string query = "Insert into Users(First_Name_, Last_Name_, Username_, Password_, Bio_, Picture_, Logged_) values( '" + firstname + "', '" + lastname + "', '" + username + "', '" + password + "', '" + bio + "', '" + profilePicture + "', " + Convert.ToInt32(logged) + ");";

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = sqlCommand.ExecuteReader();

                rdr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());
            }
        }

        public string CheckIfUsernameIsTaken(string username)
        {
            string query = "select Username_ from users where Username_ Like '%" + username + "%';";

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = sqlCommand.ExecuteReader();

                string value = null;

                while(rdr.Read())
                {
                    value += rdr[0].ToString() + ";";
                }

                rdr.Close();

                if(!string.IsNullOrEmpty(value))
                {
                    string[] valueSplit = value.Split(';');

                    for (int i = 0; i < valueSplit.Length; ++i)
                    {
                        if (valueSplit[i].ToLower() == username.ToLower())
                        {
                            value = "EMPTY";

                            break;
                        }
                    }

                    if (value == "EMPTY")
                    {
                        return value;
                    }
                    else
                    {
                        value = username;

                        return value;
                    }
                }
                else
                {
                    return "GOOD";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());

                return "EMPTY";
            }
        }

        public string CheckIfEmailIsTaken(string email)
        {
            string query = "Select Email_ from users where Email_ Like '%" + email + "%';";

            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = mySqlCommand.ExecuteReader();

                string emailValue = string.Empty;

                while(rdr.Read())
                {
                    emailValue += rdr[0].ToString() + ";";
                }

                rdr.Close();

                if(!string.IsNullOrEmpty(emailValue))
                {
                    string[] emailValueSplit = emailValue.Split(';');

                    for (int i = 0; i < emailValueSplit.Length; ++i)
                    {
                        if (emailValueSplit[i].ToLower() == email.ToLower())
                        {
                            emailValue = "EMPTY";

                            break;
                        }
                    }

                    if (emailValue == "EMPTY")
                    {
                        return "EMPTY";
                    }
                    else
                    {
                        emailValue = email;

                        return emailValue;
                    }
                }
                else
                {
                    return "GOOD";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());

                return "EMPTY";
            }
        }

        public string GetUserInfo(string password)
        {
            string query = "Select ID_, First_Name_, Last_Name_, Bio_, Picture_ from users where Password_ = '" + password + "';";

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = sqlCommand.ExecuteReader();
                string userInfo = string.Empty;

                while (rdr.Read())
                {
                    userInfo = rdr[0].ToString() + ";" + rdr[1].ToString() + ";" + rdr[2].ToString() + ";" + rdr[3].ToString() + ";" + rdr[4].ToString();
                }

                rdr.Close();

                return userInfo;
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());

                return "EMPTY";
            }
        }

        public string CheckIfUserIsLogged(int userID)
        {
            string query = "Select Logged_ from users where ID_ = " + userID + ";";

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = sqlCommand.ExecuteReader();

                string isLogged = string.Empty;

                while(rdr.Read())
                {
                    isLogged = rdr[0].ToString();
                }

                rdr.Close();

                Console.WriteLine(" >> USER STATUS -> " + isLogged + "\n");

                return isLogged;
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());

                return "EMPTY";
            }
        }

        public void ChangeLoggedValue(int userID,  int newValue)
        {
            string query = "Update users set Logged_ = " + newValue + " where ID_ = " + userID + ";";

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = sqlCommand.ExecuteReader();

                rdr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());
            }
        }

        public string GetChannels()
        {
            string query = "Select * from channels;";

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = sqlCommand.ExecuteReader();

                string allChannels = string.Empty;

                while (rdr.Read())
                {
                    allChannels += rdr[0] + "," + rdr[1] + ";";
                }

                rdr.Close();

                Console.WriteLine(" >> " + allChannels + "\n");

                return allChannels;
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());

                return string.Empty;
            }
        }

        public string GetAllUsers()
        {
            string query = "Select ID_, First_Name_, Last_Name_, Bio_, Picture_ from users;";

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = sqlCommand.ExecuteReader();

                string allUsers = string.Empty;

                while (rdr.Read())
                {
                    allUsers += rdr[0] + "," + rdr[1] + "," + rdr[2] + "," + rdr[3] + "," + rdr[4] + ";";
                }

                rdr.Close();

                Console.WriteLine(" >> " + allUsers + "\n");

                return allUsers;
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());

                return string.Empty;
            }
        }

        public string GetAllChats(string clientID)
        {
            string query = "Select Target_ID_, Target_First_Name_, Target_Last_Name_, Target_Bio_, Target_Picture_ from user_Chats where Client_ID_ = " + Convert.ToInt32(clientID) + ";";

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = sqlCommand.ExecuteReader();

                string allUsers = string.Empty;

                while (rdr.Read())
                {
                    allUsers += rdr[0] + "," + rdr[1] + "," + rdr[2] + "," + rdr[3] + "," + rdr[4] + ";";
                }

                rdr.Close();

                if(!string.IsNullOrEmpty(allUsers))
                {
                    Console.WriteLine(" >> " + allUsers + "\n");

                    return allUsers;
                }
                else
                {
                    return "EMPTY";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());

                return "EMPTY" ;
            }
        }


        public string GetAllChannelMessages(string channelID)
        {
            string query = "Select User_First_Name_, Message_ from Channel_Messages where Channel_ID_ = " + channelID + ";";

            try
            {
                MySqlCommand queryCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = queryCommand.ExecuteReader();

                string allMessages = string.Empty;

                while (rdr.Read())
                {
                    allMessages += rdr[0] + "," + rdr[1] + ";";
                }

                rdr.Close();

                if (allMessages == string.Empty)
                {
                    return "EMPTY";
                }
                else
                {
                    return allMessages;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());

                return string.Empty;
            }
        }

        public string GetAllDirectMessages(int senderID, int receiverID)
        {
            string query = "select Sender_Name_, Message_ from direct_messages where Sender_ID_ = " + senderID + " and Receiver_ID_ = " + receiverID + " or Sender_ID_ = " + receiverID + " and Receiver_ID_ = " + senderID + ";";

            try
            {
                MySqlCommand queryCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = queryCommand.ExecuteReader();

                string allDirectMessages = string.Empty;

                while (rdr.Read())
                {
                    allDirectMessages += rdr[0] + "," + rdr[1] + ";";
                }

                rdr.Close();

                if (string.IsNullOrEmpty(allDirectMessages))
                {
                    return "EMPTY";
                }
                else
                {
                    return allDirectMessages;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());
                return "EMPTY";
            }
        }

        public void AddNewChat(string[] info)
        {
            string query = "Insert into user_chats(client_id_, target_id_, target_first_name_, target_last_name_, target_bio_, target_picture_) values(" + Convert.ToInt32(info[1]) + ", " + Convert.ToInt32(info[2]) + ", '" + info[3] + "', '" + info[4] + "', '" + info[5] + "', '" + info[6] + "');";

            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = mySqlCommand.ExecuteReader();

                rdr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());
            }
        }

        public void RemoveChat(string targetID)
        {
            string query = "Delete from user_chats where Target_ID_ = " + Convert.ToInt32(targetID) + ";";

            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = mySqlCommand.ExecuteReader();

                rdr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());
            }
        }

        public void EditUser(string[] edit)
        {
            string query = "Update Users set First_Name_ = '" + edit[2] + "', Last_Name_ = '" + edit[3] + "', Bio_ = '" + edit[4] + "', Picture_ = '" + edit[5] + "' where ID_ = " + edit[1] + ";";

            try
            {
                MySqlCommand queryCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = queryCommand.ExecuteReader();

                rdr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());
            }
        }

        public void InsertChannelMessage(string[] messageInfo)
        {
            string query = "insert into channel_messages (Channel_ID_, User_First_Name_, Message_) values( " + messageInfo[1] + ", '" + messageInfo[2] + "', '" + messageInfo[3] + "');";

            try
            {
                MySqlCommand queryCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = queryCommand.ExecuteReader();

                rdr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());
            }
        }

        public void InsertDirectMessage(string[] messageInfo)
        {
            string query = "insert into direct_messages (Sender_ID_, Sender_Name_, Receiver_ID_, Message_) values( " + messageInfo[1] + ", '" + messageInfo[2] + "', " + messageInfo[3] + ", '" + messageInfo[4] + "');";

            try
            {
                MySqlCommand queryCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = queryCommand.ExecuteReader();

                rdr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(" >> " + e.ToString());
            }
        }

        public void CloseDatabaseConnection()
        {
            Console.WriteLine(" >> Closing database connection \n");

            this.conn_.Close();
        }

        public bool ServerRestartOrBoot()
        {
            string query = "update users set Logged_ = " + 0 + " where Logged_ = " + 1 + ";";

            try
            {
                MySqlCommand mySqlCommand = new MySqlCommand(query, this.conn_);
                MySqlDataReader rdr = mySqlCommand.ExecuteReader();

                rdr.Close();

                return true;
            }
            catch (Exception e)
            {
                Console.Write(" >> " + e.ToString());

                return false;
            }
        }
    }
}
