using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Range(0,100)]
    public int CritDamatePer;
    public IHealth health;
    private void Start()
    {
        health = GetComponentInParent<IHealth>();
    }
    public void TakeDamage(Vector3 hitPoint,int damage)
    {
        if (health == null) return;
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
