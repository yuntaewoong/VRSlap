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
    public bool isAvoid;
    
    // Enemy의 turn일 경우, 공격할 때
    private int attackTime;
    public Coroutine attackCoroutine;
    private AudioSource slapsound;

    void PlaySlapSound(int volume)
    {
        slapsound.volume = volume * 0.01f;
        slapsound.Play();

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
    }

    bool CanEnemyAct()
    {
        if (isHit) return false;
        if (GameManager.Instance.isStopTimer) return false;
        if (GameManager.Instance.isGameOver) return false;
        if (GameManager.Instance.timerCount < 1) return false;

        return true;
    }

    void OnTriggerEnter(Collider other)
    {   //적이 싸대기 맞은 경우
        if (other.gameObject.tag != "Player")
            return;
        Vector3 rightHandVelocity = GameManager.Instance.player.GetRightHandVelocity();

        // Player가 때릴 차례이고, Enemy가 피하지 않았으며
        // Enemy가 해당 턴에 맞지 않은 경우에만 Slap 처리함.
        if (GameManager.Instance.Turn == ETurn.Player && !isHit)
        {
            if (!isAvoid) GetSlapped(rightHandVelocity.magnitude);
        }
    }
    public void GetSlapped(float handVelocity)
    {
        if (handVelocity > 0 && CanEnemyAct())
        {
            Debug.Log("Slapped Enemy");
            hp--;
            isHit = true;

            // Sound
            PlaySlapSound(90);
            
            if (hp > 0)
            {
                Debug.Log(handVelocity);
                if (handVelocity >= 0.1) animator.SetTrigger("Slapped3");
                else if (handVelocity >= 0.06) animator.SetTrigger("Slapped2");
                else if (handVelocity >= 0.001) animator.SetTrigger("Slapped1");
                GameManager.Instance.isStopTimer = true;

                /*
                // Animation
                animator.SetBool("IsSlapped", true);
                StartCoroutine(ReturnToIdle());

                GameManager.Instance.isStopTimer = true;
                */
            }
            else
            {
                Debug.Log("승리");
                // Animation
                animator.SetTrigger("Defeat");
                GameManager.Instance.isStopTimer = true;
                GameManager.Instance.isGameOver = true;
            }
        }
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
        if (GameManager.Instance.Turn == ETurn.Player)
        {
            while (GameManager.Instance.timerCount > 0)
            {

                // 다음에 피할 시간 정해 놓음
                if (nextAvoidTime == -10 && avoidTime.Count > 0) nextAvoidTime = avoidTime.Pop();

                // 현재 시간==피할 시간 이라면
                // 회피함
                if (GameManager.Instance.timerCount == nextAvoidTime + 1 && CanEnemyAct() && GameManager.Instance.Turn == ETurn.Player)
                {
                    animator.SetTrigger("Avoid");
                    nextAvoidTime = -10;
                }
                yield return new WaitForSeconds(1.0f);
            }
        }
        else yield return null;
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

        if (GameManager.Instance.Turn == ETurn.Enemy)
        {
            while (GameManager.Instance.timerCount > 0)
            {
                if (GameManager.Instance.timerCount == attackTime && CanEnemyAct())
                {
                    animator.SetTrigger("Attack");
                    break;
                }
                yield return new WaitForSeconds(1.0f);
            }
        }
        else yield return null;
    }

    private void Attack()
    {
        Debug.Log("Attack");
        StopCoroutine(attackCoroutine);
        StartCoroutine(SwitchTurnAfterAttack());
    }

    IEnumerator SwitchTurnAfterAttack()
    {
        animator.speed = 0.0f;
        GameManager.Instance.isStopTimer = true;
        yield return new WaitForSeconds(1.5f);
        animator.speed = 1.0f;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            if (GameManager.Instance.Turn == ETurn.Player && !isHit)
            {
                if (!isAvoid) GetSlapped(0.2f);
                //else counter;
            }
        }
    }
}
