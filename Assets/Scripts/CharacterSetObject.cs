using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class CharacterSetObject : MonoBehaviourPunCallbacks
{

    public CharacterSet characterSet;
    public Transform rootBonesParent;
    public Transform targetBonesParent;
    public Transform CharacterInstance;
    public Transform[] rootBones;
    public Transform[] targetBones;
    public Transform rootBonePosition;

    void Start()
    {
        CharacterInstance = Instantiate(characterSet.characterModel, transform).transform;
        CharacterInstance.transform.localPosition = rootBonePosition.localPosition;
        Transform[] tmp = CharacterInstance.GetComponentsInChildren<Transform>();
        foreach (var i in tmp)
        {
            if (i.gameObject.name == "Hips" || i.gameObject.name == "mixamorig:Hips")
            {
                targetBonesParent = i;
                break;
            }
        }
        List<Transform> tmpBones = new List<Transform>();
        rootBones = rootBonesParent.GetComponentsInChildren<Transform>();
        foreach (var i in rootBones)
        {
            if (i.gameObject.name.Contains("mixamorig")) tmpBones.Add(i);
        }
        rootBones = tmpBones.ToArray();
        targetBones = targetBonesParent.GetComponentsInChildren<Transform>();
        GameManager.instance.SetPlayerInfo(characterSet.artWork, PhotonNetwork.LocalPlayer.NickName +" - "+ characterSet.characterName);
    }

    void Update()
    {
        for (int i = 0; i < rootBones.Length; i++)
        {
            targetBones[i].localPosition = rootBones[i].localPosition;
            targetBones[i].localRotation = rootBones[i].localRotation;
        }
    }
}
