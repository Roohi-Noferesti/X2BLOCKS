using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BlockColor))]
[RequireComponent(typeof(BlockUI))]
[RequireComponent(typeof(BlockMover))]
[RequireComponent(typeof(BlockMergeActionManager))]

public class Block : MonoBehaviour
{
    #region properties
    public ulong Value { get; private set; }

    bool isActive;
    public GameObject crown;
    public bool IsActive { get { return isActive; }  set {
            isActive = value;
           
        } }  //active blocks are the destination of merge
    #endregion
    #region fields
    BlockColor blockcolor;
    public BlockUI blockUI;
    public BlockMergeActionManager mergeActionManager;
    public Image _topMergeShadow, leftMergeShadow, rightMergeShadow,bottomMergeShadow;
    public BlockMover mover;
    public bool reserved;

    #endregion
    public ulong shadowValue;
    public List<Block> MergeRequesterList;
    private void Awake()
    {
        MergeRequesterList = new List<Block>();
        blockcolor = GetComponent<BlockColor>();
        blockUI = GetComponent<BlockUI>();
        mergeActionManager = GetComponent<BlockMergeActionManager>();
        mover = GetComponent<BlockMover>();
    }
    
    

    public void  ShootBlock()
    {
        SetBlockValue(GameBrain.Instance.CurrentBlock);
        blockUI.SetValueText(GameBrain.Instance.CurrentBlock);
    }

    public void BlocKDebug()
    {
        
       SetBlockValue(Value);
      blockUI.SetValueText(Value);
    }

    void Fade(bool fadeIn)
    {
       
        if (fadeIn)
        {
            blockcolor.FadeIn();
            
        }
        else
        {
            blockcolor.FadeOut();
            
        }
    }


    private void OnEnable()
    {
        
       reserved = false;
       LevelOrganizer.CrownHighDelegate += CrowStatus;
        GridActionManager.OnGridError += BlocKDebug;
        LevelOrganizer.FadeBlocksDelegate += Fade;
    }
    
    

    public void CrowStatus()
    {
       
        if (LevelOrganizer.instance == null) return;

        if(LevelOrganizer.instance.HighestBlock==0)
        {
            if (Value == 0)
            {
                crown.SetActive(true);
            }
            else
            {
                crown.SetActive(false);
            }
        }        
        else
        {
            if (Value == LevelOrganizer.instance.HighestBlock && Value > 256)
            {
                crown.SetActive(true);
            }
            else
            {
                crown.SetActive(false);
            }
        }
        
    }

    private ulong level;

    public ulong Level
    {
        get { return level; }
        set { level = value; }
    }

    private void OnDisable()
    {
        MergeRequesterList.Clear();
        Level = 0;
        blockUI.DisableInfinity();
        blockUI.EnableText();
        LevelOrganizer.CrownHighDelegate -= CrowStatus;
        GridActionManager.OnGridError -= BlocKDebug;
        LevelOrganizer.FadeBlocksDelegate -= Fade;
    }

    public void SetBlockValue(ulong val)
    {
        if(val==0)
        {
            blockUI.DisAbleText();
            blockUI.EnableInfinity();
        }
        Value = val;
        shadowValue = val;  
        CrowStatus();
        blockcolor.SetColor(Value);        
    }

   

    
}
