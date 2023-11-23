using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Xml;
using System.Xml.Linq;

namespace Raycaster.Levels
{
    internal class LevelUnpacker
    {
        const string FOLDERNAME = "Assets/Levels";
        const string METADATAFILE = "metadata";
        const string METADATAFILEEXT = "levels";

        const string LEVELFILEEXT = "level";

        private string LevelFile(string FileName) => $"{FileName}.{LEVELFILEEXT}";

        private string MetaDataFile => $"{METADATAFILE}.{METADATAFILEEXT}";

        private string InFolder(string file) => $"{FOLDERNAME}\\{file}";

        private string[] GetLevelFiles()
        {
            Debug.WriteLine("Loading Levels...");
            XDocument xdoc = XDocument.Load(InFolder(MetaDataFile));
            return xdoc.Root.Elements("level")
                           .Select(element => element.Value)
                           .ToArray();
        }

        public Level[] GetLevels()
        {
            string[] levels = GetLevelFiles();

            Level[] levelObjects = new Level[levels.Length];

            for (int i = 0; i < levels.Length; i++)
            {
                string level = levels[i];

                string[] data = File.ReadAllLines(InFolder(LevelFile(level)));
                if (data.Length == 0) Debug.WriteLine($"No data found for {level}!");
                int[,] mapData = ToMapData(data);

                levelObjects[i] = new Level(level, mapData);
            }

            return levelObjects;
        }

        private int[,] ToMapData(string[] data)
        {
            int rowCount = data.Length;
            int colCount = data[0].Split(',').Length;
            int[,] result = new int[rowCount, colCount];

            for (int i = 0; i < rowCount; i++)
            {
                string[] values = data[i].Split(',');
                for (int j = 0; j < colCount; j++)
                {
                    if (int.TryParse(values[j], out int value))
                    {
                        result[i, j] = value;
                    }
                    else
                    {
                        Debug.WriteLine($"Error parsing value at row {i + 1}, column {j + 1}");
                        result[i, j] = 0;
                    }
                }
            }

            return result;
        }
    }
}
