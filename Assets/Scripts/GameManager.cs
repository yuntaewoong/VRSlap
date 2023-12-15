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
    [SerializeField] public Enemy enemy;

    [SerializeField] private ETurn turn;
    private Coroutine timerCorutine;

    public bool isStopTimer;
    public int timerCount;
    public bool isGameOver;

    public ETurn Turn
    {
        get => turn;
        set
        {
            turn = value;
            StartTurn();
        }
    }
    public void StartGame()
    {
        StartTurn();
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
        }
        else
        {
            enemy.SetAvoidTime(maxTimerCount);
            enemy.isHit = false;
        };
    }

    public IEnumerator Timer()
    {
        while (!isGameOver)
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

                yield return new WaitForSeconds(stopTime);

                if (!isGameOver)
                {
                    isStopTimer = false;

                    if (turn == ETurn.Player) Turn = ETurn.Enemy;
                    else Turn = ETurn.Player;
                }
                yield return null;
            }
        }

        if (isGameOver)
        {
            isStopTimer = true;
        }
    }

    void Awake()
    {
        isGameOver = false;
        turn = ETurn.Player;
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
