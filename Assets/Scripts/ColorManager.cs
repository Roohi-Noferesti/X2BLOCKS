using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance { get; private set; }
    private void Awake()
    {
        if(Instance!=null && Instance!=this )
        {
            Destroy(this);
        }
        else
        {
            Instance = this;    
        }
        
    }
    [SerializeField]
    private List<Color> Colors;

    public void ImageColorLerp(Image image,Color _old,Color _new,float time )
    {
        StartCoroutine(Co_ImageColorLerp(image, _old, _new,time));
    }

    IEnumerator Co_ImageColorLerp (Image image,Color _old,Color _new,float time)
    {
        float start = Time.time;
        while(Time.time-start < time)
        {
            image.color = Color.Lerp(_old, _new,(Time.time-start)/time);
            yield return null;
        }
        image.color = _new;
    }

    //c
    public Color GetColor(ulong value)
    {
        if(value==0)
        {
            return Colors[40];
        }
        ulong res;
        int index = -1;
        do
        {
            index++;
            res = value / 2;
            value = value / 2;
        } while (res != 1);

        return Colors[index];


    }
}
