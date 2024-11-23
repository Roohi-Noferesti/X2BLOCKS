using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public Camera MainCamera;
    public Canvas MainCanvas;
    public TextMeshProUGUI gemstxt, livescoretxt, highestScoretxt,survivalScore,SurvivalHighScore;
    public float Scalex, ScaleY, ScaleZ;
    [SerializeField] GameObject SurviveMode;
    [SerializeField]
    private GameObject _column_prefab;
    [SerializeField]
    TextMeshProUGUI HammerCost,SwapCost,AdReward;
    
    public GameObject RemoveAdPurchase,GameRestart;

    private void Start()
    {
        if(Debug.isDebugBuild)
        {
            if(!GameStateManager.instance.Home)
            {
                RemoveAdPurchase.SetActive(true);
            }
            
        }
    }
    public static UiManager instance
    {
        get;
        private set;
    }

    public void EnableSuriveModeMenu()
    {
        SurviveMode.SetActive(true);
    }

    public void GetCanvasRatio()
    {
          
        Scalex = MainCanvas.GetComponent<RectTransform>().localScale.x;
        ScaleY = MainCanvas.GetComponent<RectTransform>().localScale.y;
        BlockPool.instance.AuthorBlockScaledDimension();
    }
    Color ColumnDefaultColor;
    public void LighitUpAcolumnAndDarkenTheRest(int ShiningColumn)
    {
        int counter = 0;
        foreach (var col in _column_list)
        {
            if(counter !=ShiningColumn )
            {
               
                Color color = Color.black;
                color.a = 0.5f;
                col.GetComponent<Image>().color = color;
            }
            else
            {
                ColumnDefaultColor=col.GetComponent<Image>().color;
            }
            
            counter++;
        }
    }

    [ContextMenu("columnReset")]
    public void ResetColumnColors()
    {
        foreach(var col in _column_list)
        {
            ColorManager.Instance.ImageColorLerp(col.GetComponent<Image>(),
                col.GetComponent<Image>().color, ColumnDefaultColor, 1);
        }
    }


    List<GameObject> _column_list = new List<GameObject>();
    public void SetColumns()
    {
        for (int i = 0; i < GridManager.instance.GridColumn; i++)
        {
            GameObject col = Instantiate(_column_prefab);
            float x = GridManager.instance.StartX + (i * ((GridManager.instance.PanelWidth) / GridManager.instance.GridColumn));
            float y = GridManager.instance.StartY-10;
            col.transform.parent = GridManager.instance.panel;
            col.transform.position = new Vector3(x, y, 0);
            col.transform.localScale = Vector3.one;
            _column_list.Add(col);
            
        }
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

    public Image owl,crown,highest;
    

    public void SetOwlIcon()
    {
        owl.gameObject.SetActive(true); 
        crown.gameObject.SetActive(false);
        highest.gameObject.SetActive(false); 
        EnableSuriveModeMenu();
        
    }

    public void SetCostTexts()
    {
        AdReward.text = Advertisement.Instance.rewardAddGem.ToString() + "+";
        HammerCost.text = PowerManager.instance.HammerCost.ToString();
        SwapCost.text = PowerManager.instance.SwapCost.ToString();
    }

    public string ConverNumberToString(float value)
    {
        return ConverNumberToString((ulong)value);
    }

    public string ConverNumberToString(ulong value)
    {
        if (value < 10000)
        {
            return $"{value}";
        }
        else if (value < 1000000)
        {
            return $"{value / 1000}K";
        }
        else if (value < 1000000000)
        {
            return $"{value / 1000000}M";
        }
        else if (value < 1000000000000)
        {
            return $"{value / 1000000000}T";
        }
        else if (value < 1000000000000000)
        {
            return $"{value / 1000000000000}Q";
        }
        else
        {
            return string.Empty;
        }
    }
}
