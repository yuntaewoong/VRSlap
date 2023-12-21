using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHp;
    [SerializeField] private Animator animator;
    private int hp = 0;
    // �÷��̾ ���� ��, �ϴ� �� ���� ������ �� �ְ� ��
    public bool isHit;

    // ȸ���� �ð��� �̸� ���ؼ� stack�� ����
    private Stack<int> avoidTime;
    private Coroutine avoidTimerCoroutine;
    // ������ ȸ���� �ð�
    private int nextAvoidTime;
    public bool isAvoid;
    
    // Enemy�� turn�� ���, ������ ��
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
    {   //���� �δ�� ���� ���
        if (other.gameObject.tag != "Player")
            return;
        Vector3 rightHandVelocity = GameManager.Instance.player.GetRightHandVelocity();

        // Player�� ���� �����̰�, Enemy�� ������ �ʾ�����
        // Enemy�� �ش� �Ͽ� ���� ���� ��쿡�� Slap ó����.
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
                Debug.Log("�¸�");
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
        if (GameManager.Instance.Turn == ETurn.Player)
        {
            while (GameManager.Instance.timerCount > 0)
            {

                // ������ ���� �ð� ���� ����
                if (nextAvoidTime == -10 && avoidTime.Count > 0) nextAvoidTime = avoidTime.Pop();

                // ���� �ð�==���� �ð� �̶��
                // ȸ����
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
        // flag�� 0�̸� isAvoid = false
        // flag�� 1�̸� isAvoid = true
        if (flag == 1) isAvoid = true;
        else if (flag == 0) isAvoid = false; 
    }

    public IEnumerator SetAttackTime(int timeToTurn)
    {
        // �̸� ������ �ð��� ���� ����
        // 3~timeToTurn-2 ���� ������ ��
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
