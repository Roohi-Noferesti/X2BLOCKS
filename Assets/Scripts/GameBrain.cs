using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameBrain : MonoBehaviour
{
    public static GameBrain Instance { get; private set; }
    private void Awake()
    {
        if(Instance != null && Instance!=this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;    
        }
        
    }
   

    public void SetUpBlockList()
    {
        state = GameState.playing;
        if(GameStateManager.instance.State.currentBlock==0)
        {
            CurrentBlock = IndexTo2x(BlockListManager.instance.GetFirstSelectioFromBlockList());
        }
        else
        {
            CurrentBlock =(ulong) GameStateManager.instance.State.currentBlock;
        }

        if(GameStateManager.instance.State.next_block!=0)
        {
            NextBlock = (ulong) GameStateManager.instance.State.next_block;
        }

        Time.timeScale = 1.3f;
        TotalCountOfCells = GridManager.instance.GridColumn * GridManager.instance.GridRow;
    }


    ulong currentBlockCache;

    public void SetCurrentBLockToCache()
    {
        currentBlockCache = CurrentBlock;
    }

    public void SetCurrentBlockFromCach()
    {
        CurrentBlock = currentBlockCache;
    }


    private ulong _currentBlock;
    public ulong CurrentBlock { 
        get { return _currentBlock; }
        set {
            _currentBlock = value;
            SelectionUIManager.Instance.SetCurrentImage(_currentBlock);
            GameStateManager.instance.State.currentBlock = value;
            
        } 
    }
    
    public int TotalCountOfCells { get; private set; }

    private ulong _nextBlock;
    public ulong NextBlock
    {
        get { return _nextBlock; }
        set
        {
            _nextBlock = value;
            SelectionUIManager.Instance.setNextImage(_nextBlock);
        }
    }


    public void SwapCurrentToNext()
    {
        CurrentBlock = NextBlock;
    }
    public void SetNextBlock()
    {
        NextBlock = CalculateNextBlock();
        GameStateManager.instance.State.next_block = NextBlock;
    }
    public void ConsumeReadyBlock(Image readyImage)
    {

        //Object Pool Needs Connection
        CurrentBlock = _nextBlock;
        _nextBlock=CalculateNextBlock();
    }

    int getArandomIndex()
    {
        var rand = UnityEngine.Random.Range(0, 6);
        rand = BlockListManager.instance.getBlockFromList(rand);
        return rand;
    }

    public ulong CalculateNextBlock()
    {
        var rand = getArandomIndex();
        
        int HelpPercentage=0;
        getTheAvailAbleMerges();
        switch(helper)
        {
            case HelperLevel.none:
                
                return IndexTo2x(rand);
            case HelperLevel.weak:
                
                HelpPercentage = UnityEngine.Random.Range(1, 11);
                if (HelpPercentage > 8)
                {
                    rand = getforMerge(rand);
                }
                //30% helper
                return IndexTo2x(rand);
            case HelperLevel.medium:
                
                HelpPercentage = UnityEngine.Random.Range(1, 11);
                //50% helper
                if(HelpPercentage > 7)
                {
                    rand=getforMerge(rand);
                }

                return IndexTo2x(rand);
            case HelperLevel.strong:
                
                HelpPercentage = UnityEngine.Random.Range(1, 11);
                //70% helper
                if (HelpPercentage > 6)
                {
                    rand = getforMerge(rand);
                }

                return IndexTo2x(rand);
            default:
                return IndexTo2x(rand);
        }
        

    }

    int getforMerge(int rand)
    {
        
        if(availableMerges.Count == 0)
        {
            return rand;
        }
        int counter=0;
        while (!availableMerges.Contains(IndexTo2x(rand)))
        {
            rand = getArandomIndex();
            counter++;
            if(counter==50)
            {
                return rand;
            }
        }
        return rand;
    }
    public List<ulong> availableMerges;
    void getTheAvailAbleMerges()
    {
        availableMerges.Clear();
            for(int j = 0; j < GridManager.instance.GridColumn; j++)
            {
                for (int i = 0; i < GridManager.instance.GridRow; i++)
                {
                    if(GridManager.instance.GridArray[i,j]!=null)
                    {
                         availableMerges.Add(GridManager.instance.GridArray[i, j].Value);
                         break;
                    }
                }
            }
        
    }


    int blockcount;
    public void CheckForGameOver()
    {
       
        if (blockcount == TotalCountOfCells)//)
        {

            getTheAvailAbleMerges();
            
            if (!availableMerges.Contains(GameBrain.Instance.CurrentBlock))
            {
                GameOver();
            }

        }
    }
    public int BlockCount {
        get { return blockcount; }
        set
        {
            blockcount = value;           
            
            helperLevel();
        }

    }
    
    public  GameObject GameOverMenu;

    public enum GameState
    {
        playing,
        notPlaying

    }

    public GameState state;

    public bool CheckIfPlayin()
    {
        if(state==GameState.playing)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [SerializeField]
    Button hammer, swap, watchAd;
    IEnumerator Co_GameOver()
    {
        if(GameStateManager.instance.ifInSurvival())
        {
            SceneManager.LoadScene(0);
            yield break;
        }
        TouchManager.instance.StopTouch();
        yield return new WaitForSeconds(1);
        CheckForPowerUpsInGameOver();
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            watchAd.interactable = false;
        }
        else
        {
            watchAd.interactable = true;
        }


        GameOverMenu.SetActive(true);
        GameOverMenu.transform.SetAsLastSibling();
        state = GameState.notPlaying;
        TouchManager.instance.StopTouch();
    }
    public void CheckForPowerUpsInGameOver()
    {
        if (PowerManager.instance.Gems < PowerManager.instance.HammerCost)
        {
            hammer.interactable = false;
        }
        else
        {
            hammer.interactable = true;
        }

        if (PowerManager.instance.Gems < PowerManager.instance.SwapCost)
        {
            swap.interactable = false;
        }
        else
        {
            swap.interactable = true;
        }
    }
    private void GameOver()
    {
        StartCoroutine(Co_GameOver());
    }

    enum HelperLevel
    {
        weak,
        medium,
        strong,
        none
    }

    HelperLevel helper;

    void helperLevel()
    {
        
        if(blockcount> Mathf.RoundToInt(TotalCountOfCells*0.8f))
        {
            helper=HelperLevel.strong;
        }else if(blockcount > Mathf.RoundToInt(TotalCountOfCells * 0.6f))
        {
            helper = HelperLevel.medium;
        }else if (blockcount > Mathf.RoundToInt(TotalCountOfCells * 0.4f))
        {
            helper = HelperLevel.weak;
        }
        else
        {
            helper = HelperLevel.none;
        }
        if (Debug.isDebugBuild)
        {
           // Debug.Log(helper);
        }
    }

    public ulong IndexTo2x(int index)
    {
        return (ulong)Mathf.Pow(2, index);
    }

    public void removeBlockFromGrid(ulong val)
    {
        for (int i = GridManager.instance.GridRow - 1; i > -1; i--)
        {
            for (int j = 0; j < GridManager.instance.GridColumn; j++)
            {
                Block block = GridManager.instance.GridArray[i, j];
                if (block != null)
                {
                    if(block.Value==val)
                    {
                        GridManager.instance.GridArray[i, j] = null;
                        BlockPool.instance.recycleBlock(block);
                    }
                }
            }
        }
        
        GridActionManager.instance.CheckAllBlocksForFreeSpace();
        
    }

}
