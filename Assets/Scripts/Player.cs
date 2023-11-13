using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int maxHp;
    [SerializeField] private Transform playerInitTransform;
    private int hp = 0;
    public int Hp
    {
        get => hp;
    }
    void Start()
    {
        hp = maxHp;
    }
    public void OnClap()
    {//�ڵ�Ʈ��ŷ �ڼ��Է½� ����
        InitPosition();
    }


    void OnCollisionEnter(Collision collision)
    {
        //GetSlapped();
    }
    public void GetSlapped()
    {
        Debug.Log("Slapped Player");
        hp--;
        GameManager.Instance.Turn = ETurn.Enemy;
    }
    private void InitPosition()
    {
        gameObject.transform.position = playerInitTransform.position;
        gameObject.transform.rotation = playerInitTransform.rotation;
    }

}
