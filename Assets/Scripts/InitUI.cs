using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitUI : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;
        Debug.Log("Game Start Button Clicked");
        GameManager.Instance.StartGame();
        gameObject.SetActive(false);
    }
}
