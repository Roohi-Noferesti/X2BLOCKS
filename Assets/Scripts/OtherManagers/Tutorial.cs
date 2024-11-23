using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    // Start is called before the first frame update
   public static Tutorial Instance { get; private set; }
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

    public void CheckForTutorial()
    {
        if(PlayerPrefs.GetString("Tut")==string.Empty)
        {
            showTutorial();
            PlayerPrefs.SetString("Tut", "shown");
        }
    }

    public int activeColumn;
    public GameObject TutorialHand;
    public int ActiveColumn { get { return activeColumn; } set { activeColumn = value;
        MakeOtherColumnsDark();
        
        } }
    void showTutorial()
    {
        //this property invokes column darkening
        ActiveColumn = 2;

         

        

        TouchManager.instance.ActivateTutorial();
        TutorialHand.SetActive(true);   
        MakeTutorialContextBlocks();

    }


    void MakeTutorialContextBlocks()
    {
        GameObject block1 = BlockPool.instance.GetPooledBlock();
        block1.SetActive(true);
        GameObject block2 = BlockPool.instance.GetPooledBlock();
        block2.SetActive(true);

         var pos1 = GridActionManager.instance.GetPositionBasedOnGridIndex(1, 6);
          var pos2 = GridActionManager.instance.GetPositionBasedOnGridIndex(3, 6);
                

        block1.transform.position = new Vector3(pos1.x, pos1.y);
        block2.transform.position = new Vector3(pos2.x, pos2.y);

        Block blockA= block1.GetComponent<Block>();
        Block blockB = block2.GetComponent<Block>();

        GridManager.instance.GridArray[6, 1] = blockA;
        GridManager.instance.GridArray[6, 3] = blockB;


        blockA.SetBlockValue(2);
        blockB.SetBlockValue(2);




        block1.GetComponent<BlockUI>().EnableText();
        block2.GetComponent<BlockUI>().EnableText();
    }
    void MakeOtherColumnsDark()
    {
        UiManager.instance.LighitUpAcolumnAndDarkenTheRest(ActiveColumn);
    }

    public void FinishTutorial()
    {
        TutorialHand.SetActive(false);
        StartCoroutine(Co_finishTut());
    }

    IEnumerator Co_finishTut()
    {
        yield return new WaitForSeconds(0.5f);
        UiManager.instance.ResetColumnColors();
        TouchManager.instance.DeActivateTutorial();
    }






}
