using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerShooting : MonoBehaviourPunCallbacks
{
    public int gunDamage = 200;
    public float shootDelay = 0.5f;
    private float deltaDelay = 0;
    public PlayerInput inputListener;
    public Transform startRay, endRay;

    void Update()
    {
        if (!photonView.IsMine) return;
        if (inputListener.fire && deltaDelay > shootDelay)
        {
            Shoot();
            deltaDelay = 0;
        }
        else
        {
            deltaDelay += Time.deltaTime;
        }
    }
    
    void Shoot()
    {
        Ray ray = new Ray(startRay.position, endRay.position-startRay.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 400))
        {
            if (hit.transform.gameObject.layer == 7)
            {
                photonView.RPC("PlayerHitEffect", RpcTarget.All, hit.point, hit.normal);
                if (hit.collider.gameObject.GetComponent<DamageCollider>() != null)
                {
                    hit.collider.gameObject.GetComponent<DamageCollider>().TakeDamage(hit.point,gunDamage);
                    
                }
            }
            else
            {
                photonView.RPC("ShootingEffect", RpcTarget.All, hit.point, hit.normal);
            }
        }
    }
    [PunRPC]
    void PlayerHitEffect(Vector3 point, Vector3 normal)
    {
        Instantiate(Resources.Load("hit_effect"), point, Quaternion.LookRotation(normal));
        Instantiate(Resources.Load("gun_effect"), startRay.position, startRay.rotation, startRay);
    }
    [PunRPC]
    void ShootingEffect(Vector3 point, Vector3 normal)
    {
        Instantiate(Resources.Load("shoot_effect"), point, Quaternion.LookRotation(normal));
        Instantiate(Resources.Load("gun_effect"), startRay.position, startRay.rotation, startRay);
    }

}
