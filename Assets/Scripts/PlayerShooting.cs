using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public float shootDelay = 0.5f;
    private float deltaDelay = 0;
    public PlayerInput inputListener;

    void Update()
    {
        deltaDelay += Time.deltaTime;
        if (inputListener)
    }

}
