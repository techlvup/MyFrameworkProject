using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLoadingPanel : MonoBehaviour
{
    private Slider m_sliProgress;



    private void Awake()
    {
        m_sliProgress = gameObject.transform.Find("Sli_Progress").GetComponent<Slider>();
    }



    public void SetProgressSlider(float value)
    {
        m_sliProgress.value = value;

        if(value == 1)
        {
            Destroy(gameObject);
        }
    }
}