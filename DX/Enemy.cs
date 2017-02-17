using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX
{
    public class Enemy
    {
        int id = 0;
        protected string texture;
        float hp;
        protected float maxHp;
        float x, y;
        bool active=true;

        public virtual void WorkCycle(List<DX.Player> Player) { }

        public virtual void DropFunc(List<Item> DropList, Random RNG) { }
        
        public bool DeathCheck() {
            if (hp > 0) {
                return false;
            }
            active = false;
            return true;
        }
        
        public float HP
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

        public bool Active
        {
            get
            {
                return active;
            }

            set
            {
                active = value;
            }
        }
        public float MaxHp
        {
            get
            {
                return maxHp;
            }
        }

        public string Texture
        {
            get
            {
                return texture;
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
    }
}



class Ghost : DX.Enemy {

    float dy;
    float dx;

    float speed = .03f;
    float atk = .05f;

    float ddysum = 0;
    float ddy = .01f;
    DX.Player aggro;


    public override void DropFunc(List<DX.Item> DropList, Random RNG) {
        int pool = RNG.Next(100);
        if (pool < 13) DropList.Add(new DX.Potion(DX.PotionType.Health,RNG.Next(1,3),X,Y,true,DropList));
    }

    public override void WorkCycle(List<DX.Player> Player) {
        Task.Factory.StartNew(() =>
        {
            base.Y += ddy;
            ddysum += ddy;
            if (ddysum > .4f || ddysum < -.4f) { ddy *= -1; ddysum = 0; }

            foreach (DX.Player player in Player)
            {
                float dist = (float)Math.Sqrt((player.X - base.X) * (player.X - base.X) + (player.Y - base.Y) * (player.Y - base.Y));
                if (aggro == null) if (dist < 5&&player.Alive)
                    {
                        aggro = player;
                    }
                    else { }
                else
                {
                    float dist1 = (float)Math.Sqrt((aggro.X - base.X) * (aggro.X - base.X) + (aggro.Y - base.Y) * (aggro.Y - base.Y));
                    if (dist1 > 5)
                    {
                        aggro = null;
                        dx = 0;
                        dy = 0;
                    }
                    else
                    {
                        dx = (aggro.X - base.X) / dist1;
                        dy = (aggro.Y - base.Y) / dist1;
                        if (dist1 < 1&&aggro.Alive) aggro.Hp -= atk;
                        if (!aggro.Alive) {
                            aggro = null;
                            dx = 0;
                            dy = 0;
                        }
                    }
                }
            }
            base.X += dx * speed;
            base.Y += dy * speed;
        });
    }

    public Ghost() {
        base.Id = 1;
        base.maxHp = 100;
        base.HP = base.MaxHp;
        base.texture = "Ghost";
    }

    public Ghost(float X, float Y,Random RNG)
    {
        base.Id = 1;
        ddy = RNG.Next(600,1000)*.00001f;
        base.maxHp = 100;
        base.HP = base.MaxHp;
        base.texture = "Ghost";
        base.X = X;
        base.Y = Y;
    }

    
}