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

        Player_Timer.maxValue = SliderTimer;
        Player_Timer.value = SliderTimer;

        StartTimer();
    }
    public void StartTimer()
    {
        StartCoroutine(StartTimerTicker());
    }

    IEnumerator StartTimerTicker()
    {

        while (StopTimer == false)
        {
            SliderTimer -= Time.deltaTime;
            yield return new WaitForSeconds(0.001f);

            if (SliderTimer <= 0)
            {
                yield return new WaitForSeconds(15f);
                SliderTimer = 15;
            }
            if (StopTimer == false)
            {
                Player_Timer.value = SliderTimer;
            }

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
