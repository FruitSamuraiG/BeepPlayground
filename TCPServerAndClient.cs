using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestCSHARP
{
    public class TCPServerAndClient
    {
        public static async Task<string> Server(CancellationTokenSource cts)
        {
            string result = "";
            TcpListener server = new TcpListener(IPAddress.Any, 32555); //Здесь нужно указывать адрес и порт сервера, сейчас локальный адрес и порт который я сам выбрал
            try
            {
                server.Start();
                Console.WriteLine("Сервер: Ожидание соединения...");
                TcpClient client = await server.AcceptTcpClientAsync(cts.Token);
                Console.WriteLine("Сервер: Подключение установлено.");


                Task.Run(() =>
                {
                    while (!cts.IsCancellationRequested)
                    {
                        Task.Delay(1000);
                    }
                    Console.WriteLine("Сервер: Соединение разорвано клиентом");
                }
                );

                byte[] buffer = new byte[256]; // Буфер для чтения данных
                int bytesRead;

                using (NetworkStream stream = client.GetStream())
                {
                    // Чтение данных от клиента
                    try
                    {
                        byte[] msg;
                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cts.Token)) != 0)
                        {

                            string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            result += receivedData;
                            Console.WriteLine($"Сервер: Получено {bytesRead} байт данных.");

                            msg = Encoding.UTF8.GetBytes("Все данные получены!");
                            await stream.WriteAsync(msg, 0, msg.Length);
                            Console.WriteLine("Сервер: Отправлено сообщение клиенту.");

                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка потока: {ex}");
                    }

                    await stream.FlushAsync();

                    if (result.Length > 0)
                    {
                        Console.WriteLine("Сервер: Получено сообщение от клиента:");
                        Console.WriteLine(result.ToString());
                        return result;
                    }
                    else
                    {
                        Console.WriteLine("Сервер: Данные от клиента не получены.");
                        return "";
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка на сервере: {ex.Message}");
                return "";
            }
            finally
            {
                cts.Dispose();
                server.Stop();
            }
        }

        public static async Task Client(string pathToFile, IPEndPoint serverAddress, CancellationTokenSource cts)
        {
            while (true)
            {
                try
                {
                    using (TcpClient client = new TcpClient())
                    {
                        await client.ConnectAsync(serverAddress);
                        Console.WriteLine("Клиент подключён!");

                        using (NetworkStream stream = client.GetStream())
                        {
                            stream.WriteTimeout = 5000;
                            stream.ReadTimeout = 5000;
                            // Чтение файла
                            string beepMap = await File.ReadAllTextAsync(pathToFile);
                            byte[] data = Encoding.UTF8.GetBytes(beepMap);

                            // Отправка данных на сервер
                            await stream.WriteAsync(data, 0, data.Length, cts.Token);
                            Console.WriteLine("Клиент: Данные отправлены серверу.");

                            // Чтение ответа от сервера
                            byte[] dataFromServer = new byte[256];
                            int bytesRead = await stream.ReadAsync(dataFromServer, 0, dataFromServer.Length, cts.Token);
                            string response = Encoding.UTF8.GetString(dataFromServer, 0, bytesRead);

                            Console.WriteLine($"Клиент: Получено от сервера: {response}");
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    cts.Dispose();
                    Console.WriteLine($"Ошибка на клиенте: {ex.Message}");
                    Console.WriteLine("Попытка переподключения через 5с");
                    Thread.Sleep(5000);
                    continue;
                }
            }
        }
    }
}
