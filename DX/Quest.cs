using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DX
{
    class Quest
    {
        int state = 0;
        bool finished = false;
        int finishState;
        string name;

        bool popUp = false;

        List<Item> reward=new List<Item>();

        string[] desc;

        public virtual void QuestCheck(Player player)
        {
        }


        public virtual bool FinishCheck(Player player)
        {
            if (finishState == state) {
                foreach (Item item in Reward) {
                    player.Inventory.Add(item);
                }
                finished = true;
                return true;
            }
            return false;
        }

        public void StateUp() {
            state++; PopUpFunc();
        }

        protected void PopUpFunc() {
            if (PopUp) return;
            Task.Factory.StartNew(() =>
            {
                PopUp = true;
                Thread.Sleep(4000);
                PopUp = false;
                
            });
        }

        public int State
        {
            get
            {
                return state;
            }
        }

        public bool Finished
        {
            get
            {
                return finished;
            }

            set
            {
                finished = value;
            }
        }

        public int FinishState
        {
            get
            {
                return finishState;
            }

            set
            {
                finishState = value;
            }
        }

        public string[] Desc
        {
            get
            {
                return desc;
            }

            set
            {
                desc = value;
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

        public bool PopUp
        {
            get
            {
                return popUp;
            }

            set
            {
                popUp = value;
            }
        }

        public List<Item> Reward
        {
            get
            {
                return reward;
            }

            set
            {
                reward = value;
            }
        }
    }  

    class DarkSignsQuest : Quest {
        int counter = 0;
        int startkills;
        int killlimit = 7;

        public DarkSignsQuest() {
            base.Reward.Add(new Potion(PotionType.Energy, 2));
            base.FinishState = 3;
            base.Name = "Dark Signs";
            base.Desc = new string[base.FinishState];
            Desc[0] = "Andre asked for your help\nthe Scary Ghost's tourchering\npeople on the south, you should \nkill them all (0/" + killlimit.ToString()+")" ;
            Desc[1] = "Civilians saved, Ghosts were killed,\nreturn to Andre for your reward";
            Desc[2] = "Quest Complete!";
        }

        public override void QuestCheck(Player player)
        {
            switch (State) {
                case 0:
                    {
                        if (player.X > 30 && player.X < 43 && player.Y > 30 && player.Y < 43) {
                            StateUp();
                        }
                        break;
                    }
                case 1: {
                        if (counter != player.KilledEnemies[1] - startkills)
                        {
                            startkills = player.KilledEnemies[1];
                            counter = player.KilledEnemies[1] - startkills;
                            Desc[0] = "Andre asked for your help\nthe Scary Ghost's tourchering\npeople on the south, you should \nkill them all (" + counter.ToString() + "/" + killlimit.ToString() + ")";
                            if (counter >= killlimit) StateUp();
                                else PopUpFunc();
                        }
                        break;
                    }
                case 3:
                    {
                        FinishCheck(player);
                        break;
                    }
                default: break;
            }
        }
    }
}
