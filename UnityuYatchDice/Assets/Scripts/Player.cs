using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;


public class Player : MonoBehaviour
{
    public IPEndPoint endPoint;
    ClientSession clientSeesion = new ClientSession();
    public readonly Connector connector = new Connector();
    [HideInInspector]public string playerName =string.Empty;

    public bool isServerEntry = false;

    public TextMeshProUGUI text;
    void Start()
    {
         //string host = Dns.GetHostName();
        // IPHostEntry ipHost = Dns.GetHostEntry(host);
        // IPAddress ipAddr = ipHost.AddressList[0];
        IPAddress ipAddr = IPAddress.Parse("13.58.78.74");
        endPoint = new IPEndPoint(ipAddr, 7777);

        Debug.Log(ipAddr.ToString());
    }

    private void Update()
    {
        // cheat Mode
        if(clientSeesion.isMyTurn)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                clientSeesion.diceRandomValue[0].value = 6;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                clientSeesion.diceRandomValue[1].value = 6;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                clientSeesion.diceRandomValue[2].value = 6;

            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                clientSeesion.diceRandomValue[3].value = 6;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                clientSeesion.diceRandomValue[4].value = 6;
            }
        }

        //if(Input.GetKeyDown(KeyCode.Escape))
        //{
        //    clientSeesion.isGetOutRoom = true;
        //    UIManager.Instance.selectRoomTypeBaseObject.gameObject.SetActive(true);
        //    UIManager.Instance.scoreBoard.gameObject.SetActive(false);
        //    UIManager.Instance.roomInPlayerInfo.gameObject.SetActive(false);
        //    var sendBuffer = clientSeesion.Write();
        //    clientSeesion.Send(sendBuffer);
        //}
    }
    public void ConnectorServer(Connector connector, IPEndPoint endPoint)
    {
        clientSeesion.name = playerName;
        connector.Connect(endPoint, clientSeesion);

        text.text = playerName;

        if(connector.isEntery)
        {
            isServerEntry = true;
        }
    }
    public ClientSession GetClientsession() { return clientSeesion; }

}