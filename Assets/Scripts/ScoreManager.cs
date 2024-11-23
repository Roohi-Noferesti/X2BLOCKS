using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{

    
    public static ScoreManager instance
    {
        get;
        private set;
    }

    private ulong score,highestScore;
    int surviveModeCurrent, SurviveModeHigh;

    public void LoadHighestSurvive()
    {
        SurviveModeHigh = PlayerPrefs.GetInt("survivehigh");
        UiManager.instance.SurvivalHighScore.text = SurviveModeHigh.ToString(); 
    }

    public void addToSurviveMode(int surviveScore)
    {
        surviveModeCurrent+=surviveScore;
        UiManager.instance.survivalScore.text = surviveModeCurrent.ToString();
        if(surviveModeCurrent>SurviveModeHigh)
        {
            SurviveModeHigh = surviveModeCurrent;
            PlayerPrefs.SetInt("survivehigh", SurviveModeHigh);
            UiManager.instance.SurvivalHighScore.text = SurviveModeHigh.ToString();
        }
    }




    public ulong HighestScore { get { return highestScore; }
        set { highestScore = value;
        
        UiManager.instance.highestScoretxt.text = value.ToString();
             GameStateManager.instance.State.HighestScore=value;
            PlayerPrefs.SetInt("highscore", (int)HighestScore);
            
        } }

    public ulong Score
    {
        get { return score; }
        set { score = value;
        UiManager.instance.livescoretxt.text = value.ToString();  
            
        }
    }
    




    public void AddToScore(ulong value)
    {
        ulong newscore = score + value;
        GameStateManager.instance.State.currentScore = newscore;
        if(newscore > highestScore)
        {
            HighestScore = score+value;
        }
        StartCoroutine(Co_AddtoScore(value));   
    }

    IEnumerator Co_AddtoScore(ulong val)
    {
        Application.targetFrameRate = -1;
        float start=Time.time;
        ulong startScore = Score;
        ulong target = startScore+val;
        ulong delta=target-startScore;
        ulong lerp = 0;
        while(Time.time-start<0.3f)
        {

            lerp=(ulong) (((Time.time-start)/0.3f)*delta);
            Score=startScore+lerp;
            yield return null;
        }
        Score = target;
        if(!SelectionUIManager.Instance.swaping && !GridManager.instance.GridBusy)
        {
            Application.targetFrameRate = GridManager.instance.HaltFrameRate;
        }
        //d
    }


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    public void ScoreInit()
    {
        if(GameStateManager.instance.State.HighestScore!=0)
        {
            highestScore = GameStateManager.instance.State.HighestScore;
        }
        else
        {
            highestScore = (ulong)PlayerPrefs.GetInt("highscore");
        }
        
        UiManager.instance.highestScoretxt.text = highestScore.ToString();
        Score = GameStateManager.instance.State.currentScore;
    }
    
}
