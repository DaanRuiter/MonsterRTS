using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CommandWindowDrag : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        transform.parent.position += new Vector3(eventData.delta.x, eventData.delta.y);

        // magic : add zone clamping if's here.
    }
}
