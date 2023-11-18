using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class Bullet
    {
        public string Id { get; set; }
        public float x { get; set; }
        public float y { get; set; }

        public float direccionx { get; set; }

        public float direcciony { get; set; }
        public int Radius { get; set; }
        public bool Taken { get; set; }

        public int Speed { get; set; }

        public void Moverse()
        {
            x += direccionx * Speed;
            y += direcciony * Speed;
        }
        public bool Take(Player player)
        {
            if (!Taken)
            {
                var dx = player.x - x;
                var dy = player.y - y;
                var rSum = Radius + player.Radius;

                return dx * dx + dy * dy <= rSum * rSum;
            }
            else
            { return false; }
        }
    }
}
