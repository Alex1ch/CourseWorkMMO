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
        bool vendor=false;

        Item[] goods;

        string name;

        List<string> dialogs;

        float x;
        float y;

        string[] textures;
    
        public NPCClickFuncDel ClickFunc;  

        public NPCCalcAnimDel CalcAnim;
        public NPCExMarkDel ExMark;

        public NPC(float X, float Y,string _name, string[] Textures, NPCClickFuncDel clickFunc, NPCCalcAnimDel calcAnim, NPCExMarkDel exMark, List<string> _dialogs) {
            textures = Textures;
            dialogs = _dialogs;
            name = _name;
            x = X;
            y = Y;
            ClickFunc = clickFunc;
            CalcAnim = calcAnim;
            ExMark=exMark;
        }

        public NPC(float X, float Y, string _name, string[] Textures, NPCClickFuncDel clickFunc, NPCCalcAnimDel calcAnim, NPCExMarkDel exMark, List<string> _dialogs, Item[] _goods)
        {
            vendor = true;
            goods = _goods;
            textures = Textures;
            dialogs = _dialogs;
            name = _name;
            x = X;
            y = Y;
            ClickFunc = clickFunc;
            CalcAnim = calcAnim;
            ExMark = exMark;
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

        public List<string> Dialogs
        {
            get
            {
                return dialogs;
            }

            set
            {
                dialogs = value;
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

        public bool Vendor
        {
            get
            {
                return vendor;
            }

            set
            {
                vendor = value;
            }
        }
    }
}
