using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace RockPaperScissors
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var resultList = new List<ResultRPS>();
            var SubPath = @"%AppData%\rockpaperscissors";
            var FilePath = @"%AppData%\rockpaperscissors\rockpaperscissors.json";
            FilePath = Environment.ExpandEnvironmentVariables(FilePath);

            LoadJson(resultList, FilePath);

            bool KeepPlaying = true;
            while (KeepPlaying)
            {
                Console.WriteLine();
                Console.Write("Select an option: \n" +
                              "1: ROCK \n" +
                              "2: PAPER \n" +
                              "3: SCISSORS \n" +
                              "4: VIEW PAST RESULTS \n" +
                              "9: DELETE ALL DATA \n" +
                              "0: CLOSE GAME");
                Console.WriteLine();

                string line = Console.ReadLine();
                bool correctInput = int.TryParse(line, out int value);

                if (correctInput)
                {

                    var userInput = Convert.ToInt32(line);
                    var randomNumber = GenerateRandomNumber();

                    if (userInput > 0 && userInput < 4)
                    {
                        var results = new ResultRPS
                        {
                            PlayerSelection = Selection(userInput),
                            ComputerGenerated = Selection(randomNumber),
                            Winner = WhoWon(userInput, randomNumber)
                        };
                        var toStringResults = JsonConvert.SerializeObject(results);
                        resultList.Add(results);
                        Console.WriteLine(toStringResults);
                    }
                    switch (userInput)
                    {
                        case 4:
                            if (!resultList.Any())
                            {
                                Console.WriteLine("No results to display");
                            }
                            else
                            {
                                Console.WriteLine("THE PAST RESULTS ARE: ");
                                foreach (var resList in resultList)
                                {
                                    Console.WriteLine(JsonConvert.SerializeObject(resList));
                                }
                            }
                            break;

                        case 9:
                            Console.WriteLine("ALL PREVIOUS SCORES ARE BEING DELETED");
                            resultList.Clear();
                            Console.WriteLine("All History has been deleted");
                            break;

                        case 0:                            
                            Environment.Exit(1);
                            break;
                    }
                    WriteToFile(resultList, FilePath, SubPath);
                }
                else
                {
                    Console.WriteLine("please insert a correct value");
                }
            }
        }

        private static int GenerateRandomNumber()
        {
            var number = new Random();
            var randomNumber = number.Next(1, 4);
            return randomNumber;
        }

        private static string WhoWon(int userInput, int randomNumber)
        {
            if (userInput == randomNumber)
            {
                return "TIED";
            }

            if ((userInput == 1 && randomNumber == 2) || (userInput == 2 && randomNumber == 3) ||
                (userInput == 3 && randomNumber == 1))
            {
                return "COMPUTER WON";
            }
            return " PLAYER WON";
        }

        private static string Selection(int SelectedChoice)
        {
            return SelectedChoice switch
            {
                1 => "ROCK",
                2 => "PAPER",
                _ => "SCISSORS",
            };
        }

        private static void WriteToFile(List<ResultRPS> resultList, string FilePath, string SubPath)
        {
            //subpath @"%AppData%\rockpaperscissors"
            //filepath @"%AppData%\rockpaperscissors\rockpaperscissors.json"
            if (!File.Exists(FilePath))
            {
                if (!File.Exists(SubPath))
                {
                    System.IO.Directory.CreateDirectory(SubPath);
                }                
            }
            var json = JsonConvert.SerializeObject(resultList.ToArray(), Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        public static void LoadJson(List<ResultRPS> resultList, string FilePath)
        {
            //filepath @"%AppData%\rockpaperscissors\rockpaperscissors.json"
            if (File.Exists(FilePath))
            {
                try
                {
                    string jsonFromFile;
                    using (var reader = new StreamReader(FilePath))
                    {
                        jsonFromFile = reader.ReadToEnd();
                    }
                    var pastResultRps = JsonConvert.DeserializeObject<List<ResultRPS>>(jsonFromFile);
                    resultList.AddRange(pastResultRps.Select(pr => new ResultRPS
                    {
                        PlayerSelection = pr.PlayerSelection,
                        ComputerGenerated = pr.ComputerGenerated,
                        Winner = pr.Winner
                    }));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }

    internal class ResultRPS
    {
        public string PlayerSelection { get; set; }
        public string ComputerGenerated { get; set; }
        public string Winner { get; set; }
    }
}