using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public class YatchDiceScore
    {
        public int aces;
        public int deuces;
        public int thress;
        public int fours;
        public int fives;
        public int sixes;
        public int subtotal;
        public readonly int subtotalBouns = 35;


        public int choice;
        public int fourKind;
        public int fullHouse;
        public int smallStright;
        public int largeStright;
        public int yatch;

        public int GetNormalCategoriSum()
        {
            return aces + deuces + thress + fours + fives + sixes;
        }
        public int GetSpecialCategoriSum()
        {
            return choice + fourKind + fullHouse + smallStright + largeStright + yatch;
        }

        public void Reset()
        {
            aces = deuces = thress = fours = fives = sixes = 0;
            choice = fourKind = fullHouse = smallStright = largeStright = yatch = 0;
        }

        public void aa (ref YatchDiceScore score , int index)
        {
            
        }
    }
    public struct DiceRandomValue
    {
        public bool isHolding;
        public ushort value ;

        public DiceRandomValue(bool isHolding = false,ushort value = 0)
        {
            this.isHolding = isHolding;
            this.value = value;
        }
    }

    public abstract class PacketSession : Session
    {
        public static readonly ushort HeaderSize = 2;
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;
            int packetCount = 0;

            while (true)
            {
                if (buffer.Count < HeaderSize)
                    break;

                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                    break;
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
                packetCount++;

                processLen += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
            }

            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
        public sealed override ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = 0;

            count += sizeof(ushort);
            count += sizeof(ushort);
            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(name, 0, name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
            Array.Copy(BitConverter.GetBytes(nameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            count += nameLen;
            Array.Copy(BitConverter.GetBytes((int)selectRoomType), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);
            Array.Copy(BitConverter.GetBytes(roomId), 0, segment.Array, segment.Offset + count, sizeof(int));
            count += sizeof(int);
            Array.Copy(BitConverter.GetBytes(gameReady), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);
            Array.Copy(BitConverter.GetBytes(isGetOutRoom), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);

            for(int i=0;i< diceRandomValue.Length;++i)
            {
                Array.Copy(BitConverter.GetBytes(diceRandomValue[i].isHolding), 0, segment.Array, segment.Offset + count, sizeof(bool));
                count += sizeof(bool);
                Array.Copy(BitConverter.GetBytes((ushort)diceRandomValue[i].value), 0, segment.Array, segment.Offset + count, sizeof(ushort));
                count += sizeof(ushort);
            }
            {
                Array.Copy(BitConverter.GetBytes(sharedDiceScore.aces), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);
                Array.Copy(BitConverter.GetBytes(sharedDiceScore.deuces), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);
                Array.Copy(BitConverter.GetBytes(sharedDiceScore.thress), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);
                Array.Copy(BitConverter.GetBytes(sharedDiceScore.fours), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);
                Array.Copy(BitConverter.GetBytes(sharedDiceScore.fives), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);
                Array.Copy(BitConverter.GetBytes(sharedDiceScore.sixes), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);
                Array.Copy(BitConverter.GetBytes(sharedDiceScore.subtotal), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);
                Array.Copy(BitConverter.GetBytes(sharedDiceScore.choice), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);
                Array.Copy(BitConverter.GetBytes(sharedDiceScore.fourKind), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);
                Array.Copy(BitConverter.GetBytes(sharedDiceScore.fullHouse), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);
                Array.Copy(BitConverter.GetBytes(sharedDiceScore.smallStright), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);
                Array.Copy(BitConverter.GetBytes(sharedDiceScore.largeStright), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);
                Array.Copy(BitConverter.GetBytes(sharedDiceScore.yatch), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);
               
            }


            Array.Copy(BitConverter.GetBytes(isStart), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);
            Array.Copy(BitConverter.GetBytes(isWaiting), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);
            Array.Copy(BitConverter.GetBytes(isDiceRolledShared), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);
            Array.Copy(BitConverter.GetBytes((int)currentDiceRoolState),0,segment.Array,segment.Offset + count, sizeof(int));
            count += sizeof(int);
            Array.Copy(BitConverter.GetBytes(isMyTurn), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);
            Array.Copy(BitConverter.GetBytes(sharedScore), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);
            Array.Copy(BitConverter.GetBytes(turnEnd), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);
            Array.Copy(BitConverter.GetBytes(gameOver), 0, segment.Array, segment.Offset + count, sizeof(bool));
            count += sizeof(bool);

            ushort Player1NameLength = (ushort)Encoding.Unicode.GetBytes(Player1Name, 0, Player1Name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
            //Array.Copy(BitConverter.GetBytes(Player1NameLength), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            count += Player1NameLength;
            ushort Player2NameLength = (ushort)Encoding.Unicode.GetBytes(Player2Name, 0, Player2Name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
            //Array.Copy(BitConverter.GetBytes(Player2NameLength), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            count += Player2NameLength;

            ushort logLen = (ushort)Encoding.Unicode.GetBytes(log, 0, log.Length, segment.Array, segment.Offset + count + sizeof(ushort));
            Array.Copy(BitConverter.GetBytes(logLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            count += logLen;


            Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

            return SendBufferHelper.Close(count);
        }

        public sealed override void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            ushort nameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, nameLen);
            count += nameLen;
            selectRoomType = (SelectRoomType)BitConverter.ToUInt64(segment.Array, segment.Offset + count);
            count += sizeof(int);
            roomId = BitConverter.ToUInt16(segment.Array,segment.Offset+count);
            count += sizeof(int);
            gameReady = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);
            isGetOutRoom = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);

            for (int i = 0; i < diceRandomValue.Length; ++i)
            {
                diceRandomValue[i].isHolding = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
                count += sizeof(bool);
                diceRandomValue[i].value = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(ushort);
            }

            {
                sharedDiceScore.aces = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(int);
                sharedDiceScore.deuces = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(int);
                sharedDiceScore.thress = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(int);
                sharedDiceScore.fours = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(int);
                sharedDiceScore.fives = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(int);
                sharedDiceScore.sixes = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(int);
                sharedDiceScore.subtotal = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(int);
                sharedDiceScore.choice = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(int);
                sharedDiceScore.fourKind = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(int);
                sharedDiceScore.fullHouse = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(int);
                sharedDiceScore.smallStright = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(int);
                sharedDiceScore.largeStright = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(int);
                sharedDiceScore.yatch = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
                count += sizeof(int);
            }


            isStart = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);
            isWaiting = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);
            isDiceRolledShared = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);
            currentDiceRoolState = (DiceRollState)BitConverter.ToUInt64(segment.Array, segment.Offset + count);
            count += sizeof(int);
            isMyTurn = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);
            sharedScore = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);
            turnEnd = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);
            gameOver = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
            count += sizeof(bool);


            ushort Player1NameLength = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            Player1Name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, Player1NameLength);
            count += Player1NameLength;

            ushort Player2NameLength = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            Player2Name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, Player2NameLength);
            count += Player2NameLength;

            ushort logLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            log = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, logLen);
            count += logLen;
        }
    }
    public abstract class Session
    {
        public enum SelectRoomType
        {
            None,
            MakeRoom,
            GetRoomList,
            JoinRoom,
        }

        public enum DiceRollState
        {
            None,
            isDiceRollPending,
            isDiceSelectionPending,
        }

        Socket socket;

        int disconnected = 0;

        protected object blocking = new object();


        Queue<ArraySegment<byte>> sendQueue = new Queue<ArraySegment<byte>>();
        List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();

        RecvBuffer recvBuffer = new RecvBuffer(65535);

        public DiceRandomValue[] diceRandomValue = new DiceRandomValue[5];

        public YatchDiceScore sharedDiceScore = new YatchDiceScore();

        public string name= string.Empty; 
        public SelectRoomType selectRoomType = SelectRoomType.None;
        public int roomId = 0; 
        public bool gameReady = false;
        public bool isGetOutRoom = false;


        public bool isStart = false;
        public bool isWaiting = false;
        public bool isDiceRolledShared = false;
        public DiceRollState currentDiceRoolState = DiceRollState.None;
        public bool isMyTurn = false;
        public bool sharedScore = false;
        public bool turnEnd = false;
        public bool gameOver = false;

        public string Player1Name { get; protected set; } = string.Empty;
        public string Player2Name { get; protected set; } = string.Empty;


        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);
        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> segment);

        public string log = string.Empty;  
        public void Start(Socket socket)
        {
            this.socket = socket;


            sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted); 
            recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);

            RegisterRecv();
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref disconnected, 1) == 1)
                return;

            OnDisconnected(socket.RemoteEndPoint);
            if(socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            if(UIManager.Instance != null) 
            {
                UIManager.Instance.funcQueue.Clear();
                
            }


            
            Clear();
        }
        void Clear()
        {
            lock (blocking)
            {
                sendQueue.Clear();
                pendingList.Clear();
            }
        }

        public void Send(ArraySegment<byte> sendBuff)
        {
            lock (blocking)
            {
                sendQueue.Enqueue(sendBuff);
                if (pendingList.Count == 0)
                    RegisterSend();
            }
        }
        void RegisterSend()
        {

            while (sendQueue.Count > 0)
            {
                ArraySegment<byte> buff = sendQueue.Dequeue();
                pendingList.Add(buff);
            }
            sendArgs.BufferList = pendingList;


            try
            {
                bool pending = true;
                if (socket != null)
                {
                    pending = socket.SendAsync(sendArgs);
                }
                if (pending == false)
                    OnSendCompleted(null, sendArgs);

            }
            catch (Exception e)
            {
                Console.WriteLine($"RegisterSend Failed {e}");
            }
        }
        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (blocking)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        sendArgs.BufferList = null;
                        pendingList.Clear();
                        OnSend(sendArgs.BytesTransferred);
                        if (sendQueue.Count > 0)
                            RegisterSend();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }
        void RegisterRecv()
        {
            if (disconnected == 1)
                return;
            recvBuffer.Clean();
            ArraySegment<byte> segment = recvBuffer.WriteSegment;
            recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);
            try
            {
                bool pending = socket.ReceiveAsync(recvArgs);
                if (pending == false)
                    OnRecvCompleted(null, recvArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine($"RegisterRecv Failed {e}");
            }
        }

        void OnRecvCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    if (recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }
                    int processLen = OnRecv(recvBuffer.ReadSegment);
                    if (processLen < 0 || recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }
                    if (recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }
                    RegisterRecv();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }
            }
            else
            {
                Disconnect();
            }
        }

        
    }
}
