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
    public bool gameOver;

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
        {
            enemy.attackCoroutine = StartCoroutine(enemy.SetAttackTime(maxTimerCount));
            enemy.timer.StartTimer();
        }
        else
        {
            enemy.SetAvoidTime(maxTimerCount);
            player.timer.StartTimer();
            enemy.isHit = false;
        };
    }

    public IEnumerator Timer()
    {
        while (!gameOver)
        {
            if (timerCount == 0)
            {
                if (turn == ETurn.Player) Turn = ETurn.Enemy;
                else Turn = ETurn.Player;
                break;
            }

            if (!isStopTimer)
            {
                yield return new WaitForSeconds(1.0f);
                timerCount--;
                //Debug.Log(timerCount);
            }
            else // Player나 Enemy가 맞은 경우
            {
                if (turn == ETurn.Player) enemy.timer.stopTimer();
                else player.timer.stopTimer();

                yield return new WaitForSeconds(stopTime);

                if (!gameOver)
                {
                    isStopTimer = false;

                    if (turn == ETurn.Player) Turn = ETurn.Enemy;
                    else Turn = ETurn.Player;
                }
                yield return null;
            }
        }

        if (gameOver)
        {
            isStopTimer = true;
            if (turn == ETurn.Player) enemy.timer.stopTimer();
            else player.timer.stopTimer();
        }
    }

    void Awake()
    {
        gameOver = false;
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
