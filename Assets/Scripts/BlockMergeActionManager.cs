using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockMergeActionManager : MonoBehaviour
{
    Block block;
    Vector3 _originalScale;
    WaitForSeconds LastMergingDelay;
    private void Awake()
    {
        block = GetComponent<Block>();
        _originalScale = transform.localScale;
         LastMergingDelay=new WaitForSeconds(0.23f);
    }



    /*public void TopMerge()
    {
         MergeShadow(block._topMergeShadow.gameObject);
    }*/
    private bool _merging;
    public bool Merging
    {
        get { return _merging; }
        set { _merging = value;
            block.Level = 0;
        }
    }


    
    IEnumerator LerpColor(float t,ulong newVal,Image target)
    {
        Color old = ColorManager.Instance.GetColor(GetComponent<Block>().Value);
        Color newColor= ColorManager.Instance.GetColor(newVal);
        float startTime = Time.time;

        while(Time.time-startTime<t)
        {
            target.color=Color.Lerp(old,newColor,(Time.time-startTime)/t);
            yield return null;
        }
        target.color=newColor;
    }
    
    public void MergeShadow(GameObject shadow,float t,int neighborRow,int neighborColumn,ulong newVal)
    {
        
        StartCoroutine(LerpColor(t,newVal, gameObject.GetComponent<Image>()));
        StartCoroutine(LerpColor(t, newVal, shadow.GetComponent<Image>()));

        Merging = true;
        block.blockUI.DisAbleText();
       // LeanTween.alpha(shadow, 1, 0.3f);
       shadow.SetActive(true);
        Vector3 OriginLocalPOsition=shadow.transform.localPosition;
        
        LeanTween.moveLocal(shadow, Vector3.zero, t).setOnComplete(()=>ResetShadow(shadow,OriginLocalPOsition,neighborRow,neighborColumn));
    }

    public void PopBlock(int neighborRow, int neighborColumn)
    {
        //LeanTween.scale(block.blockUI.BlockValueText, new Vector3(1.2f, 1.2f, 1.2f), 0.2f);
        LeanTween.scale(gameObject, new Vector3(_originalScale.x* 1.2f, _originalScale.y* 1.2f, _originalScale.z*1.2f), 0.2f).setOnComplete(()=>UnpopBlock(neighborRow, neighborColumn));
        
    }

    public void UnpopBlock(int neighborRow, int neighborColumn)
    {
        LeanTween.scale(gameObject, _originalScale, 0.2f);
        LeanTween.scale(block.blockUI.BlockValueText, Vector3.one, 0.2f);
        try
        {
            StartCoroutine(FinishMerging(neighborRow, neighborColumn));
        }
        catch
        {
            SetMergedNeighborToNull(neighborRow, neighborColumn);
            Merging = false;
        }
       
    }

    private void ResetShadow(GameObject shadow,Vector3 pos,int neighborRow,int neighborColumn)
    {
        shadow.SetActive(false);
        shadow.transform.localPosition = pos;
        
        block.blockUI.EnableText();
        PopBlock(neighborRow, neighborColumn);
        
    }

    IEnumerator FinishMerging(int neighborRow, int neighborColumn)
    {
        yield return LastMergingDelay;
        if(neighborRow!=-1)
        {
            //-1 is sent when column is filled and there is no neighbor
            SetMergedNeighborToNull(neighborRow, neighborColumn);
        }
        
        Merging = false;
    }

    private void SetMergedNeighborToNull(int row,int column)
    {
        GridManager.instance.GridArray[row, column] = null;
    }
}
