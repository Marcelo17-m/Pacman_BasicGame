using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models.Services
{
    public class ScoreService
    {
        private static string _path = GetDirectoryPath("C:\\Users\\PC\\source\\repos\\AvaloniaApplication1\\AvaloniaApplication1\\Assets\\Scores");

        private static string _filePath = Path.Combine(_path, "scores.json");

        private static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true // Esto es para que se guarde en formato todo junto y luego pueda leerlo normal
        };

        private static string GetDirectoryPath(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);

            if (!dir.Exists)
            {
                //throw new Exception("Folder not found");
                Console.WriteLine("Folder not found");
                return "";

            }

            return dir.FullName;
        }

        public static void SaveScore(List<Score> scores)
        {
            var json = JsonSerializer.Serialize(scores, _serializerOptions);
            File.WriteAllText(_filePath, json);
        }

        public static List<Score>? LoadScores()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Score>();
            }
            
            var json = File.ReadAllText(_filePath);

            try
            {
                return JsonSerializer.Deserialize<List<Score>>(json);
            }
            catch
            {
                return new List<Score>();
            }
            // si la lista esta vacia no detecta el json y da error
        }

    }
}

