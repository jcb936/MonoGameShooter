using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THClone
{
    class GameInfo
    {
        public static int CurrentScore
        {
            get => currentScore;
            set
            {
                currentScore = value;
                ScoreUpdated.Invoke(currentScore);
            }
        }

        public static int Health
        {
            get => health;
            set
            {
                health = value;
                HealthUpdated.Invoke(health);
            }
        }

        private static int health;


        private static int currentScore;

        public static event Action<int> ScoreUpdated;

        public static event Action<int> HealthUpdated;

        public static THGame GameInstance;

        public static void ExitGame()
        {
            GameInstance.Exit();
        }
    }
}
