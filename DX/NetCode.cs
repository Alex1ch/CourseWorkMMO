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
        //Выводит символы с одного индекса по другой
        public static string StrInd(string Str, int b1, int b2)
        {
            string OutStr = "";
            for (int i = 0; i < b2 - b1; i++)
            {
                OutStr += Str[b1 + i];
            }
            return OutStr;
        }
        //Команда пакета
        public static string cmd(byte[] packet)
        {
            string cmd = Encoding.UTF8.GetString(packet);
            int b1 = cmd.IndexOf("command=");
            int b2 = cmd.IndexOf(";data=");

            if (b1 < 0 || b2 < 0) return "null";
            else
            {
                cmd = StrInd(cmd, b1 + 8, b2);
                return cmd;
            }
        }
        //Данные пакета
        public static string data(byte[] packet)
        {
            string data = Encoding.UTF8.GetString(packet); ;
            int b1 = data.IndexOf("data=");
            int b2 = data.Length;

            if (b1 < 0) return "null";
            else
            {
                data = StrInd(data, b1 + 5, b2);
                return data;
            }
        }
        //Номер пакета
        public static string npack(byte[] packet)
        {
            string packn = Encoding.UTF8.GetString(packet);
            int b1 = packn.IndexOf("packet=");
            int b2 = packn.IndexOf(";command=");

            if (b1 < 0 || b2 < 0) return "null";
            else
            {
                packn = StrInd(packn, b1 + 7, b2);
                return packn;
            }
        }

        //Заносим в координаты в переменную перса клиента
        public static void Parse_Player_XYD(ref string[] Player_XYD, string char_name, string msg)
        {
            int x = msg.IndexOf("¶X");
            int y = msg.IndexOf("¶Y");
            int d = msg.IndexOf("¶D");

            Player_XYD[0] = StrInd(msg, x + 2, y);
            Player_XYD[1] = StrInd(msg, y + 2, d);
            Player_XYD[2] = StrInd(msg, d + 2, msg.Length);
        }
        //Заносим в координаты и имена других персов которые нам передали
        public static void Parse_Char_XYD(ref Dictionary<string, string[]> Characters, string msg)
        {
            int x = msg.IndexOf("¶X");
            int y = msg.IndexOf("¶Y");
            int d = msg.IndexOf("¶D");

            if (Characters.ContainsKey(StrInd(msg, 0, x))) Characters[StrInd(msg, 0, x)] = new string[] { StrInd(msg, x + 2, y), StrInd(msg, y + 2, d), StrInd(msg, d + 2, msg.Length) };
            else Characters.Add(StrInd(msg, 0, x), new string[] { StrInd(msg, x + 2, y), StrInd(msg, y + 2, d), StrInd(msg, d + 2, msg.Length) });
        }
        //Заносим в координаты и имена мобов которые нам передали
        public static void Parse_Mob_XYD(ref Dictionary<string, string[]> Mobs, string msg)
        {
            int x = msg.IndexOf("¶X");
            int y = msg.IndexOf("¶Y");
            int d = msg.IndexOf("¶D");

            Mobs.Add(StrInd(msg, 0, x), new string[] { StrInd(msg, x + 2, y), StrInd(msg, y + 2, d), StrInd(msg, d + 2, msg.Length) });
        }
    }

    class NetGame
    {
        static UdpClient client_udp;
        static IPEndPoint login_addr;
        static IPEndPoint game_addr;
        static IPEndPoint ipendpoint;
        static int log_port;
        static int game_port;
        static int my_port;
        static string ip;

        static Thread thRec;                    //поток для прослушки
        static bool rec = false;         //true чтобы прервать вайл в прослушке   
        static bool session_activity = false; //Индикатор того запущена сессия или нет
        static bool connected = false;//Если нашли порт который можно прослушивать

        //Для нумерации принятых и отправленных пакетов
        static public int nrec_packets = 0;
        static public int nsend_packets = 0;

        //Сколько раз неудалось подключиться
        static public int conn_failed_times = 0;
        public int Conn_Failure { get { return conn_failed_times; } }

        //Отправлено байт
        private int numberOfSentBytes = 0;
        public int Sent_Bytes { get { return numberOfSentBytes; } }

        static private string rec_msg = "";  //сообщения без отдельной переменной сюда записываются
        static private string copy = "";     //копия для свойства
        public string Recv_Msg
        {
            get
            {
                copy = rec_msg;
                rec_msg = "null";
                return copy;
            }
        }

        static private string exit_status = "";  //статус выхода
        static private string exit_copy = "";     //копия для свойства
        public string Exit_Status
        {
            get
            {
                exit_copy = exit_status;
                exit_status = "null";
                return exit_copy;
            }
        }

        static private string char_name = "";

        //Координаты персонажа клиента
        static string[] Player_XYD = new string[] { "55.0", "55.0", "S" };
        public string[] Get_XYD
        {
            get
            {
                return Player_XYD;
            }
        }
        //Координаты других персов
        static Dictionary<string, string[]> Characters = new Dictionary<string, string[]>(15);
        public Dictionary<string, string[]> Characters_XYD
        {
            get
            {
                //IsReading = true;
                if (Characters.Keys.Contains(char_name)) Characters.Remove(char_name);
                return Characters;
            }
        }
        //Координаты мобов
        static Dictionary<string, string[]> Mobs = new Dictionary<string, string[]>(15);
        public Dictionary<string, string[]> Mobs_XYD
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
            if (Parse_Port()) connected = true;
            //Адреса логин и гейм серва
            login_addr = new IPEndPoint(IPAddress.Parse(ip), log_port);
            game_addr = new IPEndPoint(IPAddress.Parse(ip), game_port);
        }

        public bool Connect(string IP, int Log_Port, int Game_Port)
        {
            log_port = Log_Port;
            game_port = Game_Port;
            ip = IP;
            login_addr = new IPEndPoint(IPAddress.Parse(ip), log_port);
            game_addr = new IPEndPoint(IPAddress.Parse(ip), game_port);

            return true;
        }

        //Ищет свободный порт для UDP клиента. Если найден и установлен, то true 
        public bool Parse_Port()
        {
            for (int i = my_port; i < 60000; i++)
            {
                try
                {
                    client_udp = new UdpClient(i);
                    Console.WriteLine("Connected to port: " + i);
                    return true;
                }
                catch (Exception e)
                { Console.WriteLine(e.Message); }
            }
            return false;
        }

        //Проверяет на возможность подключения
        //Interval - время, между отправкой и получением пакета (1000 = 1сек)
        public bool IsConnectable(int Sleep_Interval = 200, int Attempts = 5)
        {
            if (connected == false) { Console.WriteLine("UDP client not set"); return false; }
            //Если поток на прослушку не начат, то начинаем
            if (thRec == null)
            {
                rec = true;
                thRec = new Thread(new ThreadStart(Receive));
            }
            if (!thRec.IsAlive) thRec.Start();

            for (int i = 0; i < Attempts; i++)
            {
                Console.WriteLine("      Attempt: " + i);
                //Посылаем сообщение о проверки сервера и ждем время до ответа
                Send("CHECK", "", login_addr);
                Thread.Sleep(Sleep_Interval);
                //Читаем сообщение полученное, если нужный ответ то останавливаем поток
                if (Recv_Msg == "ALIVE") return true;
            }
            return false;
        }

        //Устанавливаем сессию с заданным именем
        public bool LogNstart_Session(string Char_Name, int Sleep_Interval = 200, int Attempts = 5)
        {
            if (connected == false) { Console.WriteLine("UDP client not set"); return false; }

            char_name = Char_Name;

            //Если поток прослушки выключен, запускаем его
            if (thRec == null)
            {
                rec = true;
                thRec = new Thread(new ThreadStart(Receive));
            }
            if (!thRec.IsAlive) thRec.Start();

            //Пытаемся начать сессию указанное количество раз
            for (int i = 0; i < Attempts; i++)
            {
                Console.WriteLine("      Attempt: " + i);
                //Отсылаем сообщение начала сессии
                Send("BEGIN", Char_Name, login_addr);
                Thread.Sleep(Sleep_Interval);
                string copy = Recv_Msg;//--------<?costyl'?
                switch (copy)
                {
                    case "STARTED":
                        session_activity = true;
                        return true;
                    case "DENIED": return false;
                }
            }
            rec = false;
            thRec.Abort();
            return false;
        }

        // Для остановкм слушания
        public bool End_Session(int Sleep_Interval = 200, int Attempts = 5)
        {
            //Пытаемся сказать клиенту что я кончаю
            for (int i = 0; i < Attempts; i++)
            {
                Console.WriteLine("      Attempt: " + i);
                Send("END", char_name, game_addr);
                Thread.Sleep(Sleep_Interval);
                if (Exit_Status == "SUCCESSFUL")
                {
                    // Останавливаем цикл в дополнительном потоке
                    rec = false;
                    session_activity = false;

                    // Принудительно закрываем объект класса UdpClient 
                    if (client_udp != null) client_udp.Close();
                    // Для корректного завершения дополнительного потока подключаем его к основному потоку.
                    if (thRec != null) thRec.Abort();
                    Console.WriteLine("Closed successfully");
                    return true;
                }
            }
            //Если не получили ответа то и фиг с ним, всеравно кончать надо
            rec = false;
            session_activity = false;
            if (thRec != null) thRec.Abort();
            if (client_udp != null) client_udp.Close();
            Console.WriteLine("Closed without server notification");
            return false;
        }

        //Отправка сообщения
        static public void Send(string command, string msg, IPEndPoint addr)
        {
            int pointer = 0;

            byte[] s_num = new byte[6];
            byte[] s_comm = new byte[6];
            byte[] s_data = new byte[500];
            byte[] s_packet = new byte[512];

            s_num = Encoding.UTF8.GetBytes("packet=" + nsend_packets.ToString() + ";");
            s_comm = Encoding.UTF8.GetBytes("command=" + command + ";");
            s_data = Encoding.UTF8.GetBytes("data=" + msg);

            for (int i = 0; i < s_num.Length; i++, pointer++) s_packet[pointer] = s_num[i];
            for (int i = 0; i < s_comm.Length; i++, pointer++) s_packet[pointer] = s_comm[i];
            for (int i = 0; i < s_data.Length; i++, pointer++) s_packet[pointer] = s_data[i];

            try
            {
                client_udp.Send(s_packet, pointer, addr); //можно указать s_packet.Length, чтобы отправлять весь пакет (с пустотами)
            }
            catch (Exception ex)
            {
                Console.WriteLine("Возникло исключение в отправке на: " + addr + ex.ToString() + "\n  " + ex.Message);
            }
        }

        static public void Send(byte command, byte[] msg, IPEndPoint addr)
        {
            byte[] s_packet = new byte[1+msg.Length];
            
            for (int i = 1; i < msg.Length; i++) s_packet[i] = msg[i];

            try
            {
                client_udp.Send(s_packet, s_packet.Length, addr); //можно указать s_packet.Length, чтобы отправлять весь пакет (с пустотами)
            }
            catch (Exception ex)
            {
                Console.WriteLine("Возникло исключение в отправке на: " + addr + ex.ToString() + "\n  " + ex.Message);
            }
        }

        // Функция извлекающая пришедшие сообщения работающая в отдельном потоке.
        static void Receive()
        {
            byte[] msg = new byte[] { };

            try
            {
                while (rec == true)
                {
                    ipendpoint = null;
                    if (rec) { msg = client_udp.Receive(ref ipendpoint); }
                    //Console.WriteLine(Cons.cmd(msg));
                    //Реакция если сообщение получено только с гейм или лог сервера
                    if ((login_addr.ToString() == ipendpoint.ToString()) && (login_addr.Port == ipendpoint.Port)
                        || (game_addr.ToString() == ipendpoint.ToString()) && (game_addr.Port == ipendpoint.Port))
                    {
                        //"Сортировка" принятых сообщений
                        switch (Cons.cmd(msg))
                        {
                            case "ALIVE":  //Ответ сервера когда он проверяется на коннект
                                rec_msg = "ALIVE";
                                break;
                            case "ALIVE_CHECK": //Сервер проверяет на коннект ----
                                Console.WriteLine("PROVERKA SERVEROM NA ONLINE");
                                Send("ALIVE", "", game_addr);
                                break;
                            case "STARTED": //Сервер сообщает о начале сессии
                                Console.WriteLine("Started session, my ID: " + Cons.data(msg));
                                rec_msg = "STARTED";
                                break;
                            case "DENIED"://Отклонение сервером в начале сессии
                                Console.WriteLine("Can't start session: " + Cons.data(msg));
                                rec_msg = "DENIED";
                                break;
                            case "SUCCESSFUL"://Уведомление о том что успешно завершена сессия
                                exit_status = "SUCCESSFUL";
                                break;
                            case "CH_XYD"://Сервер присылает координаты всех чаров и их имен
                                if (Cons.data(msg).Contains(char_name)) Cons.Parse_Player_XYD(ref Player_XYD, char_name, Cons.data(msg));
                                Cons.Parse_Char_XYD(ref Characters, Cons.data(msg));
                                break;
                            case "MOB_XYD"://Сервер присылает координаты всех мобов
                                Cons.Parse_Mob_XYD(ref Mobs, Cons.data(msg));
                                break;
                        }
                    }
                }
            }
            catch 
            {
                Receive();
                // Console.WriteLine("Receiver: {no connection}: "+ ex.Message);
            }
            finally { conn_failed_times++; }
        }


        //Отправить серверу координаты персонажа
        public bool SetXYD(float X, float Y, byte Direction)
        {
            if (session_activity == true)
            {
                byte[] Xb = BitConverter.GetBytes(X);//кодируем флоат в массив байтов
                byte[] Yb = BitConverter.GetBytes(Y);

                byte[] output = new byte[4 + 4 + 1];//склеиваем все в один массив
                Array.Copy(Xb, output, 4);
                Array.Copy(Yb, 0, output, 4, 4);
                output[8] = Direction;//(0-нету, 1-право, 2-вверх, 3- влево, 4-вниз
                Send(28, output,game_addr);
                return true;
            }
            else { Console.WriteLine("Cant send XYD, try to start session before"); return false; }
        }
    }

}
