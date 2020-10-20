using System;
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

        public void CreateAccount(string firstname, string lastname, string username, string password, string bio, string profilePicture)
        {
            string query = "Insert into Users(First_Name_, Last_Name_, Username_, Password_, Bio_, Picture_) values( '" + firstname + "', '" + lastname + "', '" + username + "', '" + password + "', '" + bio + "', '" + profilePicture + "');";

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
                    //UTF8Encoding utf8 = new UTF8Encoding();
                    //byte[] utf8Byte = utf8.GetBytes(allMessages);

                    //string decode = utf8.GetString(utf8Byte);

                    //Console.WriteLine("DECODE: " + decode);
                    //allMessages = decode;
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
            this.conn_.Close();
        }
    }
}
