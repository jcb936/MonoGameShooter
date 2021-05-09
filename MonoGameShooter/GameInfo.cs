using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace THClone
{
    [Serializable]
    public struct LevelInfo
    {
        [XmlArray]
        public WaveInfo[] Waves;
    }

    [Serializable]
    public struct WaveInfo
    {
        [XmlElement]
        public int NumberOfEnemies;

        [XmlElement]
        public bool Left;
    }

    [Serializable]
    public struct Leaderboard
    {
        public LeaderboardEntry Entry;
    }

    [Serializable]
    public struct LeaderboardEntry
    {
        public string Name;
        public int Score;
    }


    class GameInfo
    {
        public static int CurrentScore
        {
            get => currentScore;
            set
            {
                currentScore = value;
                ScoreUpdated?.Invoke(currentScore);
            }
        }

        public static int Health
        {
            get => health;
            set
            {
                health = value;
                HealthUpdated?.Invoke(health);
            }
        }

        private static int health;

        private static int currentScore;

        public static event Action<int> ScoreUpdated;

        public static event Action<int> HealthUpdated;

        public static THGame GameInstance;

        private static Leaderboard leaderboard = new();

        private static LevelInfo currentLevelinfo;

        private static string PlayerName = "Player";

        public static void ExitGame()
        {
            GameInstance.Exit();
        }

        public static void CheckIfHighscore()
        {
            ReadXMLLeaderboards("Content/leaderboard.xml");
            if (currentScore > leaderboard.Entry.Score)
            {
                leaderboard.Entry.Name = PlayerName;
                leaderboard.Entry.Score = currentScore;
                WriteXMLLeaderboard("Content/leaderboard.xml");
            }
        }

        public static Leaderboard GetHighscores()
        {
            ReadXMLLeaderboards("Content/leaderboard.xml");
            return leaderboard;
        }

        public static LevelInfo GetLevelInfo()
        {
            ReadXMLLevelInfo("Content/levelinfo.xml");
            return currentLevelinfo;
        }

        public static void ReadXMLLeaderboards(string filename)
        {
            try
            {
                using StreamReader reader = new StreamReader(filename);
                var lb = (Leaderboard)new XmlSerializer(typeof(Leaderboard)).Deserialize(reader.BaseStream);
                leaderboard = lb;
            }
            catch (Exception e)
            {
                // If we've caught an exception, output an error message
                // describing the error
                Console.WriteLine("ERROR: XML File could not be deserialized!");
                Console.WriteLine("Exception Message: " + e.Message);
            }
        }

        public static void ReadXMLLevelInfo(string filename)
        {
            try
            {
                using StreamReader reader = new StreamReader(filename);
                var levelinfo = (LevelInfo)new XmlSerializer(typeof(LevelInfo)).Deserialize(reader.BaseStream);
                currentLevelinfo = levelinfo;
            }
            catch (Exception e)
            {
                // If we've caught an exception, output an error message
                // describing the error
                Console.WriteLine("ERROR: XML File could not be deserialized!");
                Console.WriteLine("Exception Message: " + e.Message);
            }
        }

        public static void WriteXMLLeaderboard(string filename)
        {
            try
            {
                FileStream stream = File.OpenWrite(filename);
                new XmlSerializer(typeof(Leaderboard)).Serialize(stream, leaderboard);
            }
            catch (Exception e)
            {
                // If we've caught an exception, output an error message
                // describing the error
                Console.WriteLine("ERROR: Struct could not be serialized!");
                Console.WriteLine("Exception Message: " + e.Message);
            }
        }
    }
}
