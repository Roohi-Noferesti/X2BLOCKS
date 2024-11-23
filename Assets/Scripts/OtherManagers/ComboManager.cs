using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ComboManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI ComboText;
    Vector3 ComBotextOrigin;
    int combo;

    public static   ComboManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null && instance != this )
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
        ComBotextOrigin=ComboText.transform.position;
    }
    public void ResetComboVal()
    {
        combo = 0;
    }
    public void ComboUp()
    {
        combo++;
        
    }

    public void CheckCombo()
    {
        ScoreManager.instance.addToSurviveMode(combo);
        if (combo < 3)
        { 
            ResetComboVal();
            return;
        }
        Application.targetFrameRate = -1;
        ComboText.text = $"Combo Level{combo}!";
        ComboText.gameObject.SetActive(true);
        ComboText.GetComponent<CanvasGroup>().alpha = 1;
        
        StartCoroutine(ComboRemove());
    }
    //c
    WaitForSeconds oneSec=new WaitForSeconds(0.5f);
    IEnumerator ComboRemove()
    {
        yield return oneSec;
        LeanTween.move(ComboText.gameObject,
            new Vector3(ComboText.transform.position.x, GridManager.instance.panel.position.y), 0.2f)
            .setOnComplete(() => restComboPos()); ;
        LeanTween.alphaCanvas(ComboText.gameObject.GetComponent<CanvasGroup>(), 0, 0.2f);
            

    }

    void restComboPos()
    {
        ComboText.rectTransform.position = ComboText.transform.parent.position;
        
        ComboText.gameObject.SetActive(false);
       ResetComboVal();
    }
}
