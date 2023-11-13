using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHp;
    [SerializeField] private Animator animator;
    private int hp = 0;
    void Start()
    {
        hp = maxHp;
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
        GameManager.Instance.Turn = ETurn.Player;
        animator.SetBool("IsSlapped", true);
        StartCoroutine(ReturnToIdle());
    }
    IEnumerator ReturnToIdle()
    { 
        yield return new WaitForSeconds(3.0f);
        if(animator.GetBool("IsSlapped"))
        {
            animator.SetBool("IsSlapped", false);
        }
        
    }
}
