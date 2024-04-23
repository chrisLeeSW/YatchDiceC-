using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace YatchServer
{
    public class GameRoom
    {

        static GameRoom instance = new GameRoom();
        public static GameRoom Instance { get { return instance; } }

        public Dictionary<int, List<ServerSession>> rooms = new Dictionary<int, List<ServerSession>>();
        public Dictionary<int, int> roomsPlayerCount = new Dictionary<int, int>();


        public DiceRandomValue[] diceRandomValue = new DiceRandomValue[5];
        public bool isMyTurn = false;

        Random random = new Random();

        private readonly int DurationPlayerCount = 2;

        public int PlayerCount { get; set; } = 0;

        public ushort gameTurn = 3;
        public readonly ushort gameTurnDuration = 3;
        public void MakeGameRoom(ServerSession session)
        {
            while (true)
            {
                if (!rooms.ContainsKey(session.roomId))
                {
                    roomsPlayerCount.Add(session.roomId, 0);
                    rooms.Add(session.roomId, new List<ServerSession>());
                    rooms[session.roomId].Add(session);
                    return;
                }
                else
                {
                    session.roomId = random.Next(1, 1001);
                }
            }
        }

        public List<string> GetRoomList()
        {
            List<string> roomListStr = new List<string>();
            foreach (var t in rooms)
            {
                foreach (var r in t.Value)
                {
                    roomListStr.Add($"이름 : {r.name} / 방 번호 : {t.Key}");
                }
            }
            return roomListStr;
        }

        public bool JoinRoom(int id, ServerSession session)
        {
            if (rooms.ContainsKey(id))
            {
                if (rooms[id].Count >= DurationPlayerCount) return false;
                else if (rooms[id].Count != 0)
                {
                    for (int i = 0; i < rooms[id].Count; ++i)
                    {
                        if (rooms[id][i].name.Equals(session.name))
                            return false;
                    }
                    rooms[session.roomId].Add(session);
                }

                return true;
            }
            return false;
        }
        public void RemoveSession(ServerSession session)
        {
            if (rooms.ContainsKey(session.roomId))
            {
                if (rooms[session.roomId].Count > 0)
                    rooms[session.roomId].Remove(session);
                if (rooms[session.roomId].Count >0)
                {
                    rooms[session.roomId][0].gameOver = true;
                    var sendBuffer = rooms[session.roomId][0].Write();
                    rooms[session.roomId][0].Send(sendBuffer);
                }
            }
            if (rooms[session.roomId].Count == 0)
            {
                rooms.Remove(session.roomId);
            }
        }

        public bool GameStart(ServerSession session)
        {
            if (rooms[session.roomId].Count != DurationPlayerCount) return false;
            foreach (var t in rooms[session.roomId])
            {
                if (t.gameReady == false)
                    return false;
            }
            return true;
        }
        public void StartGame(Session session, int Count = 0)
        {
            GetRandomDiceValue();
            DiceRandomValue[] dice = diceRandomValue;

            int c = roomsPlayerCount[session.roomId];
            for (int i = 0; i < diceRandomValue.Length; ++i)
            {
                diceRandomValue[i] = dice[i];
            }
            rooms[session.roomId][c % 2].diceRandomValue = diceRandomValue;
            rooms[session.roomId][(c + 1) % 2].diceRandomValue = diceRandomValue;
            rooms[session.roomId][(c + 1) % 2].log = $"현재 플레이어의 턴 : {rooms[session.roomId][c % 2].name}의 턴입니다.";
            if (gameTurn <= 0)
            {
                rooms[session.roomId][c % 2].turnEnd = true;
                rooms[session.roomId][c % 2].isWaiting = false;
                rooms[session.roomId][c % 2].currentDiceRoolState = Session.DiceRollState.isDiceRollPending;


                rooms[session.roomId][(c + 1) % 2].turnEnd = false;
                rooms[session.roomId][(c + 1) % 2].isWaiting = false;

                gameTurn = gameTurnDuration;
            }
            else
            {
                rooms[session.roomId][c % 2].sharedScore = false;
                rooms[session.roomId][c % 2].turnEnd = false;
                rooms[session.roomId][c % 2].isWaiting = true;
                rooms[session.roomId][c % 2].currentDiceRoolState = Session.DiceRollState.isDiceSelectionPending;


                rooms[session.roomId][(c + 1) % 2].sharedScore = false;
                rooms[session.roomId][(c + 1) % 2].turnEnd = false;
                rooms[session.roomId][(c + 1) % 2].isWaiting = true;
            }


            var sendBuffer1 = rooms[session.roomId][c % 2].Write();
            if (sendBuffer1 != null)
                rooms[session.roomId][c % 2].Send(sendBuffer1);
            var sendBuffer2 = rooms[session.roomId][(c + 1) % 2].Write();
            if (sendBuffer2 != null)
                rooms[session.roomId][(c + 1) % 2].Send(sendBuffer2);
        }
        #region 주사위 관련 함수들
        public void GetRandomDiceValue()
        {


            switch (gameTurn)
            {
                case 3:
                    GetValue();
                    break;
                case 2:
                    GetValue();
                    break;
                case 1:
                    GetValue();
                    break;
            }
        }
        public void GetValue()
        {
            gameTurn--;
            for (int i = 0; i < diceRandomValue.Length; ++i)
            {
                if (!diceRandomValue[i].isHolding)
                    diceRandomValue[i].value = (ushort)random.Next(1, 7);
            }
        }

        public void HoldingRandomDice(bool[] isHolding)
        {
            for (int i = 0; i < isHolding.Length; ++i)
            {
                if (isHolding[i])
                    diceRandomValue[i].isHolding = isHolding[i];
            }
        }

        public void GameEnd(int roomid)
        {
            roomsPlayerCount[roomid] = 0;
        }

        public void ResetDiceValue(Session session)
        {
            for (int i = 0; i < diceRandomValue.Length; ++i)
            {
                diceRandomValue[i].value = 0;
                diceRandomValue[i].isHolding = false;
            }
            int c = roomsPlayerCount[session.roomId];

           


            

            if (roomsPlayerCount[session.roomId] >=23)
            {
                roomsPlayerCount[session.roomId] = -1;

                session.gameOver = true;
                session.sharedScore = false;
                session.isStart = false;
                session.isWaiting = false;
                session.gameReady = false;
                session.isMyTurn = false;
                session.currentDiceRoolState = Session.DiceRollState.None;


                rooms[session.roomId][(c + 1) % 2].gameOver = true;
                rooms[session.roomId][(c + 1) % 2].isWaiting = false;
                rooms[session.roomId][(c + 1) % 2].sharedScore = false;
                rooms[session.roomId][(c + 1) % 2].isStart= false;
                rooms[session.roomId][(c + 1) % 2].gameReady = false;
                rooms[session.roomId][(c + 1) % 2].isMyTurn = false;
                rooms[session.roomId][(c + 1) % 2].sharedDiceScore = session.sharedDiceScore;
                rooms[session.roomId][(c + 1) % 2].currentDiceRoolState = Session.DiceRollState.None;
            }
            else
            {
                session.sharedScore = false;
                session.isWaiting = true;
                session.isMyTurn = false;
                session.currentDiceRoolState = Session.DiceRollState.None;


                rooms[session.roomId][(c + 1) % 2].isWaiting = true;
                rooms[session.roomId][(c + 1) % 2].isMyTurn = true;
                rooms[session.roomId][(c + 1) % 2].sharedScore = true;
                rooms[session.roomId][(c + 1) % 2].sharedDiceScore = session.sharedDiceScore;
                rooms[session.roomId][(c + 1) % 2].currentDiceRoolState = Session.DiceRollState.isDiceRollPending;
            }

            var sendBuffer1 = session.Write();
            if (sendBuffer1 != null)
                session.Send(sendBuffer1);

            var sendBuffer2 = rooms[session.roomId][(c + 1)%2].Write();
            if (sendBuffer2 != null)
                rooms[session.roomId][(c + 1) % 2].Send(sendBuffer2);
            roomsPlayerCount[session.roomId]++;
        }
        public int[] PrintDiceRandomValue()
        {
            int[] printValue = new int[diceRandomValue.Length];
            for (int i = 0; i < printValue.Length; ++i)
            {
                printValue[i] = diceRandomValue[i].value;
            }
            return printValue;
        }
        #endregion
    }
}
