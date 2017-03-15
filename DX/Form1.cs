using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.FreeGlut;
using Tao.Platform.Windows;
using Tao.OpenGl;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;

namespace DX
{
    public partial class Form1 : Form
    {
        Task Reader;

        NetGame connect;
        Random RNG;

        

        const int MAXPLAYERS = 200;
        const int MAXENEMY = 1000;

        string LoginScreenErr = "";

        Dictionary<string, int> Textures;
        Enemy[] EnemyList=new Enemy[MAXENEMY];
        Player[] PlayersList=new Player[MAXPLAYERS];
        List<Item> DropList;
        List<Player> AllPlayers;
        List<NPC> NPCList;

        int[,] map;
        int[,] obj_map;
        byte[,] col_map;

        int pingcounter = 0;

        //float x, y;
        float ScaleH, ScaleW;

        float ScrH, ScrW;
        int MouseX, MouseY;
        float MouseOnMatrixX, MouseOnMatrixY;
        float MouseOnMapX, MouseOnMapY;

        bool QuestMenu = false;
        bool InvMenu = false;
        bool LoginScreen = true;

        float CamX = 30, CamY = 30;

        Player player;



        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            Gl.glClearDepth(.5);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glClearColor(255, 255, 255, 1);
            Gl.glLoadIdentity();


            Gl.glPushMatrix();
            Gl.glTranslatef(-player.X + ScrW / 2, -player.Y + ScrH / 2, 0);



            // Отрисовка окружения


            for (int i = (int)player.X-8; i < player.X + 8; i ++) {
                for (int j = (int)player.Y - 5; j < player.Y + 5; j++)
                {
                    if (!player.Alive) Gl.glColor3f(.5f, .5f, .5f);
                    else Gl.glColor3f(1f, 1f, 1f);
                    //Текстуры карты
                    if (i < 0 || j < 0 || i > map.GetUpperBound(0) || j > map.GetUpperBound(1)) continue;
                    if (map[i, j] == 0) Draw2DText(i, j, 0, 1f, 1f, Textures["Grass"]);
                    if (map[i, j] == 1) Draw2DText(i, j, 0, 1f, 1f, Textures["Ice"]);
                    if (map[i, j] == 2) Draw2DText(i, j, 0, 1f, 1f, Textures["Sand"]);
                    if (map[i, j] == 3) Draw2DText(i, j, 0, 1f, 1f, Textures["Ground"]);
                    if (map[i, j] == 4) Draw2DText(i, j, 0, 1f, 1f, Textures["StWall"]);
                    if (map[i, j] == 5) Draw2DText(i, j, -2.1f, 1f, 1f, Textures["StBlack"]);
                    if (map[i, j] == 6) Draw2DText(i, j, 0, 1f, 1f, Textures["StFloor"]);
                    if (map[i, j] == 7) Draw2DText(i, j, 0, 1f, 1f, Textures["WFloor"]);
                    if (map[i, j] == 8) Draw2DText(i, j, -2.1f, 1f, 1f, Textures["StBlack"]);

                    //Текстуры объектов
                    if (obj_map[i, j] == 1) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadD"]);
                    if (obj_map[i, j] == 2) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadL"]);
                    if (obj_map[i, j] == 3) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadLD"]);
                    if (obj_map[i, j] == 4) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadLU"]);
                    if (obj_map[i, j] == 5) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadR"]);
                    if (obj_map[i, j] == 6) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadRD"]);
                    if (obj_map[i, j] == 7) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadRU"]);
                    if (obj_map[i, j] == 8) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadU"]);
                    if (obj_map[i, j] == 9) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadZDL"]);
                    if (obj_map[i, j] == 10) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadZDR"]);
                    if (obj_map[i, j] == 11) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadZUL"]);
                    if (obj_map[i, j] == 12) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadZUR"]);
                }
            }

            Gl.glTranslatef(player.X - ScrW / 2, player.Y - ScrH / 2, 0);
            Gl.glPopMatrix();


            //отрисовка героя
            

            MousePosOnAnt(out MouseX, out MouseY, out MouseOnMatrixX, out MouseOnMatrixY);

            Gl.glColor3f(1,1,1);

            Gl.glPushMatrix();

            Gl.glTranslatef(ScrW / 2, ScrH / 2, 0);

            if (player.Alive)
            {

                Draw2DTextCent(0, .60f, OnScreenYtoZ(player.Y), 1.5f, 1.5f, Textures[player.Texture]);

            }
            else
            {
                Gl.glRotatef(90f,0,0,1);
                Draw2DTextCent(0, .60f, OnScreenYtoZ(player.Y), 1.5f, 1.5f, Textures["IdleR"]);
            }
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glTranslatef(-player.X + ScrW / 2, -player.Y + ScrH / 2, 0);


            //
            //
            
            for(int i=1;i<PlayersList.Length;i++)
            {
                if (PlayersList[i] == null) continue;
                if (Math.Sqrt((player.X - PlayersList[i].X) * (player.X - PlayersList[i].X) + (player.Y - PlayersList[i].Y) * (player.Y - PlayersList[i].Y)) > 11f) continue;
                PlayersList[i].CalcAnim();
                if (PlayersList[i].Alive)
                {
                    Console.WriteLine(PlayersList[i].Name + " " + PlayersList[i].X + " " + PlayersList[i].Y);
                    DrawStringCent(PlayersList[i].X - 2, PlayersList[i].X + 2, PlayersList[i].Y + 1.3f, -3.5f, Glut.GLUT_BITMAP_HELVETICA_12, PlayersList[i].Name, 1, 1, 1, true);
                    Gl.glColor3f(0f, 0f, 0f);
                    Draw2DText(PlayersList[i].X - .45f, PlayersList[i].Y + 1.1f, -3, .9f, .15f, Textures["HPBAR"]);
                    Gl.glColor3f(1f, 0f, 0f);
                    Draw2DText(PlayersList[i].X - .45f, PlayersList[i].Y + 1.1f, -3, .9f / PlayersList[i].MaxHp * PlayersList[i].Hp, .15f, Textures["HPBAR"]);
                    Gl.glColor3f(1, 1, 1);
                    Draw2DTextCent(PlayersList[i].X, PlayersList[i].Y+0.6f, OnScreenYtoZ(PlayersList[i].Y), 1.5f, 1.5f, Textures[PlayersList[i].Texture]);
                }
                else
                {
                    Gl.glRotatef(90f, 0, 0, 1);
                    DrawStringCent(PlayersList[i].X - 2, PlayersList[i].X + 2, PlayersList[i].Y + .9f, -3.5f, Glut.GLUT_BITMAP_HELVETICA_12, PlayersList[i].Name, 1, 1, 1, true);
                    Draw2DTextCent(PlayersList[i].X, PlayersList[i].Y+0.6f, OnScreenYtoZ(PlayersList[i].Y), 1.5f, 1.5f, Textures["IdleR"]);
                }
            }



            //отрисовка дропа
            foreach (Item item in GetNearbyItems(player.X, player.Y, DropList))
            {
                Gl.glColor3f(1, 1, 1);
                Draw2DTextCent(item.X, item.Y, OnScreenYtoZ(item.Y), .4f, .4f, Textures[item.Texture]);
            }


            //отрисовка противников

            foreach (Enemy enemy in EnemyList)
            {
                if (enemy == null) continue;
                if (enemy.Active)
                {
                    if (!player.Alive) Gl.glColor3f(.5f, .5f, .5f);
                    else Gl.glColor3f(1f, 1f, 1f);
                    //Gl.glClearDepth(1);
                    Draw2DTextCent(enemy.X, enemy.Y+.6f, OnScreenYtoZ(enemy.Y), 1.2f, 1.2f, Textures[enemy.Texture]);
                    Gl.glColor3f(0f, 0f, 0f);
                    Draw2DText(enemy.X - .6f, enemy.Y + 1.25f, -3, .9f, .15f, Textures["HPBAR"]);
                    Gl.glColor3f(1f, 0f, 0f);
                    Draw2DText(enemy.X - .6f, enemy.Y + 1.25f, -3, .9f / enemy.MaxHp * enemy.HP, .15f, Textures["HPBAR"]);
                }
            }


            //Отрисовка NPC


            foreach (NPC npc in NPCList) {
                Gl.glColor3f(1, 1, 1);
                Draw2DTextCent(npc.X, npc.Y + .6f, OnScreenYtoZ(npc.Y), 1.5f, 1.5f, Textures[npc.CalcAnim(npc)]);
                DrawStringCent(npc.X-2,npc.X+2, npc.Y + 1.2f,-3.1f,Glut.GLUT_BITMAP_HELVETICA_12,npc.Name,1,1,1,true);
                if (npc.ExMark(player)) Draw2DTextCent(npc.X, npc.Y+1.75f, -2, .7f, .7f, Textures["ExMark"]);
            }


            Gl.glTranslatef(player.X - ScrW / 2, player.Y - ScrH / 2, 0);
            Gl.glPopMatrix();



            //Отрисовка меню
            //Инвентарь
            if (InvMenu)
            {
                string quantitystr="";
                Gl.glColor3f(.3f, .15f, 0);
                Draw2DText(1,2,-3.5f,4,7,Textures["Menu"]);
                Gl.glLoadIdentity();
                DrawStringCent(1,5, 8.25f, -3.5f, Glut.GLUT_BITMAP_HELVETICA_18, "Inventory", 1f, 1f, 1f, true);
                for (int i = 0; i < player.Inventory.Size; i++) {
                    Gl.glColor3f(1, 1, 1);
                    Draw2DText(1.5f+i%player.Inventory.Width*3f/4f,
                               7-(i-i%player.Inventory.Width)/player.Inventory.Width * 3f / 4f,
                               -3.5f, 3f/4f,3f/4f,
                               Textures["ItemBG"]);
                    if (player.Inventory.Items[i] != null) {
                        Draw2DText(1.5f + i % player.Inventory.Width * 3f / 4f,
                                   7 - (i - i % player.Inventory.Width) / player.Inventory.Width * 3f / 4f,
                                   -3.5f, 3f / 4f, 3f / 4f,
                                   Textures[player.Inventory.Items[i].Texture]);
                    }
                }

                for (int i = 0; i < player.Inventory.Size; i++)
                {
                    Gl.glColor3f(1, 1, 1);
                    if (player.Inventory.Items[i] != null) if (player.Inventory.Items[i].Quantity > 1 || true)
                        {
                            int quantity = player.Inventory.Items[i].Quantity;
                            quantitystr = "";
                            if (quantity > 10000 && quantity < 10000000)
                            {
                                quantitystr = (quantity / 1000).ToString() + "k";
                            }
                            else if (quantity > 10000000)
                            {
                                quantitystr = (quantity / 1000000).ToString() + "kk";
                            }
                            else quantitystr = quantity.ToString();
                            DrawStringFromRight(1.5F+ 3f/4f+i % player.Inventory.Width * 3f / 4f ,
                                   7 - (i - i % player.Inventory.Width) / player.Inventory.Width * 3f / 4f,
                                   -3.5f, Glut.GLUT_BITMAP_HELVETICA_12, quantitystr,
                                   1, 1, 1, true);
                        }
                }

                    int X = (int)((MouseOnMatrixX - 1.5f) * 4f / 3f);
                int Y = (int)((7f + 3f / 4f - MouseOnMatrixY) * 4f / 3f);
                if (X >= 0 && X < player.Inventory.Width && Y >= 0 && Y < player.Inventory.Height && player.Inventory.Items[Y*player.Inventory.Width+X]!=null)
                {
                    
                    float Width = Glut.glutBitmapLength(Glut.GLUT_BITMAP_HELVETICA_18, player.Inventory.Items[Y * player.Inventory.Width + X].Name ) * ScaleW
                        >= Glut.glutBitmapLength(Glut.GLUT_BITMAP_HELVETICA_12, player.Inventory.Items[Y * player.Inventory.Width + X].Desc + "\n\nQuantity:" + player.Inventory.Items[Y * player.Inventory.Width + X].Quantity.ToString()) * ScaleW ?
                        Glut.glutBitmapLength(Glut.GLUT_BITMAP_HELVETICA_18, player.Inventory.Items[Y * player.Inventory.Width + X].Name) * ScaleW:
                        Glut.glutBitmapLength(Glut.GLUT_BITMAP_HELVETICA_12, player.Inventory.Items[Y * player.Inventory.Width + X].Desc + "\n\nQuantity:" + player.Inventory.Items[Y * player.Inventory.Width + X].Quantity.ToString()) * ScaleW;
                    float Height = Glut.glutBitmapHeight(Glut.GLUT_BITMAP_HELVETICA_12) * ScaleH;
                    int lines = 1;
                    string outputdesc = player.Inventory.Items[Y * player.Inventory.Width + X].Desc + "\n\nQuantity:" + player.Inventory.Items[Y * player.Inventory.Width + X].Quantity.ToString();
                    for (int i = 0; i < outputdesc.Length; i++) {
                        if (outputdesc[i] == '\n') lines++;
                    }
                    Gl.glColor4f(.3f, .15f, 0,1);
                    Draw2DText(MouseOnMatrixX+.5f,MouseOnMatrixY-Height*lines-.4f,-4.5f,Width+.2f, Height*lines + .4f,Textures["DescBG"]);
                    //Gl.glColor3f(player.Inventory.Items[Y * player.Inventory.Width + X].NameR, player.Inventory.Items[Y * player.Inventory.Width + X].NameG, player.Inventory.Items[Y * player.Inventory.Width + X].NameB);
                    DrawString(MouseOnMatrixX+.6f,
                        MouseOnMatrixY  - .3f,
                        -5,
                        Glut.GLUT_BITMAP_HELVETICA_18,
                        player.Inventory.Items[Y * player.Inventory.Width + X].Name,
                        player.Inventory.Items[Y * player.Inventory.Width + X].NameR,
                        player.Inventory.Items[Y * player.Inventory.Width + X].NameG,
                        player.Inventory.Items[Y * player.Inventory.Width + X].NameB,true
                        );

                    DrawString(MouseOnMatrixX + .6f,
                        MouseOnMatrixY - .3f-Height,
                        -5,
                        Glut.GLUT_BITMAP_HELVETICA_12,
                        player.Inventory.Items[Y * player.Inventory.Width + X].Desc+"\n\nQuantity:" + player.Inventory.Items[Y * player.Inventory.Width + X].Quantity.ToString(), .8f,.8f,.8f,
                        true
                        );
                }
            }

            if (QuestMenu)
            {
                float TextCursor=0;
                float Height;
                Gl.glColor3f(.3f, .15f, 0);
                Draw2DText(11,2,-3.5f,4,7,Textures["Menu"]);
                Gl.glLoadIdentity();
                DrawStringCent(11, 15, 8.25f, -3.5f, Glut.GLUT_BITMAP_HELVETICA_18, "Quests", 1f, 1f, 1f, true);
                foreach (Quest quest in player.Quests) {
                    if (quest.State != 0&&quest.Finished==false)
                    {
                        int lines = 1;
                        if(quest.Desc[quest.State - 1]!=null)for (int i = 0; i < quest.Desc[quest.State - 1].Length; i++)
                            if (quest.Desc[quest.State - 1][i] == '\n') lines++;
                        Height = Glut.glutBitmapHeight(Glut.GLUT_BITMAP_HELVETICA_18) * ScaleH;
                        DrawStringCent(11, 15, 7.57f-TextCursor, -3.5f, Glut.GLUT_BITMAP_HELVETICA_18, quest.Name, 1f, 1f, .5f, true);
                        TextCursor += Height;
                        Height=lines*Glut.glutBitmapHeight(Glut.GLUT_BITMAP_HELVETICA_12) * ScaleH;
                        DrawStringCent(11, 15, 7.57f-TextCursor, -3.5f, Glut.GLUT_BITMAP_HELVETICA_12, quest.Desc[quest.State - 1], 1f, 1f, .5f, true);
                        TextCursor += Height+Glut.glutBitmapHeight(Glut.GLUT_BITMAP_HELVETICA_18) * ScaleH;

                    }
                }
            }

            if (player.DialogMenu) {
                if (!player.LastNPC.Trade)
                {
                    Gl.glColor3f(.3f, .15f, 0);
                    Draw2DText(5.5f, .5f, -3.5f, 5, 2.5f, Textures["Menu"]);
                    DrawString(5.75f, 2.65f, -3.5f, Glut.GLUT_BITMAP_HELVETICA_12, player.LastNPC.Name+":\n"+player.LastNPC.DialogMenuText, 1f, 1f, 1f, true);
                    Gl.glLoadIdentity();
                }
                else
                {
                    Gl.glColor3f(.3f, .15f, 0);
                    Draw2DText(5.5f, .5f, -3.5f, 5, 4.5f, Textures["Menu"]);
                    for (int i = 0; i < player.LastNPC.Goods.Length;i++)
                    {
                        Gl.glColor3f(1, 1, 1);
                        Draw2DText(5.75f+(i%6)*3f/4f,
                            4f - (i/6)*3f/4f,
                            -3.5f,
                            3f/4f,3f/4f,Textures["ItemBG"]);
                        Draw2DText(5.75f + (i % 6) * 3f / 4f,
                            4f - (i / 6) * 3f / 4f,
                            -3.5f,
                            3f / 4f, 3f / 4f, Textures[player.LastNPC.Goods[i].Texture]);
                        DrawString(5.75f + (i % 6) * 3f / 4f,
                            4f - (i / 6) * 3f / 4f,
                            -3.5f, Glut.GLUT_BITMAP_HELVETICA_12, player.LastNPC.Prices[i].ToString(), 1, 1f, .0f, true);
                    }
                    DrawString(5.75f,.75f,-3.51f,Glut.GLUT_BITMAP_HELVETICA_12,"2.Exit",1,1,1,true);
                }
            }

            //Уведомления изменения квестов
            foreach (Quest quest in player.Quests) {
                if (quest.PopUp)
                {
                    if (InvMenu) {

                        DrawString(6, 9, -3.5f, Glut.GLUT_BITMAP_HELVETICA_18, quest.Name, 1f, 1f, .3f, true);
                        DrawString(6, 8.5f, -3.5f, Glut.GLUT_BITMAP_HELVETICA_12, quest.Desc[quest.State - 1], 1f, 1f, .3f, true);
                    }
                    else { 
                        DrawString(1, 9, -3.5f, Glut.GLUT_BITMAP_HELVETICA_18, quest.Name, 1f, 1f, .3f, true);
                        DrawString(1, 8.5f, -3.5f, Glut.GLUT_BITMAP_HELVETICA_12, quest.Desc[quest.State - 1], 1f, 1f, .3f, true);
                    }
                    break;
                }
            }


            // Отрисовка курсора и хп бара

            Gl.glColor4f(1, 1, 1,1);
            Draw2DText(MouseOnMatrixX, MouseOnMatrixY - .5f, -4, .5f, .5f,Textures["Cursor"]);
            Gl.glColor3f(0, 0, 0);
            Draw2DText(1, 1, -4, 4, .3f, Textures["HPBAR"]);
            Gl.glColor3f(1f, 0, 0);
            Draw2DText(1, 1, -4, 4*((float)player.Hp / (float)player.MaxHp) > 0 ? 4 * ((float)player.Hp / (float)player.MaxHp):0, .3f , Textures["HPBAR"]);



            //OtherShieeeet

            DrawString(.3f,.3f,-4,Glut.GLUT_BITMAP_HELVETICA_10,player.X.ToString()+";"+player.Y.ToString(),1,1,1,true);


            Gl.glFlush();
            AnT.Invalidate();
        }


        private void LoginScreenRenderTimer_Tick(object sender, EventArgs e)
        {
            Gl.glClearDepth(.5);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glClearColor(255, 255, 255, 1);
            if (LoginScreen) {
                Gl.glLoadIdentity();
                CamX += .01f;
                CamY += .01f;

                Gl.glPushMatrix();
                Gl.glTranslatef(-CamX + ScrW / 2, -CamY + ScrH / 2, 0);

                // Отрисовка окружения

                for (int i = (int)CamX - 8; i < CamX + 8; i++)
                {
                    for (int j = (int)CamY - 5; j < CamY + 5; j++)
                    {
                        Gl.glColor3f(1f, 1f, 1f);
                        //Текстуры карты
                        if (i < 0 || j < 0 || i > map.GetUpperBound(0) || j > map.GetUpperBound(1)) continue;
                        if (map[i, j] == 0) Draw2DText(i, j, 0, 1f, 1f, Textures["Grass"]);
                        if (map[i, j] == 1) Draw2DText(i, j, 0, 1f, 1f, Textures["Ice"]);
                        if (map[i, j] == 2) Draw2DText(i, j, 0, 1f, 1f, Textures["Sand"]);
                        if (map[i, j] == 3) Draw2DText(i, j, 0, 1f, 1f, Textures["Ground"]);
                        if (map[i, j] == 4) Draw2DText(i, j, 0, 1f, 1f, Textures["StWall"]);
                        if (map[i, j] == 5) Draw2DText(i, j, -2.1f, 1f, 1f, Textures["StBlack"]);
                        if (map[i, j] == 6) Draw2DText(i, j, 0, 1f, 1f, Textures["StFloor"]);
                        if (map[i, j] == 7) Draw2DText(i, j, 0, 1f, 1f, Textures["WFloor"]);
                        if (map[i, j] == 8) Draw2DText(i, j, -2.1f, 1f, 1f, Textures["StBlack"]);

                        //Текстуры объектов
                        if (obj_map[i, j] == 1) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadD"]);
                        if (obj_map[i, j] == 2) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadL"]);
                        if (obj_map[i, j] == 3) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadLD"]);
                        if (obj_map[i, j] == 4) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadLU"]);
                        if (obj_map[i, j] == 5) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadR"]);
                        if (obj_map[i, j] == 6) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadRD"]);
                        if (obj_map[i, j] == 7) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadRU"]);
                        if (obj_map[i, j] == 8) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadU"]);
                        if (obj_map[i, j] == 9) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadZDL"]);
                        if (obj_map[i, j] == 10) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadZDR"]);
                        if (obj_map[i, j] == 11) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadZUL"]);
                        if (obj_map[i, j] == 12) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadZUR"]);
                        if (obj_map[i, j] == 13) Draw2DText(i, j, 0, 1f, 1f, Textures["RoadW"]);
                    }
                    Thread.Sleep(1);
                }
                Gl.glLoadIdentity();

                MousePosOnAnt(out MouseX, out MouseY, out MouseOnMatrixX, out MouseOnMatrixY);
                Gl.glColor4f(1, 1, 1, 1);
                Draw2DText(MouseOnMatrixX, MouseOnMatrixY - .5f, -4, .5f, .5f, Textures["Cursor"]);
                DrawStringCent(0,ScrW,3,-4,Glut.GLUT_BITMAP_HELVETICA_18,LoginScreenErr,1,0,0,true);

                Gl.glFlush();
                AnT.Invalidate();
            }
            else
            {

                player = new Player(63, 60, textBox1.Text);
                AllPlayers = new List<Player>();
                AllPlayers.Add(player);
                PlayersList[0] = player;


                /*EnemyList[0]= new Ghost(80, 70, RNG);
                EnemyList[1]= new Ghost(82, 69, RNG);
                EnemyList[2]= new Ghost(84, 68, RNG);
                EnemyList[3]= new Ghost(85, 69, RNG);
                EnemyList[4]= new Ghost(84, 68, RNG);
                EnemyList[5]= new Ghost(79, 68, RNG);
                EnemyList[6]= new Ghost(81, 69, RNG);*/

                DropList = new List<Item>();

                //Инициализация NPC

                StreamReader SR = new StreamReader(File.OpenRead("NPCDialogs"));
                
                NPCList = new List<NPC>();

                NPCDelegates _NPCDelegates = new NPCDelegates();

                string Name;
                List<string> dialogs = ReadDialog(out Name, SR);

                NPCList.Add(new DX.NPC(76, 63,Name, new string[] { "IdleD" }, new NPCClickFuncDel(NPCDelegates.AndreClickFunc), new NPCCalcAnimDel(NPCDelegates.AndreCalcAnim), new NPCExMarkDel(NPCDelegates.AndreExMark),dialogs));

                object[,] Goodies = new object[,] { { new Potion(PotionType.Health, 1),25 }, { new Potion(PotionType.Energy, 1), 50 }, { new Potion(PotionType.Energy, 1), 50 }
                , { new Potion(PotionType.Energy, 1), 50 } , { new Potion(PotionType.Energy, 1), 50 } , { new Potion(PotionType.Energy, 1), 50 }
                , { new Potion(PotionType.Energy, 1), 50 } , { new Potion(PotionType.Energy, 1), 50 } , { new Potion(PotionType.Energy, 1), 50 }
                , { new Potion(PotionType.Energy, 1), 50 } , { new Potion(PotionType.Energy, 1), 50 } , { new Potion(PotionType.Energy, 1), 50 }
                , { new Potion(PotionType.Energy, 1), 50 } , { new Potion(PotionType.Energy, 1), 50 } , { new Potion(PotionType.Energy, 1), 50 } };
                dialogs = ReadDialog(out Name, SR);
                NPCList.Add(new DX.NPC(78,60,Name, new string[] { "IdleL" },new NPCClickFuncDel(NPCDelegates.VendorClickFunc),new NPCCalcAnimDel(NPCDelegates.VendorCalcAnim),new NPCExMarkDel(NPCDelegates.VendorExMark),dialogs,Goodies));

                Item.Drop = DropList;

                player.Inventory.Add(new Potion(PotionType.Health, 3));
                player.Inventory.Add(new Gold(int.MaxValue));

                RenderTimer.Start();
                LogicTimer.Start();
                QuestCheckTimer.Start();
                InterpolationTimer.Start();


                Reader=new Task(() => {
                    while (true) {
                        Thread.Sleep(100);
                        if (this.IsDisposed) {
                            Process.GetCurrentProcess().Kill();
                            return;
                        }
                    }

                });

                Reader.Start();

                LoginScreenRenderTimer.Stop();
            }
        }


        private void LogicTimer_Tick(object sender, EventArgs e)
        {
            player.CheckDeath();
            player.CalcAnim();
            player.CalcRotation(MouseX, MouseY, AnT.Width / 2, AnT.Height / 2);

            byte Direction=0;

            if (player.LEFT) Direction = 3;
            if (player.RIGHT) Direction = 1;
            if (player.DOWN) Direction = 4;
            if (player.UP) Direction = 2;
            
            if (!player.LEFT && !player.RIGHT && !player.UP && !player.DOWN)
            {
                if(pingcounter<5) connect.SendXYD(player.X, player.Y, player.Rotation, Direction);
                if (pingcounter >= 200)
                {
                    pingcounter = 0;
                    connect.SendXYD(player.X, player.Y, player.Rotation, Direction);
                }
                pingcounter++;
            }
            else
            {
                pingcounter = 0;
                connect.SendXYD(player.X, player.Y, player.Rotation, Direction);
            }
            for(int i=0;i<EnemyList.Length;i++)
            {
                if (EnemyList[i] == null) continue;

                EnemyList[i].CalcAnim();
                EnemyList[i].DeathCheck();
                //EnemyList[i].WorkFunc(AllPlayers,DropList,RNG);
            }
            textBox3.Text = "";
        }




        private void QuestCheckTimer_Tick(object sender, EventArgs e)
        {
            foreach (Player player in AllPlayers)
            {
                player.CheckQuests();
            }
        }




        public Form1()
        {

            InitializeComponent();
            AnT.InitializeContexts();
            Textures = new Dictionary<string, int>();
        }



        int LoadTexture(string name) //Функция загрузки текстуры
        {
            int texID;
            Gl.glGenTextures(1, out texID);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, texID);

            var bmp = new Bitmap(name);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    int clr = bmp.GetPixel(i, j).ToArgb();
                    byte R = (byte)((clr >> 24) & 255);
                    byte G = (byte)((clr >> 16) & 255);
                    byte B = (byte)((clr >> 8) & 255);
                    byte A = (byte)(clr & 255);

                    byte[] inbuffer = BitConverter.GetBytes(clr);

                    byte wat=inbuffer[0];
                    inbuffer[0] = inbuffer[2];
                    inbuffer[2] = wat;

                    int outbuffer = BitConverter.ToInt32(inbuffer, 0);
                    
                    var clrout = Color.FromArgb(outbuffer);
                    
                    bmp.SetPixel(i, j, clrout);
                }
            }

            var bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            

            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, (int)Gl.GL_RGBA,
                bmp.Width, bmp.Height, 0, Gl.GL_RGBA,
                Gl.GL_UNSIGNED_BYTE, bmpData.Scan0);

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);       // Linear Filtering
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);

            return texID;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            connect = new NetGame(4446, ref PlayersList, ref EnemyList, ref DropList); //Server ip, log port, game port, my port
            RNG = new Random(Environment.TickCount);

            ScrW = 16;
            ScrH = 10;

            ScaleH = ScrH / AnT.Height;
            ScaleW = ScrW / AnT.Width;


            Glut.glutInit();
            // инициализация режима экрана 
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE);

            // установка цвета очистки экрана (RGBA) 
            Gl.glClearColor(255, 255, 255, 1);

            // установка порта вывода 
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);

            // активация проекционной матрицы 
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            // очистка матрицы 
            Gl.glLoadIdentity();

            Gl.glOrtho(0.0, ScrW, 0.0, ScrH, 10, -10);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_ALPHA_TEST);
            Gl.glAlphaFunc(Gl.GL_GREATER, .0f);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LEQUAL);
            Gl.glClearDepth(1.0);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Map MapReader = new Map("map.bmp");

            map = MapReader.MapArray();

            Map ObjMapReader = new Map("obj_map.bmp");

            obj_map = ObjMapReader.MapArray();

            col_map = new byte[ObjMapReader.X, ObjMapReader.Y];

            for (int i = 0; i < ObjMapReader.X; i++) {
                for (int j = 0; j < ObjMapReader.Y; j++) {
                    if (map[i, j] == 3 || map[i, j] == 4 || map[i, j] == 5) col_map[i, j] = 1; else col_map[i, j] = 0;
                }
            }

            //objects
            //Загрузил 12 текстурок, вместо 3, ведь вращать не додумался 
            Textures.Add("RoadR", LoadTexture("Tex//RoadText//R.png"));
            Textures.Add("RoadD", LoadTexture("Tex//RoadText//D.png"));
            Textures.Add("RoadU", LoadTexture("Tex//RoadText//U.png"));
            Textures.Add("RoadL", LoadTexture("Tex//RoadText//L.png"));
            Textures.Add("RoadRU", LoadTexture("Tex//RoadText//RU.png"));
            Textures.Add("RoadRD", LoadTexture("Tex//RoadText//RD.png"));
            Textures.Add("RoadLD", LoadTexture("Tex//RoadText//LD.png"));
            Textures.Add("RoadLU", LoadTexture("Tex//RoadText//LU.png"));
            Textures.Add("RoadZUR", LoadTexture("Tex//RoadText//ZUR.png"));
            Textures.Add("RoadZDR", LoadTexture("Tex//RoadText//ZDR.png"));
            Textures.Add("RoadZDL", LoadTexture("Tex//RoadText//ZDL.png"));
            Textures.Add("RoadZUL", LoadTexture("Tex//RoadText//ZUL.png"));
            Textures.Add("RoadW", LoadTexture("Tex//RoadText//W.png"));


            //textures
            Textures.Add("Ice", LoadTexture("Tex//Ice.png"));
            Textures.Add("Sand", LoadTexture("Tex//Sand.png"));
            Textures.Add("Ground", LoadTexture("Tex//Ground.png"));
            Textures.Add("Grass", LoadTexture("Tex//Grass.png"));
            Textures.Add("StWall", LoadTexture("Tex//StWall.bmp"));
            Textures.Add("StBlack", LoadTexture("Tex//StBlack.bmp"));
            Textures.Add("StFloor", LoadTexture("Tex//StFloor.bmp"));
            Textures.Add("WFloor", LoadTexture("Tex//WFloor.bmp"));

            //misc
            Textures.Add("Cursor", LoadTexture("Tex//сursor.png"));
            Textures.Add("HPBAR", LoadTexture("Tex//Hpbar.png"));
            Textures.Add("Menu", LoadTexture("Tex//MenuScreen.png"));
            Textures.Add("ItemBG", LoadTexture("Tex//ItemBG.png"));
            Textures.Add("DescBG", LoadTexture("Tex//DescBG.png"));
            Textures.Add("ExMark", LoadTexture("Tex//ExMark.png"));

            //Items
            //Potions
            Textures.Add("ItemHP0", LoadTexture("Tex//Items//Potions//HP0.png"));
            Textures.Add("ItemSpeed0", LoadTexture("Tex//Items//Potions//Speed0.png"));
            Textures.Add("ItemEnergy0", LoadTexture("Tex//Items//Potions//Energy0.png"));
            Textures.Add("ItemGold", LoadTexture("Tex//Items//Gold.png"));


            //enemys
            Textures.Add("Ghost", LoadTexture("Tex//Ghost.png"));

            //idle
            Textures.Add("IdleD", LoadTexture("Tex//Idle//Down.png"));
            Textures.Add("IdleU", LoadTexture("Tex//Idle//Up.png"));
            Textures.Add("IdleL", LoadTexture("Tex//Idle//Left.png"));
            Textures.Add("IdleR", LoadTexture("Tex//Idle//Right.png"));
            //uprun
            Textures.Add("HeroUP0", LoadTexture("Tex//UpRun//0.png"));
            Textures.Add("HeroUP1", LoadTexture("Tex//UpRun//1.png"));
            Textures.Add("HeroUP2", LoadTexture("Tex//UpRun//2.png"));
            Textures.Add("HeroUP3", LoadTexture("Tex//UpRun//3.png"));
            Textures.Add("HeroUP4", LoadTexture("Tex//UpRun//4.png"));
            Textures.Add("HeroUP5", LoadTexture("Tex//UpRun//5.png"));
            Textures.Add("HeroUP6", LoadTexture("Tex//UpRun//6.png"));
            Textures.Add("HeroUP7", LoadTexture("Tex//UpRun//7.png"));
            //leftrun
            Textures.Add("HeroLEFT0", LoadTexture("Tex//LeftRun//0.png"));
            Textures.Add("HeroLEFT1", LoadTexture("Tex//LeftRun//1.png"));
            Textures.Add("HeroLEFT2", LoadTexture("Tex//LeftRun//2.png"));
            Textures.Add("HeroLEFT3", LoadTexture("Tex//LeftRun//3.png"));
            Textures.Add("HeroLEFT4", LoadTexture("Tex//LeftRun//4.png"));
            Textures.Add("HeroLEFT5", LoadTexture("Tex//LeftRun//5.png"));
            Textures.Add("HeroLEFT6", LoadTexture("Tex//LeftRun//6.png"));
            Textures.Add("HeroLEFT7", LoadTexture("Tex//LeftRun//7.png"));
            //downrun
            Textures.Add("HeroDOWN0", LoadTexture("Tex//DownRun//0.png"));
            Textures.Add("HeroDOWN1", LoadTexture("Tex//DownRun//1.png"));
            Textures.Add("HeroDOWN2", LoadTexture("Tex//DownRun//2.png"));
            Textures.Add("HeroDOWN3", LoadTexture("Tex//DownRun//3.png"));
            Textures.Add("HeroDOWN4", LoadTexture("Tex//DownRun//4.png"));
            Textures.Add("HeroDOWN5", LoadTexture("Tex//DownRun//5.png"));
            Textures.Add("HeroDOWN6", LoadTexture("Tex//DownRun//6.png"));
            Textures.Add("HeroDOWN7", LoadTexture("Tex//DownRun//7.png"));
            //downrun
            Textures.Add("HeroRIGHT0", LoadTexture("Tex//RightRun//0.png"));
            Textures.Add("HeroRIGHT1", LoadTexture("Tex//RightRun//1.png"));
            Textures.Add("HeroRIGHT2", LoadTexture("Tex//RightRun//2.png"));
            Textures.Add("HeroRIGHT3", LoadTexture("Tex//RightRun//3.png"));
            Textures.Add("HeroRIGHT4", LoadTexture("Tex//RightRun//4.png"));
            Textures.Add("HeroRIGHT5", LoadTexture("Tex//RightRun//5.png"));
            Textures.Add("HeroRIGHT6", LoadTexture("Tex//RightRun//6.png"));
            Textures.Add("HeroRIGHT7", LoadTexture("Tex//RightRun//7.png"));

            //attack
            Textures.Add("HeroAtkR0", LoadTexture("Tex//Attack//right_0.png"));
            Textures.Add("HeroAtkR1", LoadTexture("Tex//Attack//right_1.png"));
            Textures.Add("HeroAtkL0", LoadTexture("Tex//Attack//left_0.png"));
            Textures.Add("HeroAtkL1", LoadTexture("Tex//Attack//left_1.png"));
            Textures.Add("HeroAtkU0", LoadTexture("Tex//Attack//up_0.png"));
            Textures.Add("HeroAtkU1", LoadTexture("Tex//Attack//up_1.png"));
            Textures.Add("HeroAtkD0", LoadTexture("Tex//Attack//down_0.png"));
            Textures.Add("HeroAtkD1", LoadTexture("Tex//Attack//down_1.png"));



            LoginScreenRenderTimer.Start();
            // активация таймера, вызывающего функцию для визуализации 
        }

        private void ControlTimer_Tick(object sender, EventArgs e)
        {
            if (!LoginScreen && player != null)
            {
                bool W = IsKeyDown(Keys.W);
                bool A = IsKeyDown(Keys.A);
                bool S = IsKeyDown(Keys.S);
                bool D = IsKeyDown(Keys.D);
                player.MovedByControl(W, A, S, D, col_map);
            }
        }



        private void AnT_MouseClick(object sender, MouseEventArgs e)
        {
            if (!LoginScreen&&player!=null)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (player.Alive)
                    {
                        connect.SendHit(1, e.X, e.Y, 2, 3);//Удар, Параметры оказались ненужны, пох на них
                        player.AttackAnimFunc();
                    }
                    //player.AttackFunc(GetNearbyEnemies(player.X, player.Y, EnemyList));
                }
                if (e.Button == MouseButtons.Right)
                {
                    if (InvMenu)
                    {
                        int X = (int)((MouseOnMatrixX - 1.5f) * 4f / 3f);
                        int Y = (int)((7f + 3f / 4f - MouseOnMatrixY) * 4f / 3f);
                        if (X >= 0 && X < player.Inventory.Width && Y >= 0 && Y < player.Inventory.Height)
                        {
                            player.Inventory.Activate((int)Y * player.Inventory.Width + (int)X);
                            return;
                        }
                    }
                    if (player.DialogMenu&&player.LastNPC.Trade)
                    {
                        int X = (int)((MouseOnMatrixX - 5.75f) * 4f / 3f);
                        int Y = (int)((4f + 3f / 4f - MouseOnMatrixY) * 4f / 3f);

                        if (X >= 0 && X < 6 && Y >= 0 && Y < player.LastNPC.Goods.Length/6)
                        {
                            if (player.LastNPC.Goods != null)
                                player.LastNPC.Sell(player, X + Y * 6);
                            return;
                        }
                    }
                    MousePosOnMap();
                    foreach (NPC npc in GetNearbyNPCs(player.X, player.Y, NPCList))
                    {
                        if (MouseOnMapX > npc.X - .5f && MouseOnMapX < npc.X + .5f && MouseOnMapY > npc.Y && MouseOnMapY < npc.Y + 1)
                        {
                            if (!player.DialogMenu)
                            {
                                player.LastNPC = npc;
                                player.LastNPC.Trade = false;
                                npc.ClickFunc(player, npc);
                            }
                        }
                    }
                }
            }
        }

        private void AnT_KeyUp(object sender, KeyEventArgs e)
        {
            if (!LoginScreen && player != null)
            {
                if (e.KeyCode == Keys.E)
                {
                    Item Nearest = GetNearestItem();
                    if (Nearest != null)
                    {
                        Item pickup = (Item)Nearest.Clone();
                        DropList.Remove(Nearest);
                        player.Inventory.Add(pickup);
                    }
                }
                if (InvMenu)
                {
                    if (e.KeyCode == Keys.G)
                    {
                        int X = (int)((MouseOnMatrixX - 1.5f) * 4f / 3f);
                        int Y = (int)((7f + 3f / 4f - MouseOnMatrixY) * 4f / 3f);
                        if (X >= 0 && X < player.Inventory.Width && Y >= 0 && Y < player.Inventory.Height) player.Inventory.Items[(int)Y * player.Inventory.Width + (int)X] = null;
                    }

                    if (e.KeyCode == Keys.Y)
                    {
                        int X = (int)((MouseOnMatrixX - 1.5f) * 4f / 3f);
                        int Y = (int)((7f + 3f / 4f - MouseOnMatrixY) * 4f / 3f);
                        if (X >= 0 && X < player.Inventory.Width && Y >= 0 && Y < player.Inventory.Height && player.Inventory.Items[(int)Y * player.Inventory.Width + (int)X] != null) player.Inventory.Drop((int)Y * player.Inventory.Width + (int)X);
                    }
                    if (e.KeyCode == Keys.T)
                    {
                        int X = (int)((MouseOnMatrixX - 1.5f) * 4f / 3f);
                        int Y = (int)((7f + 3f / 4f - MouseOnMatrixY) * 4f / 3f);
                        if (X >= 0 && X < player.Inventory.Width && Y >= 0 && Y < player.Inventory.Height && player.Inventory.Items[(int)Y * player.Inventory.Width + (int)X] != null) player.Inventory.DropOne((int)Y * player.Inventory.Width + (int)X);
                    }
                }
                if (e.KeyCode == Keys.J) QuestMenu = !QuestMenu;
                if (e.KeyCode == Keys.I) {
                    //Console.Write("Инвентарь открылся ууууух!");
                    InvMenu = !InvMenu; }
                if (e.KeyCode == Keys.Escape) { InvMenu = false; QuestMenu = false; }
            }
        }

        void MousePosOnAnt(out int X,out int Y, out float MouseOnMatrixX, out float MouseOnMatrixY) {
            X = Form1.MousePosition.X - this.Location.X - 8;
            Y = AnT.Height-(Form1.MousePosition.Y - this.Location.Y - 32);
            MouseOnMatrixX = (float)X / (float)AnT.Width * (float)ScrW;
            MouseOnMatrixY = (float)Y / (float)AnT.Height * (float)ScrH;
        }

        void MousePosOnMap() {
            MousePosOnAnt(out MouseX, out MouseY, out MouseOnMatrixX, out MouseOnMatrixY);
            MouseOnMapX = player.X + MouseOnMatrixX - ScrW/2;
            MouseOnMapY = player.Y + MouseOnMatrixY - ScrH / 2;
        }


        void Draw2DText(float X, float Y, float Z, float XW, float YW, int Text)
        {
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Text);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(0, 1); Gl.glVertex3f(X, Y, Z);
            Gl.glTexCoord2f(0, 0); Gl.glVertex3f(X, Y + YW, Z);
            Gl.glTexCoord2f(1, 0); Gl.glVertex3f(X + XW, Y + YW, Z);
            Gl.glTexCoord2f(1, 1); Gl.glVertex3f(X + XW, Y, Z);
            Gl.glEnd();
        }

        void Draw2DText(float X, float Y, float Z, float XW, float YW)
        {
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2f(0, 1); Gl.glVertex3f(X, Y,Z);
            Gl.glTexCoord2f(0, 0); Gl.glVertex3f(X, Y + YW,Z);
            Gl.glTexCoord2f(1, 0); Gl.glVertex3f(X + XW, Y + YW,Z);
            Gl.glTexCoord2f(1, 1); Gl.glVertex3f(X + XW, Y,Z);
            Gl.glEnd();
        }

        void Draw2DTextCent(float X, float Y, float Z, float XW, float YW, int Text)
        {
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Text);
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord3f(0, 1,Z); Gl.glVertex3f(X - XW / 2, Y - YW / 2,Z);
            Gl.glTexCoord3f(0, 0,Z); Gl.glVertex3f(X - XW / 2, Y + YW / 2,Z);
            Gl.glTexCoord3f(1, 0,Z); Gl.glVertex3f(X + XW / 2, Y + YW / 2,Z);
            Gl.glTexCoord3f(1, 1,Z); Gl.glVertex3f(X + XW / 2, Y - YW / 2,Z);
            Gl.glEnd();
        }

        void Draw2DTextCent(float X, float Y, float Z, float XW, float YW)
        {
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord3f(0, 1, Z); Gl.glVertex3f(X - XW / 2, Y - YW / 2, Z);
            Gl.glTexCoord3f(0, 0, Z); Gl.glVertex3f(X - XW / 2, Y + YW / 2, Z);
            Gl.glTexCoord3f(1, 0, Z); Gl.glVertex3f(X + XW / 2, Y + YW / 2, Z);
            Gl.glTexCoord3f(1, 1, Z); Gl.glVertex3f(X + XW / 2, Y - YW / 2, Z);
            Gl.glEnd();
        }

        void DrawString(float X, float Y, float Z, IntPtr font, string text, float R, float G, float B, bool shadow) {
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            
            if (shadow) {
                Gl.glColor3f(0, 0, 0);
                Gl.glRasterPos3f(X+.02f, Y+.02f, Z);
                Glut.glutBitmapString(font, text);
            }
            Gl.glColor3f(R, G, B);
            Gl.glRasterPos3f(X, Y, Z);
            Glut.glutBitmapString(font, text);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
        }

        void DrawStringCent(float X1,float X2, float Y, float Z, IntPtr font, string text, float R, float G, float B, bool shadow)
        {
            float X=X1+(X2-X1-Glut.glutBitmapLength(font,text)*ScrW/AnT.Width)/2;
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            if (shadow)
            {
                Gl.glColor3f(0, 0, 0);
                Gl.glRasterPos3f(X + .02f, Y + .02f, Z);
                Glut.glutBitmapString(font, text);
            }
            Gl.glColor3f(R, G, B);
            Gl.glRasterPos3f(X, Y, Z);
            Glut.glutBitmapString(font, text);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
        }

        void DrawStringFromRight(float X, float Y, float Z, IntPtr font, string text, float R, float G, float B, bool shadow)
        {
            X -= Glut.glutBitmapLength(font, text) * ScrW / AnT.Width;
            Gl.glDisable(Gl.GL_TEXTURE_2D);
            if (shadow)
            {
                Gl.glColor3f(0, 0, 0);
                Gl.glRasterPos3f(X + .04f, Y + .02f, Z);
                Glut.glutBitmapString(font, text);
            }
            Gl.glColor3f(R, G, B);
            Gl.glRasterPos3f(X, Y, Z);
            Glut.glutBitmapString(font, text);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
        }


        float OnScreenYtoZ(float Y) {
            return -.15f-ScrH/6 + (Y - player.Y + ScrH / 2)/6;
        }


        Enemy[] GetNearbyEnemies(float playerX, float playerY, Enemy[] Enemies) {
            Enemy[] output = new Enemy[Enemies.Length];
            int index=0;

            for (int i=0;i<Enemies.Length;i++) {
                if (Enemies[i] == null) continue;
                if (Math.Sqrt((playerX - Enemies[i].X) * (playerX - Enemies[i].X) + (playerY - Enemies[i].Y) * (playerY - Enemies[i].Y)) < 11f) {
                    output[index]=Enemies[i];
                    index++;
                }
            }


            return output;
        }

        List<Item> GetNearbyItems(float playerX, float playerY,  List<Item> Items)
        {
            List<Item> output = new List<Item>();
            foreach (Item enemy in Items)
            {
                if (Math.Sqrt((playerX - enemy.X) * (playerX - enemy.X) + (playerY - enemy.Y) * (playerY - enemy.Y)) < 11f)
                {
                    output.Add(enemy);
                }
            }
            return output;
        }

        private void button1_Click(object sender, EventArgs e)
        {
                if (textBox1.Text == "Offline")
                {
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    button1.Enabled = false;
                    textBox1.Visible = false;
                    textBox2.Visible = false;
                    button1.Visible = false;
                    LoginScreen = false;
                }

                IPAddress useless;
                string ip;
                string char_name;
                if (textBox1.Text != "") char_name = textBox1.Text;
                else
                {
                    LoginScreenErr = "Empty Name"; return;
                }

                if (textBox2.Text != "") ip = textBox2.Text;
                else
                {
                    LoginScreenErr = "Empty IP"; return;
                }

                if (!IPAddress.TryParse(ip, out useless))
                {
                    LoginScreenErr = "Wrong IP Format"; return;
                };

                connect.Set_Addr(ip, 5001, 5000);
                if (!connect.IsConnectable())
                {
                    LoginScreenErr = "Server unreachable";
                    return;
                }

                if (connect.LogAndGame(char_name, 200, 5))
                {
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    button1.Enabled = false;
                    textBox1.Visible = false;
                    textBox2.Visible = false;
                    button1.Visible = false;
                    LoginScreen = false;
                }
                else 
                {
                    LoginScreenErr = connect.denied_info;
                    return;
                }
                

               
        }

        List<NPC> GetNearbyNPCs(float playerX, float playerY, List<NPC> NPCs)
        {
            List<NPC> output = new List<NPC>();
            foreach (NPC enemy in NPCs)
            {
                if (Math.Sqrt((playerX - enemy.X) * (playerX - enemy.X) + (playerY - enemy.Y) * (playerY - enemy.Y)) < 11f)
                {
                    output.Add(enemy);
                }
            }
            return output;
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            connect.End_Session();
        }

        private void InterpolationTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 1; i < PlayersList.Length; i++) {
                if (PlayersList[i] == null) continue;
                PlayersList[i].Interpolation();
            }
            for (int i = 0; i < EnemyList.Length; i++) {
                if (EnemyList[i] == null) continue;
                EnemyList[i].Interpolation();
            }
        }
        

        Item GetNearestItem() {
            if (DropList.Count == 0) return null;
            Item Nearest = DropList.First();
            double ThisDist;
            double NearestDist = Math.Sqrt((Nearest.X - player.X) * (Nearest.X - player.X) + (Nearest.Y - player.Y) * (Nearest.Y - player.Y));
            foreach (Item drop in DropList)
            {
                ThisDist = Math.Sqrt((drop.X - player.X) * (drop.X - player.X) + (drop.Y - player.Y) * (drop.Y - player.Y));
                if (ThisDist < NearestDist)
                {
                    NearestDist = ThisDist;
                    Nearest = drop;
                }
            }
            if (NearestDist < 1.5f) return Nearest;
            else return null;
            
        }

        private void Form1_Resize_1(object sender, EventArgs e)
        {
            if (!LoginScreen)
            {
                RenderTimer.Stop();
                Thread.Sleep(20);
                ResizeGlScene();
                RenderTimer.Start();
            }
            else {
                LoginScreenRenderTimer.Stop();
                Thread.Sleep(20);
                ResizeGlScene();
                LoginScreenRenderTimer.Start();
            }
        }

        public static bool IsKeyDown(Keys key)
        {
            return (GetKeyState(Convert.ToInt16(key)) & 0X80) == 0X80;
        }

        private void AnT_MouseEnter(object sender, EventArgs e)
        {
            Cursor.Hide();
        }

        private void AnT_MouseLeave(object sender, EventArgs e)
        {
            Cursor.Show();
        }


        List<string> ReadDialog(out string Name,StreamReader SR) {
            Name = SR.ReadLine();
            List<string> dialogs = new List<string>();
            string sum = "";
            string thisstring = "";
            thisstring = SR.ReadLine();
            while (thisstring != "<end>")
            {
                if (thisstring == "<next>")
                {
                    dialogs.Add(sum);
                    sum = "";
                }
                else
                    sum += thisstring + "\n";
                thisstring = SR.ReadLine();
            }

            return dialogs;
        }


        void ResizeGlScene()
        {
            // Предупредим деление на нуль;
            if (AnT.Height == 0)
            {
                AnT.Height = 1;
            }
            // инициализация режима экрана 
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE);

            // установка цвета очистки экрана (RGBA) 
            Gl.glClearColor(255, 255, 255, 1);

            // установка порта вывода 
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);

            // активация проекционной матрицы 
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            // очистка матрицы 
            Gl.glLoadIdentity();

            Gl.glOrtho(0.0, ScrW, 0.0, ScrH, 10, -10);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();
        }



        [DllImport("user32.dll")]
        public extern static Int16 GetKeyState(Int16 nVirtKey);
    }
    
}
