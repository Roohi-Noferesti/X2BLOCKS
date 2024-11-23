using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeUI : MonoBehaviour
{
    // Start is called before the first frame update
    public TMPro.TextMeshProUGUI gemtxt, highesttxt;
    [SerializeField]
    Block block;
    public TextMeshProUGUI SurviveHigh;
    public static HomeUI instance { get; private set; }
    public Text versiontext;
    private void Start()
    {
        string verInfo = null;
        if (Debug.isDebugBuild)
        {
            verInfo = $"development build version:{Application.version}";
            
            
        }
        else
        {
             verInfo = $"{Application.version}";
        }
        versiontext.text = verInfo; 
        SurviveHigh.text = PlayerPrefs.GetInt("survivehigh").ToString();
    }
    private void Awake()
    {
        if(instance!=null &&instance!=this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;    
        }
    }
    public void GoToGameScene(int mode)
    {
        switch(mode)
        {
            case 0:
                PlayerPrefs.SetString("gameMode", "normal");
                break;
                case 1:
                PlayerPrefs.SetString("gameMode", "survival");
                break ;
        }

        SceneManager.LoadScene(1);
    }

    public void SetUpUi()
    {
        highesttxt.text=PlayerPrefs.GetInt("highscore").ToString();
        block.SetBlockValue(GameStateManager.instance.State.HighestBlock);
        block.GetComponent<BlockUI>().SetValueText(GameStateManager.instance.State.HighestBlock);   
    }
}
