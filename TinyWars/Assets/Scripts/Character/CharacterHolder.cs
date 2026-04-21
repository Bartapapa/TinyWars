using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum HolderState
{
    None,
    Grabbed,
    Anchoring,
    CannotBeDragged,
    CannotBeInteractedWith,
}

public class CharacterHolder : Holder
{
    [Header("CHARACTER HOLDER OBJECT REFERENCES")]
    [SerializeField] private CombatHandler _combatHandler;
    public CombatHandler CombatHandler { get { return _combatHandler; } }
    [SerializeField] private TagHandler _tagHandler;
    [SerializeField] private TW_AH_C_Combat _animHandler;

    [Header("GRABBING")]
    [SerializeField] private Transform _grabAnchor;
    public Vector3 RootToGrabAnchorOffset { get { return -_grabAnchor.localPosition; } }

    [Header("ANCHORING")]
    [SerializeField] private AnimationCurve _anchorCurve;

    public bool IsAnchoring { get { return _tagHandler.HasTag(HolderState.Anchoring); } }

    private float _mZCoord;
    private Vector3 _originPos;
    private Coroutine _anchorCo = null;
    public override bool IsInteractible
    {
        get { return _canBeInteractedWith && !_combatHandler.TagHandler.HasTag(HolderState.CannotBeInteractedWith); }
    }
    public override bool CanBeDragged
    {
        get { return _canBeDragged && !_combatHandler.TagHandler.HasTag(HolderState.CannotBeDragged); }
    }

    #region Pointer interface implementation
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsInteractible) return;
        if (!CanBeDragged) return;

        _tagHandler.AddTag(HolderState.Grabbed);
        _animHandler.PlayAnimation("Holder_Grabbed");

        _mZCoord = Camera.main.WorldToScreenPoint(_combatHandler.transform.position).z;
        _originPos = _combatHandler.transform.position;

        PointerManager.Instance.SetNewHeldHolder(this);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (!IsInteractible) return;
        if (!CanBeDragged) return;
        if (PointerManager.Instance.CurrentlyHeldHolder != this) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _mZCoord;
        _combatHandler.transform.position = Camera.main.ScreenToWorldPoint(mousePos) + RootToGrabAnchorOffset;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (!IsInteractible) return;
        if (!CanBeDragged) return;
        if (PointerManager.Instance.CurrentlyHeldHolder != this) return;

        _tagHandler.RemoveTag(HolderState.Grabbed);

        //if there is a current Holder Slot, call OnRelease()
        if (PointerManager.Instance.CurrentHolderSlot != null)
        {
            HolderSlotData data = new HolderSlotData(this, this);
            PointerManager.Instance.CurrentHolderSlot.OnGrabbingHolderRelease(data);
        }
        else
        {
            Anchor(_originPos);
        }

        PointerManager.Instance.SetNewHeldHolder(null);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractible) return;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractible) return;
        _animHandler.PlayAdditiveAnimation("Holder_Hovered", 1);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!IsInteractible) return;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!IsInteractible) return;
    }

    #endregion

    public void Anchor(Vector3 toPos, float overTime = .25f)
    {
        if (_anchorCo != null)
        {
            StopCoroutine(_anchorCo);
            _tagHandler.RemoveTag(HolderState.Anchoring);
            _tagHandler.RemoveTag(HolderState.CannotBeDragged);
            _tagHandler.RemoveTag(HolderState.CannotBeInteractedWith);
        }
        _anchorCo = StartCoroutine(AnchorCo(toPos, overTime));
    }
    private IEnumerator AnchorCo(Vector3 toPos, float overTime)
    {
        _tagHandler.AddTag(HolderState.Anchoring);
        _tagHandler.AddTag(HolderState.CannotBeDragged);
        _tagHandler.AddTag(HolderState.CannotBeInteractedWith);

        Vector3 fromPos = _combatHandler.transform.position;
        float timer = 0f;
        float alpha = 0f;
        Vector3 currentPos;
        while (timer < overTime)
        {
            timer += Time.deltaTime;
            alpha = _anchorCurve.Evaluate(timer / overTime);
            currentPos = Vector3.Lerp(fromPos, toPos, alpha);
            _combatHandler.transform.position = currentPos;
            yield return null;
        }

        _combatHandler.transform.position = toPos;

        _tagHandler.RemoveTag(HolderState.Anchoring);
        _tagHandler.RemoveTag(HolderState.CannotBeDragged);
        _tagHandler.RemoveTag(HolderState.CannotBeInteractedWith);

        _anchorCo = null;
    }
}
