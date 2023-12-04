using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHp;
    [SerializeField] private Animator animator;
    private int hp = 0;
    // 플레이어가 때릴 때, 턴당 한 번만 공격할 수 있게 함
    public bool isHit;

    // 회피할 시간을 미리 정해서 stack에 쌓음
    private Stack<int> avoidTime;
    private Coroutine avoidTimerCoroutine;
    // 다음에 회피할 시각
    private int nextAvoidTime;
    private bool isAvoid;
    
    // Enemy의 turn일 경우, 공격할 때
    private int attackTime;
    public Coroutine attackCoroutine;
    private AudioSource slapsound;
    [SerializeField] Image[] hpImage = null;

    void PlaySlapSound(int volume)
    {
        this.slapsound.volume = volume * 0.01f;
        this.slapsound.Play();

    }

    void SettingHpImage()
    {
        for (int i = 0; i < hpImage.Length; i++)
        {
            if (i < hp)
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
        isHit = false;
        avoidTime = new Stack<int>();
        nextAvoidTime = -10;
        slapsound = GetComponent<AudioSource>();
        isAvoid = false;
        if (GameManager.Instance.Turn == ETurn.Player)
        {
            attackCoroutine = StartCoroutine(SetAttackTime(GameManager.Instance.maxTimerCount));
        }
    }

    void OnTriggerEnter(Collider other)
    {   //적이 싸대기 맞은 경우
        Debug.Log("OnTriggerEnter");
        // Player가 때릴 차례이고, Enemy가 피하지 않았으며
        // Enemy가 해당 턴에 맞지 않은 경우에만 Slap 처리함.
        if (GameManager.Instance.Turn == ETurn.Enemy && !isHit)
        {
            if (!isAvoid) GetSlapped();
            //else counter;
        }
    }
    public void GetSlapped()
    {
        Debug.Log("Slapped Enemy");
        hp--;
        isHit = true;

        // Sound
        PlaySlapSound(100);

        if (hp > 0)
        {
            // Animation
            animator.SetBool("IsSlapped", true);
            StartCoroutine(ReturnToIdle());

            GameManager.Instance.isStopTimer = true;
        }
        else
        {
            Debug.Log("승리");
            // Animation
            animator.SetTrigger("Defeat");
            GameManager.Instance.isStopTimer = true;
            GameManager.Instance.gameOver = true;
        }

        // UI
        SettingHpImage();
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
    { // 1~2초마다 때림
        avoidTime.Clear();
        int tempTime = 0;
        while (true) {
            int randomTime = Random.Range(1, 3); // 1부터 2 중 임의의 값
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

    private void SetIsAvoid(int flag)
    {
        // flag가 0이면 isAvoid = false
        // flag가 1이면 isAvoid = true
        if (flag == 1) isAvoid = true;
        else if (flag == 0) isAvoid = false; 
    }

    public IEnumerator SetAttackTime(int timeToTurn)
    {
        // 미리 공격할 시간을 정해 놓음
        // 3~timeToTurn-2 사이 임의의 값
        attackTime = Random.Range(3, timeToTurn-1);

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

    private void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            if (GameManager.Instance.Turn == ETurn.Enemy && !isHit)
            {
                if (!isAvoid) GetSlapped();
                //else counter;
            }
        }
    }
}
