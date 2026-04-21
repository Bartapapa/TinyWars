using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBehavior : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [SerializeField] private TagHandler _tagHandler;
    [SerializeField] private Character _character;
    [SerializeField] private GameObject _UICanvas;

    [Header("DEATH ACTION")]
    [SerializeField] private Action_Die _dieAction;

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
        Vector3 randomDirection = (_character.transform.up * (Random.Range(.7f, 1f)) + (facing * _character.transform.forward * (Random.Range(.2f, 1f))));
        randomDirection = randomDirection.normalized;
        float forceIntensity = Random.Range(.5f, 1f) * 10f;
        Vector3 randomTorque = new Vector3(0, 0, Random.Range(-1f, 1f) * 5f);
        _character.Mesh.Yeet(randomDirection * forceIntensity, randomTorque);

        //Hide stats.
        ToggleUICanvas(false);
    }

    private void ToggleUICanvas(bool show)
    {
        _UICanvas.gameObject.SetActive(show);
    }

    public void DeathBehaviorDie()
    {
        _tagHandler.AddTag(CombatState.Dead, this);

        if (EventDispatcher.Instance)
        {
            List<GameObject> target = new List<GameObject>();
            target.Add(this.gameObject);
            TWAction newAction = _dieAction.GenerateAction(this.gameObject, target);
            ActionContext newContext = new ActionContext(newAction);
            EventDispatcher.Instance.Message_ActionCalled(ref newContext);
        }
    }

    public void DeathBehaviorRevive()
    {
        _tagHandler.RemoveTag(CombatState.Dead, true);

        //Show stats.
        ToggleUICanvas(true);
    }
}
