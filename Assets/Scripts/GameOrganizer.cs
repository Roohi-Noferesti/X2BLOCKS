using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOrganizer : MonoBehaviour
{
    [SerializeField]
    GameObject GameMenu,decision,confirm;

    enum menuState
    {
        reset,
        quit,
        play
    }

    int BlockGenerate;
   
    public void addToGeneratedBlocks()
    {
        BlockGenerate++;
        if(Debug.isDebugBuild)
        {
            if (BlockGenerate % 20 == 0)
            {
                Advertisement.Instance.interstitialSetUp();
            }
            else if (BlockGenerate % 5 == 0)
            {
                if (GameStateManager.instance.ifInSurvival())
                {

                    SurvaivalMode.Instance.StartSurvival();
                }
            }
        }
        else
        {
            if (BlockGenerate % 200 == 0)
            {
                Advertisement.Instance.interstitialSetUp();
            }
            else if (BlockGenerate % 5 == 0)
            {
                if (GameStateManager.instance.ifInSurvival())
                {

                    SurvaivalMode.Instance.StartSurvival();
                }
            }
        }
        
        
    
    }
    

    public void ShowMenu()
    {
        if(GameMenu.activeSelf)
        {
            GameMenu.SetActive(false);
            decision.SetActive(true);
            confirm.SetActive(false);
            TouchManager.instance.ResumeTouch();
            return;
        }
        
        GameMenu.SetActive(true);
        decision.SetActive(true);
        if(GameStateManager.instance.ifInSurvival())
        {
            UiManager.instance.GameRestart.SetActive(false);
        }
        
        confirm.SetActive(false);    
        TouchManager.instance.StopTouch();
    }



    menuState state;
    public void Yes()
    {
        switch (state)
        {
            case menuState.reset:

                restart();
                break;
            case menuState.quit:
                Quit();
                break;


        }
    }

    public void No()
    {
        play();
        GameMenu.SetActive(false);
        StartCoroutine(resume());
    }
        
    IEnumerator resume()
    {
        yield return new WaitForSeconds(0.3f);
        TouchManager.instance.ResumeTouch();
    }

    public void RestartBtn()
    {
        decision.SetActive(false);
        confirm.SetActive(true);
        state = menuState.reset;
    }
    void restart()
    {
        GameStateManager.instance.DeleteState();               
    }
    public void QuitBtn()
    {
        decision.SetActive(false);
        confirm.SetActive(true);
        state=menuState.quit;
    }
    private void Quit()
    {
        GameStateManager.instance.save();
        SceneManager.LoadScene(0);
    }

    public void play()
    {
        state = menuState.play;
    }

    private void Update()
    {
        if (GameMenu.gameObject.activeSelf)
        {
            GameMenu.transform.SetAsLastSibling();
        }
    }

    public static GameOrganizer instance { get; private set; }
    private void Awake()
    {
        if(instance != null && instance !=this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }


}
