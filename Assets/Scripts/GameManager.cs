using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ETurn { Player, Enemy };
public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    [SerializeField] public int maxTimerCount;
    [SerializeField] private float stopTime;
    [SerializeField] public Player player;
    [SerializeField] private Enemy enemy;

    [SerializeField] private ETurn turn;
    private Coroutine timerCorutine;

    public bool isStopTimer;
    public int timerCount;

    public ETurn Turn
    {
        get => turn;
        set
        {
            turn = value;
            StartTurn();
        }
    }
    private void StartTurn()
    {
        if (timerCorutine != null)
            StopCoroutine(timerCorutine);
        timerCount = maxTimerCount;
        timerCorutine = StartCoroutine(Timer());

        if (turn == ETurn.Player) 
            enemy.attackCoroutine = StartCoroutine(enemy.SetAttackTime(maxTimerCount));
        else enemy.SetAvoidTime(maxTimerCount);
    }
    public IEnumerator Timer()
    {
        if (isStopTimer)
        {
            yield return new WaitForSeconds(stopTime);
            isStopTimer = false;
        }
        
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            timerCount--;
            //Debug.Log(timerCount);

            if (timerCount == 0)
            {
                if (turn == ETurn.Player) Turn = ETurn.Enemy;
                else Turn = ETurn.Player;
                break;
            }
        }
    }

    void Awake()
    {
        turn = ETurn.Player;
        StartTurn();
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static GameManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
}
