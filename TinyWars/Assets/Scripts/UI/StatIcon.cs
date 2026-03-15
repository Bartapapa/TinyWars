using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class StatIcon : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [SerializeField] private TMP_Text _statValueText;
    [SerializeField] private CombatHandler _combatHandler;
    [SerializeField] private CharacterStat _characterStat = CharacterStat.Health;

    private Statistic _associatedStat;

    private void Start()
    {
        switch (_characterStat)
        {
            case CharacterStat.Health:
                _associatedStat = _combatHandler.Health;
                break;
            case CharacterStat.Attack:
                _associatedStat = _combatHandler.Attack;
                break;
            default:
                break;
        }

        _statValueText.text = _associatedStat.Value.ToString();

        _associatedStat.StatisticValueChanged -= OnStatisticValueChanged;
        _associatedStat.StatisticValueChanged += OnStatisticValueChanged;
    }

    public void OnStatisticValueChanged(float from, float to)
    {
        _statValueText.text = Mathf.Round(to).ToString();
    }
}
