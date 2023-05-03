using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemDescription : MonoBehaviour
{
    private TMP_Text text;

    private void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
    }

    public void UpdateInfo(string name, string description)
    {
        text.text = name;

        if (description != string.Empty) 
            text.text += "\n\n" + description;
    }
}
