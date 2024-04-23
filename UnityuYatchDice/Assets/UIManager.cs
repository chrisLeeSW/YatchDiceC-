using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public int width = 960; // 원하는 가로 크기
    public int height = 540; // 원하는 세로 크기
    public bool isFullScreen = false; // 전체 화면 여부

    static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameManagerObject = new GameObject("GameManager");
                instance = gameManagerObject.AddComponent<UIManager>();
                DontDestroyOnLoad(gameManagerObject);
            }
            return instance;
        }
    }

    public Player player;
    public GameObject insertServerButtonObject;
    public GameObject nameSettingObject;
    public TextMeshProUGUI nameText;
    public Image insertServer;

    public GameObject selectRoomTypeBaseObject;
    public List<Button> selectRoomTypes;

    public GameObject roomInfo;
    public TextMeshProUGUI roomInfoText;

    public RoomInPlayerInfo roomInPlayerInfo;

    public GameObject joinRoomInputRoomId;

    public BoardScore scoreBoard;

    public Queue<Action> funcQueue = new Queue<Action>();

    public GameObject connectedFailServerObject;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void Start()
    {
        selectRoomTypeBaseObject.gameObject.SetActive(false);
        for (int i = 0; i < selectRoomTypes.Count; i++)
        {
            Button button = selectRoomTypes[i].GetComponent<Button>();
            switch (i)
            {
                case 0:
                    button.onClick.AddListener(player.GetClientsession().MakeRoom);
                    button.onClick.AddListener(() =>
                    {
                        selectRoomTypeBaseObject.gameObject.SetActive(false);
                        roomInfo.gameObject.SetActive(true);
                    });
                    break;
                case 1:
                    button.onClick.AddListener(player.GetClientsession().FindRoom);
                    break;
                case 2:
                    button.onClick.AddListener(() =>
                    {
                        joinRoomInputRoomId.gameObject.SetActive(true);
                        selectRoomTypeBaseObject.gameObject.SetActive(false);
                    });
                    break;
            }
        }
        SetWindowSize(width, height, isFullScreen);
    }

    public void Update()
    {
        if (funcQueue.Count >= 1)
        {
            funcQueue.Dequeue().Invoke();
        }

    }
    #region
    public void EndSetting()
    {
        string name = "";
        for (int i = 0; i < nameText.text.Length - 1; ++i)
        {
            name += nameText.text[i];
        }
        player.playerName = name;
        insertServerButtonObject.SetActive(true);
        nameSettingObject.SetActive(false);
    }
    #endregion

    #region Room
    public void JoinServer()
    {

        if (!player.isServerEntry)
        {
            player.ConnectorServer(player.connector, player.endPoint);
        }
    }
    public void PrintRoomList()
    {
        string str = player.GetClientsession().log;
        roomInfo.gameObject.SetActive(true);
        roomInfoText.text = str;
    }

    public void JoinRoom()
    {
        var idText = joinRoomInputRoomId.GetComponent<TMP_InputField>();
        player.GetClientsession().JoinRoom(int.Parse(idText.text));
    }
    #endregion

    public void SetPlayerName()
    {
        if (scoreBoard.isSetting) return;
        scoreBoard.isSetting = true;
        scoreBoard.NormalCategorisObjectSetting(
            player.GetClientsession().Player1Name, player.GetClientsession().Player2Name);
       
    }

    public void OnConnectedServerSucced()
    {
        insertServer.gameObject.SetActive(false);
        selectRoomTypeBaseObject.gameObject.SetActive(true);
        player.isServerEntry = true;
    }
    public void OnConnectedServerFail()
    {
        StartCoroutine(OnConnectedServerFailCortinue());
    }
    private IEnumerator OnConnectedServerFailCortinue()
    {
        connectedFailServerObject.SetActive(true);
        yield return new WaitForSeconds(3);
        var child = connectedFailServerObject.transform.GetChild(1);
        if(child != null)
        {
            child.gameObject.SetActive(true);
            var button = child.GetComponent<Button>();
            button.onClick.AddListener(() => { Application.Quit(); });
        }
    }

    private void SetWindowSize(int width, int height, bool isFullScreen)
    {
        Screen.SetResolution(width, height, isFullScreen);
    }
}
