﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DX
{
    public class Player
    {
        public NPC LastNPC;

        bool Visible = false;
        long lastPack;

        byte level;
        long exp;

        byte direction = 0;

        bool online = true;

        int id;
        float x=32, y=32;
        float rotation;
        float basespeed = .05f;
        float speedbuff = 0;
        float hp = 100;
        float maxHp = 100;
        float hpUp=0;

        string name;

        int[] killedEnemies = new int[100];

        float sX, sY;

        bool up = false;
        bool dialogMenu=false;
        bool down = false;
        bool left = false;
        bool right = false;
        bool attack = false;
        bool alive = true;
        float atkDmg = 28;

        string texture;
        
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

        public Player(float pX, float pY, Inventory _inventory, string _name)
        {
            lastPack = DateTime.Now.Ticks;
            name = _name;
            quests.Add(new DarkSignsQuest());
            x = pX; y = pY;
            sX = x; sY = y;
            inventory = _inventory;
        }


        public void Interpolation() {
            if (Math.Abs(sX - x) > basespeed || Math.Abs(sY - y) > basespeed)
            {
                x += (sX - x) * basespeed*4;
                y += (sY - y) * basespeed*4;
            }
            else {
                return; }
        }


        public Player(float pX, float pY,string _name)
        {
            lastPack = DateTime.Now.Ticks;
            name = _name;
            quests.Add(new DarkSignsQuest());
            x = pX; y = pY;
            inventory = new Inventory(this);
        }
        
        internal void CheckQuests()
        {
            foreach (Quest quest in quests) {
                if(quest.State!=0 && !quest.Finished) quest.QuestCheck(this);
            }
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

            Player player = this;

            if (!player.LEFT && !player.RIGHT && !player.UP && !player.DOWN && !player.Attack)
            {
                player.ResetAnim();
                if (player.Rotation < 45 && player.Rotation >= -45) texture = "IdleR";
                if (player.Rotation >= 45 && player.Rotation < 135) texture = "IdleU";
                if (player.Rotation >= 135 || player.Rotation < -135) texture = "IdleL";
                if (player.Rotation >= -135 && player.Rotation < -45) texture = "IdleD";
            }


            if (player.Attack)
            {
                if (player.Rotation < 45 && player.Rotation >= -45) texture = "HeroAtkR" + player.AttackAtState.ToString();
                if (player.Rotation >= 45 && player.Rotation < 135) texture = "HeroAtkU" + player.AttackAtState.ToString();
                if (player.Rotation >= 135 || player.Rotation < -135) texture = "HeroAtkL" + player.AttackAtState.ToString();
                if (player.Rotation >= -135 && player.Rotation < -45) texture = "HeroAtkD" + player.AttackAtState.ToString();
            }
            else
            {

                if (player.LEFT)
                {
                    texture = "HeroLEFT" + player.RunAtState.ToString();
                }
                if (player.UP)
                {
                    texture = "HeroUP" + player.RunAtState.ToString();
                }
                if (player.DOWN)
                {
                    texture = "HeroDOWN" + player.RunAtState.ToString();
                }
                if (player.RIGHT)
                {
                    texture = "HeroRIGHT" + player.RunAtState.ToString();
                }
            }
        }

        public void MovedByControl(bool W, bool A, bool S, bool D, byte[,] map) {
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


        public void CalcSpriteDir(bool W, bool A, bool S, bool D)
        {
            if (alive)
            {
                if (W && !A && !D && !S) 
                    UP = true;
                else UP = false;

                if (A && !W && !S && !D)
                    LEFT = true;
                else LEFT = false;

                if (S && !A && !D && !W) 
                    DOWN = true;
                else DOWN = false;

                if (D && !W && !S && !A) 
                    RIGHT = true;
                else RIGHT = false;


                bool diagonal = false;

                if (!LEFT && !RIGHT && !UP && !DOWN) diagonal = true;

                if (W && D && !A && !S)
                {
                        if (diagonal) RIGHT = true;
                    
                }
                if (W && A && !D && !S)
                {
                        if (diagonal) LEFT = true;
                    
                }
                if (S && D && !A && !W)
                {
                        if (diagonal) RIGHT = true;
                        
                }
                if (S && A && !D && !W)
                {
                        if (diagonal) LEFT = true;
                    
                }
            }
        }


        public void CalcDir(bool W, bool A, bool S, bool D)
        {
            if (alive)
            {
                if (W && !A && !D && !S)
                    direction = 3;
                else UP = false;

                if (A && !W && !S && !D)
                    direction = 5;
                else LEFT = false;

                if (S && !A && !D && !W)
                    direction = 7;
                else DOWN = false;

                if (D && !W && !S && !A)
                    direction = 1;
                else RIGHT = false;


                bool diagonal = false;

                if (!LEFT && !RIGHT && !UP && !DOWN) diagonal = true;

                if (W && D && !A && !S)
                {
                    if (diagonal) direction = 2;
                }
                if (W && A && !D && !S)
                {
                    if (diagonal) direction = 4;
                }
                if (S && D && !A && !W)
                {
                    if (diagonal) direction = 8;
                }
                if (S && A && !D && !W)
                {
                    if (diagonal) direction = 6;
                    
                }
            }
            if ((!W && !A && !S && !D)|| !W && A && !S && D|| W && !A && S && !D) direction = 0;
        }


        public void ResetAnim() {
            RunAnState = 0;
        }

        public void AttackFunc(Enemy[] EnemyList) {
            if(!Attack&&alive)Task.Factory.StartNew(() => {
                Attack = true;
                float rotationtoenemy;
                for (int i = 0; i < AttackFrames; i++)
                {
                    AttackAtState = i;
                    Thread.Sleep(attackRate);
                }
                
                for(int i=0;i<EnemyList.Length;i++)
                {
                    if (EnemyList[i] == null) break;
                    if (!EnemyList[i].Active) break;
                    rotationtoenemy = (float)(Math.Atan2((EnemyList[i].Y - y), (EnemyList[i].X - x)) / Math.PI * 180);
                    if (Math.Sqrt((x - EnemyList[i].X) * (x - EnemyList[i].X) + (y - EnemyList[i].Y) * (y - EnemyList[i].Y)) < 1.8f&&
                        (Math.Abs(rotation-rotationtoenemy)<50||Math.Abs(rotationtoenemy-rotation)>310))
                    {
                        EnemyList[i].HP -= atkDmg;
                        if (EnemyList[i].DeathCheck()) KilledEnemies[EnemyList[i].Id]++;
                    }
                }
                Attack = false;
            });
        }




        public void AttackAnimFunc()
        {
            if (!Attack && alive) Task.Factory.StartNew(() => {
                Attack = true;
                for (int i = 0; i < AttackFrames; i++)
                {
                    AttackAtState = i;
                    Thread.Sleep(attackRate);
                }

                Attack = false;
            });
        }




        public bool CheckCollisionR(byte[,] map) {
            float speed = basespeed + speedbuff;
            int X = (int)(x + speed +.3f);
            int Y = (int)y;
            if(X<0||X>map.GetUpperBound(0)||Y<0||Y>map.GetUpperBound(1))return true;
            if (map[X, Y] == 1) return false;
            return true;
        }

        public bool CheckCollisionL(byte[,] map)
        {
            float speed = basespeed + speedbuff;
            int X = (int)(x - speed - .3f);
            int Y = (int)y;
            if (X < 0 || X > map.GetUpperBound(0) || Y < 0 || Y > map.GetUpperBound(1)) return true;
            if (map[X, Y] == 1) return false;
            return true;
        }

        public bool CheckCollisionU(byte[,] map)
        {
            float speed = basespeed + speedbuff;
            int X = (int)x;
            int Y = (int)(y+speed + .18f);
            if (X < 0 || X > map.GetUpperBound(0) || Y < 0 || Y > map.GetUpperBound(1)) return true;
            if (map[X, Y] == 1) return false;
            return true;
        }

        public bool CheckCollisionD(byte[,] map)
        {
            float speed = basespeed + speedbuff;
            int X = (int)x;
            int Y = (int)(y - speed - .1f);
            if (X < 0 || X > map.GetUpperBound(0) || Y < 0 || Y > map.GetUpperBound(1)) return true;
            if (map[X, Y] == 1) return false;
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
                sX = value;
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
                sY = value;
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



        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }


        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string Texture
        {
            get
            {
                return texture;
            }

            set
            {
                texture = value;
            }
        }

        public float SX
        {
            get
            {
                return sX;
            }

            set
            {
                sX = value;
            }
        }

        public float SY
        {
            get
            {
                return sY;
            }

            set
            {
                sY = value;
            }
        }

        public byte Direction
        {
            get
            {
                return direction;
            }

            set
            {
                direction = value;
            }
        }

        public bool Online
        {
            get
            {
                return online;
            }

            set
            {
                online = value;
            }
        }

        public byte Level
        {
            get
            {
                return level;
            }

            set
            {
                level = value;
            }
        }

        public long Exp
        {
            get
            {
                return exp;
            }

            set
            {
                exp = value;
            }
        }

        public float Speed
        {
            get
            {
                return basespeed+speedbuff;
            }
        }

        public bool Visible1
        {
            get
            {
                return Visible;
            }

            set
            {
                Visible = value;
            }
        }

        public long LastPack
        {
            get
            {
                return lastPack;
            }

            set
            {
                lastPack = value;
            }
        }
    }
}
