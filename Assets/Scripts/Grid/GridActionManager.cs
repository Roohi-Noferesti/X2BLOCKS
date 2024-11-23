using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridActionManager : MonoBehaviour
{
    [SerializeField]
    private float _mergingTime;

    public float MergingTime
    {
        get
        {
            return _mergingTime;
        }
        
    }

    public float span;

    public static GridActionManager instance { get; private set; }  

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
    
    public (float x,float y) GetPositionBasedOnGridIndex(int column,int row)
    {
        float x = GridManager.instance.StartX + (column * ((GridManager.instance.PanelWidth) / GridManager.instance.GridColumn)) + (BlockPool.instance.BlockX / 2);
        float y = GridManager.instance.StartY + (row * ((GridManager.instance.PanelHeight) / GridManager.instance.GridRow)) + (BlockPool.instance.Blocky / 2);
        return (x,y);
    }


    public (Vector2 destination, int row) LandingPosition(int column,int startRow)
    {
        
        int emptyrow = -1;
        for (int i = startRow; i < GridManager.instance.GridRow; i++)
        {
            if(GridManager.instance.GridArray[i,column]==null)
            {
                emptyrow=i;

            }
            else
            {
                break;
            }
        }

        if(emptyrow!=-1)
        {
            
           var positions= GetPositionBasedOnGridIndex(column,emptyrow);
            
            return (new Vector2(positions.x, positions.y),emptyrow);
        }
        else
        {
            return (Vector2.zero,-1);
        }

    }//cc

    public void CurseFill()
    {
       StartCoroutine(CO_startCurse());
    }

    bool CurseFillRevesre;

    IEnumerator CO_startCurse()
    {
        
        while(GridManager.instance.GridBusy==true)
        {
            yield return null;
        }


        for (int i = 0; i < GridManager.instance.GridColumn; i++)
        {
            if(CurseFillRevesre)
            {
                if(i%2==0)
                {
                    GameBrain.Instance.CurrentBlock = GameBrain.Instance.IndexTo2x(BlockListManager.instance.getBlockFromList(0)); 
                }
                else
                {
                    GameBrain.Instance.CurrentBlock = GameBrain.Instance.IndexTo2x(BlockListManager.instance.getBlockFromList(4));
                }
            }
            else
            {
                if (i % 2 == 0)
                {
                    GameBrain.Instance.CurrentBlock = GameBrain.Instance.IndexTo2x(BlockListManager.instance.getBlockFromList(4));
                }
                else
                {
                    GameBrain.Instance.CurrentBlock = GameBrain.Instance.IndexTo2x(BlockListManager.instance.getBlockFromList(0));
                }
            }
            


            
            BlockPool.instance._generateBlock(i);   
           

        }
        CurseFillRevesre = !CurseFillRevesre;
        yield return new WaitForSeconds(0.3f);
        TouchManager.instance.ResumeTouch();
    }

    [ContextMenu ("check for free")]

    public void removeErrorCells()
    {
        for (int i = GridManager.instance.GridRow - 1; i > -1; i--)
        {
            for (int j = 0; j < GridManager.instance.GridColumn; j++)
            {
                if (GridManager.instance.GridArray[i, j] != null)
                {
                    if (!GridManager.instance.GridArray[i, j].gameObject.activeInHierarchy)
                    {
                        GridManager.instance.GridArray[i, j] = null;
                    }

                }
            }
        }
       // StartCoroutine(Co_RemoveErrorsCells());
    }

    [ContextMenu("errr")]
    public void BlockDebug()
    {
        OnGridError();
        List<Block> IngridBlocks = new List<Block>(); 
        var alblocks=BlockPool.instance.returnAllBlocks();

        foreach (var cell in GridManager.instance.GridArray)
        {
              IngridBlocks.Add(cell);   
        }

        foreach(var block in alblocks)
        {
            if(block.gameObject.activeInHierarchy && !IngridBlocks.Contains(block))
            {
                Debug.Log(block.name);  
                block.gameObject.SetActive(false);  
            }
        }

    }


    IEnumerator Co_RemoveErrorsCells()
    {
        yield return new WaitForEndOfFrame();
        for (int i = GridManager.instance.GridRow - 1; i > -1; i--)
        {
            for (int j = 0; j < GridManager.instance.GridColumn; j++)
            {
                if (GridManager.instance.GridArray[i, j] != null)
                {
                    if (!GridManager.instance.GridArray[i, j].gameObject.activeInHierarchy)
                    {
                        GridManager.instance.GridArray[i, j] = null;
                    }

                }
            }
        }
    }
    public delegate void GridError();
    public static event GridError OnGridError;
    public void  CheckAllBlocksForFreeSpace()
    {

       // removeErrorCells();
        
        bool HaveMoves=false;
        for(int i = GridManager.instance.GridRow-1; i > -1; i--)
        {
            for(int j = 0; j < GridManager.instance.GridColumn; j++)
            {
                if(GridManager.instance.GridArray[i,j]!=null)
                {
                    //GameStateManager.Instance.State.GridArray[i, j] = GridManager.instance.GridArray[i, j].Value;
                   
                   var landinginfo= LandingPosition(j, i+1);
                    if(landinginfo.row!=-1)
                    {
                        Block activeBlock = GridManager.instance.GridArray[i, j];
                        activeBlock.IsActive = true;
                        activeBlock.mover.
                            MoveToFirstEmpty(j,landinginfo.row,landinginfo.destination,i);
                        HaveMoves=true;
                    }
                    else
                    {
                        Block activeBlock = GridManager.instance.GridArray[i, j];
                        FindNeighborBlocks(i, j, activeBlock);
                        
                    }
                }
                
                
            }
        }
        if(HaveMoves)
        {
            GridManager.instance.GridBusy = true;
        }
        else
        {
           // BlockDebug();
        }
        
    }

    private void FindNeighborBlocks(int i,int j,Block active)
    {
       
        
        if(!active.IsActive) return ;
        if (active.mover.IsMoving()) return;
        if(active.mergeActionManager.Merging) return ;
        
        StartCoroutine(Co_FindNeighborBlocks(i,j,active));       
       
    }

   

    private IEnumerator Co_FindNeighborBlocks(int i, int j, Block active)
    {
        
        ulong levelUp = 0;
        BlockMergeActionManager blockMerg = active.mergeActionManager;
        bool top=false, bottom = false, right = false, left = false;
        Block topB=null,bottomB=null,leftB=null,rightB=null;
        List<Block> listOfSameNeighbors=new List<Block>();
        
        if (j != 0)
        {
            // block is not in left most Column

             leftB = GridManager.instance.GridArray[i, j - 1];
            
            if (leftB != null && leftB.Value == active.Value && !leftB.mergeActionManager.Merging &&!leftB.mover.IsMoving() &&!leftB.reserved )
            {
                leftB.IsActive = false;
                //merge left to active
                leftB.reserved = true;
                if(!leftB.MergeRequesterList.Contains(active)) leftB.MergeRequesterList.Add(active);
                left = true;
                if(!listOfSameNeighbors.Contains(leftB)) listOfSameNeighbors.Add(leftB);

                levelUp++;
            }
        }
        if (j < GridManager.instance.GridColumn - 1)
        {
            //block is not in Right most Column
             rightB = GridManager.instance.GridArray[i, j + 1];
            
            if (rightB != null && rightB.Value == active.Value && !rightB.mergeActionManager.Merging && !rightB.mover.IsMoving()&& !rightB.reserved)
            {
                rightB.IsActive = false;
                rightB.reserved = true;
                if(!rightB.MergeRequesterList.Contains(active)) rightB.MergeRequesterList.Add(active);

                right = true;
                //merge right to active

                if(!listOfSameNeighbors.Contains(rightB)) listOfSameNeighbors.Add((rightB));
                
                levelUp++;
            }
            
        }
        if (i < GridManager.instance.GridRow - 1)
        {
            // block is not in top-most row

             topB = GridManager.instance.GridArray[i + 1, j];
               
            if (topB != null && topB.Value == active.Value && !topB.mergeActionManager.Merging && !topB.mover.IsMoving()&& !topB.reserved)
            {
                topB.IsActive = false;
                topB.reserved = true;   
                //merge top to active
                top=true;
                if (!topB.MergeRequesterList.Contains(active)) topB.MergeRequesterList.Add(active);
                
                if(!listOfSameNeighbors.Contains(topB))listOfSameNeighbors.Add(topB);
                                                      
                levelUp++;
            }
        }
        if(i>0)
        {
            bottomB = GridManager.instance.GridArray[i - 1, j];
            if (bottomB != null && bottomB.Value == active.Value && !bottomB.mergeActionManager.Merging && !bottomB.mover.IsMoving() &&!bottomB.reserved)
            {

                bottomB.IsActive = false;
                bottomB.reserved = true;
                bottom = true;
                if (!bottomB.MergeRequesterList.Contains(active)) bottomB.MergeRequesterList.Add(active);

                if (!listOfSameNeighbors.Contains(bottomB)) listOfSameNeighbors.Add(bottomB);

                levelUp++;
            }
        }
        
        active.Level = levelUp;
        if(levelUp==0)
        {
            GridManager.instance.GridBusy = false;
            
            active.IsActive = false;
            yield break;
        }
        yield return null;
        bool neighborToactive = true;




        foreach (var neighbor in listOfSameNeighbors)
        {
            if(neighbor.MergeRequesterList.Count>1)
            {
                //there is conflict in merge-two blocks try to merge into a same block in different ways

                neighbor.reserved = false;
                ulong lastlevel = 0;
                Block lastrequester=null;
                foreach(var requester in neighbor.MergeRequesterList)
                {
                    requester.IsActive = false;
                    ulong currentlevel = requester.Level;
                    if (lastlevel != 0)
                    {
                        if (currentlevel > lastlevel)
                        {
                            requester.IsActive = true;
                            neighborToactive = false;
                        }
                        else if (currentlevel < lastlevel)
                        {
                            lastrequester.IsActive = true;
                            neighborToactive = false;
                        }
                    }
                    requester.Level = 0;
                    lastrequester = requester; 
                    lastlevel=currentlevel;
                }
                if(neighborToactive)
                {
                    neighbor.IsActive = true;                    
                }
                neighbor.MergeRequesterList=new List<Block>();
                //active.Inprocess = false;
                //StartCoroutine(Co_checkforfreespaces());
                CheckForMerge();
                yield break;
            }
            else
            {
                neighbor.MergeRequesterList = new List<Block>();
            }
        }
        if(top)
        {
            if(levelUp==1)
            {
                active.IsActive = false;
                
                topB.IsActive = true;
                topB.reserved = false;
                //active.Inprocess = false;
                CheckForMerge();
                yield break;

            }
        }
       
        ulong newval = 0;
        if (levelUp>0)
        {
            newval = LevelUp(levelUp, active);
        }
        
        if (top)
        {

            blockMerg.MergeShadow(active._topMergeShadow.gameObject, _mergingTime, i + 1, j, newval);

            MergeSameBlocksToActive(topB);
        }
        if(bottom)
        {
            blockMerg.MergeShadow(active.bottomMergeShadow.gameObject, _mergingTime, i -1, j, newval);
            MergeSameBlocksToActive(bottomB);
        }
        if(right)
        {
            blockMerg.MergeShadow(active.rightMergeShadow.gameObject, _mergingTime, i, j + 1, newval);
            MergeSameBlocksToActive(rightB);
        }
        if(left)
        {
            blockMerg.MergeShadow(active.leftMergeShadow.gameObject, _mergingTime, i, j - 1, newval);
            MergeSameBlocksToActive(leftB);
        }

        if (levelUp > 0)
        {
            TouchManager.instance.StopTouch();
            BlockMergeActionManager blockMerger=active.mergeActionManager;
            
            active.blockUI.SetValueText(newval);
            active.shadowValue=newval;
            GridManager.instance.GridBusy = true;

            AudiManager.instance.PlayMerge();
            ComboManager.instance.ComboUp();
            while (blockMerger.Merging)
            {
                yield return null;
                
            }
            active.SetBlockValue(newval);
            active.IsActive = true;
            GridManager.instance.GridBusy = false;
            
            LevelOrganizer.instance.UpdateHighestBlock(newval);
            
            
            
            CheckAllBlocksForFreeSpace();
            
        }
        

    }

    private void MergeSameBlocksToActive( Block neighbor)
    {        
        BlockPool.instance.recycleBlock(neighbor);
        
    }

    

    private ulong LevelUp(ulong level,Block active)
    {
        
        ulong score = (active.Value * ((ulong)Mathf.Pow(2, level)));
        if(score>Mathf.Pow(2,30))
        {
            return 0;
        }
        ScoreManager.instance.AddToScore(score);
        return score;
        
    }


    
    public void CheckForMerge()
    {
        for(int i = 0; i < GridManager.instance.GridRow; i++)
        {
            for(int j = 0; j < GridManager.instance.GridColumn; j++)
            {
                Block block = GridManager.instance.GridArray[i,j];
                if (block == null) continue;
                
                 FindNeighborBlocks(i,j,block);                
                
            }  
        }
    }


    public bool CheckForDoubleMerge(int neighborColumn,int neighborRow)
    {

        return false;
    }
}
