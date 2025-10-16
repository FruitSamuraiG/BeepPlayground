using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestCSHARP {


    enum MenuOptions
    {
        None = 0,
        YES,
        NO
    }

    class Test
    {
        static async Task Main()
        {

            Console.Title = "Всем привет я новенькей";
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("------------------------");
            Console.Clear();

            string usersInput = "";
            int usersChoice = 1;

            Console.WriteLine("Сыграть музыку по вашему запросу?\n1. Да\n2. Нет");
            while (true)
            {
                usersInput = Console.ReadLine();
                int.TryParse(usersInput, out usersChoice);
                if (usersChoice == (int)MenuOptions.YES)
                {
                    Task intro = Intro.PlayBeepFromHTTP();
                    await Task.WhenAll(intro);
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

            Console.WriteLine("Сыграть интро из сети?\n1. Да\n2. Нет");
            while (true)
            {
                usersInput = Console.ReadLine();
                int.TryParse(usersInput, out usersChoice);
                if (usersChoice == (int)MenuOptions.YES)
                {
                    Task intro = Intro.PlayIntroTCP();
                    await Task.WhenAll(intro);
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

            Console.WriteLine("Сыграть интро?\n1. Да\n2. Нет");
            while (true) {
                usersInput = Console.ReadLine();
                int.TryParse(usersInput, out usersChoice);
                if (usersChoice == (int)MenuOptions.YES)
                {
                    Task intro = Intro.PlayIntro();
                    await Task.WhenAll(intro);
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

            string connString = "Host=localhost;Port=11111;Username=12345678;Password=12345678;Database=beepbase"; // Данные для PostgreSQL-сервера
            string names = "";
            Console.WriteLine("Сыграть песню из базы данных?\n1. Да\n2. Нет");
            while (true)
            {
                usersInput = Console.ReadLine();
                int.TryParse(usersInput, out usersChoice);
                if (usersChoice == (int)MenuOptions.YES)
                {
                    Console.WriteLine("Названия песен из базы данных:");
                    DatabaseCommunication.GetSongsFromDatabase(connString, ref names);
                    Console.WriteLine(names);
                    Console.WriteLine("Введите название песни, которую нужно сыграть");
                    while (true) {
                        string songName = Console.ReadLine() ?? "Mario";
                        if (Regex.Matches(names, $@"{songName}").Count() > 0)
                        {
                            string beepsData = DatabaseCommunication.GetBeepsDataFromDatabase(connString, songName);
                            string[] beepsPairs = beepsData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                            int[] beepsFrequency = new int[beepsPairs.Length];
                            int[] beepsDuration = new int[beepsPairs.Length];
                            for (int i = 0; i < beepsPairs.Length; i++)
                            {
                                string[] splittedLine = beepsPairs[i].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                                int.TryParse(splittedLine[0], out beepsFrequency[i]);
                                int.TryParse(splittedLine[1], out beepsDuration[i]);
                            }
                            for (int i = 0; i < beepsPairs.Length; i++)
                            {
                                Console.Beep(beepsFrequency[i], beepsDuration[i]);
                            }
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Неверное имя песни, попробуйте ещё раз");
                            continue;
                        }
                    }
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
    }
}