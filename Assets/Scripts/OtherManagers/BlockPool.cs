using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPool : MonoBehaviour
{
    public static BlockPool instance { get; private set; }
    private List<GameObject> pooledBlocks;
    public float BlockX, Blocky;
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
    int _maximumBlocks;
    

    public void InitializeBlocks()
    {
        pooledBlocks = new List<GameObject>();
        _maximumBlocks = GridManager.instance.GridColumn * (GridManager.instance.GridRow + 1);

        GameObject block;
        for (int i = 0; i < _maximumBlocks; i++)
        {
            block = Instantiate(_block, gameObject.transform.position, Quaternion.identity);
            block.name = i.ToString();
            block.SetActive(false);
            pooledBlocks.Add(block);
            block.transform.parent = GridManager.instance.panel;
            block.transform.localScale = Vector3.one;
        }
        listOfBlocks();
    }

    void listOfBlocks()
    {
        foreach (GameObject block in pooledBlocks)
        {
            blocks.Add(block.GetComponent<Block>());
        }
    }
    List<Block> blocks = new List<Block>();
    public List<Block> returnAllBlocks()
    {
        return blocks;
    }
    public void AuthorBlockScaledDimension()
    {
        BlockX = pooledBlocks[0].GetComponent<RectTransform>().sizeDelta.x * UiManager.instance.Scalex;
        Blocky = pooledBlocks[0].GetComponent<RectTransform>().sizeDelta.y * UiManager.instance.ScaleY;
    }

    [SerializeField]
    private GameObject _block;

    public GameObject GetPooledBlock()
    {
        for(int i=0; i < _maximumBlocks;i++)
        {
            if(!pooledBlocks[i].activeInHierarchy)
            {
                GameBrain.Instance.BlockCount++;
                
                return pooledBlocks[i];
            }
            
        }
        return null;
    }

    IEnumerator waitForBlockMerg(BlockMergeActionManager active,int column,ulong newVal )
    {

        while(active.Merging)
        {
            yield return null;
        }
        
        active.GetComponent<Block>().SetBlockValue(newVal);
        yield return 0;
        GridActionManager.instance.CheckAllBlocksForFreeSpace();
    }


    public void _generateBlock(int Column)
    {
        GridActionManager.instance.removeErrorCells();
        var landingInfo = GridActionManager.instance.LandingPosition(Column,0);//var is a TUPLE
        if (landingInfo.row == -1)
        {
            Block FirstIncolumn = GridManager.instance.GridArray[0, Column];
            FirstIncolumn.IsActive=true;
            if (FirstIncolumn.Value==GameBrain.Instance.CurrentBlock)
            {
                ulong newval = FirstIncolumn.Value * 2;
                
                FirstIncolumn.blockUI.SetValueText(newval); 
                FirstIncolumn.GetComponent<BlockMergeActionManager>()
                    .MergeShadow(FirstIncolumn.bottomMergeShadow.gameObject,
                    GridActionManager.instance.MergingTime, -1, Column,newval );
                SelectionUIManager.Instance.ReadyImageReload();
                StartCoroutine(waitForBlockMerg(
                    FirstIncolumn.GetComponent<BlockMergeActionManager>()
                    , Column, newval));

                AudiManager.instance.Playshoot();
                
            }
            //column is full of blocks
            return;
        }
        //counting the generations
        
        SelectionUIManager.Instance.ReadyImageReload();
        Vector2 origin=BlockOriginPOsition(Column);
        AudiManager.instance.Playshoot();
        GameObject newBlock = GetPooledBlock();

        if(newBlock != null)
        {
            newBlock.transform.position = origin;
            newBlock.SetActive(true);

            Block block= newBlock.GetComponent<Block>();

            block.ShootBlock();
            block.blockUI.EnableText();            
            block.mover.MoveToFirstEmpty(Column,
            landingInfo.row, landingInfo.destination);
        }
    }


       
        

    Vector2 BlockOriginPOsition(int Column)
    {

        //this method needs refactoring
        float x = GridManager.instance.StartX + (Column * ((GridManager.instance.PanelWidth) / GridManager.instance.GridColumn)) + (BlockPool.instance.BlockX / 2);
        float y = GridManager.instance.StartY - (GridManager.instance.DeltaY );
        return new Vector2(x, y);   
    }

    public void recycleBlock(Block block)
    {
        GameBrain.Instance.BlockCount--;
        block.GetComponent<BlockUI>().DisAbleText();
        block.gameObject.SetActive(false);  
    }

    private void OnEnable()
    {
        TouchManager.NewBlockRequestEvent += _generateBlock;
    }
    private void OnDisable()
    {
        TouchManager.NewBlockRequestEvent -= _generateBlock;
    }
}

