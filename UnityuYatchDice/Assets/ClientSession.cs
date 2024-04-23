using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static ServerCore.Session;

public class ClientSession : PacketSession
{
    System.Random random = new System.Random();
    bool connectedServer = false;
    bool isFristStartSetting = true;
    public override void OnConnected(EndPoint endPoint)
    {
        Debug.Log($"OnConnected : {endPoint}");
        connectedServer = true;
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Debug.Log($"OnDisConnected : {endPoint}");
        Application.Quit();
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        Read(buffer);
        if (selectRoomType == SelectRoomType.MakeRoom)
        {
            UIManager.Instance.funcQueue.Enqueue(() =>
            {
                UIManager.Instance.joinRoomInputRoomId.gameObject.SetActive(false);
                UIManager.Instance.roomInfo.gameObject.SetActive(false);
                UIManager.Instance.selectRoomTypeBaseObject.gameObject.SetActive(false);
                UIManager.Instance.scoreBoard.gameObject.SetActive(true);

                UIManager.Instance.roomInPlayerInfo.gameObject.SetActive(true);
                UIManager.Instance.roomInPlayerInfo.roomInPlayerInfoText.text = log;
            });
        }
        if (selectRoomType == SelectRoomType.GetRoomList)
        {
            selectRoomType = SelectRoomType.None;
            UIManager.Instance.funcQueue.Enqueue(UIManager.Instance.PrintRoomList);
        }
        if (roomId != 0 && selectRoomType == SelectRoomType.None && !gameReady && isFristStartSetting)
        {
            UIManager.Instance.funcQueue.Enqueue(() =>
            {
                UIManager.Instance.joinRoomInputRoomId.gameObject.SetActive(false);
                UIManager.Instance.selectRoomTypeBaseObject.gameObject.SetActive(true);
            });
        }
        else if (roomId != 0 && selectRoomType == SelectRoomType.JoinRoom && !gameReady && isFristStartSetting)
        {

            UIManager.Instance.funcQueue.Enqueue(() =>
            {
                UIManager.Instance.joinRoomInputRoomId.gameObject.SetActive(false);
                UIManager.Instance.roomInfo.gameObject.SetActive(false);
                UIManager.Instance.selectRoomTypeBaseObject.gameObject.SetActive(false);
                UIManager.Instance.scoreBoard.gameObject.SetActive(true);

                UIManager.Instance.roomInPlayerInfo.gameObject.SetActive(true);
                UIManager.Instance.roomInPlayerInfo.roomInPlayerInfoText.text = log;
            });
        }
        if (gameReady && isStart && (currentDiceRoolState == DiceRollState.isDiceRollPending || currentDiceRoolState == DiceRollState.None) && !isMyTurn)
        {

            if (isFristStartSetting)
            {
                isFristStartSetting = false;
                UIManager.Instance.funcQueue.Enqueue(UIManager.Instance.SetPlayerName);
            }
            UIManager.Instance.funcQueue.Enqueue(() => { UIManager.Instance.scoreBoard.diceRollingObject.gameObject.SetActive(false); });

            UIManager.Instance.funcQueue.Enqueue(() =>
            {
                UIManager.Instance.scoreBoard.GetResultDiceValue();
            });

        }
        if (gameReady && isStart && isWaiting && (currentDiceRoolState == DiceRollState.isDiceRollPending || currentDiceRoolState == DiceRollState.None) && isMyTurn)
        {
            if (isFristStartSetting)
            {
                isFristStartSetting = false;
                UIManager.Instance.funcQueue.Enqueue(UIManager.Instance.SetPlayerName);
            }
            UIManager.Instance.funcQueue.Enqueue(() =>
            {
                UIManager.Instance.scoreBoard.diceRollingObject.gameObject.SetActive(true);
                UIManager.Instance.scoreBoard.GetResultDiceValue();

            });
        }
        if (gameReady && isStart && isWaiting && currentDiceRoolState == DiceRollState.isDiceSelectionPending && isMyTurn)
        {
            UIManager.Instance.funcQueue.Enqueue(() => { UIManager.Instance.scoreBoard.GetResultDiceValue(); });
        }
        if (gameReady && turnEnd)
        {
            UIManager.Instance.funcQueue.Enqueue(() => { UIManager.Instance.scoreBoard.GetResultDiceValue(); });
            UIManager.Instance.funcQueue.Enqueue(() =>
            {
                UIManager.Instance.scoreBoard.diceRollingObject.gameObject.SetActive(false);
                UIManager.Instance.scoreBoard.GetResultDiceValue();
            });
        }
        if (sharedScore)
        {
            UIManager.Instance.funcQueue.Enqueue(() =>
            {
                UIManager.Instance.scoreBoard.BoardScoreUpdate(sharedDiceScore);
            });
            sharedScore = false;
        }
        if(gameOver)
        {
            UIManager.Instance.funcQueue.Enqueue(() =>
            {
                UIManager.Instance.scoreBoard.UpdateWinfo();
                UIManager.Instance.roomInPlayerInfo.gameObject.SetActive(true);
            });
        }
    }

    public override void OnSend(int numOfBytes)
    {

    }

    public void MakeRoom()
    {
        if (connectedServer == false)
            return;
        selectRoomType = SelectRoomType.MakeRoom;
        roomId = random.Next(1, 1001);
        log = $"{name} is Make Room";
        SendBufferByServer();
    }

    public void FindRoom()
    {
        selectRoomType = SelectRoomType.GetRoomList;
        SendBufferByServer();
    }

    public void JoinRoom(int id)
    {
        selectRoomType = SelectRoomType.JoinRoom;
        roomId = id;
        log = $"{name} is Join Room : {roomId}";
        SendBufferByServer();
    }
    public void GameReady()
    {
        gameReady = true;
        selectRoomType = SelectRoomType.None;
        SendBufferByServer();
    }

    static int test = 1;
    public void RollingDice(bool[] bools)
    {
        Debug.Log($"{test++}번 주사위굴림");
        isWaiting = false;
        for (int i = 0; i < diceRandomValue.Length; ++i)
        {
            diceRandomValue[i].isHolding = bools[i];
        }
        SendBufferByServer();
    }
    public void TurnEnd(YatchDiceScore score)
    {
        UIManager.Instance.scoreBoard.HoldingDiceReset();
        sharedScore = true;
        isWaiting = false;
        sharedDiceScore = score;
        SendBufferByServer();

    }

    public void ResetGame()
    {
        gameReady = false;
        isWaiting = false;
        sharedDiceScore.Reset();
        selectRoomType = SelectRoomType.None;
    }
    private void SendBufferByServer()
    {
        var sendBuffer = Write();
        if (sendBuffer != null)
            Send(sendBuffer);
    }


}
