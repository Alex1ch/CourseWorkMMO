using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace DX
{
    class Cons
    {
        //Взять данные с пакета (только где в дате только имя)
        public static string GetData(byte[] data)
        {
            string name = "";
            byte[] nameB = new byte[data.Length - 1];
            Array.Copy(data, 1, nameB, 0, data.Length - 1);
            name = Encoding.UTF8.GetString(nameB);
            return name;
        }

        //Изьятия имени из пакетов XYD (с 10го байта по конец)
        public static string GetName17b(byte[] pack)
        {
            string name = "";
            byte[] nameB = new byte[pack.Length - 18];
            Array.Copy(pack, 17, nameB, 0, pack.Length - 18);
            name = Encoding.UTF8.GetString(nameB);
            return name;
        }

        //Заносим в координаты в переменную перса клиента
        public static void Parse_MyXYD(Player player, byte[] msg)
        {
            byte[] x = new byte[4];
            byte[] y = new byte[4];
            byte[] r = new byte[4];
            byte[] hp = new byte[4];
            byte[] d = new byte[1];

            Array.Copy(msg, 0, x, 0, 4);
            Array.Copy(msg, 4, y, 0, 4);
            Array.Copy(msg, 8, r, 0, 4);
            Array.Copy(msg, 12, d, 0, 1);
            Array.Copy(msg, 13, hp, 0, 4);

            // вытаскиваю имя перса из пакета
            string name = GetName17b(msg);

            player.X = BitConverter.ToSingle(x, 0);
            player.Y = BitConverter.ToSingle(y, 0);
            player.Rotation = BitConverter.ToSingle(r, 0);
            if (d[0] == 0)
            {
                player.LEFT = false;
                player.UP = false;
                player.RIGHT = false;
                player.DOWN = false;
            }
            if (d[0] == 1)
            {
                player.RIGHT = true;
                player.DOWN = false;
                player.LEFT = false;
                player.UP = false;
            }
            if (d[0] == 2)
            {
                player.RIGHT = false;
                player.DOWN = false;
                player.LEFT = false;
                player.UP = true;
            }
            if (d[0] == 3)
            {
                player.RIGHT = false;
                player.DOWN = false;
                player.LEFT = true;
                player.UP = false;
            }
            if (d[0] == 4)
            {
                player.RIGHT = false;
                player.DOWN = true;
                player.LEFT = false;
                player.UP = false;
            }
            player.Hp = BitConverter.ToSingle(hp, 0);


        }

        //Заносим в координаты и имена других персов которые нам передали
        public static void Parse_CharsXYD(ref Dictionary<string, Player> Characters, byte[] msg)
        {
            byte[] x = new byte[4];
            byte[] y = new byte[4];
            byte[] r = new byte[4];
            byte[] hp = new byte[4];
            byte[] d = new byte[1];

            Array.Copy(msg, 0, x, 0, 4);
            Array.Copy(msg, 4, y, 0, 4);
            Array.Copy(msg, 8, r, 0, 4);
            Array.Copy(msg, 12, d, 0, 1);
            Array.Copy(msg, 13, hp, 0, 4);

            // вытаскиваю имя перса из пакета
            string name = GetName17b(msg);

            if (Characters.ContainsKey(name))
            {
                Player player = new Player(BitConverter.ToSingle(x, 0), BitConverter.ToSingle(y, 0), name);
                Characters[name] = player;
                player.Rotation = BitConverter.ToSingle(r, 0);
                if (d[0] == 0)
                {
                    player.LEFT = false;
                    player.UP = false;
                    player.RIGHT = false;
                    player.DOWN = false;
                }
                if (d[0] == 1)
                {
                    player.RIGHT = true;
                    player.DOWN = false;
                    player.LEFT = false;
                    player.UP = false;
                }
                if (d[0] == 2)
                {
                    player.RIGHT = false;
                    player.DOWN = false;
                    player.LEFT = false;
                    player.UP = true;
                }
                if (d[0] == 3)
                {
                    player.RIGHT = false;
                    player.DOWN = false;
                    player.LEFT = true;
                    player.UP = false;
                }
                if (d[0] == 4)
                {
                    player.RIGHT = false;
                    player.DOWN = true;
                    player.LEFT = false;
                    player.UP = false;
                }
                player.Hp = BitConverter.ToSingle(hp, 0);

            }
            else
            {
                Characters.Add(name, new Player(BitConverter.ToSingle(x, 0), BitConverter.ToSingle(y, 0), name));
                Player player = Characters[name];
                player.Rotation = BitConverter.ToSingle(r, 0);
                if (d[0] == 0)
                {
                    player.LEFT = false;
                    player.UP = false;
                    player.RIGHT = false;
                    player.DOWN = false;
                }
                if (d[0] == 1)
                {
                    player.RIGHT = true;
                    player.DOWN = false;
                    player.LEFT = false;
                    player.UP = false;
                }
                if (d[0] == 2)
                {
                    player.RIGHT = false;
                    player.DOWN = false;
                    player.LEFT = false;
                    player.UP = true;
                }
                if (d[0] == 3)
                {
                    player.RIGHT = false;
                    player.DOWN = false;
                    player.LEFT = true;
                    player.UP = false;
                }
                if (d[0] == 4)
                {
                    player.RIGHT = false;
                    player.DOWN = true;
                    player.LEFT = false;
                    player.UP = false;
                }
                player.Hp = BitConverter.ToSingle(hp, 0);
            }
        
        
        }


        public static void Parse_CharsXYD(ref Dictionary<string, Player> Characters, string name, byte[] msg)
    {
        byte[] x = new byte[4];
        byte[] y = new byte[4];
        byte[] r = new byte[4];
        byte[] hp = new byte[4];
        byte[] d = new byte[1];

        Array.Copy(msg, 0, x, 0, 4);
        Array.Copy(msg, 4, y, 0, 4);
        Array.Copy(msg, 8, r, 0, 4);
        Array.Copy(msg, 12, d, 0, 1);
        Array.Copy(msg, 13, hp, 0, 4);
            

        if (Characters.ContainsKey(name))
        {
            Player player = new Player(BitConverter.ToSingle(x, 0), BitConverter.ToSingle(y, 0),name);
            Characters[name] = player;
            player.Rotation = BitConverter.ToSingle(r, 0);
            if (d[0] == 0)
            {
                player.LEFT = false;
                player.UP = false;
                player.RIGHT = false;
                player.DOWN = false;
            }
            if (d[0] == 1)
            {
                player.RIGHT = true;
                player.DOWN = false;
                player.LEFT = false;
                player.UP = false;
            }
            if (d[0] == 2)
            {
                player.RIGHT = false;
                player.DOWN = false;
                player.LEFT = false;
                player.UP = true;
            }
            if (d[0] == 3)
            {
                player.RIGHT = false;
                player.DOWN = false;
                player.LEFT = true;
                player.UP = false;
            }
            if (d[0] == 4)
            {
                player.RIGHT = false;
                player.DOWN = true;
                player.LEFT = false;
                player.UP = false;
            }
            player.Hp = BitConverter.ToSingle(hp, 0);

        }
        else
        {
            Characters.Add(name, new Player(BitConverter.ToSingle(x, 0), BitConverter.ToSingle(y, 0), name));
            Player player = Characters[name];
            player.Rotation = BitConverter.ToSingle(r, 0);
            if (d[0] == 0)
            {
                player.LEFT = false;
                player.UP = false;
                player.RIGHT = false;
                player.DOWN = false;
            }
            if (d[0] == 1)
            {
                player.RIGHT = true;
                player.DOWN = false;
                player.LEFT = false;
                player.UP = false;
            }
            if (d[0] == 2)
            {
                player.RIGHT = false;
                player.DOWN = false;
                player.LEFT = false;
                player.UP = true;
            }
            if (d[0] == 3)
            {
                player.RIGHT = false;
                player.DOWN = false;
                player.LEFT = true;
                player.UP = false;
            }
            if (d[0] == 4)
            {
                player.RIGHT = false;
                player.DOWN = true;
                player.LEFT = false;
                player.UP = false;
            }
            player.Hp = BitConverter.ToSingle(hp, 0);
        }
    }
    }


    class NetGame
    {
        Dictionary<string,Player> Players;
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
        static bool session_ON = false;       //Индикатор того запущена сессия с сервером или нет
        static bool available_port = false;   //Для обозначения того, что нашелся ли свободный порт для клиента
        static bool IsExit = false; //Происходит отключение или нет

        //Для счета отправленных пакетов
        static public int sentPackets = 0;
        public int SentPackets { get { return sentPackets; } }

        //Отправлено байт
        private int sentBytes = 0;
        public int SentBytes { get { return sentBytes; } }

        //Сколько раз неудалось подключиться
        static public int cfl = 0;
        public int CFL { get { return cfl; } }

        //Полученное от сервера сообщение
        static private string rec_msg = "";
        public string Recv_Msg
        {
            get
            {
                string copy = rec_msg;
                rec_msg = "null";
                return copy;
            }
        }

        //Cтатус выхода
        static private string exit_status = "";
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
        static private string char_name = "";

        //Координаты персонажа клиента (0-нету, 1-право, 2-вверх, 3- влево, 4-вниз)
        static float[] Player_XYD = new float[] { 55.5F, 55.5F, 4F };
        public float[] Get_XYD
        {
            get
            {
                return Player_XYD;
            }
        }

        //Координаты других персов
        static Dictionary<string, float[]> Characters = new Dictionary<string, float[]>(15);
        public Dictionary<string, float[]> Characters_XYD
        {
            get
            {
                if (Characters.Keys.Contains(char_name)) Characters.Remove(char_name);
                return Characters;
            }
        }

        //Координаты мобов
        static Dictionary<string, float[]> Mobs = new Dictionary<string, float[]>(15);
        public Dictionary<string, float[]> Mobs_XYD
        {
            get
            {
                return Mobs;
            }
        }

        //IP, Port
        public NetGame(string IP, int Log_Port, int Game_Port, int My_Port)
        {
            log_port = Log_Port;
            game_port = Game_Port;
            my_port = My_Port;
            ip = IP;
            //Ищем свободный порт
            if (Parse_Port()) available_port = true;
            Console.WriteLine("Connected to port: " + my_port);
            //Адреса логин и гейм серва
            login_addr = new IPEndPoint(IPAddress.Parse(ip), log_port);
            game_addr = new IPEndPoint(IPAddress.Parse(ip), game_port);
        }

        //Только с использованием Set_Addr, Расчитано на много попыток подключения (под разными имеми и аддресами)
        public NetGame(int My_Port,ref Dictionary<string,Player> dict)
        {
            Players = dict;
            my_port = My_Port;
        }

        //Поменять аддрес для подключения
        public void Set_Addr(string IP, int Log_Port, int Game_Port)
        {
            log_port = Log_Port;
            game_port = Game_Port;
            ip = IP;

            //Ищем свободный порт
            if (Parse_Port()) available_port = true;
            Console.WriteLine("Connected to port: " + my_port);
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
                    my_port = i;
                    my_addr = new IPEndPoint(IPAddress.Parse("127.0.0.1"), my_port);
                    available_port = true;
                    return true;
                }
                catch (Exception e)
                { //Console.WriteLine(e.Message); 
                }
            }
            return false;
        }

        //Проверяет на возможность подключения
        //Interval - время, между отправкой и получением пакета (1000 = 1сек)
        public bool IsConnectable(int Sleep_Interval = 200, int Attempts = 5)
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
                Console.WriteLine("Connection: " + e.Message);
                //listen = false;
                Send(100, "", my_addr);
                //thrListen.Abort();
                thrListen.Join(1);

                return false;
            }

            for (int i = 0; i < Attempts; i++)
            {
                Console.WriteLine("      Connection check: " + i);

                //Посылаем сообщение проверки сервера и ждем ответ
                Send(1, "", login_addr);
                Thread.Sleep(Sleep_Interval);

                //Читаем полученное сообщение и освобождаем ресурсы
                if (Recv_Msg == "ALIVE")
                {
                    listen = false;
                    thrListen.Abort();
                    return true;
                }
            }
            listen = false;
            thrListen.Abort();
            return false;
        }

        //Устанавливаем сессию с заданным именем
        public bool LogNstart_Session(string Char_Name, int Sleep_Interval = 200, int Attempts = 5)
        {
            if (available_port == false) { Console.WriteLine("UDP client not set"); return false; }

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
            catch (Exception e) { Console.WriteLine("LogNstart: " + e.Message); }

            //Пытаемся начать сессию указанное количество раз
            for (int i = 0; i < Attempts; i++)
            {
                Console.WriteLine("      Attempt: " + i);
                //Отсылаем сообщение начала сессии
                Send(2, Char_Name, login_addr); // BEGIN
                Thread.Sleep(Sleep_Interval);
                string copy = Recv_Msg;//--------<?costyl'?
                switch (copy)
                {
                    case "STARTED":
                        IsExit = false;
                        session_ON = true;
                        return true;
                    case "DENIED": return false;
                }
            }
            return false;
        }

        // Для остановкм слушания
        public bool End_Session(int Sleep_Interval = 200, int Attempts = 5)
        {
            IsExit = true;
            //Пытаемся сказать клиенту что я кончаю
            for (int i = 0; i < Attempts; i++)
            {
                Console.WriteLine("      Attempt: " + i);
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
                    if (thrListen != null) thrListen.Abort();
                    Console.WriteLine("Closed successfully");
                    return true;
                }
            }
            //Если не получили ответа то и фиг с ним, всеравно кончать надо
            listen = false;
            session_ON = false;
            if (thrListen != null) thrListen.Abort();
            if (client_udp != null) client_udp.Close();
            Console.WriteLine("Closed without server notification");
            return false;
        }

        // Функция извлекающая пришедшие сообщения работающая в отдельном потоке.
        void Listener()
        {
            byte[] msg = new byte[512];
            try
            {
                while (listen)
                {
                    ipendpoint = null;
                    msg = client_udp.Receive(ref ipendpoint);

                    //if(msg.Length>10)Console.WriteLine("Received:  " + Cons.GetName10b(msg));

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
                                Console.WriteLine("Started session, my ID: " + Cons.GetData(msg));
                                Send(5, "OK" + Cons.GetData(msg), login_addr);
                                rec_msg = "STARTED";
                                break;
                            //Отказ сервером начать сессию
                            case 253:
                                Console.WriteLine("Can't start session: " + Cons.GetData(msg));
                                rec_msg = "DENIED";
                                break;
                            //Уведомление сервером о том, что сессия успешно завершена 
                            case 252:
                                Console.WriteLine("Exit message: " + Cons.GetData(msg));
                                if (IsExit) exit_status = "SUCCESSFUL"; //Чтобы сессию не завершить по ошибке какой-либо сделал проверку
                                break;
                            //Сервер присылает координаты всех чаров и их имен
                            case 251://CH_XYD
                                // вытаскиваю имя перса из пакета
                                string name = Cons.GetName17b(msg);

                                //Если в пакете есть имя перса клиента
                                if (name == char_name) ;// Cons.Parse_MyXYD(Players[name], msg);
                                else { Cons.Parse_CharsXYD(ref Players, msg); }
                                break;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Если произошла ошибка перезапуск
                if (listen) Listener();
            }
        }

        public void Send(byte command, byte[] msg, IPEndPoint addr)
        {
            byte[] s_packet = new byte[1 + msg.Length];
            for (int i = 1; i < msg.Length; i++) s_packet[i] = msg[i];
            s_packet[0] = command;

            try
            {
                client_udp.Send(s_packet, s_packet.Length, addr);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Возникло исключение в отправке по аддресу: " + addr + ex.ToString() + "\n " + ex.Message);
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
                Console.WriteLine("Возникло исключение в отправке по аддресу: " + addr + ex.ToString() + "\n " + ex.Message);
            }
        }

        //Отправить серверу координаты персонажа 
        public bool SetXYD(float X, float Y, float Rotation,float HP , byte Direction)
        {
            //Если сессия не установлена, то отправить сообщение такого рода нельзя
            if (!session_ON)
            {
                Console.WriteLine("Cant send XYD, try to start session before");
                return false;
            }

            byte[] Xb = BitConverter.GetBytes(X);//кодируем флоат в массив байтов 
            byte[] Yb = BitConverter.GetBytes(Y);
            byte[] Rotb = BitConverter.GetBytes(Rotation);
            byte[] HPb = BitConverter.GetBytes(HP);
            byte[] Char_Name = Encoding.UTF8.GetBytes(char_name);//20+(n/2)×4 байт
            int name_len = Char_Name.Length;
            byte[] output = new byte[4 + 4 + 4 + 1 + 4 + name_len];//склеиваем все в один массив 

            Array.Copy(Xb, output, 4);
            Array.Copy(Yb, 0, output, 4, 4);
            Array.Copy(Rotb, 0, output, 8, 4);
            Array.Copy(HPb, 0, output, 13, 4);
            output[12] = Direction;//(0-нету, 1-право, 2-вверх, 3- влево, 4-вниз 
            Array.Copy(Char_Name, 0, output, 17, name_len);

            Send(4, output, game_addr);
            return true;
        }
    }

}
