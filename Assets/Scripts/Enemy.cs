using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHp;
    private int hp = 0;
    void Start()
    {
        hp = maxHp;
    }
    void OnCollisionEnter(Collision collision)
    {
        GetSlapped();
    }
    public void GetSlapped()
    {
        Debug.Log("Slapped Enemy");
        hp--;
        GameManager.Instance.Turn = ETurn.Player;
    }
}
