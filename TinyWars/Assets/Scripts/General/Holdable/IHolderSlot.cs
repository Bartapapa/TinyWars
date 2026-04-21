using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHolderSlot
{
    void OnGrabbingHolderEnter(HolderSlotData data);

    void OnGrabbingHolderExit(HolderSlotData data);

    void OnGrabbingHolderRelease(HolderSlotData data);
}
