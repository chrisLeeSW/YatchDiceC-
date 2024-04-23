using ServerCore;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardScore : MonoBehaviour
{
    public Button diceRollingObject;
    public List<GameObject> diceResultObject = new List<GameObject>();
    private bool[] diceHoldings = new bool[5];
    public TextMeshProUGUI[] playerNameallPlayersNames = new TextMeshProUGUI[2];
    public bool isSetting = false;

    public PlayerScoreInfo Player1;
    public PlayerScoreInfo Player2;

    public GameObject winInfo;
    readonly YatchDiceScore yatchDiceScoreEmpty = new YatchDiceScore();
    public void Start()
    {
        diceRollingObject.onClick.AddListener(() => { UIManager.Instance.player.GetClientsession().RollingDice(diceHoldings); });
        for (int i = 0; i < diceResultObject.Count; i++)
        {
            var button = diceResultObject[i].GetComponent<Button>();
            if (button != null)
            {
                int index = i;
                button.onClick.AddListener(() => { HoldingDice(index); });
            }
        }
    }

    public void GetResultDiceValue()
    {
        var diceValue = UIManager.Instance.player.GetClientsession().diceRandomValue;
        for (int i = 0; i < diceValue.Length; ++i)
        {
            var textMesh = diceResultObject[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.text = $"{diceValue[i].value}";
            }
        }
    }
    private void HoldingDice(int index)
    {
        if (!UIManager.Instance.player.GetClientsession().isMyTurn)
            return;
        diceHoldings[index] = !diceHoldings[index];
        if (diceHoldings[index])
        {
            var image = diceResultObject[index].GetComponent<Image>();
            image.color = Color.red;
        }
        else if (!diceHoldings[index])
        {
            var image = diceResultObject[index].GetComponent<Image>();
            image.color = Color.white;
        }
    }
    public void HoldingDiceReset()
    {
        for (int i = 0; i < diceHoldings.Length; i++)
        {
            diceHoldings[i] = false;
            var image = diceResultObject[i].GetComponent<Image>();
            image.color = Color.white;
        }
    }

    public void NormalCategorisObjectSetting(string p1, string p2)
    {
        playerNameallPlayersNames[0].text = p1;
        playerNameallPlayersNames[1].text = p2;
        Player1.SetButton(p1);
        Player2.SetButton(p2);
        Player1.CheckAnotherPlayer(UIManager.Instance.player.playerName);
        Player2.CheckAnotherPlayer(UIManager.Instance.player.playerName);
    }


    public void BoardScoreUpdate(YatchDiceScore score)
    {
        string currentPlayerName = UIManager.Instance.player.playerName;
        if(Player1.playerName.Equals(currentPlayerName))
        {
            Player2.AnotherPlayerScoreBoardUpdate(score, Player2.myScoreSubTotal, 
                Player2.myScoreSubBouns, Player2.myCurrentScore);
        }
        else if(Player2.playerName.Equals(currentPlayerName))
        {
            Player1.AnotherPlayerScoreBoardUpdate(score, Player1.myScoreSubTotal, 
                Player1.myScoreSubBouns, Player1.myCurrentScore);
        }
        
    }

    public void UpdateWinfo()
    {
        winInfo.gameObject.SetActive(true);
        for (int i = 0; i < diceResultObject.Count; ++i)
        {
            var textMesh = diceResultObject[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.text = $"{0}";
            }
            diceResultObject[i].gameObject.SetActive(false);
        }
        var p1Result = int.Parse(Player1.myCurrentScore.text);
        var p2Result = int.Parse(Player2.myCurrentScore.text);

        var child = winInfo.transform.GetChild(0);
        var text = child.GetComponent<TextMeshProUGUI>();
        if(p1Result>p2Result)
        {
            text.text = Player1.playerName;
        }
        else if(p1Result <p2Result)
        {
            text.text = Player2.playerName;
        }
        else
        {
            text.text = "Draw";
            winInfo.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void OffWinfo()
    {
        winInfo.gameObject.SetActive(false);
        var child = winInfo.transform.GetChild(0);
        var text = child.GetComponent<TextMeshProUGUI>();
        text.text = string.Empty;

        var child2 = winInfo.transform.GetChild(1).gameObject;
        if (!child2.activeSelf)
            child2.SetActive(true);

        for (int i = 0; i < diceResultObject.Count; ++i)
        {
            diceResultObject[i].gameObject.SetActive(true);
        }

        Player1.Reset();
        Player2.Reset();

        yatchDiceScoreEmpty.Reset();
        BoardScoreUpdate(yatchDiceScoreEmpty);
    }
}
