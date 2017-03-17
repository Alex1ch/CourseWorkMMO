using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace DX
{
    class Cons
    {
        /// <summary>
        /// Найти индекс персонажа с заданным именем в массиве, -1 если нет такого
        /// </summary>
        /// <param name="players"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int HaveEntry(Player[] players, string name)
        {
            for (int i = 1; i < players.Length; i++)
            {
                if (players[i] != null)
                    if (players[i].Name == name) return i;
            }
            return -1;
        }

        /// <summary>
        /// Найти пустое место в массиве персонажей, -1 если нет таких
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        public static int GetNull(Player[] players)
        {
            for (int i = 0; i < players.Length; i++)
                if (players[i] == null) return i;

            return -1;
        }

        //Взять данные с пакета (только где в дате только имя)
        public static string GetData(byte[] data)
        {
            string name = "";
            byte[] nameB = new byte[data.Length - 1];
            Array.Copy(data, 1, nameB, 0, data.Length - 1);
            name = Encoding.UTF8.GetString(nameB);
            return name;
        }

        //Изьятия имени из пакетов XYD (с 18го байта по конец)
        public static string GetName18b(byte[] pack)
        {
            byte[] nameB = new byte[pack.Length - 18];
            Array.Copy(pack, 18, nameB, 0, pack.Length - 18);
            return Encoding.UTF8.GetString(nameB);
        }

        //Заносим в координаты в переменную перса клиента
        public static void Parse_MyXYD(Player player, byte[] msg)
        {
            if (player == null) return;

            byte[] x = new byte[4];
            byte[] y = new byte[4];
            byte[] r = new byte[4];
            byte[] hp = new byte[4];
            byte d = 0;

            Array.Copy(msg, 1, x, 0, 4);
            Array.Copy(msg, 5, y, 0, 4);
            Array.Copy(msg, 9, r, 0, 4);
            d = msg[13];
            Array.Copy(msg, 14, hp, 0, 4);

            float X = BitConverter.ToSingle(x, 0);
            float Y = BitConverter.ToSingle(y, 0);
            player.Hp = BitConverter.ToSingle(hp, 0);

            player.SX = X;
            player.SY = Y;

            player.Rotation = BitConverter.ToSingle(r, 0);
            if (d == 0)
            {
                player.LEFT = false;
                player.UP = false;
                player.RIGHT = false;
                player.DOWN = false;
            }
            if (d == 1)
            {
                player.RIGHT = true;
                player.DOWN = false;
                player.LEFT = false;
                player.UP = false;
            }
            if (d == 2)
            {
                player.RIGHT = false;
                player.DOWN = false;
                player.LEFT = false;
                player.UP = true;
            }
            if (d == 3)
            {
                player.RIGHT = false;
                player.DOWN = false;
                player.LEFT = true;
                player.UP = false;
            }
            if (d == 4)
            {
                player.RIGHT = false;
                player.DOWN = true;
                player.LEFT = false;
                player.UP = false;
            }


        }

        //Заносим в координаты и имена других персов которые нам передали
        public static void Parse_CharsXYD(ref Player[] players, byte[] msg)
        {
            byte[] x = new byte[4];
            byte[] y = new byte[4];
            byte[] r = new byte[4];
            byte[] hp = new byte[4];
            byte d = 0;

            Array.Copy(msg, 1, x, 0, 4);
            Array.Copy(msg, 5, y, 0, 4);
            Array.Copy(msg, 9, r, 0, 4);
            d = msg[13];
            Array.Copy(msg, 14, hp, 0, 4);

            // вытаскиваю имя перса из пакета
            string name = GetName18b(msg);

            int index = HaveEntry(players, name);

            if (index == -1)
            {
                Player player = new Player(BitConverter.ToSingle(x, 0), BitConverter.ToSingle(y, 0), name);
                players[GetNull(players)] = player;
                player.Rotation = BitConverter.ToSingle(r, 0);

                if (d == 0)
                {
                    player.LEFT = false;
                    player.UP = false;
                    player.RIGHT = false;
                    player.DOWN = false;
                }
                if (d == 1)
                {
                    player.LEFT = false;
                    player.UP = false;
                    player.DOWN = false;
                    player.RIGHT = true;
                }
                if (d == 2)
                {
                    player.LEFT = false;
                    player.RIGHT = false;
                    player.DOWN = false;
                    player.UP = true;
                }
                if (d == 3)
                {
                    player.UP = false;
                    player.RIGHT = false;
                    player.DOWN = false;
                    player.LEFT = true;
                }
                if (d == 4)
                {
                    player.LEFT = false;
                    player.UP = false;
                    player.RIGHT = false;
                    player.DOWN = true;
                }
                player.Hp = BitConverter.ToSingle(hp, 0);

            }
            else
            {
                Player player = players[index];
                player.SX = BitConverter.ToSingle(x, 0);
                player.SY = BitConverter.ToSingle(y, 0);
                player.Rotation = BitConverter.ToSingle(r, 0);

                if (d == 0)
                {
                    player.LEFT = false;
                    player.UP = false;
                    player.RIGHT = false;
                    player.DOWN = false;
                }
                if (d == 1)
                {
                    player.LEFT = false;
                    player.UP = false;
                    player.DOWN = false;
                    player.RIGHT = true;
                }
                if (d == 2)
                {
                    player.LEFT = false;
                    player.RIGHT = false;
                    player.DOWN = false;
                    player.UP = true;
                }
                if (d == 3)
                {
                    player.UP = false;
                    player.RIGHT = false;
                    player.DOWN = false;
                    player.LEFT = true;
                }
                if (d == 4)
                {
                    player.LEFT = false;
                    player.UP = false;
                    player.RIGHT = false;
                    player.DOWN = true;
                }
                player.Hp = BitConverter.ToSingle(hp, 0);
            }


        }

        public static bool DeletePlayer(ref Player[] players, string name)
        {
            int index = HaveEntry(players, name);

            if (index == -1) return false;
            else
            {
                players[index] = null;
                return true;
            }
        }

        //248		MOB_XYD		| 1-4[x] 5-8[y] 8-11[rotation] 12[d] 13-16[hp] 17-20[type] 21-end[id]
        public static void Parse_MobsXYD(ref Enemy[] enemies, byte[] msg)
        {
            byte[] x = new byte[4];
            byte[] y = new byte[4];
            byte[] r = new byte[4];
            byte d = 0;
            byte[] hp = new byte[4];
            byte[] type = new byte[4];
            byte[] id = new byte[4];

            Array.Copy(msg, 1, x, 0, 4);
            Array.Copy(msg, 5, y, 0, 4);
            Array.Copy(msg, 9, r, 0, 4);
            d = msg[13];
            Array.Copy(msg, 14, hp, 0, 4);
            Array.Copy(msg, 18, type, 0, 4);
            Array.Copy(msg, 22, id, 0, 4);

            int ID = BitConverter.ToInt32(id, 0);
            Random rnd = new Random();

            switch (BitConverter.ToInt32(type, 0))
            {
                case 1:
                    if (enemies[ID] == null)
                    {
                        enemies[ID] = new Ghost(BitConverter.ToSingle(x, 0), BitConverter.ToSingle(y, 0), rnd);
                        enemies[ID].HP = BitConverter.ToSingle(hp, 0);
                    }
                    else
                    {
                        enemies[ID].SX = BitConverter.ToSingle(x, 0);
                        enemies[ID].SY = BitConverter.ToSingle(y, 0);
                        enemies[ID].HP = BitConverter.ToSingle(hp, 0);
                    }
                    break;
            }
        }

        //247		DROP_SET 	| 1-4[x] 5-8[y] 9-12[item] 13-16[quantity]
        public static bool Parse_DropItems(ref List<Item> Items, byte[] msg)
        {
            if (Items == null) { Console.WriteLine("Empty items list"); return false; }

            byte[] xb = new byte[4];
            byte[] yb = new byte[4];
            byte[] itemb = new byte[4];
            byte[] quantityb = new byte[4];
            byte[] idb = new byte[4];

            Array.Copy(msg, 1, xb, 0, 4);
            Array.Copy(msg, 5, yb, 0, 4);
            Array.Copy(msg, 9, itemb, 0, 4);
            Array.Copy(msg, 13, quantityb, 0, 4);
            Array.Copy(msg, 17, idb, 0, 4);

            int item = BitConverter.ToInt32(itemb, 0);
            int exemplar = BitConverter.ToInt32(idb, 0);

            if (Items != null)
            {
                //Console.WriteLine("Items count: " + Items.Count);
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i] != null)
                        if (Items[i].Id_exemplar == exemplar) return false;
                }
            }

            switch (item)
            {
                case 0:
                    //Console.WriteLine("Health");
                    Items.Add(new Potion(PotionType.Health, BitConverter.ToInt32(quantityb, 0), BitConverter.ToSingle(xb, 0), BitConverter.ToSingle(yb, 0), BitConverter.ToInt32(idb, 0)));
                    return true;
                case 1:
                    //Console.WriteLine("Speed");
                    Items.Add(new Potion(PotionType.Speed, BitConverter.ToInt32(quantityb, 0), BitConverter.ToSingle(xb, 0), BitConverter.ToSingle(yb, 0), BitConverter.ToInt32(idb, 0)));
                    return true;
                case 2:
                    //Console.WriteLine("Energy");
                    Items.Add(new Potion(PotionType.Energy, BitConverter.ToInt32(quantityb, 0), BitConverter.ToSingle(xb, 0), BitConverter.ToSingle(yb, 0), BitConverter.ToInt32(idb, 0)));
                    return true;
                case 69:
                    //Console.WriteLine("Gold");
                    Items.Add(new Gold(BitConverter.ToSingle(xb, 0), BitConverter.ToSingle(yb, 0), BitConverter.ToInt32(quantityb, 0), BitConverter.ToInt32(idb, 0)));
                    return true;
                default:
                    return false;
            }
        }
    }

    class NetGame
    {
        Player[] Players;
        Enemy[] Mobs;
        List<Item> Items;

        static UdpClient client_udp;
        static IPEndPoint login_addr;
        static IPEndPoint game_addr;
        static IPEndPoint my_addr;
        static IPEndPoint ipendpoint;
        static int log_port;
        static int game_port;
        static int my_port;
        static string ip;

        static Thread thrListen;              //поток для прослушки
        static bool listen = false;           //true чтобы прервать while в прослушке   
        public bool session_ON = false;       //Индикатор того запущена сессия с сервером или нет
        static bool available_port = false;   //Для обозначения того, что нашелся ли свободный порт для клиента
        static bool IsExit = false; //Происходит отключение или нет

        //Полученное от сервера сообщение
        static private string rec_msg = "null";
        public string Recv_Msg
        {
            get
            {
                string copy = rec_msg;
                rec_msg = "null";
                return copy;
            }
        }

        byte[] start_info = new byte[256];
        public string denied_info = "Error while logging";

        //Cтатус выхода
        static private string exit_status = "null";
        public string Exit_Status
        {
            get
            {
                string exit_copy = exit_status;
                exit_status = "null";
                return exit_copy;
            }
        }

        //Имя персонажа данного клиента
        static private string char_name = "null";

        Dictionary<byte, string> Permissions = new Dictionary<byte, string>(255);

        //Только с использованием Set_Addr, Расчитано на много попыток подключения (под разными имеми и аддресами)
        public NetGame(int My_Port, ref Player[] dict, ref Enemy[] enemyd, ref List<Item> items)
        {
            Players = dict;
            Mobs = enemyd;
            Items = items;

            my_port = My_Port;

            for (int i = 0; i < 255; i++) Permissions.Add((byte)i, null);

            //Ищем свободный порт
            if (Parse_Port()) available_port = true;
            Console.WriteLine("Connected to port: " + my_port);
        }

        //Поменять аддрес для подключения
        public void Set_Addr(string IP, int Log_Port, int Game_Port)
        {
            log_port = Log_Port;
            game_port = Game_Port;
            ip = IP;

            //Адреса логин и гейм серва
            login_addr = new IPEndPoint(IPAddress.Parse(ip), log_port);
            game_addr = new IPEndPoint(IPAddress.Parse(ip), game_port);
        }

        //Ищет свободный порт для UDP клиента. Если найден и установлен, то true 
        private bool Parse_Port()
        {
            for (int i = my_port; i < 60000; i++)
            {
                try
                {
                    client_udp = new UdpClient(i);
                    uint IOC_IN = 0x80000000;
                    uint IOC_VENDOR = 0x18000000;
                    uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                    client_udp.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);

                    my_port = i;
                    my_addr = new IPEndPoint(IPAddress.Parse("127.0.0.1"), my_port);
                    available_port = true;
                    return true;
                }
                catch (Exception e) { }
            }
            return false;
        }

        //Проверяет на возможность подключения
        //Interval - время, между отправкой и получением пакета (1000 = 1сек)
        public bool IsConnectable(int Sleep_Interval = 100, int Attempts = 5)
        {
            //Если не установлен ЮДП
            if (available_port == false) { Console.WriteLine("UDP client not set"); return false; }

            //Если поток не инициализован
            if (thrListen == null) thrListen = new Thread(new ThreadStart(Listener));

            //Попытки запустить поток, если он прерывается в данный момент
            try
            {
                listen = true;
                thrListen = new Thread((Listener));
                if (!thrListen.IsAlive) thrListen.Start();
            }
            catch (Exception e)
            {
                listen = false;
                Console.WriteLine("[Connection check] Catch: " + e.Message);
                Send(100, "", my_addr);
                //thrListen.Abort();
                thrListen.Join(1);
                return false;
            }

            for (int i = 0; i < Attempts; i++)
            {
                Console.WriteLine("[Connection check] Attempt: " + i);

                //Посылаем сообщение проверки сервера и ждем ответ
                Send(1, "", login_addr);
                Thread.Sleep(Sleep_Interval);

                //Читаем полученное сообщение и освобождаем ресурсы
                if (Recv_Msg == "ALIVE")
                {
                    listen = false;
                    thrListen.Join(1);
                    return true;
                }
            }
            listen = false;
            thrListen.Join(1);
            return false;
        }

        //Устанавливаем сессию с заданным именем
        public bool LogAndGame(string Char_Name, int Sleep_Interval = 100, int Attempts = 30)
        {
            if (available_port == false) { Console.WriteLine("[LogAndGame]: UDP client not set"); return false; }

            char_name = Char_Name;

            //Если поток прослушки выключен, запускаем его
            if (thrListen == null)
            {
                thrListen = new Thread(new ThreadStart(Listener));
            }
            try
            {
                listen = true;
                if (!thrListen.IsAlive) thrListen.Start();
            }
            catch (Exception e) { Console.WriteLine("[LogAndGame] Catch:" + e.Message); }

            //Пытаемся начать сессию указанное количество раз
            for (int i = 0; i < Attempts; i++)
            {
                Console.WriteLine("[LogAndGame] Attempt: " + i);
                //Отсылаем сообщение начала сессии
                Send(2, Char_Name, login_addr); // BEGIN
                Thread.Sleep(Sleep_Interval);

                if (session_ON)
                {
                    IsExit = false;
                    Console.WriteLine("Started session, my ID: " + Cons.GetData(start_info));
                    return true;
                }
            }
            return false;
        }


        // Для остановкм слушания
        public bool End_Session(int Sleep_Interval = 200, int Attempts = 5)
        {
            IsExit = true;

            //Пытаемся сказать серверу, что я кончаю
            for (int i = 0; i < Attempts; i++)
            {
                Console.WriteLine("[End Session] Attempt: " + i);
                Send(3, char_name, game_addr);//END
                Thread.Sleep(Sleep_Interval);

                if (Exit_Status == "SUCCESSFUL")
                {
                    // Останавливаем цикл в дополнительном потоке
                    listen = false;
                    session_ON = false;

                    // Принудительно закрываем объект класса UdpClient 
                    if (client_udp != null) client_udp.Close();

                    // Для корректного завершения дополнительного потока подключаем его к основному потоку.
                    if (thrListen != null)
                    {
                        //thrListen.Abort();
                        thrListen.Join(1);
                    }
                    Console.WriteLine("[End Session]: Performed successfully");
                    return true;
                }
            }
            //Если не получили ответа то и фиг с ним, всеравно кончать надо
            listen = false;
            session_ON = false;
            if (client_udp != null) client_udp.Close();
            if (thrListen != null)
            {
                //thrListen.Abort();
                thrListen.Join(1);
            }
            Console.WriteLine("[End Session]: Client closed without server notification");
            return false;
        }

        // Функция извлекающая пришедшие сообщения работающая в отдельном потоке.
        void Listener()
        {
            byte[] msg = new byte[256];
            try
            {
                while (listen)
                {
                    ipendpoint = null;
                    msg = client_udp.Receive(ref ipendpoint);
                    //Console.WriteLine("Received:  " + Cons.GetData(msg));

                    //Реакция если сообщение получено только с гейм или лог сервера
                    if ((login_addr.ToString() == ipendpoint.ToString()) && (login_addr.Port == ipendpoint.Port)
                        || (game_addr.ToString() == ipendpoint.ToString()) && (game_addr.Port == ipendpoint.Port))
                    {
                        switch (msg[0])
                        {
                            //Ответ сервера когда на IsConnectable
                            case 255:
                                rec_msg = "ALIVE";
                                break;

                            //Сервер сообщает о старте Сессии   
                            case 254:
                                if (session_ON == true) continue;
                                Console.WriteLine("Received:  " + Cons.GetData(msg));
                                session_ON = true;
                                Send(5, "OK", login_addr);
                                start_info = msg;
                                break;

                            //Отказ сервером начать сессию
                            case 253:
                                if (session_ON == true) break;
                                session_ON = false;
                                Console.WriteLine("Can't start session: " + Cons.GetData(msg));
                                denied_info = Cons.GetData(msg);
                                break;

                            //Уведомление сервером о том, что сессия успешно завершена 
                            case 252:
                                Console.WriteLine("Exit message: " + Cons.GetData(msg));
                                //Проверка, чтобы не завершить сессию по какой-либо ошибке
                                if (IsExit) exit_status = "SUCCESSFUL";
                                break;

                            //Сервер присылает координаты всех чаров и их имен
                            case 251://CH_XYD
                                if (!session_ON) break;
                                // вытаскиваю имя перса из пакета
                                string name = Cons.GetName18b(msg);
                                //Если в пакете есть имя перса клиента
                                if (name != char_name)
                                    Cons.Parse_CharsXYD(ref Players, msg);
                                break;

                            //PLAYER_INFO	| 1[lvl] 2-5[maxHP] 6-end[charname]	
                            case 250:
                                break;

                            //249		PLAYER_END	| 1-end[charname]
                            case 249:
                                if (!session_ON) break;
                                string data = Cons.GetData(msg);
                                Console.WriteLine("Player END: " + data);
                                Send(6, "OK" + data, game_addr);
                                Cons.DeletePlayer(ref Players, data);
                                break;

                            //248		MOB_XYD		| 1-4[x] 5-8[y] 9-12[rotation] 13[d] 14-17[hp] 18-21[type] 22-end[id]
                            case 248:
                                if (!session_ON) break;
                                Cons.Parse_MobsXYD(ref Mobs, msg);
                                break;

                            //247		DROP_SET 	| 1-4[x] 5-8[y] 9-12[item] 13-16[quantity] 17-end[id_ex]
                            case 247:
                                if (!session_ON) break;
                                string data8 = Cons.GetData(msg);
                                Cons.Parse_DropItems(ref Items, msg);
                                break;

                            //246 	SEND_MYXYD	| 1-4[x] 5-8[y] 9-12[rotation] 13[d] 14-17[hp] 17-end[char_name]
                            case 246:
                                if (!session_ON) break;
                                Cons.Parse_MyXYD(Players[0], msg);
                                break;

                            //245		PICK_UP_OK	| 1-end["OK"+same_data]
                            case 245:
                                Console.WriteLine("245::" + Cons.GetData(msg));
                                Permissions[245] = Cons.GetData(msg);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Listener exception: " + ex.ToString());
                //Если произошла ошибка перезапуск
                if (listen) Listener();
            }
        }

        public void Send(byte command, byte[] msg, IPEndPoint addr)
        {
            byte[] s_packet = new byte[1 + msg.Length];
            for (int i = 1; i < msg.Length + 1; i++) s_packet[i] = msg[i - 1];
            s_packet[0] = command;

            try
            {
                client_udp.Send(s_packet, s_packet.Length, addr);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send(byte) catch: " + addr + ex.ToString() + "\n " + ex.Message);
            }
        }
        public void Send(byte command, string message, IPEndPoint addr)
        {
            byte[] msg = Encoding.UTF8.GetBytes(message);

            byte[] s_packet = new byte[1 + msg.Length];
            for (int i = 1; i < msg.Length + 1; i++) s_packet[i] = msg[i - 1];
            s_packet[0] = command;

            try
            {
                client_udp.Send(s_packet, s_packet.Length, addr);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Send(string) catch: " + addr + ex.ToString() + "\n " + ex.Message);
            }
        }

        public async Task<bool> SendS(byte command, byte[] msg, IPEndPoint addr)
        {
            byte[] s_packet = new byte[1 + msg.Length];
            for (int i = 1; i < msg.Length + 1; i++) s_packet[i] = msg[i - 1];
            s_packet[0] = command;

            byte callback_cmd;
            switch (command) { case 8: callback_cmd = 245; break; default: return false; }

            try
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("SENDS:[" + i + "] To:" + addr + " Message:[" + Encoding.UTF8.GetString(msg) + "]");

                    client_udp.Send(s_packet, s_packet.Length, addr);
                    await Task.Delay(50);

                    Console.WriteLine(" Received:[" + Permissions[callback_cmd] + "]");

                    if (Permissions[callback_cmd] == "OK" + Encoding.UTF8.GetString(msg)) { Permissions[callback_cmd] = null; return true; }
                    else if (Permissions[callback_cmd] == "NO" + Encoding.UTF8.GetString(msg)) { Permissions[callback_cmd] = null; return false; }
                }
                Permissions[callback_cmd] = null;
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Game serv: SendS exception: To::" + addr + "::" + ex.ToString());
                return false;
            }
        }

        Stopwatch time = Stopwatch.StartNew();
        //Отправить серверу координаты персонажа 
        public bool SendXYD(float X, float Y, float Rotation, byte Direction)
        {
            //Если сессия не установлена, то отправить сообщение такого рода нельзя
            if (!session_ON)
            {
                Console.WriteLine("Cant send XYD, try to start session before");
                return false;
            }

            byte[] Xb = BitConverter.GetBytes(X);
            byte[] Yb = BitConverter.GetBytes(Y);
            byte[] Rotb = BitConverter.GetBytes(Rotation);
            //byte[] HPb = BitConverter.GetBytes(HP);
            byte[] Char_Name = Encoding.UTF8.GetBytes(char_name);//20+(n/2)×4 байт
            int name_len = Char_Name.Length;
            byte[] output = new byte[4 + 4 + 4 + 1 + name_len];//склеиваем все в один массив 

            Array.Copy(Xb, output, 4);
            Array.Copy(Yb, 0, output, 4, 4);
            Array.Copy(Rotb, 0, output, 8, 4);
            output[12] = Direction;//(0-нету, 1-право, 2-вверх, 3- влево, 4-вниз 
            //Array.Copy(HPb, 0, output, 13, 4);
            Array.Copy(Char_Name, 0, output, 13, name_len);

            Send(4, output, game_addr);
            return true;
        }

        //HIT  | 0[type of attack] 1-4[x] 5-8[y] 9[npc or player] 10-13[id or name]
        public bool SendHit(byte type_hit, float x, float y, byte who_hit, int id)
        {
            //Если сессия не установлена, то отправить сообщение такого рода нельзя
            if (!session_ON)
            {
                Console.WriteLine("Cant send HIT, try to start session before");
                return false;
            }

            byte[] xB = BitConverter.GetBytes(x);
            byte[] yB = BitConverter.GetBytes(y);
            byte[] idB = BitConverter.GetBytes(id);

            byte[] output = new byte[1 + 4 + 4 + 1 + 4];

            output[0] = type_hit;
            Array.Copy(xB, 0, output, 1, 4);
            Array.Copy(yB, 0, output, 5, 4);
            output[9] = who_hit;
            Array.Copy(yB, 0, output, 10, 4);

            Send(7, output, game_addr);
            return true;
        }

        //8		DROP_TAKEN  | 1-4[id] 5-8[ex_id]
        public async Task<bool> Take_Drop(int ID, int Ex_ID)
        {
            //Если сессия не установлена, то отправить сообщение такого рода нельзя
            if (!session_ON)
            {
                Console.WriteLine("Cant Take Drop, try to start session before");
                return false;
            }

            byte[] output = new byte[4 + 4];
            byte[] id = BitConverter.GetBytes(ID);
            byte[] ex = BitConverter.GetBytes(Ex_ID);

            Array.Copy(id, 0, output, 0, 4);
            Array.Copy(ex, 0, output, 4, 4);

            return await SendS(8, output, game_addr);
        }
    }

}
