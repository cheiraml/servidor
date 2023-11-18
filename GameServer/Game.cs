using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    [Serializable]
    public class Axis
    {
        public int Vertical;
        public int Horizontal;
    }
    public class Posiciondireccion
    {
        public float x;
        public float y;
        public float direccionx;
        public float direcciony;
    }
    public class Rotacion
    {
        public float x, y;
    }
    public class GameState
    {
        public List<Player> Players { get; set; }
        public List<Coin> Coins { get; set; }
        public List<Bullet> Bullets { get; set; }
        public GameState()
        {
            Players = new List<Player>();
            Coins = new List<Coin>();
            Bullets = new List<Bullet>();
        }
    }
    internal class Game
    {
        const int WorldWidth = 500;
        const int WorldHeigh = 400;
        const int LoopPeriod = 10;
        const int MaxCoins = 15;
        const int Gravity = 5;
        const int JumpForce = 10;
        public GameState State { get; set; }

        private  Dictionary<string, Axis> Axes;
        public Game()
        {
            State = new GameState();
            Axes = new Dictionary<string, Axis>();

            StartGameLoop();
            StartSpawnCoins();
        }
        public void SpawnPlayer(string id,string username)
        {
            Random random = new Random();
            State.Players.Add(new Player()
            {
                Id = id,
                Username = username,
                x = random.Next(10, WorldWidth - 10),
                y = random.Next(10, WorldHeigh - 10),
                Speed = 2,
                Radius = 10
            });

            Axes[id] = new Axis{Horizontal= 0,Vertical= 0 };

        }

        public void SetAxis(string id,Axis axis)
        {
            Axes[id]= axis;
        }

        public void Update()
        {
            List<string> takedCoinsIds= new List<string>();

            foreach (var player in State.Players)
            {
                var axis = Axes[player.Id];

                if (axis.Horizontal > 0 && player.x < WorldWidth - player.Radius)
                {
                    player.x += player.Speed;
                }
                else if (axis.Horizontal < 0 && player.x > 0 + player.Radius)
                {
                    player.x -= player.Speed;
                }
                if (axis.Vertical > 0 && player.y < WorldHeigh - player.Radius)
                {
                    player.y += player.Speed;
                }
                else if (axis.Vertical < 0 && player.y > 0 + player.Radius)
                {
                    player.y -= player.Speed;
                }

               

                State.Coins = State.Coins.Where(coin => {
                    if (!coin.Take(player))
                    {
                        return true;
                    }
                    else
                    {
                        player.Score += coin.Points;
                        Console.WriteLine(player.Username+":"+player.Score);
                        return false;
                    }
                }).ToList();
                
                foreach(Bullet bullet in State.Bullets)
                {
                    bullet.Moverse();
                    if (bullet.x < 0 + bullet.Radius || bullet.x > WorldWidth - bullet.Radius || bullet.y < 0 + bullet.Radius || bullet.y > WorldHeigh - bullet.Radius) 
                        {
                        bullet.Taken = true;
                        }
                    State.Bullets = State.Bullets.Where(bullet => {
                        if (bullet.Take(player))
                        {
                            player.Vida--;
                            if( player.Vida == 0)
                            {
                                Muerte(player);
                            }
                            bullet.Taken = true;
                            return false;
                        }
                        else if(bullet.Taken)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }).ToList();
                }

            }
            

        }
        public void RemovePlayer(string id)
        {
            State.Players = State.Players.Where(player => player.Id != id).ToList();
            Axes.Remove(id);
        }

        async Task StartGameLoop()
        {
            while (true)
            {
                Update();
                await Task.Delay(TimeSpan.FromMilliseconds(LoopPeriod)); 
            }
        }

        async Task StartSpawnCoins()
        {
            while (true)
            {
                SpawnCoin();
                await Task.Delay(TimeSpan.FromSeconds(2)); 
            }
        }

        void SpawnCoin()
        {
            Random random = new Random();

            if (State.Coins.Count <= MaxCoins) {
                Coin coin = new Coin {
                    Id = Guid.NewGuid().ToString(),
                    x = random.Next(10, WorldWidth - 10),
                    y = random.Next(10, WorldHeigh - 10),
                    Radius = 10,
                    Points = 1
                };
                State.Coins.Add(coin);
                Console.WriteLine(State.Coins.Count+":New Coin " +coin.Id);
            }
        }
        public void SpawnBullet(Posiciondireccion este)
        {
            Random random = new Random();

            Bullet bullet = new Bullet
            {
                Id = Guid.NewGuid().ToString(),
                x = este.x,
                y = este.y,
                direccionx = este.direccionx,
                direcciony = este.direcciony,
                Radius = 10,
                Speed = 5
            };
            State.Bullets.Add(bullet);
            
        }
        public void Muerte(Player player)
        {
            player.Convida = true;
            Revive_Player(player);

        }

        async Task Revive_Player(Player player)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            Random random = new Random();
            player.x = random.Next(10, WorldWidth - 10);
            player.y = random.Next(10, WorldHeigh - 10);
            player.Convida = false;
        }
        public void SetRotatio(Rotacion rot, Player play)
        {
            play.rotacionx = rot.x;
            play.rotaciony = rot.y;
        }

    }
}
