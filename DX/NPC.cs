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
        bool trade = false;
        string dialogMenuText = "";

        bool vendor=false;

        Item[] goods;
        int[] prices;

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

        public NPC(float X, float Y, string _name, string[] Textures, NPCClickFuncDel clickFunc, NPCCalcAnimDel calcAnim, NPCExMarkDel exMark, List<string> _dialogs, object[,] _goods)
        {
            vendor = true;
            goods = new Item[_goods.Length / 2];
            prices = new int[_goods.Length / 2];
            for (int i = 0; i < _goods.Length/2; i++) {
                Goods[i] = (Item)_goods[i, 0];
                prices[i] = (int)_goods[i, 1];
            }
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

        public void Sell(Player player, int index) {
            Item clone = (Item)goods[index].Clone();
            if(player.Inventory.Take(69, prices[index]))
            player.Inventory.Add(clone);
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
        }

        public bool Vendor
        {
            get
            {
                return vendor;
            }
        }

        public bool Trade
        {
            get
            {
                return trade;
            }
            set {
                trade = value;
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

        public int[] Prices
        {
            get
            {
                return prices;
            }
        }

        public Item[] Goods
        {
            get
            {
                return goods;
            }
        }
    }
}
