using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("OBJECT REFS")]
    [SerializeField] private CharacterMesh _mesh;
    public CharacterMesh Mesh { get { return _mesh; } }

    [Header("CHARACTER DATA")]
    [SerializeField] private SOCharacterData _characterData;
    public SOCharacterData CharacterData {get { return _characterData; }}
}
