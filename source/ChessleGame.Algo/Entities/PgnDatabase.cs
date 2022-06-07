using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ChessleGame.Algo.Entities
{
    public class PgnDatabase
    {
        private const char CsvSplitter = ';';

        public PgnDatabase()
        {
            LinesAndInfo = new Dictionary<string, PgnVariantInfo>();
        }

        public Dictionary<string, PgnVariantInfo> LinesAndInfo { get; set; }

        public void CreateDatabase(string databaseDir)
        {
            var fileInfo = new FileInfo(databaseDir);
            if (!fileInfo.Exists) return;

            switch (fileInfo.Extension)
            {
                case ".csv":
                    {
                        ReadFromCsv(databaseDir);
                        return;
                    }
                case ".pgn":
                    {
                        ReadFromPgn(databaseDir);
                        return;
                    }
                default: return;
            }
        }

        public void SaveToCsv(string filename)
        {
            using var sw = new StreamWriter(filename);
            foreach (var newLine in LinesAndInfo.Select(line => $"{line.Key}{CsvSplitter}{line.Value.TotalCount}{CsvSplitter}{line.Value.GameInfo}"))
            {
                sw.WriteLine(newLine);
            }
            sw.Flush();
        }

        public void ReadFromCsv(string filename)
        {
            LinesAndInfo = new Dictionary<string, PgnVariantInfo>();

            using var sr = new StreamReader(filename);
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine()?.Split(CsvSplitter);

                if (line is { Length: 3 })
                {
                    LinesAndInfo[line[0]] = new PgnVariantInfo(int.Parse(line[1]), line[2]);
                }
            }
        }

        public void ReadFromPgn(string filename)
        {
            LinesAndInfo = new Dictionary<string, PgnVariantInfo>();

            var pgnLines = File.ReadAllLines(filename);

            var whitePlayer = "NN";
            var blackPlayer = "NN";
            var eco = "?";
            var year = "?";

            foreach (var pgnLine in pgnLines)
            {
                if (pgnLine.StartsWith("[White "))
                {
                    whitePlayer = GetLastName(pgnLine);
                    continue;
                }

                if (pgnLine.StartsWith("[Black "))
                {
                    blackPlayer = GetLastName(pgnLine);
                    continue;
                }

                if (pgnLine.StartsWith("[ECO "))
                {
                    eco = GetEco(pgnLine);
                    continue;
                }

                if (pgnLine.StartsWith("[EventDate "))
                {
                    year = GetYear(pgnLine);
                    continue;
                }

                if (pgnLine.StartsWith("1."))
                {
                    var splittedPgnLine = pgnLine.Split();

                    var lineForDictionary = "";
                    var sixthMoveIndex = 0;

                    while (splittedPgnLine[sixthMoveIndex] != "6.")
                    {
                        lineForDictionary += splittedPgnLine[sixthMoveIndex];
                        lineForDictionary += " ";
                        sixthMoveIndex++;
                    }

                    if (LinesAndInfo.TryGetValue(lineForDictionary, out var val))
                    {
                        LinesAndInfo[lineForDictionary].TotalCount++;
                    }
                    else
                    {
                        var gameInfo = $"{whitePlayer} - {blackPlayer}, {year} ({eco})";
                        LinesAndInfo[lineForDictionary] = new PgnVariantInfo(1, gameInfo);

                        whitePlayer = "NN";
                        blackPlayer = "NN";
                        eco = "?";
                        year = "?";
                    }
                }
            }

            SaveToCsv("database.csv");
        }

        private string GetLastName(string fullString)
        {
            var info = fullString.Split("\"");

            if (info.Length > 2)
            {
                if (!info[1].Contains(',')) return info[1];

                var fullNameArray = info[1].Split(',');
                return fullNameArray[0];
            }

            return "NN";
        }

        private string GetEco(string fullString)
        {
            var info = fullString.Split("\"");

            if (info.Length > 2)
            {
                return info[1];
            }

            return "?";
        }

        private string GetYear(string fullString)
        {
            var info = fullString.Split("\"");

            if (info.Length > 2 && info[1] != string.Empty)
            {
                var date = info[1].Split('.');

                return date[0];
            }

            return "?";
        }
    }
}
