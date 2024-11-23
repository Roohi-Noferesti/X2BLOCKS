using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    #region properties

    [field: SerializeField]
    public int GridRow { get; private set; }

    [field: SerializeField]
    public int GridColumn { get; private set; }

    [field: SerializeField]
    public float DeltaX { get; private set; }

    [field: SerializeField]
    public float DeltaY { get; private set; }

    [field: SerializeField]
    public float StartX { get; private set; }//x position of BOTTOM-Left (row=0,column=0)

    [field: SerializeField]
    public float StartY { get; private set; } //y position of BOTTOM-Left (row=0,column=0)

    [field: SerializeField]
    public float LenghtOfColumn { get; private set; }
    #endregion
    #region Fields    
    public static GridManager instance { get; private set; }
    public Block[,] GridArray;

    private float _lenghofColumn;

    #endregion


    public RectTransform panel;


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

        GridArray = new Block[GridRow, GridColumn];
    }
    private void Start()
    {
        TouchActivationDelay = new WaitForSeconds(0.2f);
    }
    private bool _gridBusy;

    public bool GridBusy
    {
        get { return _gridBusy; }
        set { _gridBusy = value;

            if (!value)
            {
                StartCoroutine(GridStop());
            }
            else
            {
                Application.targetFrameRate = -1;
            }

        }
    }
    int calls;
    WaitForSeconds TouchActivationDelay;
    [Range(5, 45)]
    public int HaltFrameRate;
    IEnumerator GridStop()
    {
        
        calls++;
        yield return new WaitForSeconds(0.1f);
        calls--;
        if (calls != 0) yield break;
        if (GridBusy) yield break;
        if(!SelectionUIManager.Instance.swaping)
        {
            Application.targetFrameRate = HaltFrameRate;
        }
        
        //TakeScreenShot();
        GridActionManager.instance.CheckAllBlocksForFreeSpace();
        ComboManager.instance.CheckCombo();
        LevelOrganizer.instance.CheckForNextGoal();
        GameBrain.Instance.CheckForGameOver();
        
        yield return TouchActivationDelay;
        TouchManager.instance.ResumeTouch();
        
           

    }
    
    private void TakeScreenSHot()
    {
        StartCoroutine(TakeShot()); 
    }
    int ImageCounter;
   IEnumerator TakeShot()
    {
        while(SelectionUIManager.Instance.swaping)
        {
            yield return null;
        }
        if(SelectionUIManager.Instance)
        yield return new WaitForEndOfFrame();
        ImageCounter++;
        string name= (ImageCounter % 10).ToString();
        if(name=="0")
        {
            name = "10";
        }
        ScreenCapture.CaptureScreenshot($"{name}.png");

        
    }

    private void TakeScreenShotToGallery()
    {
        StartCoroutine(Co_TakeScreenShotToGallery());
    }

    IEnumerator Co_TakeScreenShotToGallery()
    {
        yield return new WaitForEndOfFrame();
        Texture2D texture=new Texture2D(Screen.width, Screen.height,TextureFormat.RGB24,false);

        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
        
        string name = "screenshot.png";
        NativeGallery.SaveImageToGallery(texture, "X2Blocks", name);
        
        Debug.Log("taking screen");

        Destroy(texture);


    }

    public float PanelWidth, PanelHeight;
    

    public void GridSetup()
    {
        StartX = panel.position.x + 10;
        StartY = panel.position.y;

        _lenghofColumn = LenghtOfColumn;

        PanelHeight = panel.sizeDelta.y * UiManager.instance.Scalex;
        PanelWidth = panel.sizeDelta.x * UiManager.instance.ScaleY;
    }

    
    public  int ChooseGridColumn (float x)
    {
        float leftMosOfTouchArea = StartX ;
        

        float hitPointDistanceFromLeft = x - leftMosOfTouchArea;
        //print($"startx is{StartX}-hitpoint x is{x}-hitpointdistanse is{hitPointDistanceFromLeft}");


        int column = Mathf.Clamp((int)(hitPointDistanceFromLeft /
            ((GridManager.instance.panel.sizeDelta.x*UiManager.instance.Scalex)/GridManager.instance.GridColumn)), 0, GridManager.instance.GridColumn-1);
       
        return column;

    }

    public int chooseGridRow(float y)
    {
        float Bottom=StartY ;
        float hitpointDistanceFrombottom = y - Bottom;
        int row = Mathf.Clamp((int)(hitpointDistanceFrombottom /
            ((GridManager.instance.panel.sizeDelta.y * UiManager.instance.ScaleY) / GridManager.instance.GridRow)), 0, GridManager.instance.GridRow - 1);
        return row;
    }

    public void  SaveGrid()
    {
        for (int i = GridManager.instance.GridRow - 1; i > -1; i--)
        {
            for (int j = 0; j < GridManager.instance.GridColumn; j++)
            {
                if (GridManager.instance.GridArray[i, j] != null)
                {
                    GameStateManager.instance.State.GridArray[i, j] = GridManager.instance.GridArray[i, j].Value;
                }
                else
                {
                    GameStateManager.instance.State.GridArray[i, j] = 0;
                }
            }
        }
        
    }

    Vector2 LoadPosition(int row,int column)
    {
        
        var pos= GridActionManager.instance.GetPositionBasedOnGridIndex(column, row);

        return new Vector2(pos.x, pos.y);
    }


    public void LoadGrid()
    { 
        
        for (int i = GridManager.instance.GridRow - 1; i > -1; i--)
        {
            for (int j = 0; j < GridManager.instance.GridColumn; j++)
            {
                if (GameStateManager.instance.State.GridArray[i, j] != 0)
                {
                    
                    Block block = BlockPool.instance.GetPooledBlock().GetComponent<Block>();

                    if (block != null)
                    {
                        
                        GridManager.instance.GridArray[i, j] = block;
                        block.SetBlockValue(GameStateManager.instance.State.GridArray[i, j]);
                        block.blockUI.SetValueText(GameStateManager.instance.State.GridArray[i, j]);
                        block.blockUI.EnableText();
                        block.gameObject.SetActive(true);                        
                        block.transform.position = LoadPosition(i,j);
                    }
                    
                }
                else
                {
                    // this index is null-possible future features 
                }
            }
        }
        
    }

    
}
