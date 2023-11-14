using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHp;
    [SerializeField] private Animator animator;
    private int hp = 0;

    // ȸ���� �ð��� �̸� ���ؼ� stack�� ����
    private Stack<int> avoidTime;
    private Coroutine avoidTimerCoroutine;
    // ������ ȸ���� �ð�
    private int nextAvoidTime;
    
    // Enemy�� turn�� ���, ������ ��
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
    { // 2��~4�ʸ��� ����
        avoidTime.Clear();
        int tempTime = 0;
        while (true) {
            int randomTime = Random.Range(2, 5); // 2���� 4 �� ������ ��
            if (timeToTurn > tempTime + randomTime)
            {
                tempTime += randomTime;
                avoidTime.Push(tempTime); // avoidTime�� ���� �ð��� �̸� ���� ����
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

            // ������ ���� �ð� ���� ����
            if (nextAvoidTime == -10 && avoidTime.Count > 0) nextAvoidTime = avoidTime.Pop();

            // ���� �ð�==���� �ð� �̶��
            // ȸ����
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
        // �̸� ������ �ð��� ���� ����
        // 3~timeToTurn-1 ���� ������ ��
        attackTime = Random.Range(3, timeToTurn);

        if (attackTimerCoroutine != null)
            StopCoroutine(attackTimerCoroutine);
        attackTimerCoroutine = StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        while (GameManager.Instance.timerCount > 0)
        {
            // ���� �ð�==���� �ð� �̶��
            // ����
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
        // �÷��̾� ����
        Debug.Log("Player Attacked!");
        yield return new WaitForSeconds(0.7f);
        GameManager.Instance.Turn = ETurn.Enemy;
    }
}
