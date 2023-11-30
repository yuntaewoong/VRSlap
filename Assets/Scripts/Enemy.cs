using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            //hp가 0이 될 시
            Debug.Log("승리");
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

    }
    void OnTriggerEnter(Collider other)
    {//적이 싸대기 맞은 경우
        Debug.Log("OnTriggerEnter");
        GetSlapped();
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
        GameManager.Instance.player.GetSlapped();
        yield return new WaitForSeconds(0.3f);
        animator.speed = 0.0f;
        yield return new WaitForSeconds(1.5f);
        animator.speed = 1.0f;
    }
}
