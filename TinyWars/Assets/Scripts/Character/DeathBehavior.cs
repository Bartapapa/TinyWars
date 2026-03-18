using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBehavior : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [SerializeField] private Character _character;
    [SerializeField] private CombatHandler _combatHandler;
    [SerializeField] private GameObject _UICanvas;

    private void Start()
    {
        if (EventDispatcher.Instance)
        {
            EventDispatcher.Instance.FighterCorpseCleared -= OnFighterCorpseCleared;
            EventDispatcher.Instance.FighterCorpseCleared += OnFighterCorpseCleared;
        }
    }

    private void OnDisable()
    {
        if (EventDispatcher.Instance)
        {
            EventDispatcher.Instance.FighterCorpseCleared -= OnFighterCorpseCleared;
        }
    }

    private void OnFighterCorpseCleared(FighterContext context)
    {
        if (context.Fighter.gameObject != this.gameObject) return;

        //Play corpse animation.
        float facing = _character.Mesh.FacingRight ? -1f : 1f;
        Vector3 randomDirection = (_combatHandler.transform.up * (Random.Range(.7f, 1f)) + (facing * _combatHandler.transform.forward * (Random.Range(.2f, 1f))));
        randomDirection = randomDirection.normalized;
        float forceIntensity = Random.Range(.5f, 1f) * 10f;
        Vector3 randomTorque = new Vector3(0, 0, Random.Range(-1f, 1f) * 5f);
        _combatHandler.Character.Mesh.Yeet(randomDirection * forceIntensity, randomTorque);

        //Hide stats.
        _UICanvas.gameObject.SetActive(false);
    }
}
