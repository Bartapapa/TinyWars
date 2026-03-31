using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "TinyWars/Data/UniversalData", fileName = "UniversalData")]
public class SOUniversalData : ScriptableObject
{
    [Header("XP TRACK")]
    [SerializeField] private AnimationCurve _xpTrack = new AnimationCurve();
    public AnimationCurve XPTrack { get { return _xpTrack; } }

    [Header("COMBAT")]
    [SerializeField] private int _maxTeamSlots = 5;
    public int MaxTeamSlots { get { return _maxTeamSlots; } }

    private int IntValueFromCurve(AnimationCurve curve, float time)
    {
        return (int)Math.Round(curve.Evaluate(time), 0);
    }

    public int GetXPValueByLevel(int level)
    {
        return IntValueFromCurve(_xpTrack, level);
    }
}
