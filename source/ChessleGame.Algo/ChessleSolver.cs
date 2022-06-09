using ChessleGame.Algo.Entities;
using System;
using System.Collections.Generic;

namespace ChessleGame.Algo
{
    public class ChessleSolver
    {
        private List<PgnVariant> _possibleAnswers;
        private Random _random;

        public ChessleSolver()
        {
            _random = new Random();
            _possibleAnswers = new List<PgnVariant>();
        }

        public void InitSolver(string databaseDir)
        {
            _random = new Random();
            _possibleAnswers = new List<PgnVariant>();

            var database = new PgnDatabase();
            database.CreateDatabase(databaseDir);

            _possibleAnswers = new List<PgnVariant>();

            foreach (var line in database.LinesAndInfo.Keys)
            {
                _possibleAnswers.Add(new PgnVariant(line, database.LinesAndInfo[line].TotalCount));
            }
        }

        public string[] GetSubmission()
        {
            return _possibleAnswers.Count == 0 ? null : _possibleAnswers[_random.Next(_possibleAnswers.Count)].Moves;
        }

        public void UpdateSolver(string[] submission, char[] bullsCows)
        {
            var newPossibleAnswers = new List<PgnVariant>();

            foreach (var variant in _possibleAnswers)
            {
                if (variant.CanBeSolutionFor(new PgnVariant { Moves = submission }, bullsCows))
                {
                    newPossibleAnswers.Add(variant);
                }
            }

            _possibleAnswers = newPossibleAnswers;
        }
    }
}
