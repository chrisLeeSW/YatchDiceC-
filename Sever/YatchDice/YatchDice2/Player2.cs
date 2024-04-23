using ServerCore;
using ServerCore.DateType;
using System;
using System.Net;
using System.Text;
using static ServerCore.Session;

namespace YatchDice2
{
    class ClientSession : PacketSession
    {
        
        Random random = new Random();
        public bool newPrint = false;
        public bool startGame = false;

        public bool[] hold = new bool[5];
        public bool test = false;
        public override void OnConnected(EndPoint endPoint)
        {
            //Console.WriteLine($"Playqer1 OnConnected  : {endPoint}");

            //lock (blocking)
            //{
            //    name = "Player2";
            //    var sendBuffer = Wrtie();
            //    if (sendBuffer != null)
            //        Send(sendBuffer);
            //}

        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            Read(buffer);
            Console.WriteLine($"chat : {log}");
            if (gameReady)
            {
                for (int i = 0; i < diceRandomValue.Length; i++)
                {
                    Console.WriteLine($"dice Value[{i}] : {diceRandomValue[i].value}");
                    Console.WriteLine($"dice Holding[{i}] : {diceRandomValue[i].isHolding}");
                }
            }
            if (roomId != 0 && selectRoomType == SelectRoomType.None && !gameReady)
            {
                newPrint = false; 
                Console.WriteLine("JoinRoom Fail");
            }
            else if (roomId != 0 && selectRoomType == SelectRoomType.JoinRoom && !gameReady)
            {
                newPrint = true;
                Console.WriteLine("JoinRoom Succes");
            }
            if (gameReady && isStart && isWaiting)
            {
                Console.WriteLine("1~5까지 번호를 누르면 번호를 제외한 나머지 숫자의 결과 값이 나옵니다.");
                string str = Console.ReadLine();

                if (str.Contains('1'))
                    hold[0] = true;
                if (str.Contains('2'))
                    hold[1] = true;
                if (str.Contains('3'))
                    hold[2] = true;
                if (str.Contains('4'))
                    hold[3] = true;
                if (str.Contains('5'))
                    hold[4] = true;

                isWaiting = false;
                for (int i = 0; i < 5; ++i)
                {
                    diceRandomValue[i].isHolding = hold[i];
                }
                var sendBuffer = Write();
                Send(sendBuffer);
            }
            if (gameReady && !isStart && turnEnd)
            {
                Console.WriteLine("Turn END");
                turnEnd = false;
                isStart = false;
                isWaiting = false;
                var sendBuffer = Write();
                Send(sendBuffer);
            }
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"OnSend : Player2");
        }

        public void MakeRoom()
        {
            lock (blocking)
            {
                // isHost = true;
                selectRoomType = SelectRoomType.MakeRoom;
                roomId = random.Next(1, 1001);
                log = $"{name} is Make Room";
                var sendBuffer = Write();
                if (sendBuffer != null)
                    Send(sendBuffer);
            }
            newPrint = true;
        }
        public void PrintListGameRoom()
        {

            lock (blocking)
            {
                //getRoomList = true;
                selectRoomType = SelectRoomType.GetRoomList;
                var sendBuffer = Write();
                if (sendBuffer != null)
                    Send(sendBuffer);
            }
        }
        public void JoinRoom(int id)
        {
            lock (blocking)
            {
                // joinRoom = true;
                selectRoomType = SelectRoomType.JoinRoom;
                 roomId = id;
                var sendBuffer = Write();
                if (sendBuffer != null)
                    Send(sendBuffer);
            }
        }
        public void GameReady()
        {
            lock (blocking)
            {
                selectRoomType = SelectRoomType.None;
                gameReady = true;
                var sendBuffer = Write();
                if (sendBuffer != null)
                    Send(sendBuffer);
            }
        }
        public void GetOutRoom()
        {
            lock (blocking)
            {
                isGetOutRoom = true;
                selectRoomType = SelectRoomType.None;
                var sendBuffer = Write();
                if (sendBuffer != null)
                    Send(sendBuffer);
            }
        }


    }
    internal class Player2
    {
        static Language language = new Language();
        public static string name = "ClientPlayer2";



        private static bool isServerEntry = false;
        private static bool roomWaiting = false;
        private static bool isReady = false;
        private static bool isGameExit = false;

        public static bool roomWaitingPrint = false;

        private static ClientSession serverSeesion = new ClientSession();

        static void Main(string[] args)
        {
            language.language = (int)Language.LangugaeType.KOR;


            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();

            language.OpneGame(language.language);

            while (isGameExit == false)
            {
                if (!isServerEntry)
                {
                    isServerEntry = ConnectorServer(connector, endPoint);
                }

                else if (isServerEntry && serverSeesion.newPrint == false)
                {
                    Console.WriteLine("1 : Make Room / 2 : Find Romm / 3 : Join");
                    var str = Console.ReadLine();
                    switch (str[0])
                    {
                        case '1':
                            serverSeesion.MakeRoom();
                            break;
                        case '2':
                            serverSeesion.PrintListGameRoom();
                            roomWaitingPrint = false;
                            break;
                        case '3':
                            Console.Write("Room Id : ");
                            int id = int.Parse(Console.ReadLine());
                            serverSeesion.JoinRoom(id);
                            break;
                        default:
                            isGameExit = true;
                            break;
                    }
                }
                else if (isServerEntry && isReady == false && serverSeesion.newPrint == true)
                {
                    Console.WriteLine("Press : 1 -> GameReady / 2 -> Get out Room");
                    var str2 = Console.ReadLine();
                    if (str2[0] == '1')
                    {
                        isReady = true;
                        serverSeesion.GameReady();
                    }
                    else if (str2[0] == '2')
                    {
                        serverSeesion.GetOutRoom();
                        serverSeesion.newPrint = false;
                    }
                    else
                    {

                        continue;
                    }
                }
                else if (isServerEntry && isReady == true && serverSeesion.newPrint == true)
                {
                    //Console.WriteLine("GameStart");

                    Thread.Sleep(1000);
                }
            }

        }


        static bool ConnectorServer(Connector connector, IPEndPoint endPoint)
        {
            var check = Console.ReadLine();
            if (check[0] == '1')
            {
                serverSeesion.name = name;
                connector.Connect(endPoint, serverSeesion);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
