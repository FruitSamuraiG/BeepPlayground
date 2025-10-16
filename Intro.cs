using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestCSHARP
{
    public class Intro
    {
        public static async Task PlayBeepFromHTTP()
        {

            Console.WriteLine("Введите название песни");
            string songName = Console.ReadLine() ?? "Mario Main Theme";

            string apiKey = "yourapikey";
            string apiEndpoint = "https://router.huggingface.co/v1/chat/completions"; //Я использовал Hugging Face API

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

                var requestData = new //json зависит от API так что думайте
                {
                    messages = new[]
                    {
                        new
                        {
                        role = "user",
                        content = $"Make list that containts frequency of the sound and duration of the sound to make a music out of it, name of song is {songName}" +
                        "\nFor example:" +
                        "\n330 300"
                        }
                    },
                    model = "deepseek-ai/DeepSeek-V3.2-Exp:novita"
                };

                string jsonRequest = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    // Чтение ответа
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic responseJson = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    if (responseJson.TryGetProperty("choices", out JsonElement choicesArray) && choicesArray.GetArrayLength() > 0)
                    {
                        var firstChoice = choicesArray[0].GetProperty("message");
                        string generatedText = firstChoice.GetProperty("content").GetString();

                        // Текст ответа на запрос
                        Console.WriteLine("Сгенерированный текст от Hugging Face:");
                        Console.WriteLine(generatedText);

                        // Парсинг результата на нахождение нужных данных 
                        MatchCollection beepPairs = Regex.Matches(generatedText, @"(\d{2,5}(\s|,|,\s)\d{2,5})");

                        if (beepPairs.Count > 0)
                        {

                            int amountOfBeeps = beepPairs.Count;
                            int[] frequencyOfBeeps = new int[amountOfBeeps];
                            int[] durationOfBeeps = new int[amountOfBeeps];


                            string[] listOfBeeps = null;
                            for (int i = 0; i < amountOfBeeps; i++)
                            {
                                listOfBeeps = beepPairs[i].ToString().Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                                int.TryParse(listOfBeeps[0], out frequencyOfBeeps[i]);
                                int.TryParse(listOfBeeps[1], out durationOfBeeps[i]);
                            }

                            // БИПЫ!!!
                            for (int i = 0; i < amountOfBeeps; i++)
                            {
                                Console.Beep(frequencyOfBeeps[i], durationOfBeeps[i]);
                            }

                            Console.WriteLine("Сохранить песню в базу данных?\n1. Да\n2. Нет");
                            string usersInput = "";
                            while (true)
                            {
                                usersInput = Console.ReadLine();
                                int.TryParse(usersInput, out int usersChoice);
                                if (usersChoice == (int)MenuOptions.YES)
                                {
                                    string beepsData = "";
                                    for (int i = 0; i < beepPairs.Count; i++)
                                    {
                                        beepsData += beepPairs[i].ToString();
                                        beepsData += '\n';
                                    }
                                    string connString = "Host=localhost;Port=11111;Username=FruitSamuraiG;Password=azaza1234;Database=beepbase";
                                    DatabaseCommunication.SaveIntoDatabase(connString, songName, beepsData);
                                    break;
                                }
                                else
                                {
                                    if (usersChoice == (int)MenuOptions.NO) break;
                                    else
                                    {
                                        Console.WriteLine("Неверная комманда, попробуйте ещё раз: ");
                                        continue;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Не удалось извлечь данные из ответа на запрос!");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Ответ на запрос не содержит нужных данных!");
                    }
                }
                else
                {
                    Console.WriteLine($"Ошибка: {response.StatusCode}");
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
        }
        public static async Task PlayIntroTCP()
        {
            string result = "";
            IPAddress serverIP;
            IPAddress.TryParse("127.0.0.1", out serverIP);
            int defaultPort = 32555;
            IPEndPoint serverAdress = new IPEndPoint(serverIP, defaultPort);

            //Клиент-серверный движ.
            CancellationTokenSource cts = new CancellationTokenSource();
            Task<string> serverTask = TCPServerAndClient.Server(cts);
            await Task.Delay(2000);
            Task clientTask = TCPServerAndClient.Client("BeepMap.txt", serverAdress,cts);
            await Task.WhenAll(serverTask, clientTask);
            result = serverTask.Result;
            cts.Dispose();

            //Разбираем полученные данные
            int numberOfBeeps = result.Count(x => x == '\n');
            numberOfBeeps++;
            int[] durationOfBeeps = new int[numberOfBeeps];
            int[] frequencyOfBeeps = new int[numberOfBeeps];
            int stringIterator = 0;
            for (int i = 0; i < numberOfBeeps; i++)
            {
                string line = "";
                char buffer = 'a';
                if (stringIterator == result.Length) break;
                while (buffer != '\0' && buffer != '\n')
                {
                    if (stringIterator == result.Length) break;
                    buffer = result[stringIterator];
                    stringIterator++;
                    line += buffer;
                }
                string[] splittedLine = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                int.TryParse(splittedLine[0], out durationOfBeeps[i]);
                int.TryParse(splittedLine[1], out frequencyOfBeeps[i]);
            }
            for (int i = 0; i < numberOfBeeps; i++)
            {
                await Task.Run(() => Console.Beep(frequencyOfBeeps[i], durationOfBeeps[i]));

            }
        }
        private static Task PlayBeep(int frequency, int duration)
        {
            return Task.Run( () => Console.Beep(frequency, duration));
        }

        public static async Task <int> PlayIntro()
        {
            string[] fileNames = Directory.GetFiles("PixelMap\\").Order().ToArray();
            int numberOfImages = fileNames.Length;
            string[] images = new string[numberOfImages];
            for (int i = 0; i < numberOfImages; i++)
            {
                using (var streamReader = new StreamReader(fileNames[i]))
                {
                    images[i] = streamReader.ReadToEnd();
                }
            }

            string pathToBeepMap = "BeepMap.txt";
            int lineCounter = 0;
            int[] durationBetweenBeeps = new int[lineCounter];
            int[] frequencyBeeps = new int[lineCounter];
            try
            {
                using (var stream = new FileStream(pathToBeepMap, FileMode.Open, FileAccess.Read))
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    while (reader.ReadLine() != null) lineCounter++;
                    durationBetweenBeeps = new int[lineCounter];
                    frequencyBeeps = new int[lineCounter];
                    stream.Seek(0, SeekOrigin.Begin);
                    string line = "";
                    string[] splittedLine = new string[2];
                    for (int i = 0; i < lineCounter; i++)
                    {
                        line = reader.ReadLine();
                        splittedLine = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        int.TryParse(splittedLine[0], out durationBetweenBeeps[i]);
                        int.TryParse(splittedLine[1], out frequencyBeeps[i]);
                    }
                }
                int currentImage = 0;
                for (int i = 0; i < lineCounter; i++)
                {
                    Task sound = PlayBeep(frequencyBeeps[i], durationBetweenBeeps[i]);

                    if (i % 2 == 0 && numberOfImages!=0)
                    {
                        Console.Clear();
                        Console.WriteLine(images[currentImage]);
                        if (currentImage < numberOfImages - 2) currentImage++;
                        else currentImage = numberOfImages - 1;
                    }

                    await Task.WhenAll(sound);

                }
                Console.Clear();
                return 0;
            } catch (Exception) {
                Console.WriteLine("Не удалось открыть файл с картой музыкального интро, NO MUSIC FOR YOU!");
                return -1;
            }
 
        }


    }
}
