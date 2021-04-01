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

        private static Leaderboard leaderboard;

        private static string PlayerName = "Jacob";

        public static void ExitGame()
        {
            GameInstance.Exit();
        }

        public static void CheckIfHighscore()
        {
            ReadXML("D:/City/GameArch/MonoGameShooter/MonoGameShooter/Content/leaderboard.xml");
            if (currentScore > leaderboard.Entry.Score)
            {
                leaderboard.Entry.Name = PlayerName;
                leaderboard.Entry.Score = currentScore;
                WriteXML("D:/City/GameArch/MonoGameShooter/MonoGameShooter/Content/leaderboard.xml");
            }
        }

        public static Leaderboard GetHighscores()
        {
            ReadXML("D:/City/GameArch/MonoGameShooter/MonoGameShooter/Content/leaderboard.xml");
            return leaderboard;
        }

        public static void ReadXML(string filename)
        {
            try
            {
                using StreamReader reader = new StreamReader(filename);
                leaderboard = (Leaderboard)new XmlSerializer(typeof(Leaderboard)).Deserialize(reader.BaseStream);
            }
            catch (Exception e)
            {
                // If we've caught an exception, output an error message
                // describing the error
                Console.WriteLine("ERROR: XML File could not be deserialized!");
                Console.WriteLine("Exception Message: " + e.Message);
            }
        }

        public static void WriteXML(string filename)
        {
            try
            {
                using StreamReader reader = new StreamReader(filename);
                new XmlSerializer(typeof(Leaderboard)).Serialize(reader.BaseStream, leaderboard);
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
