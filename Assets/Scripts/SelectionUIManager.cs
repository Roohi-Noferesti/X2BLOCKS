using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionUIManager : MonoBehaviour
{
    public static SelectionUIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }



    [SerializeField]
    private Transform _readyBlocFrame, _nextBlockFrame;
    
    [SerializeField]
    private Image _readyImage, _nextImage;

    [SerializeField]
    private TextMeshProUGUI _readyText, _nextText;
    
    public void  SetUp()
    {
        _readyImage.gameObject.SetActive(true);
        _nextImage.gameObject.SetActive(true);

        _readyImage.transform.position = _readyBlocFrame.position;
        _nextImage.transform.position = _nextBlockFrame.position;
        _nextImage.transform.localScale = _nextBlockFrame.localScale;

    }
    [ContextMenu("Reload")]
    public void ReadyImageReload()
    {
               
        _readyImage.transform.position = _nextBlockFrame.position;
        _readyImage.transform.localScale = _nextBlockFrame.localScale;
        _nextImage.transform.localScale = Vector3.zero;
        
        StartCoroutine(ImageSwap(0.3f));

    }

    public void SetCurrentImage(ulong val)
    {
        _readyText.text = val.ToString();
        _readyImage.color=ColorManager.Instance.GetColor(val);
    }

    
    

    public void setNextImage(ulong val)
    {
        _nextText.text = val.ToString();
        _nextImage.color=ColorManager.Instance.GetColor(val);
    }
    public bool swaping { get; private set; }
    IEnumerator ImageSwap(float duration)
    {
        swaping = true;
        yield return 0;
        GameBrain.Instance.SwapCurrentToNext();
        GameBrain.Instance.SetNextBlock();
        float start=Time.time;
        while(Time.time-start<duration)
        {
            float t = (Time.time - start) / duration;
            _readyImage.transform.position = Vector2.Lerp(_nextBlockFrame.position,
                _readyBlocFrame.position, t);
            _readyImage.transform.localScale=new Vector3(t,t,t);
            
            
            yield return null;
        }
        _readyImage.transform.position=_readyBlocFrame.position;
        start = Time.time;
        float pop = 0.3f;
        while (Time.time - start < pop)
        {
            float t = (Time.time - start) / pop;
            _nextImage.transform.localScale =
                new Vector3(t * _nextBlockFrame.localScale.x, t * _nextBlockFrame.localScale.y
                , t * _nextBlockFrame.localScale.z);
            yield return null;  
        }
        swaping = false;
        if(!GridManager.instance.GridBusy)
        {
            Application.targetFrameRate = GridManager.instance.HaltFrameRate;
        }

    }


}
