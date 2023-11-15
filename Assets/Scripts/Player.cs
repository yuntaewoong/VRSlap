using Oculus.Interaction.Samples;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.Oculus;
using UnityEditor.XR.LegacyInputHelpers;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int maxHp;
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
    {//핸드트래킹 박수입력시 실행
        InitPosition();
    }
    private void OnTriggerEnter(Collider other)
    {//플레이어가 싸대기 맞은 경우
        GetSlapped();
    }
    public void GetSlapped()
    {
        Debug.Log("Slapped Player");
        hp--;
        GameManager.Instance.Turn = ETurn.Enemy;
    }
    private void InitPosition()
    {
        OVRManager.display.RecenterPose();
    }
}
