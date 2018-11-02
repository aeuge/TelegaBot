using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace TelegaBot
{
    class Program
    {
        static string GetName(string id)
        {
            string serverName = "81.24.136.1"; // Адрес сервера (для локальной базы пишите "localhost")
            string userName = "telegramUSER"; // Имя пользователя
            string dbName = "telegramDB"; //Имя базы данных
            string port = "3306"; // Порт для подключения
            string password = "password"; // Пароль для подключения
            string connStr = "server=" + serverName +
                ";user=" + userName +
                ";database=" + dbName +
                ";port=" + port +
                ";password=" + password + ";";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string sql = "SELECT Username FROM users where guid="+id+" LIMIT 1"; // Строка запроса
            //Console.WriteLine(sql);
            MySqlCommand sqlCom = new MySqlCommand(sql, conn);
            sqlCom.ExecuteNonQuery();
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCom);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);

            var myData = dt.Select();
            if (myData.Length == 1) {
                return myData[0].ItemArray[0].ToString();
            } else
            {
                return "";
            }
            //for (int i = 0; i < myData.Length; i++)
            //{
            //    for (int j = 0; j < myData[i].ItemArray.Length; j++)
            //        richTextBox1.Text += myData[i].ItemArray[j] + " ";
            //    richTextBox1.Text += "\n";
            //}
        }
        static void SetName(string id, string name)
        {
            string serverName = "81.24.136.166"; // Адрес сервера (для локальной базы пишите "localhost")
            string userName = "telegramUSER"; // Имя пользователя
            string dbName = "telegramDB"; //Имя базы данных
            string port = "3306"; // Порт для подключения
            string password = "DES100b10F"; // Пароль для подключения
            string connStr = "server=" + serverName +
                ";user=" + userName +
                ";database=" + dbName +
                ";port=" + port +
                ";password=" + password + ";";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string sql = "insert into users(guid,Username) values(" + id+",'"+name+"')"; // Строка запроса
//            string sql = "update users set guid=" + id + ", username='" + name + "'"; // Строка запроса
            Console.WriteLine(sql);
            MySqlCommand sqlCom = new MySqlCommand(sql, conn);
            sqlCom.ExecuteNonQuery();
        }

        static void UpdateName(string id, string name)
        {
            string serverName = "81.24.136.166"; // Адрес сервера (для локальной базы пишите "localhost")
            string userName = "telegramUSER"; // Имя пользователя
            string dbName = "telegramDB"; //Имя базы данных
            string port = "3306"; // Порт для подключения
            string password = "DES100b10F"; // Пароль для подключения
            string connStr = "server=" + serverName +
                ";user=" + userName +
                ";database=" + dbName +
                ";port=" + port +
                ";password=" + password + ";";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string sql = "update users set username='" + name + "' where guid=" + id; // Строка запроса
            Console.WriteLine(sql);
            MySqlCommand sqlCom = new MySqlCommand(sql, conn);
            sqlCom.ExecuteNonQuery();
        }

        static void Main(string[] args)
        {
            int update_id = 0;
            string token = "428376454:AAGWEhWRMXGSVOmnKYswTtnOknZe9FVEW8g";
            string start_url = $"https://api.telegram.org/bot{token}";
            string message_from_id = "";
            string message_from_name="";
            string message_text = "";
            string from = "";
            string input_text;
            #region TelegaBot
            WebClient WebCli = new WebClient();
            while (true) {
                string url = $"{start_url}/getUpdates?offset={update_id+1}";
                string get = WebCli.DownloadString(url);
                var array = JObject.Parse(get)["result"].ToArray();
                foreach (var msg in array)
                {
                    update_id = Convert.ToInt32(msg["update_id"]);
                    try
                    {
                        message_from_id = msg["message"]["from"]["id"].ToString();
                        //message_text = msg["message"]["text"].ToString();
                        message_from_name = GetName(message_from_id);
                        Console.WriteLine($"{message_from_name}");
                        input_text = msg["message"]["text"].ToString();
                        Console.WriteLine(input_text.ToLower().IndexOf("имя:").ToString());
                        int findstr = input_text.ToLower().IndexOf("имя:");
                        if ( findstr > -1)
                        {
                            string newname = input_text.Substring(findstr + 4, input_text.Length - findstr - 4);
                            newname = newname.Trim();
                            //Console.WriteLine(newname);
                            if (message_from_name.Length>0)
                            {
                                UpdateName(message_from_id, newname);
                            }
                            else
                            {
                                SetName(message_from_id, newname);
                            }
                            message_from_name = newname;
                        }

                        if (message_from_name.Length > 0) {
                            message_text = "Вас зовут: " + message_from_name+ " Вы в любой момент можете изменить имя набрав текст: имя: любое имя";
                        } else
                        {
                            from = msg["message"]["from"]["first_name"].ToString() + " " + msg["message"]["from"]["last_name"].ToString();
                            message_text = "Привет. Предположительно Вас зовут: " + from+". Вы в любой момент можете изменить имя набрав текст: имя:любое имя";
                        }
                        Console.WriteLine($"{update_id} {message_from_id} {message_text}");

                        url = $"{start_url}/sendMessage?chat_id={message_from_id}&text={message_text}";
                        WebCli.DownloadString(url);
                    }
                    catch
                    {
                        Console.WriteLine("ошибко");
                    }

                }

                Thread.Sleep(200);
            }
//            Console.WriteLine(get);
            Console.ReadKey();
            #endregion
        }
    }
}
