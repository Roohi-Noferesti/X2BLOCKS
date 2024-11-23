using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockColor : MonoBehaviour
{
    [SerializeField] private Image BlockImage;
    [SerializeField] Block block;
    
    public void FadeOut()
    {
        Color color = BlockImage.color;
        color.a = 0.1f;
        BlockImage.color = color;
        block.blockUI.FadeoutText();
    }
    

    public void SetColor(ulong value)
    {
        BlockImage.color = ColorManager.Instance.GetColor(value);
        block._topMergeShadow.color = ColorManager.Instance.GetColor(value);
        block.leftMergeShadow.color = ColorManager.Instance.GetColor(value);
        block.rightMergeShadow.color = ColorManager.Instance.GetColor(value);
        block.bottomMergeShadow.color = ColorManager.Instance.GetColor(value);  
                 
    }

    IEnumerator Co_setColor(ulong value)
    {
        while(block.mergeActionManager.Merging)
        {
            yield return null;
        }
        BlockImage.color = ColorManager.Instance.GetColor(value);
        block._topMergeShadow.color = ColorManager.Instance.GetColor(value);
        block.leftMergeShadow.color = ColorManager.Instance.GetColor(value);
        block.rightMergeShadow.color = ColorManager.Instance.GetColor(value);
    }

    internal void FadeIn()
    {
        Color color = BlockImage.color;
        color.a = 1f;
        BlockImage.color = color;
        Debug.Log("in");
        //k
        block.blockUI.FadeInText();
    }
}
