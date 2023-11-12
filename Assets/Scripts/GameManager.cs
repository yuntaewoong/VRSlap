using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ETurn { Player, Enemy };
public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    [SerializeField] private int maxTimerCount;
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;

    private ETurn turn = ETurn.Player;
    private int timerCount;

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
        timerCount = maxTimerCount;
        StartCoroutine(Timer());
    }
    IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            timerCount--;
            Debug.Log(timerCount);
            if (timerCount == 0)
            {//제한시간 종료
                switch (turn)
                {
                    case ETurn.Player:
                        player.GetSlapped();
                        break;
                    case ETurn.Enemy:
                        enemy.GetSlapped();
                        break;
                }
                break;//코루틴 종료
            }
        }
    }

    void Awake()
    {
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
