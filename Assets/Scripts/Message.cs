using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;

    public void SetText(string str)
    {
        messageText.text = str;
    }
}
