using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHp;
    [SerializeField] private Animator animator;
    private int hp = 0;

    // 회피할 시간을 미리 정해서 stack에 쌓음
    private Stack<int> avoidTime;
    private Coroutine avoidTimerCoroutine;
    // 다음에 회피할 시각
    private int nextAvoidTime;
    
    // Enemy의 turn일 경우, 공격할 때
    private int attackTime;
    private Coroutine attackTimerCoroutine;

    void Start()
    {
        hp = maxHp;
        avoidTime = new Stack<int>();
        nextAvoidTime = -10;
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        GetSlapped();
    }
    public void GetSlapped()
    {
        Debug.Log("Slapped Enemy");
        hp--;
        animator.SetBool("IsSlapped", true);
        GameManager.Instance.isStopTimer = true;
        StartCoroutine(ReturnToIdle());
    }
    IEnumerator ReturnToIdle()
    {
        yield return new WaitForSeconds(1.0f);
        if (animator.GetBool("IsSlapped"))
        {
            animator.SetBool("IsSlapped", false);
        }
    }

    public void SetAvoidTime(int timeToTurn)
    { // 2초~4초마다 때림
        avoidTime.Clear();
        int tempTime = 0;
        while (true) {
            int randomTime = Random.Range(2, 5); // 2부터 4 중 임의의 값
            if (timeToTurn > tempTime + randomTime)
            {
                tempTime += randomTime;
                avoidTime.Push(tempTime); // avoidTime에 피할 시간을 미리 정해 놓음
                Debug.Log("tempTime" + tempTime);
            }
            else break;
        }

        if (avoidTime.Count > 0)
        {
            if (avoidTimerCoroutine != null)
                StopCoroutine(avoidTimerCoroutine);
            avoidTimerCoroutine = StartCoroutine(Avoid());
        }
    }

    IEnumerator Avoid()
    {
        while (GameManager.Instance.timerCount > 0) {

            // 다음에 피할 시간 정해 놓음
            if (nextAvoidTime == -10 && avoidTime.Count > 0) nextAvoidTime = avoidTime.Pop();

            // 현재 시간==피할 시간 이라면
            // 회피함
            if (GameManager.Instance.timerCount == nextAvoidTime + 1)
            {
                animator.SetTrigger("Avoid");
                nextAvoidTime = -10;
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
    public void SetAttackTime(int timeToTurn)
    {
        // 미리 공격할 시간을 정해 놓음
        // 3~timeToTurn-1 사이 임의의 값
        attackTime = Random.Range(3, timeToTurn);

        if (attackTimerCoroutine != null)
            StopCoroutine(attackTimerCoroutine);
        attackTimerCoroutine = StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        while (GameManager.Instance.timerCount > 0)
        {
            // 현재 시간==공격 시간 이라면
            // 공격
            if (GameManager.Instance.timerCount == attackTime)
            {
                Debug.Log("Attack");
                animator.SetTrigger("Attack");
                StartCoroutine(SwitchTurnAfterAttack());
                break;
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator SwitchTurnAfterAttack()
    {
        yield return new WaitForSeconds(1.8f);
        // 플레이어 맞음
        Debug.Log("Player Attacked!");
        yield return new WaitForSeconds(0.7f);
        GameManager.Instance.Turn = ETurn.Enemy;
    }
}
