using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("CHARACTER DATA")]
    [SerializeField] private SOCharacterData _characterData;
    public SOCharacterData CharacterData {get { return _characterData; }}
}
