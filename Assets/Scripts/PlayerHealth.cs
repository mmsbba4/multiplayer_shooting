using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerHealth : MonoBehaviourPunCallbacks
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
    public void TakeDamage(int value)
    {
        photonView.RPC("mTakeDamage", RpcTarget.All, value);
    }
    [PunRPC]
    void mTakeDamage(int value)
    {
        CurrentHealth -= value;
        if (CurrentHealth == 0)
        {
            Death();
        }
    }
    void Death()
    {
        characterModel.transform.parent = null;
        GetComponent<PlayerMovement>().mAnimator.SetTrigger("death");
        if (photonView.IsMine) GameManager.instance.Death();
        Destroy(gameObject);
        
    }
}
