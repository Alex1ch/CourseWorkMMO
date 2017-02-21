using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DX
{
    public class Player
    {
        float x=32, y=32;
        float rotation;
        float basespeed = .05f;
        float speedbuff;
        float hp = 100;
        float maxHp = 100;
        float hpUp=0;

        int[] killedEnemies = new int[100];

        bool up = false;
        bool dialogMenu=false;
        string dialogMenuText = "";
        bool down = false;
        bool left = false;
        bool right = false;
        bool attack = false;
        bool alive = true;
        float atkDmg = 28;
        
        int frameskip = 0;
        int frameskiplimit;
        int RunAnState = 0;
        int attackAtState = 0;
        int attackFrames = 2;
        int attackRate = 200;

        Inventory inventory;
        List<Quest> quests=new List<Quest>();

        public void CheckDeath() {
            if (Hp <= 0) alive = false;
        }

        public void CalcRotation(int X, int Y, int x, int y) {
            rotation=(float)(Math.Atan2((Y - y) , (X - x)) / Math.PI * 180); 
        }

        public Player(float pX, float pY, Inventory _inventory)
        {
            quests.Add(new DarkSignsQuest());
            x = pX; y = pY;
            inventory = _inventory;
        }


        internal void CheckQuests()
        {
            foreach (Quest quest in quests) {
                if(quest.State!=0 && !quest.Finished) quest.QuestCheck(this);
            }
        }

        public Player(float pX, float pY)
        {
            quests.Add(new DarkSignsQuest());
            x = pX; y = pY;
            inventory = new Inventory(this);
        }

        public void CalcAnim() {
            frameskiplimit = (int)(0.2f/(basespeed + speedbuff));
            if (!Attack)
            {
                if (LEFT)
                {
                    if (frameskip > frameskiplimit)
                    {
                        if (RunAnState < 7) RunAnState++; else RunAnState = 0;
                        frameskip = 0;
                    }
                    frameskip++;
                }
                if (UP)
                {
                    if (frameskip > frameskiplimit)
                    {
                        if (RunAnState < 7) RunAnState++; else RunAnState = 0;
                        frameskip = 0;
                    }
                    frameskip++;
                }
                if (DOWN)
                {
                    if (frameskip > frameskiplimit)
                    {
                        if (RunAnState < 7) RunAnState++; else RunAnState = 0;
                        frameskip = 0;
                    }
                    frameskip++;
                }
                if (RIGHT)
                {
                    if (frameskip > frameskiplimit)
                    {
                        if (RunAnState < 7) RunAnState++; else RunAnState = 0;
                        frameskip = 0;
                    }
                    frameskip++;
                }
            }
        }

        public void MovedByControl(bool W, bool A, bool S, bool D, int[,] map) {
            float speed = basespeed + speedbuff;
            if (alive)
            {
                if (W && !A && !D && !S) if (CheckCollisionU(map))
                    {
                        Y += speed;
                        UP = true;
                    }
                    else UP = false;
                else UP = false;

                if (A && !W && !S && !D) if (CheckCollisionL(map))
                    {
                        X -= speed;
                        LEFT = true;
                    }
                    else LEFT = false;
                else LEFT = false;

                if (S && !A && !D && !W) if (CheckCollisionD(map))
                    {
                        Y -= speed;
                        DOWN = true;
                    }
                    else DOWN = false;
                else DOWN = false;

                if (D && !W && !S && !A) if (CheckCollisionR(map))
                    {
                        X += speed;
                        RIGHT = true;
                    }
                    else RIGHT = false;
                else RIGHT = false;


                bool diagonal = false;

                if (!LEFT && !RIGHT && !UP && !DOWN) diagonal = true;

                if (W && D && !A && !S)
                {
                    if (CheckCollisionU(map))
                    {
                        Y += speed * 0.707f;
                        if (diagonal) RIGHT = true;
                    }
                    if (CheckCollisionR(map))
                    {
                        X += speed * 0.707f;
                        if (diagonal) RIGHT = true;
                    }
                }
                if (W && A && !D && !S)
                {
                    if (CheckCollisionU(map))
                    {
                        Y += speed * 0.707f;
                        if (diagonal) LEFT = true;
                    }
                    if (CheckCollisionL(map))
                    {
                        X -= speed * 0.707f;
                        if (diagonal) LEFT = true;
                    }
                }
                if (S && D && !A && !W)
                {
                    if (CheckCollisionD(map))
                    {
                        Y -= speed * 0.707f;
                        if (diagonal) RIGHT = true;
                    }
                    if (CheckCollisionR(map))
                    {
                        X += speed * 0.707f;
                        if (diagonal) RIGHT = true;

                    }
                }
                if (S && A && !D && !W)
                {
                    if (CheckCollisionD(map))
                    {
                        Y -= speed * 0.707f;
                        if (diagonal) LEFT = true;
                    }
                    if (CheckCollisionL(map))
                    {
                        X -= speed * 0.707f;
                        if (diagonal) LEFT = true;
                    }
                }
            }
        }



        public void ResetAnim() {
            RunAnState = 0;
        }

        public void AttackFunc(Dictionary<int,Enemy> EnemyList) {
            if(!Attack&&alive)Task.Factory.StartNew(() => {
                Attack = true;
                float rotationtoenemy;
                for (int i = 0; i < AttackFrames; i++)
                {
                    AttackAtState = i;
                    Thread.Sleep(attackRate);
                }
                foreach (KeyValuePair<int, Enemy> enemy in EnemyList)
                {
                    rotationtoenemy = (float)(Math.Atan2((enemy.Value.Y - y), (enemy.Value.X - x)) / Math.PI * 180);
                    if (Math.Sqrt((x - enemy.Value.X) * (x - enemy.Value.X) + (y - enemy.Value.Y) * (y - enemy.Value.Y)) < 1.8f&&
                        (Math.Abs(rotation-rotationtoenemy)<50||Math.Abs(rotationtoenemy-rotation)>310))
                    {
                        enemy.Value.HP -= atkDmg;
                        if (enemy.Value.DeathCheck()) KilledEnemies[enemy.Value.Id]++;
                    }
                }
                Attack = false;
            });
        }

        
        public bool CheckCollisionR(int[,] map) {
            float speed = basespeed + speedbuff;
            int X = (int)(x + speed +.3f);
            int Y = (int)y;
            if(X<0||X>map.GetUpperBound(0)||Y<0||Y>map.GetUpperBound(1))return true;
            if (map[X, Y] >= 3&& map[X, Y] <= 5) return false;
            return true;
        }

        public bool CheckCollisionL(int[,] map)
        {
            float speed = basespeed + speedbuff;
            int X = (int)(x - speed - .3f);
            int Y = (int)y;
            if (X < 0 || X > map.GetUpperBound(0) || Y < 0 || Y > map.GetUpperBound(1)) return true;
            if (map[X, Y] >= 3 && map[X, Y] <= 5) return false;
            return true;
        }

        public bool CheckCollisionU(int[,] map)
        {
            float speed = basespeed + speedbuff;
            int X = (int)x;
            int Y = (int)(y+speed + .18f);
            if (X < 0 || X > map.GetUpperBound(0) || Y < 0 || Y > map.GetUpperBound(1)) return true;
            if (map[X, Y] >= 3 && map[X, Y] <= 5) return false;
            return true;
        }

        public bool CheckCollisionD(int[,] map)
        {
            float speed = basespeed + speedbuff;
            int X = (int)x;
            int Y = (int)(y - speed - .1f);
            if (X < 0 || X > map.GetUpperBound(0) || Y < 0 || Y > map.GetUpperBound(1)) return true;
            if (map[X, Y] >= 3 && map[X, Y] <= 5) return false;
            return true;
        }

        public float X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public float Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public float Rotation
        {
            get
            {
                return rotation;
            }

            set
            {
                rotation = value;
            }
        }

        public float SpeedBuff
        {
            get
            {
                return speedbuff;
            }

            set
            {
                speedbuff = value;
            }
        }

        public bool UP
        {
            get
            {
                return up;
            }

            set
            {
                up = value;
            }
        }

        public bool DOWN
        {
            get
            {
                return down;
            }

            set
            {
                down = value;
            }
        }

        public bool LEFT
        {
            get
            {
                return left;
            }

            set
            {
                left = value;
            }
        }

        public bool RIGHT
        {
            get
            {
                return right;
            }

            set
            {
                right = value;
            }
        }

        public int RunAtState
        {
            get
            {
                return RunAnState;
            }

            set
            {
                RunAnState = value;
            }
        }

        public bool Attack
        {
            get
            {
                return attack;
            }

            set
            {
                attack = value;
            }
        }

        public int AttackFrames
        {
            get
            {
                return attackFrames;
            }

            set
            {
                attackFrames = value;
            }
        }

        public int AttackRate
        {
            get
            {
                return attackRate;
            }

            set
            {
                attackRate = value;
            }
        }

        public int AttackAtState
        {
            get
            {
                return attackAtState;
            }

            set
            {
                attackAtState = value;
            }
        }

        public float Hp
        {
            get
            {
                return hp;
            }

            set
            {
                hp = value;
            }
        }

        public float MaxHp
        {
            get
            {
                return maxHp;
            }

            set
            {
                maxHp = value;
            }
        }

        public bool Alive
        {
            get
            {
                return alive;
            }

            set
            {
                alive = value;
            }
        }

        public float HpUp
        {
            get
            {
                return hpUp;
            }

            set
            {
                hpUp = value;
            }
        }

        public Inventory Inventory
        {
            get
            {
                return inventory;
            }

            set
            {
                inventory = value;
            }
        }

        public int[] KilledEnemies
        {
            get
            {
                return killedEnemies;
            }

            set
            {
                killedEnemies = value;
            }
        }

        internal List<Quest> Quests
        {
            get
            {
                return quests;
            }
        }

        public bool DialogMenu
        {
            get
            {
                return dialogMenu;
            }

            set
            {
                dialogMenu = value;
            }
        }

        public string DialogMenuText
        {
            get
            {
                return dialogMenuText;
            }

            set
            {
                dialogMenuText = value;
            }
        }
    }
}
