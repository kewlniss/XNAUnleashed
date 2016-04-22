using System;

namespace TunnelVision
{
    public class Level
    {
        public int Missiles;
        public int Enemies;
        public int Time;
        public float EnemySpeed;

        public Level(int missiles, int enemies, int time, float enemySpeed)
        {
            Missiles = missiles;
            Enemies = enemies;
            Time = time + 1;
            EnemySpeed = enemySpeed;
        }
    }
}
