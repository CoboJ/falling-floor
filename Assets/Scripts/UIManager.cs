using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using Doozy.Engine.Progress;
using Doozy.Engine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Color[] rankColors;
    public List<Players> players = new List<Players>();

    private void Start() 
    {
        foreach (var playerData in players)
        {
            string firstChar = playerData.Gamertag.ElementAt(0).ToString();
            string lastChar = playerData.Gamertag.ElementAt((playerData.Gamertag.Length - 1)).ToString();

            playerData.nameAbreviature.SetText(firstChar.ToUpper() + lastChar.ToUpper());
        }
    }

    private void LateUpdate()
    {
        UpdateLeaderboard();
    }

    private void UpdateLeaderboard()
    {
        int num = 1; 

        foreach (var playerData in players.OrderByDescending(x => x.Score))
        {
            playerData.scoreBoard.SetText(playerData.Score.ToString("F2"));

            if (playerData.Rank != num)
            {
                playerData.positionProgressor.SetValue(num - 1);
                playerData.boardPosition.SetText((num).ToString());

                playerData.Rank = num;
            }

            num++; 
        }
    }

    public void UpdateGameOverUI()
    {
        int num = 1; 

        foreach (var playerData in players.OrderByDescending(x => x.Score))
        {
            playerData.goScoreboard.SetText(playerData.Score.ToString("F2"));

            playerData.goLeaderboardPos.SetText((num).ToString());
            playerData.rankImage.color = rankColors[num - 1];

            playerData.goNameText.SetText(playerData.Gamertag.ToString());
            playerData.Rank = num;

            num++; 
        }
    }

    public void Timer(float time){
        if(time.Equals(0))
            GameEventMessage.SendEvent("GameOver");
    }
}

[Serializable] 
public class Players {
    [Header("Players Variables")]
    [SerializeField] private PlayerData playerData;

    public int Rank
    {
        get { return playerData.RuntimeRank; }
        set { playerData.RuntimeRank = value; }
    }

    public string Gamertag { get { return playerData.RuntimeGamertag; } }

    public float Score { get { return playerData.RuntimeScore; } }

    [Header("Players Components")] 
    public Progressor positionProgressor; 
    public TextMeshProUGUI scoreBoard; 
    public TextMeshProUGUI boardPosition; 
    public TextMeshProUGUI nameAbreviature;
    
    [Header("GameOver Components")]
    public TextMeshProUGUI goScoreboard;
    public TextMeshProUGUI goNameText;
    public TextMeshProUGUI goLeaderboardPos;
    public Image rankImage;
}