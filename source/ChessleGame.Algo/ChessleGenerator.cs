using ChessleGame.Algo.Entities;
using System;
using System.Collections.Generic;

namespace ChessleGame.Algo
{
    public class ChessleGenerator
    {
        private readonly PgnDatabase _pgnDatabase;

        public ChessleGenerator(string databaseDir)
        {
            _pgnDatabase = new PgnDatabase();
            _pgnDatabase.CreateDatabase(databaseDir);
        }

        public PgnVariant GetRandomVariant()
        {
            var variantsList = new List<PgnVariant>();

            foreach (var line in _pgnDatabase.LinesAndInfo.Keys)
            {
                variantsList.Add(new PgnVariant(line, _pgnDatabase.LinesAndInfo[line].TotalCount, _pgnDatabase.LinesAndInfo[line].GameInfo));
            }

            var rand = new Random();

            return variantsList.Count > 0 ? variantsList[rand.Next(variantsList.Count)] : new PgnVariant();
        }
    }
}
