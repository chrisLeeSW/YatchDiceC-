using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace YatchServer
{


    public class ServerSession : PacketSession
    {
        public bool StartGame = false;
        private const int durationCurrentGameTurn = 24;
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            Read(buffer);
            if (selectRoomType == SelectRoomType.MakeRoom)
            {
                GameRoom.Instance.MakeGameRoom(this);
                selectRoomType = SelectRoomType.MakeRoom;
                log = $"0번 플레이어의 이름 : {name}\n";
                var sendBuffer = Write();
                if (sendBuffer != null)
                    Send(sendBuffer);
            }
            if (selectRoomType == SelectRoomType.GetRoomList)
            {
                List<string> roomlist = GameRoom.Instance.GetRoomList();
                log = string.Empty;
                if (roomlist != null)
                {
                    for (int i = 0; i < roomlist.Count; i++)
                    {
                        log += roomlist[i] + "\n";
                    }
                    var sendBuffer = Write();
                    if (sendBuffer != null)
                        Send(sendBuffer);
                }
                else
                {
                    log = $"Empty Rooms";
                    var sendBuffer = Write();
                    if (sendBuffer != null)
                        Send(sendBuffer);
                }
            }
            if (selectRoomType == SelectRoomType.JoinRoom)
            {
                if (GameRoom.Instance.JoinRoom(roomId, this))
                {
                    string logtemp = string.Empty;
                    for (int i = 0; i < 2; ++i)
                    {
                        logtemp += $"{i}번 플레이어의 이름 : {GameRoom.Instance.rooms[roomId][i].name}\n";
                    }
                    for (int i = 0; i < 2; ++i)
                    {
                        GameRoom.Instance.rooms[roomId][i].log = logtemp;
                        GameRoom.Instance.rooms[roomId][i].selectRoomType = SelectRoomType.JoinRoom;
                        var sendBuffer = GameRoom.Instance.rooms[roomId][i].Write();
                        if (sendBuffer != null)
                        {
                            GameRoom.Instance.rooms[roomId][i].Send(sendBuffer);
                        }
                    }
                }
                else
                {
                    log = $"Room id diff :{roomId}";
                    selectRoomType = SelectRoomType.None;
                    var sendBuffer = Write();
                    if (sendBuffer != null)
                        Send(sendBuffer);
                }
            }
            if (isGetOutRoom)
            {
                GameRoom.Instance.RemoveSession(this);
                selectRoomType = SelectRoomType.None;
                roomId = 0;
                gameReady = false;
                isGetOutRoom = false;
                var sendBuffer = Write();
                if (sendBuffer != null)
                    Send(sendBuffer);
            }
            if (gameReady && !isStart)
            {
                if (GameRoom.Instance.GameStart(this))
                {
                    for (int i = 0; i < 2; ++i)
                    {
                        GameRoom.Instance.rooms[roomId][i].isStart = true;
                        GameRoom.Instance.rooms[roomId][i].isWaiting = true;
                        GameRoom.Instance.rooms[roomId][i].currentDiceRoolState = DiceRollState.isDiceRollPending;
                        GameRoom.Instance.rooms[roomId][i].player1Name = GameRoom.Instance.rooms[roomId][0].name;
                        GameRoom.Instance.rooms[roomId][i].player2Name = GameRoom.Instance.rooms[roomId][1].name;
                        GameRoom.Instance.rooms[roomId][i].gameOver = false;
                        if (i == 0)
                            GameRoom.Instance.rooms[roomId][i].isMyTurn = true;
                        else
                            GameRoom.Instance.rooms[roomId][i].isMyTurn = false;
                        var sendBuffer = GameRoom.Instance.rooms[roomId][i].Write();
                        if (sendBuffer != null)
                        {
                            GameRoom.Instance.rooms[roomId][i].Send(sendBuffer);
                        }
                    }
                }
            }
            if (isStart && !isWaiting && isStart && !sharedScore)
            {
                lock (blocking)
                {
                    bool[] holding = new bool[5];
                    for (int i = 0; i < 5; ++i)
                    {
                        holding[i] = diceRandomValue[i].isHolding;
                    }
                    GameRoom.Instance.HoldingRandomDice(holding);
                    GameRoom.Instance.StartGame(this, 0);
                }
            }
            if (sharedScore)
            {

                GameRoom.Instance.gameTurn = GameRoom.Instance.gameTurnDuration;
                GameRoom.Instance.ResetDiceValue(this);
                //if (GameRoom.Instance.roomsPlayerCount[roomId] >= durationCurrentGameTurn)
                //{
                //    GameRoom.Instance.rooms[roomId][0].gameOver = true;
                //    GameRoom.Instance.rooms[roomId][0].sharedScore = false;
                //    var sendBuffer1 = GameRoom.Instance.rooms[roomId][0].Write();
                //    if (sendBuffer1 != null)
                //        GameRoom.Instance.rooms[roomId][0].Read(sendBuffer1);


                //    GameRoom.Instance.rooms[roomId][1].gameOver = true;
                //    GameRoom.Instance.rooms[roomId][1].sharedScore = false;

                //    var sendBuffer2 = GameRoom.Instance.rooms[roomId][1].Write();
                //    if (sendBuffer2 != null)
                //        GameRoom.Instance.rooms[roomId][1].Read(sendBuffer2);

                //    GameRoom.Instance.GameEnd(roomId);
                //    return;
                //}
            }
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"OnSend");
        }
    }
}
