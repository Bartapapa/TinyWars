using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityPopup : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [SerializeField] private TW_AH_UserInterface _animHandler;
    public TW_AH_UserInterface AnimationHandler { get { return _animHandler; } }
    [SerializeField] private TextMeshProUGUI _abilityText;
    [SerializeField] private Image _abilityIcon;

    public void Popup(TWAbility ability)
    {
        //Populate the popup with necessary data, such as icon and name, etc.
        _abilityText.text = ability.AbilityName;
        _abilityIcon.sprite = ability.AbilityIcon;

        //Play animation.
        AnimationHandler.PlayAnimation("Popup");
    }
}
