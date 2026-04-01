using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Moorhase
{
    public class HighScoreManager
    {
        private static readonly string HighScorePath = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "Moorhase", "highscores.json");

        private List<HighScore> highScores;

        public HighScoreManager()
        {
            highScores = LoadHighScores();
        }

        public void AddHighScore(string playerName, int points, string mode)
        {
            highScores.Add(new HighScore 
            { 
                PlayerName = playerName, 
                Points = points, 
                Mode = mode,
                Date = DateTime.Now
            });
            
            SaveHighScores();
        }

        public void ClearHighScores()
        {
            highScores.Clear();
            SaveHighScores();
        }

        public List<HighScore> GetTopScores(string mode, int count = 10)
        {
            return highScores
                .Where(h => h.Mode == mode)
                .OrderByDescending(h => h.Points)
                .Take(count)
                .ToList();
        }

        private void SaveHighScores()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(HighScorePath) ?? "");
                var json = JsonSerializer.Serialize(highScores, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(HighScorePath, json);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Fehler beim Speichern der Highscores: {ex.Message}");
            }
        }

        private List<HighScore> LoadHighScores()
        {
            try
            {
                if (File.Exists(HighScorePath))
                {
                    var json = File.ReadAllText(HighScorePath);
                    return JsonSerializer.Deserialize<List<HighScore>>(json) ?? new List<HighScore>();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Fehler beim Laden der Highscores: {ex.Message}");
            }

            return new List<HighScore>();
        }
    }
}
