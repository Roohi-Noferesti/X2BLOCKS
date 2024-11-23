using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GemManager : MonoBehaviour
{
    public GameObject GemBuyMenu;
    [field:SerializeField]
    public int NewBlockReward { get; private set; }

    [SerializeField]
    TextMeshProUGUI GemAdNotification;
    Vector2 GemNotOrigin;
    private void Start()
    {
        GemNotOrigin=GemAdNotification.transform.position;  
    }
    public void ShowBuyMenu()
    {
        GemBuyMenu.SetActive(true);
        TouchManager.instance.StopTouch();
        GemBuyMenu.transform.SetAsLastSibling();
        
    }
    public void HideMenu()
    {
        GemBuyMenu?.SetActive(false);
    }
    [ContextMenu("Gem_notif")]
    public void gemtest()
    {
        AnimateGemNote("100+");
    }
    public void AnimateGemNote(string NotifValue)
    {
        GemAdNotification.gameObject.SetActive(true);
        GemAdNotification.text = NotifValue;    
        GemAdNotification.transform.localScale = Vector3.zero;
        LeanTween.scale(GemAdNotification.gameObject, Vector3.one, 0.4f).setEaseInOutElastic().setOnComplete(()=>animateMoveOutAndFade());
    }
     void animateMoveOutAndFade()
    {
        AudiManager.instance.PlayGem();
        LeanTween.move(GemAdNotification.gameObject, GemAdNotification.transform.position + new Vector3(-400, 0, 0), 7f).setOnComplete(() => resetGemNotif());
        LeanTween.alphaCanvas(GemAdNotification.GetComponent<CanvasGroup>(), 0, 3f);
    }

    void resetGemNotif()
    {
        GemAdNotification.transform.position = GemNotOrigin;
        GemAdNotification.GetComponent<CanvasGroup>().alpha = 1;
        GemAdNotification.gameObject.SetActive(false);
    }


    public List<GameObject> GemIcons = new List<GameObject>();

    public void DisableGemIcons()
    {
        foreach (var icon in GemIcons)
        {
            icon.SetActive(false);
            icon.transform.position = icon.GetComponent<ObjectTween>().Origin;
        }
    }
    
    public static GemManager Instance { get; private set; }
    private void Awake()
    {
        if(Instance !=null && Instance!=this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;    
        }
    }
}
