using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] roseObjects;
    [SerializeField] private GameObject[] otherObjects;

    [SerializeField] private Slider roseSlider;
    [SerializeField] private Slider otherSlider;

    [SerializeField] private TextMeshProUGUI turnText;
    private void Start()
    {
        roseSlider.maxValue = GameManager.Instance.maxTimerCount-1;
        otherSlider.maxValue = GameManager.Instance.maxTimerCount-1;
    }

    private void Update()
    {
        UpdateSlider();
        UpdateHP();
        UpdateTurnText();
    }

    private void UpdateSlider()
    {
        ETurn currentTurn = GameManager.Instance.Turn;
        int timerCount = GameManager.Instance.timerCount;
        switch(currentTurn)
        {
            case ETurn.Player:
                roseSlider.value = timerCount-1;
                break;
            case ETurn.Enemy:
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
    private void UpdateTurnText()
    {
        switch(GameManager.Instance.Turn)
        {
            case ETurn.Player:
                turnText.text = "플레이어 턴";
                turnText.color = Color.green;
                break;
            case ETurn.Enemy:
                turnText.text = "적 턴";
                turnText.color = Color.red;
                break;
        }
    }
}
