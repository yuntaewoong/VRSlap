using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTimer : MonoBehaviour
{

    public Slider Player_Timer;
    public float SliderTimer;

    public bool StopTimer = false;

    // Start is called before the first frame update
    void Start()
    {

        SliderTimer = GameManager.Instance.maxTimerCount;
        Player_Timer.maxValue = SliderTimer;
        Player_Timer.value = SliderTimer;
    }
    public void StartTimer()
    {
        StopTimer = false;
        StartCoroutine(StartTimerTicker());
    }

    IEnumerator StartTimerTicker()
    {
        SliderTimer = GameManager.Instance.maxTimerCount;
        while (StopTimer == false && SliderTimer >= 0)
        {
            SliderTimer -= Time.deltaTime;
            Player_Timer.value = SliderTimer;
            yield return new WaitForSeconds(0.001f);
        }
    }
    public void stopTimer()
    {
        StopTimer = true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
