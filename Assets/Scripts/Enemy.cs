using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Slap!");
        meshRenderer.material.color = Color.red;
        StartCoroutine(SlapDebug());
    }
    IEnumerator SlapDebug()
    {
        yield return new WaitForSeconds(1.0f);
        meshRenderer.material.color = Color.black ;
    }
}
