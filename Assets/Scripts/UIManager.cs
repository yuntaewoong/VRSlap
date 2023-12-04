using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] roseObjects;
    [SerializeField] private GameObject[] otherObjects;

    [SerializeField] private Slider roseSlider;
    [SerializeField] private Slider otherSlider;


    private void Start()
    {
        roseSlider.maxValue = GameManager.Instance.maxTimerCount-1;
        otherSlider.maxValue = GameManager.Instance.maxTimerCount-1;
    }

    private void Update()
    {
        UpdateSlider();
        UpdateHP();
    }

    private void UpdateSlider()
    {
        ETurn currentTurn = GameManager.Instance.Turn;
        int timerCount = GameManager.Instance.timerCount;
        switch(currentTurn)
        {
            case ETurn.Enemy:
                roseSlider.value = timerCount-1;
                break;
            case ETurn.Player:
                otherSlider.value = timerCount-1;
                break;
        }
    }
    private void UpdateHP()
    {
        int playerHP = GameManager.Instance.player.Hp;
        int enemyHP = GameManager.Instance.enemy.Hp;
        for(int i = 0;i<roseObjects.Length;i++)
            roseObjects[i].SetActive((i < playerHP));
        for (int i = 0; i < otherObjects.Length; i++)
            otherObjects[i].SetActive((i < enemyHP));
    }

}
