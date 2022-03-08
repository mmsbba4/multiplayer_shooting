using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Range(0,100)]
    public int CritDamatePer;
    public PlayerHealth health;
    private void Start()
    {
        health = GetComponentInParent<PlayerHealth>();
    }
    public void TakeDamage(Vector3 hitPoint,int damage)
    {
        if (health == null) return;
        print("damage to "+ health.gameObject.GetComponent<PhotonView>().Owner.NickName);
        
        int tmp = Random.Range(0, 100);
        if (tmp < CritDamatePer)
        {
            health.TakeDamage(hitPoint, damage*2);
        }
        else
        {
            health.TakeDamage(hitPoint, damage);
        }
    }
}
