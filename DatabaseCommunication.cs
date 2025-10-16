using Npgsql;
using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TestCSHARP
{

    public class DatabaseCommunication
    {
        public static void GetSongsFromDatabase(string connectionConfig, ref string container)
        {
            using (var conn = new NpgsqlConnection(connectionConfig))
            {
                bool isConnectedToDatabase = true;
                try
                {
                    conn.Open();
                }
                catch
                {
                    NpgsqlException ex = new NpgsqlException();
                    Console.WriteLine(ex.Message);
                    isConnectedToDatabase = false;
                }

                if (isConnectedToDatabase)
                {
                    using (var cmd = new NpgsqlCommand("SELECT songname FROM songs", conn))
                    {
                        using (var reader = cmd.ExecuteReader()) {
                            while (reader.Read())
                            {
                                container += reader.GetString(0);
                                container += '\n';
                            }
                        }
                    }
                }
            }


        }

        public static string GetBeepsDataFromDatabase(string connectionConfig, string songName)
        {
            string result = "";
            using (var conn = new NpgsqlConnection(connectionConfig))
            {
                bool isConnectedToDatabase = true;
                try
                {
                    conn.Open();
                }
                catch
                {
                    NpgsqlException ex = new NpgsqlException();
                    Console.WriteLine(ex.Message);
                    isConnectedToDatabase = false;
                }

                if (isConnectedToDatabase)
                {
                    using (var cmd = new NpgsqlCommand($"SELECT beepsdata FROM songs WHERE songname = '{songName}'", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result += reader.GetString(0);
                            }
                        }
                    }
                }
            }

            return result;

        }


        public static void SaveIntoDatabase(string connectionConfig, string songName, string beepsData)
        {

            using (var conn = new NpgsqlConnection(connectionConfig))
            {
                bool isConnectedToDatabase = true;
                try
                {
                    conn.Open();
                }
                catch
                {
                    NpgsqlException ex = new NpgsqlException();
                    Console.WriteLine(ex.Message);
                    isConnectedToDatabase = false;
                }

                if (isConnectedToDatabase)
                {
                    using (var cmd = new NpgsqlCommand("INSERT INTO songs (songname, beepsdata) VALUES (@value1, @value2)", conn))
                    {
                        cmd.Parameters.AddWithValue("@value1", songName);
                        cmd.Parameters.AddWithValue("@value2", beepsData);
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            Console.WriteLine("Успешное сохранение в базу.");
                        }
                        else Console.WriteLine("Ошибка сохранения в базу!");
                    }
                }
            }


        }


    }


}
