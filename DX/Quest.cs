using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX
{
    class Quest
    {
        int state = 0;
        bool finished = false;
        int finishState;
        string name;
        string[] desc;

        public virtual void QuestCheck(Player player)
        {
        }


        public virtual bool FinishCheck()
        {
            if (finishState == state) {
                return true;
                finished = true;
            }
            return false;
        }


        public int State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
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
    }

    class DarkSignsQuest : Quest {
        int counter = 0;
        int startkills;
        int killlimit = 7;

        public DarkSignsQuest() {
            base.State = 0;
            base.FinishState = 2;
            base.Name = "Dark Signs";
            base.Desc = new string[base.FinishState];

            Desc[1] = "Civilians saved, Ghosts were killed";
        }

        public override void QuestCheck(Player player)
        {
            switch (State) {
                case 0:
                    {
                        if (player.X > 30 && player.X < 43 && player.Y > 30 && player.Y < 43) {
                            startkills = player.KilledEnemies[0];
                            State++;
                        }
                        break;
                    }
                case 1: {
                        counter = player.KilledEnemies[1] - startkills;
                        Desc[0] = "You find some strange spoil\non the floor and then you saw\nthe Scary Ghost tourchering\npeople, you should kill them (" + counter.ToString()+"/"+killlimit.ToString()+")" ;
                        if (counter == killlimit) State++;
                        break;
                    }
                case 2:
                    {
                        base.FinishCheck();
                        break;
                    }
                default: break;
            }
        }
    }
}
