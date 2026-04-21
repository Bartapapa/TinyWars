using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public struct HolderSlotData
{
    public Holder Holder;
    public CharacterHolder Character;

    public HolderSlotData(Holder holder, CharacterHolder characterHolder = null)
    {
        Holder = holder;
        Character = characterHolder;
    }
}

public class Holder : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [ReadOnlyInspector][SerializeField] protected bool _canBeInteractedWith = true;
    public virtual bool IsInteractible { get { return _canBeInteractedWith; } }
    [ReadOnlyInspector]
    [SerializeField] protected bool _canBeDragged = true;
    public virtual bool CanBeDragged { get { return _canBeDragged; } }

    #region Unity-based pointer interfaces
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractible) return;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractible) return;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!IsInteractible) return;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (!IsInteractible) return;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsInteractible) return;
        if (!CanBeDragged) return;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!IsInteractible) return;
        if (!CanBeDragged) return;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!IsInteractible) return;
        if (!CanBeDragged) return;
    }

    #endregion
}
