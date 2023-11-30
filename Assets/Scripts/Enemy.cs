using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private bool isAvoid;
    
    // Enemy�� turn�� ���, ������ ��
    private int attackTime;
    public Coroutine attackCoroutine;
    private AudioSource slapsound;

    void PlaySlapSound(int volume)
    {
        this.slapsound.volume = volume * 0.01f;
        this.slapsound.Play();

    }

    //hp
    int maxHP = 4;
    int currentHp = 4;

    [SerializeField] Image[] hpImage = null;

    public void DecreaseHp(int p_num)
    {
        currentHp -= p_num;

        if (currentHp <= 0)
        {
            //hp�� 0�� �� ��
            Debug.Log("�¸�");
        }
        SettingHpImage();
    }

    void SettingHpImage()
    {
        for (int i = 0; i < hpImage.Length; i++)
        {
            if (i < currentHp)
                hpImage[i].gameObject.SetActive(true);
            else
                hpImage[i].gameObject.SetActive(false);
        }
    }

    public int Hp
    {
        get => hp;
    }
    void Start()
    {
        hp = maxHp;
        avoidTime = new Stack<int>();
        nextAvoidTime = -10;
        slapsound = GetComponent<AudioSource>();
        isAvoid = false;
        if (GameManager.Instance.Turn == ETurn.Player)
            attackCoroutine = StartCoroutine(SetAttackTime(GameManager.Instance.maxTimerCount));
    }
    void OnTriggerEnter(Collider other)
    {   //���� �δ�� ���� ���
        Debug.Log("OnTriggerEnter");
        // Player�� ���� �����̰�, Enemy�� ������ ���� ��쿡�� Slap ó����.
        if (GameManager.Instance.Turn == ETurn.Enemy)
        {
            if (!isAvoid) GetSlapped();
            //else counter;
        }
    }
    public void GetSlapped()
    {
        Debug.Log("Slapped Enemy");
        DecreaseHp(1); //hp-1
        hp--;
        PlaySlapSound(100);
        animator.SetBool("IsSlapped", true);
        GameManager.Instance.isStopTimer = true;
        StartCoroutine(ReturnToIdle());
        GameManager.Instance.Turn = ETurn.Player;
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
    { // 1~2�ʸ��� ����
        avoidTime.Clear();
        int tempTime = 0;
        while (true) {
            int randomTime = Random.Range(1, 3); // 1���� 2 �� ������ ��
            if (timeToTurn > tempTime + randomTime)
            {
                tempTime += randomTime;
                avoidTime.Push(tempTime); // avoidTime�� ���� �ð��� �̸� ���� ����
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

    private void SetIsAvoid(int flag)
    {
        // flag�� 0�̸� isAvoid = false
        // flag�� 1�̸� isAvoid = true
        if (flag == 1) isAvoid = true;
        else if (flag == 0) isAvoid = false; 
    }

    public IEnumerator SetAttackTime(int timeToTurn)
    {
        // �̸� ������ �ð��� ���� ����
        // 3~timeToTurn-1 ���� ������ ��
        attackTime = Random.Range(3, timeToTurn);

        while (GameManager.Instance.timerCount > 0)
        {
            if (GameManager.Instance.timerCount == attackTime)
            {
                animator.SetTrigger("Attack");
                break;
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void Attack()
    {
        Debug.Log("Attack");
        GameManager.Instance.player.GetSlapped();
        StopCoroutine(attackCoroutine);
        StartCoroutine(SwitchTurnAfterAttack());
    }

    IEnumerator SwitchTurnAfterAttack()
    {
        animator.speed = 0.0f;
        yield return new WaitForSeconds(1.5f);
        animator.speed = 1.0f;
    }
}
