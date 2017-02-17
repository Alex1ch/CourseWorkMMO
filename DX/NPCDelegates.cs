using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX
{
    class NPCDelegates
    {
        static public void AndreClickFunc(Player player, NPC ThisNPC)
        {
            if(player.Quests[0].State==0 || player.Quests[0].State == 2) player.Quests[0].StateUp();
        }

        static public string AndreCalcAnim(NPC ThisNPC)
        {
            return "IdleR";
        }

        static public bool AndreExMark(Player player) {
            if (player.Quests[0].State == 0 || player.Quests[0].State == 2) return true;
            return false;
        }
    }
}
