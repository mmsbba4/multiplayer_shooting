using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLookTarget : MonoBehaviour
{

    void Update()
    {
        AimingUpdate();
    }
    void AimingUpdate()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 400))
        {
            transform.position = hit.point;
        }
        else
        {
            transform.localPosition = new Vector3(0, 0, 400f);
        }
    }
}
