using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace DX
{
    class NPCDelegates
    {

        //Andre
        static public void AndreClickFunc(Player player, NPC ThisNPC)
        {
            Task.Factory.StartNew(() =>
            {
                player.DialogMenu = true;
                if (player.Quests[0].State != 3) ThisNPC.DialogMenuText = ThisNPC.Dialogs[player.Quests[0].State + 1];
                else ThisNPC.DialogMenuText = ThisNPC.Dialogs[0];

                //if (player.Quests[0].State == 0 || player.Quests[0].State == 2) player.Quests[0].StateUp();


                while (Math.Sqrt((ThisNPC.X - player.X) * (ThisNPC.X - player.X) + (ThisNPC.Y - player.Y) * (ThisNPC.Y - player.Y))<3f) {
                    Thread.Sleep(30);
                    if (IsKeyDown(Keys.D1) && (player.Quests[0].State == 0 || player.Quests[0].State == 2)) {
                        player.Quests[0].StateUp();
                        break;
                    }
                    if (IsKeyDown(Keys.D2)) break; 
                }
                player.DialogMenu = false;
            });
        }

        static public string AndreCalcAnim(NPC ThisNPC)
        {
            return ThisNPC.Texture[0];
        }

        static public bool AndreExMark(Player player) {
            if (player.Quests[0].State == 0 || player.Quests[0].State == 2) return true;
            return false;
        }




        //Vendor

        static public void VendorClickFunc(Player player, NPC ThisNPC)
        {
            Task.Factory.StartNew(() =>
            {
                player.DialogMenu = true;
                ThisNPC.DialogMenuText = ThisNPC.Dialogs[0];

                while (Math.Sqrt((ThisNPC.X - player.X) * (ThisNPC.X - player.X) + (ThisNPC.Y - player.Y) * (ThisNPC.Y - player.Y)) < 3f)
                {
                    Thread.Sleep(30);
                    if (IsKeyDown(Keys.D1)) {
                        ThisNPC.Trade = true; 
                    } 
                    if (IsKeyDown(Keys.D2)) break;
                }
                ThisNPC.Trade = false;
                player.DialogMenu = false;
            });
        }

        static public string VendorCalcAnim(NPC ThisNPC)
        {
            return ThisNPC.Texture[0];
        }

        static public bool VendorExMark(Player player)
        {
            return false;
        }




        public static bool IsKeyDown(Keys key)
        {
            return (GetKeyState(Convert.ToInt16(key)) & 0X80) == 0X80;
        }

        [DllImport("user32.dll")]
        public extern static Int16 GetKeyState(Int16 nVirtKey);
    }

}
