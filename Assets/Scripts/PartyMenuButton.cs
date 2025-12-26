using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PartyMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    public GameObject arrowImage;

    private void OnEnable()
    {
        if (arrowImage != null)
            arrowImage.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (arrowImage != null)
            arrowImage.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (arrowImage != null)
            arrowImage.SetActive(false);
    }
}
