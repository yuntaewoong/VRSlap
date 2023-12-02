using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTimer : MonoBehaviour
{

    public Slider Enemy_timer;

    public float SliderTimer;

    public bool StopTimer = false;

    // Start is called before the first frame update
    void Start()
    {
        SliderTimer = GameManager.Instance.maxTimerCount;
        Enemy_timer.maxValue = SliderTimer;
        Enemy_timer.value = SliderTimer;
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
            Enemy_timer.value = SliderTimer;
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
