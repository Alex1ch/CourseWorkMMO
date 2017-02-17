using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX
{
    public delegate void NPCClickFuncDel(Player player, NPC ThisNPC);
    public delegate string NPCCalcAnimDel(NPC ThisNPC);
    public delegate bool NPCExMarkDel(Player player);


    public class NPC
    {
        float x;
        float y;

        string[] textures;
    
        public NPCClickFuncDel ClickFunc;  

        public NPCCalcAnimDel CalcAnim;
        public NPCExMarkDel ExMark;

        public NPC(float X, float Y, string[] Textures, NPCClickFuncDel clickFunc, NPCCalcAnimDel calcAnim,NPCExMarkDel exMark) {
            textures = Textures;
            x = X;
            y = Y;
            ClickFunc = clickFunc;
            CalcAnim = calcAnim;
            ExMark=exMark;
        }

        public NPC(float X, float Y, string[] Textures, NPCClickFuncDel clickFunc)
        {
            textures = Textures;
            x = X;
            y = Y;
            ClickFunc = clickFunc;
        }


        public void CheckQuests() {

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

        public string[] Texture
        {
            get
            {
                return textures;
            }

            set
            {
                textures = value;
            }
        }
    }
}
