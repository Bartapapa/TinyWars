using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CombatRowSlot : MonoBehaviour, IHolderSlot, IPointerEnterHandler, IPointerExitHandler
{
    public CombatHandler Fighter = null;

    #region Interface implementation
    public void OnGrabbingHolderEnter(HolderSlotData data)
    {
        if (PointerManager.Instance.CurrentlyHeldHolder != null)
        {
            if ((object)PointerManager.Instance.CurrentHolderSlot == this && data.Character != null)
            {
                //Make ghost image.
            }
        }
    }

    public void OnGrabbingHolderExit(HolderSlotData data)
    {
        if (PointerManager.Instance.CurrentlyHeldHolder != null && data.Character != null)
        {
            //Remove ghost image.
        }
    }

    public void OnGrabbingHolderRelease(HolderSlotData data)
    {
        if (data.Character != null)
        {
            OnGrabbingHolderExit(data);
            PointerManager.Instance.SetNewHolderSlot(null);

            data.Character.Anchor(this.transform.position);
            Fighter = data.Character.CombatHandler;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (PointerManager.Instance.CurrentlyHeldHolder != null)
        {
            Holder holder = PointerManager.Instance.CurrentlyHeldHolder;
            CharacterHolder charHolder = (CharacterHolder)PointerManager.Instance.CurrentlyHeldHolder;

            PointerManager.Instance.SetNewHolderSlot(this);

            HolderSlotData data = new HolderSlotData(holder, charHolder);
            OnGrabbingHolderEnter(data);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (PointerManager.Instance.CurrentlyHeldHolder != null)
        {
            Holder holder = PointerManager.Instance.CurrentlyHeldHolder;
            CharacterHolder charHolder = (CharacterHolder)PointerManager.Instance.CurrentlyHeldHolder;

            PointerManager.Instance.SetNewHolderSlot(null);

            HolderSlotData data = new HolderSlotData(holder, charHolder);
            OnGrabbingHolderExit(data);
        }
    }
    #endregion
}
