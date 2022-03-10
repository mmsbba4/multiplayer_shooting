using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynceBone : MonoBehaviour
{
    public Transform[] RootBone;
    public Transform[] TargetBone;
    public GameObject object1, object2;
    private void Start()
    {
        RootBone = object1.GetComponentsInChildren<Transform>();
        TargetBone = object2.GetComponentsInChildren<Transform>();
    }
    void Update()
    {

        for (int i = 0; i< RootBone.Length; i++)
        {
            TargetBone[i].localPosition = RootBone[i].localPosition;
            TargetBone[i].localRotation = RootBone[i].localRotation;
        }
    }
}
