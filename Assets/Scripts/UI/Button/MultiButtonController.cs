using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiButtonController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] panels;
    [SerializeField]
    private Button[] buttons;

    int num = 0;
    private void Awake()
    {
        num = Mathf.Min(panels.Length, buttons.Length);
        for(int i = 0;i < num; ++i)
        {
            int captureI = i;
            buttons[i].onClick.AddListener(() => OpenPanel(captureI)); 
        }
    }
    public void OpenPanel(int index)
    {
        ClosePanels(index);

        panels[index].SetActive(true);
    }
    // index 제외한 나머지 닫기
    private void ClosePanels(int index)
    {
        for(int i =0;i< num; ++i)
        {
            if(i == index)
            {
                continue;
            }

            panels[i].SetActive(false);
        }
    }
}
