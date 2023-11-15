using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ETurn { Player, Enemy };
public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    [SerializeField] private int maxTimerCount;
    [SerializeField] private float stopTime;
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;

    private ETurn turn;
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

        if (turn == ETurn.Player) enemy.SetAttackTime(maxTimerCount);
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
            {//제한시간 종료되면 맞은걸로 친다
                switch (turn)
                {
                    case ETurn.Player:
                        player.GetSlapped();
                        Turn = ETurn.Enemy;
                        enemy.SetAvoidTime(maxTimerCount);
                        break;
                    case ETurn.Enemy:
                        enemy.GetSlapped();
                        Turn = ETurn.Player;
                        break;
                }
                break;//코루틴 종료
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
