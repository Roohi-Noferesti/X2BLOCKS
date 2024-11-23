using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BlockListManager : MonoBehaviour
{
    [SerializeField]
    private GameObject BlockMenu;
    [SerializeField]
    private Image blockImage;
    [SerializeField]
    private TextMeshProUGUI blockText,message;
    public static BlockListManager instance { get; private set; }
    private void Awake()
    {
        if(instance!=null && instance!=this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;    
        }
        
    }
    

    public void SetSelectionBlocksList()
    {
        blocks = GameStateManager.instance.State.BlockList;
    }


    bool _newBlockSequence,_oldBlockSequence,_goal,_nextGoal;
    

    public int getBlockFromList(int index)
    {
        return blocks[index];
    }

    public void Continue()
    {
        if(_newBlockSequence)
        {
            _newBlockSequence = false;
            _oldBlockSequence = true;
            message.text = "old block dismissed";

            

            blockImage.color = ColorManager.Instance.GetColor(removed);
            blockText.text = removed.ToString();
            return;

        }
        if(_oldBlockSequence)
        {
            BlockMenu.SetActive(false);
            _oldBlockSequence=false;
            
            GameBrain.Instance.removeBlockFromGrid(removed);
            NextGoal();
            return;
        }
        if(_goal)
        {
            _goal = false;
            message.text = "Your Next Goal";
            GemRewardOnNewBlock();
            blockImage.color = ColorManager.Instance.GetColor(LevelOrganizer.instance.HighestBlock*2);
            blockText.text = UiManager.instance.ConverNumberToString(LevelOrganizer.instance.HighestBlock*2);    

            _nextGoal = true;
            return;
        }
        if(_nextGoal)
        {
            _nextGoal=false;
            BlockMenu.SetActive(false); 
            StartCoroutine(ResumeGame());
        }
        //cc
    }

    IEnumerator ResumeGame()
    {
        yield return new WaitForSeconds(1);
        GridActionManager.instance.CheckAllBlocksForFreeSpace();
        yield return new WaitForSeconds(1);
        TouchManager.instance.ResumeTouch();
    }

    private List<int> blocks=new List<int>(6);

    public int GetFirstSelectioFromBlockList()
    {
        int res = blocks[0];
        return res;        

    }
    public void CheckForDismissedBlockInSelectionMenu( )
    {
        bool currentBlockNeedsChange=false;
        bool nextBlockNeedsChange = false;
        
        
            
            if (GameBrain.Instance.CurrentBlock==removed)
            {
                currentBlockNeedsChange=true;
                
            }
        
        
            if (GameBrain.Instance.NextBlock == removed)
            {
                nextBlockNeedsChange = true;
                
            }
        

        if(currentBlockNeedsChange)
        {
            channgeCurrentBlock();
        }
        if(nextBlockNeedsChange)
        {
            changeNextBlock();
        }

    }

    private void changeNextBlock()
    {
        GameBrain.Instance.NextBlock=GameBrain.Instance.CalculateNextBlock();
        SelectionUIManager.Instance.SetCurrentImage(GameBrain.Instance.CurrentBlock);
    }

    private void channgeCurrentBlock()
    {
        GameBrain.Instance.CurrentBlock = GameBrain.Instance.CalculateNextBlock();
        SelectionUIManager.Instance.setNextImage(GameBrain.Instance.CurrentBlock);
    }
   

    public void GemRewardOnNewBlock(float delay=0)
    {
        startGemAnim(PowerManager.instance.GemManager.NewBlockReward);
       // AudiManager.instance.PlayGem();
        
    }
    void startGemAnim(int toreward,float delay=0)
    {
        StartCoroutine(Co_GemRewardProcess(toreward,delay));
    }

    void newBlockReward()
    {
        PowerManager.instance.AddtoGems(PowerManager.instance.GemManager.NewBlockReward);
    }
    IEnumerator Co_GemRewardProcess(int toreward,float delay=0)
    {
        if(delay!=0)
        {
            yield return new WaitForSeconds(delay);
        }
        GemAnim(toreward);
        yield return new WaitForSeconds(1);
        newBlockReward();
    }

    [ContextMenu("GemAnim")]
    public void GemAnim(int ToReward)
    {
        if(!GameStateManager.instance.Home)
        {
            foreach (var gem in PowerManager.instance.GemManager.GemIcons)
            {
                gem.SetActive(true);
                LeanTween.move(gem, UiManager.instance.gemstxt.transform, 1)
                    .setOnComplete(() => PowerManager.instance.GemManager.DisableGemIcons());
            }
        }
        
        GemManager.Instance.AnimateGemNote($"{ToReward.ToString()}+");
    }


    ulong removed;
    
    public void NextGoal()
    {
        _goal = true;
        BlockMenu.SetActive(true);
        message.text = "New Block achived";
        blockImage.color = ColorManager.Instance.GetColor(LevelOrganizer.instance.HighestBlock);
        blockText.text = UiManager.instance.ConverNumberToString( LevelOrganizer.instance.HighestBlock);
    }

    public void UnblockNext()
    {
        
        _newBlockSequence=true;
        BlockMenu.SetActive(true);
        message.text = "new block UNBLOCKED";
        TouchManager.instance.StopTouch();
        removed = GameBrain.Instance.IndexTo2x(blocks[0]);
        blocks[0]=blocks[5]++;        
        blocks.Sort();
        //save the new list
        GameStateManager.instance.State.BlockList=blocks;
        CheckForDismissedBlockInSelectionMenu();
        ulong newb = GameBrain.Instance.IndexTo2x( blocks[5]);

        blockImage.color=ColorManager.Instance.GetColor(newb);
        blockText.text=newb.ToString(); 
        
    }

    private void Update()
    {
        if(_newBlockSequence|| _goal)
        {
            BlockMenu.transform.SetAsLastSibling();
            TouchManager.instance.StopTouch();
        }
    }


}
