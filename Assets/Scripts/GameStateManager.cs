using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public bool Home;
    public enum GameMode
    {
        normal,survival
    }
    public GameMode gameMode;
    public static GameStateManager instance { get; private set; }
    private void Awake()
    {
        
        
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        
      
    }
    JsonSerializerSettings setting;
    string Path;
    private void Start()
    {
        //temp
        

        switch(PlayerPrefs.GetString("gameMode"))
        {
            case "normal":
                gameMode = GameMode.normal;
                break;
            case "survival":
                gameMode=GameMode.survival;
                break;
                default:
                gameMode=GameMode.normal;
                break;
        }


        Path= Application.persistentDataPath+"/v2.txt";
        State = new GameState();
        setting = new JsonSerializerSettings();
        setting = new JsonSerializerSettings();
        setting.Formatting = Formatting.Indented;
        setting.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        LoadGame();
              
            if (HomeUI.instance != null)
            {
                    Home = true;
                    HomeUI.instance.SetUpUi(); 
                    PowerManager.instance.LoadGems();
            }
            else
            {
                
                    //game scene init
                    Home = false;
                    BlockPool.instance.InitializeBlocks();
                    UiManager.instance.GetCanvasRatio();
                    GridManager.instance.GridSetup();
                    SelectionUIManager.Instance.SetUp();
                    UiManager.instance.SetColumns();
                    BlockListManager.instance.SetSelectionBlocksList();
                    GameBrain.Instance.SetUpBlockList();
                    Tutorial.Instance.CheckForTutorial();
                    ScoreManager.instance.ScoreInit();
                    PowerManager.instance.LoadGems();
                    LevelOrganizer.instance.InitHighestBlock();  
                    PowerManager.instance.SetupIngameScene();
            if(gameMode == GameMode.normal)
            {
                GridManager.instance.LoadGrid();
                  
            }
                    
                    GameBrain.Instance.CheckForGameOver();
                    UiManager.instance.SetCostTexts();
            if (gameMode == GameMode.survival)
            {
                
                //turn off score counter,turn off highest score
                UiManager.instance.highestScoretxt.gameObject.SetActive(false);
                UiManager.instance.livescoretxt.gameObject.SetActive(false);
                //turn on Owl,turn on Timer
                UiManager.instance.SetOwlIcon();
                
                PowerManager.instance.DisablePowerUps();
                ScoreManager.instance.LoadHighestSurvive();
            }
            Advertisement.Instance.adiverySetUp();
                    
            }    
        
    }

    
    public bool ifInSurvival()
    {
        if(gameMode==GameMode.survival)
        {
            return true;
        }
        return false;
    }

    public void LoadGame()
    {
        StreamReader reader=null;
        Reset = false;
        try
        {
            reader = new StreamReader(Path);
            
            string savedfile= reader.ReadToEnd(); 
            reader.Close();
            if (savedfile != null)
            {
                State = JsonConvert.DeserializeObject<GameState>(savedfile);

            }
            else
            {
                
                restartGamme();
            }
        }
        catch
        {
           
            restartGamme();
        }
        
    }

    
    public void WriteToJson()
    {
        var json = JsonConvert.SerializeObject(State, setting);               
        
        
        File.WriteAllText(Path, json);
    }
    bool Reset;
   public void DeleteState()
    {
        StartCoroutine(Co_reset());
    }
    public Text resetMessage;
    IEnumerator Co_reset()
    {
        do
        {
            File.Delete(Path);
            yield return null;

        } while (File.Exists(Path));
        Reset = true;
        /*resetMessage.text = "Reset successfu";
        resetMessage.gameObject.SetActive(true);*/
        yield return new WaitForSeconds(0.7f);
        SceneManager.LoadScene(1);

    }


    public void restartGamme()
    {
        State.GridArray = new ulong[7, 5];
        
        State.Gems = 400;

        State.currentBlock = 2;
        State.next_block = 4;
        State.HighestBlock = 64;

        State.currentScore = 0;               
         
        State.BlockList=new List<int>();  
        for(int i = 1; i < 7; i++)
        {
            State.BlockList.Add(i); 
        }

        
    }


    public GameState State;
    

    private void OnApplicationFocus(bool focus)
    {
        
        if(!focus)
        {
            save();
        }
        

    }

    private void OnApplicationQuit()
    {
        save();
    }

    public void save()
    {
        if (gameMode == GameMode.survival) return;
        if (Reset) return;
        if (this == instance)
        {
            if(!GameStateManager.instance.Home)
            {
                GridManager.instance.SaveGrid();
            }
            
            WriteToJson();
        }
    }

   
    
    [Serializable]
    public  class GameState
    {

       public ulong HighestScore,HighestBlock;
       public int Gems;
       [SerializeField]
       public ulong[,] GridArray;
       public ulong currentScore;
       public ulong next_block;
       public ulong currentBlock;
       [SerializeField]
       public List<int> BlockList; 
    }
}
