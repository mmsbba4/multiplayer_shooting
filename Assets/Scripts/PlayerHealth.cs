using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerHealth : MonoBehaviourPunCallbacks, IHealth
{
    public GameObject characterModel;
    public int CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            if (value <= 0) currentHealth = 0; else currentHealth = value;
        }
    }
    private void Start()
    {
        if (photonView.IsMine)
        GameManager.instance.currentHealth = this;
    }
    [SerializeField]
    int currentHealth = 1000;
    public void TakeDamage(Vector3 hitPoint, int value)
    {
        int tmpValue = Random.Range(-10, 10);
        GameManager.instance.ShowHitDamage(hitPoint, value+ tmpValue);
        photonView.RPC("mTakeDamage", RpcTarget.All, value + tmpValue);
    }
    [PunRPC]
    void mTakeDamage(int value)
    {
        if (photonView.IsMine)
        {
            GameManager.instance.GetHit();
        }
        CurrentHealth -= value;
        if (CurrentHealth == 0)
        {
            Death();
        }
    }
    void Death()
    {
        characterModel.transform.parent = null;
        foreach (var i in GetComponentsInChildren<Collider>())
        {
            Destroy(i);
        }
        GetComponent<PlayerMovement>().mAnimator.SetTrigger("death");
        if (photonView.IsMine) GameManager.instance.OnDead.Invoke();
        Destroy(gameObject);
        
    }
}
