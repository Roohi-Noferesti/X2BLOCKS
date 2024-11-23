using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUI : MonoBehaviour
{
    [SerializeField]
    private  GameObject _blockValueTextObject;
    [SerializeField]
    private TMPro.TextMeshProUGUI _blockValueTextField;
    public GameObject BlockValueText,swapLine,infinity;

    public void FadeoutText()
    {
        Color color = _blockValueTextField.color;
        color.a = 0.1f;
        _blockValueTextField.color = color;
    }


    void SwapLineOff()
    {
        swapLine.SetActive(false);
    }

    private void OnEnable()
    {
        PowerManager.OnCancelSwap += SwapLineOff;
    }
    private void OnDisable()
    {
        PowerManager.OnCancelSwap -= SwapLineOff;
    }

    public void DisAbleText()
    {
        _blockValueTextField.gameObject.SetActive(false);
    }
    public void EnableText()
    {
        _blockValueTextField.gameObject.SetActive(true);
    }
    public void EnableInfinity()
    {
        infinity.gameObject.SetActive(true);    
    }

    public void DisableInfinity()
    {
        infinity.gameObject.SetActive(false );
    }

    public void SetValueText(ulong value)
    {
        if (_blockValueTextField == null) return ;
        if (value == 0) {
            _blockValueTextField.gameObject.SetActive(false);
            return ;
        }
        
        _blockValueTextField.text = UiManager.instance.ConverNumberToString(value); 
       
        
    }

    internal void FadeInText()
    {
        Color color = _blockValueTextField.color;
        color.a =1f;
        _blockValueTextField.color = color;
    }
}
