using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GemManager))]
public class PowerManager : MonoBehaviour
{
    public static PowerManager instance { get; private set; }
    public Dictionary<Block,List<int>> SwapList;
    private int gems;
    public GemManager GemManager;

    public GameObject HammerBottun, SwapBottun;

    public int Gems
    {
        get { return gems; }
        set { gems = value; 
        }
    }

    public void DisablePowerUps()
    {
        HammerBottun.SetActive(false); 
        SwapBottun.SetActive(false);
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
    
    private void Start()
    {
        SwapList = new Dictionary<Block, List<int>>();
        GemManager = GetComponent<GemManager>();        
        

    }
    public void SetupIngameScene()
    {
        hammerIconOrigin = hammerIcon.transform.position;
    }

    public void LoadGems()
    {
        Gems = GameStateManager.instance.State.Gems;
        if(GameStateManager.instance.Home)
        {
            HomeUI.instance.gemtxt.text = GameStateManager.instance.State.Gems.ToString();
        }
        else
        {
            UiManager.instance.gemstxt.text = Gems.ToString();
        }
        
    }

   



    public void AddtoGems(int change)
    {
        
        
        Gems = Gems + change;
        
        GameStateManager.instance.State.Gems = Gems;
        if(GameStateManager.instance.Home)
        {
            HomeUI.instance.gemtxt.text = GameStateManager.instance.State.Gems.ToString();
        }
        else
        {
            UiManager.instance.gemstxt.text =
                        GameStateManager.instance.State.Gems.ToString();
        }
        
        GameStateManager.instance.save();
        
    }

    



    [SerializeField]
    GameObject HammerGuide,swapguid,toremove;

    public void CloseShop()
    {
        
        TouchManager.instance.ResumeTouch();
    }

    public void OpenShop()
    {
        Shop.instance.OpenShop();   
        TouchManager.instance.StopTouch();
    }
    public void StartHammering()
    {
        Application.targetFrameRate = -1;
        if (Gems < HammerCost)
        {
            //Not enough gems-get gems
            OpenShop();
            return;
        }
       
        GameBrain.Instance.GameOverMenu.SetActive(false);
        HammerGuide.gameObject.SetActive(true);
        TouchManager.instance.ActivateHammer();
        TouchManager.instance.ResumeTouch();

    }

    public void StartSwaping()
    {
        Application.targetFrameRate = -1;
        if(Gems < SwapCost)
        {
            OpenShop();
            return ;
        }
        GameBrain.Instance.GameOverMenu.SetActive(false);
        swapguid.gameObject.SetActive(true);
        TouchManager.instance.ActivateSwap();
        TouchManager.instance.ResumeTouch();

    }

    public void Swap(int row,int column)
    {
        Block block_1 = null;
        Block block_2 = GridManager.instance.GridArray[row, column];
        block_2.blockUI.swapLine.SetActive(true);
        if (SwapList.Count>1)return ;
        if(SwapList.ContainsKey(block_2))  return ;
        SwapList.Add(block_2, new List<int> { row,column});
        if(SwapList.Count==2)
        {
            
            foreach(var block in SwapList.Keys)
            {
                if(block!=block_2)
                {
                    block_1 = block;
                }
            }

            LeanTween.move(block_1.gameObject, block_2.transform.position, 1);
            LeanTween.move(block_2.gameObject, block_1.transform.position, 1).setOnComplete(()=>SetSwapArray(block_1,block_2));
        }
        else
        {
            Debug.Log("select next block");
        }
    }

    void SetSwapArray(Block block1,Block block2)
    {
        block1.blockUI.swapLine.SetActive(false);   
        block2.blockUI.swapLine.SetActive(false);
        SwapList.TryGetValue(block1, out var swap2);
        SwapList.TryGetValue(block2, out var swap1);
        GridManager.instance.GridArray[swap2[0],swap2[1]]=block2 ;
        GridManager.instance.GridArray[swap1[0],swap1[1]]=block1 ;  
        block2.IsActive = true; 
        block1.IsActive = true;
        StartCoroutine(swapLastEffect());
        
    }
    IEnumerator swapLastEffect()
    {
        AddtoGems(-SwapCost);
        SwapList.Clear();
        swapguid.gameObject.SetActive(false);   
        yield return new WaitForSeconds(1);
        GridActionManager.instance.CheckAllBlocksForFreeSpace();
        TouchManager.instance.DeActivateSwap();

    }

    public void Hammer(int row, int column)
    {

        LeanTween.move(hammerIcon,            
            GridManager.instance.GridArray[row, column].gameObject.transform.position, 1.3f)
            .setEaseInBack().setOnComplete(() => hammerdestroiAnimation(row, column));
    }
    [SerializeField]
    GameObject hammerIcon;
    Vector3 hammerIconOrigin;
    void hammerdestroiAnimation(int row, int column)
    {
        StartCoroutine(Co_hmmerdestroiAnimation(row, column));
    }
     
    public int HammerCost;
    
    public int SwapCost;
    

    IEnumerator Co_hmmerdestroiAnimation(int row, int column)
    {
        //block shatter animation play
        AddtoGems(-HammerCost);
        yield return new WaitForSeconds(0.5f);
        //hammerIcon.transform.parent=HammerGuide.transform;    
        BlockPool.instance.recycleBlock(GridManager.instance.GridArray[row, column]);
        GridManager.instance.GridArray[row, column] = null;
        yield return new WaitForSeconds(1);
        LeanTween.move(hammerIcon, hammerIconOrigin, 0.5f);
        yield return new WaitForSeconds(0.5f);
        HammerGuide.SetActive(false);
        GridActionManager.instance.CheckAllBlocksForFreeSpace();
        yield return new WaitForSeconds(0.5f);
        TouchManager.instance.DeActivateHammer();
    }

    public delegate void cancelSwap();
    public static event cancelSwap OnCancelSwap;    

    public void CancelPower()
    {
        OnCancelSwap();
        SwapList.Clear();
        TouchManager.instance.DeActivateHammer();
        TouchManager.instance.DeActivateSwap();
        HammerGuide.SetActive(false); 
        swapguid.SetActive(false);
    }
    
}
