using UnityEngine;
using UnityEngine.EventSystems;

public class MenuItem : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public int index;

    public void OnPointerEnter(PointerEventData eventData)
    {
        PauseManager.Instance.SetSelectionByPointer(index);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PauseManager.Instance.SetSelectionByPointer(index);
        PauseManager.Instance.ClickCurrentSelection();
    }
}