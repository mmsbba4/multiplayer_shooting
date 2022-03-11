using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName ="Character/Character Set")]
public class CharacterSet : ScriptableObject
{
    public string characterName;
    public Sprite artWork;
    public GameObject characterModel;
}
