using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPopup : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [SerializeField] private TW_AH_UserInterface _animHandler;
    public TW_AH_UserInterface AnimationHandler { get { return _animHandler; } }

    public void Popup()
    {
        //Populate the popup with necessary data, such as icon and name, etc.

        AnimationHandler.PlayAnimation("Popup");
    }
}
