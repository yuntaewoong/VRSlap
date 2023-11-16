using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRangeLimiter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            debugText.text = "Player Out!!";
            debugText.color = Color.red;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            debugText.text = "Player In";
            debugText.color = Color.blue;
        }
    }
}
