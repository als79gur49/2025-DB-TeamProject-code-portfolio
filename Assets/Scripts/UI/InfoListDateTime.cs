using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoListDateTime : InfoList
{
    [SerializeField]
    private TextMeshProUGUI userTime;

    public void Setup(int ranking, string name, int score, DateTime time, bool isPlayer = false)
    {
        Setup(ranking, name, score, isPlayer);

        userTime.text = time.ToString();
    }
}
