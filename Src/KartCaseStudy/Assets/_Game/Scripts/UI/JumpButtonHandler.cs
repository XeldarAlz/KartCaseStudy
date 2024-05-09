using KartSystem.KartSystems;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private UIInput uiInput;

    private void Start()
    {
        uiInput = FindObjectOfType<UIInput>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        uiInput.OnPointerDown(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        uiInput.OnPointerUp(eventData);
    }
}