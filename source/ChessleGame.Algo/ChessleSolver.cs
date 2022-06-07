using ChessleGame.Algo.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessleGame.Algo
{
    public static class ChessleSolver
    {
        public static void Solve(string databaseDir, bool firstRandomSubmission = false)
        {
            var database = new PgnDatabase();
            database.CreateDatabase(databaseDir);

            var variantsList = new List<PgnVariant>();

            foreach (var line in database.LinesAndCounts.Keys)
            {
                variantsList.Add(new PgnVariant(line, database.LinesAndCounts[line]));
            }

            var submissionNumber = 1;

            variantsList = variantsList.OrderBy(x => -x.UsingsCount).ToList();

            while (variantsList.Count > 1)
            {
                var currentSubmission = (submissionNumber == 1 && firstRandomSubmission)
                    ? variantsList[new Random().Next(variantsList.Count)]
                    : variantsList.First();

                var bullsAndCowsString = "";

                while (true)
                {
                    Console.WriteLine($"Submission #{submissionNumber}: {currentSubmission}");

                    while (bullsAndCowsString.Length < 10)
                    {
                        var line = Console.ReadLine();

                        if (line != null)
                        {
                            foreach (var c in line)
                            {
                                if (c == '0' || c == '1' || c == '2')
                                {
                                    bullsAndCowsString += c;
                                }
                            }
                        }
                    }

                    if (bullsAndCowsString.Length > 10)
                    {
                        bullsAndCowsString = "";
                        Console.WriteLine("Try again");
                    }
                    else
                    {
                        break;
                    }
                }

                var possibleSolutions = new List<PgnVariant>();
                var bullsAndCowsArray = bullsAndCowsString.ToCharArray();

                foreach (var variant in variantsList)
                {
                    if (variant.CanBeSolutionFor(currentSubmission, bullsAndCowsArray))
                    {
                        possibleSolutions.Add(variant);
                    }
                }

                variantsList = possibleSolutions.OrderBy(x => -x.UsingsCount).ToList();

                submissionNumber++;
            }

            if (variantsList.Count == 1)
            {
                Console.WriteLine($"Answer = {variantsList.First()}");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine($"There is no solution");
                Console.ReadLine();
            }
        }
    }
}
