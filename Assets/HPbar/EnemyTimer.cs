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
        Enemy_timer.maxValue = SliderTimer;
        Enemy_timer.value = SliderTimer;

        StartTimer();
    }

    public void StartTimer()
    {
   

        StartCoroutine(StartTimerTicker());
    }

    IEnumerator StartTimerTicker()
    {
        yield return new WaitForSeconds(15f);

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
                
                Enemy_timer.value = SliderTimer;
           
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
