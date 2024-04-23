using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerScoreInfo
{
    public string playerName = string.Empty;
    public List<GameObject> scorePanelObject = new List<GameObject>();

    public TextMeshProUGUI myScoreSubTotal;
    public GameObject myScoreSubBouns;

    public TextMeshProUGUI myCurrentScore;

    private readonly Color myScoreColor = Color.green;
    private readonly Color anotherScoreColor = Color.yellow;

    private YatchDiceScore myScore = new YatchDiceScore();
    private List<int> underScoreList = new List<int>();
    private readonly int bonusCondition = 63;

    public void CheckAnotherPlayer(string name)
    {
        if (!playerName.Equals(name))
        {
            for (int i = 0; i < scorePanelObject.Count; i++)
            {
                var button = scorePanelObject[i].GetComponent<Button>();
                button.enabled = false;
            }
        }
    }
    public void SetButton(string name)
    {
        
        if (playerName == string.Empty)
            playerName = name;

        

        for (int i = 0; i < scorePanelObject.Count; ++i)
        {
            int index = i + 1;
            var button = scorePanelObject[i].GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => { SetScorePanelButtonCode(index); });
            }
        }
    }
    private void SetScorePanelButtonCode(int index)
    {
        int result = 0;
        var dices = UIManager.Instance.player.GetClientsession().diceRandomValue;

        
        for(int i =0;i< dices.Length;++i)
        {
            if (dices[i].value ==0)
            {
                Debug.Log("0 이있어서 점수를 얻지 못합니다.");
                return; 
            }
        }
        switch (index)
        {
            case 1:
                NormalRuleScore(dices, index, ref result);
                myScore.aces = result;
                break;
            case 2:
                NormalRuleScore(dices, index, ref result);
                myScore.deuces = result;
                break;
            case 3:
                NormalRuleScore(dices, index, ref result);
                myScore.thress = result;
                break;
            case 4:
                NormalRuleScore(dices, index, ref result);
                myScore.fours = result;
                break;
            case 5:
                NormalRuleScore(dices, index, ref result);
                myScore.fives = result;
                break;
            case 6:
                NormalRuleScore(dices, index, ref result);
                myScore.sixes = result;
                break;
            case 7:
                GetChoiceScore(dices, ref result);
                myScore.choice = result;
                break;
            case 8:
                GetFourKindOfScore(ref result);
                myScore.fourKind = result;
                break;
            case 9:
                GetFullHouse(ref result);
                myScore.fullHouse = result;
                break;
            case 10:
                GetSmallStright(ref result);
                myScore.smallStright = result;
                break;
            case 11:
                GetLargerStright(ref result);
                myScore.largeStright = result;
                break;
            case 12:
                GetYatch(ref result);
                myScore.yatch = result;
                break;
        }
        var button = scorePanelObject[index - 1].GetComponent<Button>();
        button.enabled = false;
        var child = scorePanelObject[index - 1].transform.GetChild(0);
        if (child != null)
        {
            var text = child.GetComponent<TextMeshProUGUI>();
            text.text = result.ToString();
            text.color = myScoreColor;
        }

        UpdateMyScore();
        int totalScore = 0;
        int sum = myScore.GetNormalCategoriSum();
        totalScore += sum;
        myScoreSubTotal.text = sum.ToString();
        if (sum >= bonusCondition )
        {
            myScoreSubBouns.gameObject.SetActive(true);
            totalScore += 35;
        }

        totalScore += myScore.GetSpecialCategoriSum();

        myCurrentScore.text = totalScore.ToString();
        myCurrentScore.color = myScoreColor;

        UIManager.Instance.player.GetClientsession().TurnEnd(myScore);
    }


    public void AnotherPlayerScoreBoardUpdate(YatchDiceScore score,TextMeshProUGUI diffSubTotalScore,GameObject diffSubBonus,
        TextMeshProUGUI diffCurrentScore)
    {
        int subScore = 0;
        int currentPlayerResultScore = 0;
        for (int i = 0; i < scorePanelObject.Count; ++i)
        {
            var child = scorePanelObject[i].transform.GetChild(0);
            var text = child.GetComponent<TextMeshProUGUI>();
            switch (i)
            {
                case 0:
                    text.text = score.aces.ToString();
                    currentPlayerResultScore += score.aces;
                    subScore += score.aces;
                    AnotherPlayerTextSetting(text , score.aces);
                    break;
                case 1:
                    text.text = score.deuces.ToString();
                    currentPlayerResultScore += score.deuces;
                    subScore += score.deuces;
                    AnotherPlayerTextSetting(text, score.deuces);
                    break;
                case 2:
                    text.text = score.thress.ToString();
                    currentPlayerResultScore += score.thress;
                    subScore+= score.thress;
                    AnotherPlayerTextSetting (text, score.thress);
                    break;
                case 3:
                    text.text = score.fours.ToString();
                    currentPlayerResultScore += score.fours;
                    subScore+= score.fours;
                    AnotherPlayerTextSetting(text,score.fours);
                    break;
                case 4:
                    text.text = score.fives.ToString();
                    currentPlayerResultScore += score.fives;
                    subScore+= score.fives;
                    AnotherPlayerTextSetting(text, score.fives);
                    break;
                case 5:
                    text.text = score.sixes.ToString();
                    currentPlayerResultScore += score.sixes;
                    subScore+= score.sixes;
                    AnotherPlayerTextSetting(text, score.sixes);
                    break;
                case 6:
                    text.text = score.choice.ToString();
                    currentPlayerResultScore += score.choice;
                    AnotherPlayerTextSetting(text, score.choice);
                    break;
                case 7:
                    text.text = score.fourKind.ToString();
                    currentPlayerResultScore += score.fourKind;
                    AnotherPlayerTextSetting(text, score.fourKind);
                    break;
                case 8:
                    text.text = score.fullHouse.ToString();
                    currentPlayerResultScore += score.fullHouse;
                    AnotherPlayerTextSetting(text, score.fullHouse);
                    break;
                case 9:
                    text.text = score.smallStright.ToString();
                    currentPlayerResultScore += score.smallStright;
                    AnotherPlayerTextSetting(text,score.smallStright);
                    break;
                case 10:
                    text.text = score.largeStright.ToString();
                    currentPlayerResultScore += score.largeStright;
                    AnotherPlayerTextSetting(text, score.largeStright);
                    break;
                case 11:
                    text.text = score.yatch.ToString();
                    currentPlayerResultScore+= score.yatch;
                    AnotherPlayerTextSetting(text, score.yatch);
                    break;
            }
        }
        diffSubTotalScore.text = subScore.ToString();
        diffSubTotalScore.color = anotherScoreColor;
        if (subScore >= bonusCondition )
        {
            currentPlayerResultScore += 35;
            diffSubBonus.SetActive(true);
        }

        diffCurrentScore.text = currentPlayerResultScore.ToString();
    }

    
    private void AnotherPlayerTextSetting(TextMeshProUGUI text,int number)
    {
        if (number > 0)
            text.color = anotherScoreColor;
    }
    private void UpdateMyScore()
    {
        for (int i = 0; i < scorePanelObject.Count; ++i)
        {
            var text = scorePanelObject[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            switch (i)
            {

                case 0:
                    myScore.aces = int.Parse(text.text);
                    break;
                case 1:
                    myScore.deuces = int.Parse(text.text);
                    break;
                case 2:
                    myScore.thress = int.Parse(text.text);
                    break;
                case 3:
                    myScore.fours = int.Parse(text.text);
                    break;
                case 4:
                    myScore.fives = int.Parse(text.text);
                    break;
                case 5:
                    myScore.sixes = int.Parse(text.text);
                    break;
                case 6:
                    myScore.choice = int.Parse(text.text);
                    break;
                case 7:
                    myScore.fourKind = int.Parse(text.text);
                    break;
                case 8:
                    myScore.fullHouse = int.Parse(text.text);
                    break;
                case 9:
                    myScore.smallStright = int.Parse(text.text);
                    break;
                case 10:
                    myScore.largeStright = int.Parse(text.text);
                    break;
                case 11:
                    myScore.yatch = int.Parse(text.text);
                    break;
            }
        }
    }
    public void Reset()
    {
        for(int i =0;i< scorePanelObject.Count;i++)
        {
            var button = scorePanelObject[i].GetComponent<Button>();
            button.enabled = true;
            var child = scorePanelObject[i].transform.GetChild(0);
            var text = child.GetComponent<TextMeshProUGUI>();

            text.text = "0";
            text.color = Color.black;
        }

        myScore.Reset();
    }
    #region 점수 흭득 계산식
    private void NormalRuleScore(DiceRandomValue[] dices, int condition, ref int result)
    {
        result = 0;
        for (int i = 0; i < dices.Length; ++i)
        {
            if (dices[i].value == condition)
            {
                result += condition;
            }
        }
    }
    private void GetChoiceScore(DiceRandomValue[] dices, ref int result)
    {
        result = 0;
        for (int i = 0; i < dices.Length; ++i)
        {
            result += dices[i].value;
        }
    }

    private void DownScoreList()
    {
        if (underScoreList.Count > 0)
            underScoreList.Clear();
        var dices = UIManager.Instance.player.GetClientsession().diceRandomValue;
        for (int i = 0; i < dices.Length; ++i)
        {
            underScoreList.Add(dices[i].value);
        }

        underScoreList.Sort();
    }

    private void GetFourKindOfScore(ref int result)
    {
        DownScoreList();

        int temp = underScoreList[0];
        int count = 0;
        int subScore = 0;
        for (int i = 1; i < underScoreList.Count; ++i)
        {
            if (temp == underScoreList[i])
                count++;
            else
            {
                count = 0;

                if (subScore == 0)
                {
                    subScore = temp;
                    temp = underScoreList[i];
                }
                else
                    break;
            }
            if (count >= 3)
            {
                if (subScore == 0) subScore = temp; // 5개의 값이 같을경우 사용 
                result = temp * (underScoreList.Count - 1) + subScore;
                break;
            }
        }
        if (count < 3)
            result = 0;
    }

    private void GetFullHouse(ref int result)
    {
        DownScoreList();

        int fristNumber = underScoreList[0];
        int secondNumber = 0;
        bool isFristCounting = true;
        int fristCount = 0;
        int secondCount = 0;
        for (int i = 1; i < underScoreList.Count; ++i)
        {
            if (fristNumber == underScoreList[i])
            {
                if (isFristCounting)
                    fristCount++;
                else
                    secondCount++;
            }
            else
            {
                isFristCounting = false;
                if (secondNumber == 0)
                    secondNumber = fristNumber;
                else
                    break;
                fristNumber = underScoreList[i];
            }
        }
        bool isFrsit = (fristCount >= 1) && (fristCount <= 2);
        bool isSecond = (secondCount >= 1) && (secondCount <= 2);

        if (isFristCounting)
        {
            result = fristNumber * underScoreList.Count;
        }
        else if (isFrsit && isSecond)
        {
            result = (fristCount + 1) * secondNumber;
            result += (secondCount + 1) * fristNumber;
        }
        else
            result = 0;
    }

    private void GetSmallStright(ref int result)
    {
        DownScoreList();

        int number = underScoreList[0];
        int diffCount = 0;
        for (int i = 1; i < underScoreList.Count; i++)
        {

            if (++number != underScoreList[i])
            {
                if (--number == underScoreList[i])
                    number = underScoreList[i];
                else if (number + 2 == underScoreList[i])
                    number = underScoreList[i];
                else
                    diffCount++;
                diffCount++;
            }
            if (diffCount >= 2)
            {
                result = 0;
                return;
            }
        }

        result = 15;
    }
    private void GetLargerStright(ref int result)
    {
        DownScoreList();

        int number = underScoreList[0];
        int diffCount = 0;
        for (int i = 1; i < underScoreList.Count; i++)
        {

            if (++number != underScoreList[i])
            {
                if (--number == underScoreList[i])
                    number = underScoreList[i];
                else
                    diffCount++;
                diffCount++;
            }
            if (diffCount >= 2)
            {
                result = 0;
                return;
            }
        }

        result = 30;
    }

    private void GetYatch(ref int result)
    {
        DownScoreList();

        int number = underScoreList[0];
        for (int i = 1; i < underScoreList.Count; i++)
        {
            if (number != underScoreList[i])
            {
                result = 0;
                return;
            }
        }
        result = 50;
    }
    #endregion
}

