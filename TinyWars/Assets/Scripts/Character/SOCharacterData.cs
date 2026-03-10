using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    None,
    LittleCrow,
}

[CreateAssetMenu(menuName = "TinyWars/Character/CharacterData", fileName = "CharacterName_Data")]
public class SOCharacterData : ScriptableObject
{
    [Header("CHARACTER TYPE")]
    public CharacterType Character = CharacterType.None;
}
