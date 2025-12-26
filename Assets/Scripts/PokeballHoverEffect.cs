using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PokeballHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Visuals")]
    public Image targetImage;
    public Sprite normalSprite;
    public Sprite hoverSprite;
    
    [Header("Settings")]
    public bool preserveAspect = true;

    private void OnEnable()
    {
        UpdateVisual(false);
    }

    public void OnPointerEnter(PointerEventData eventData) => UpdateVisual(true);
    public void OnPointerExit(PointerEventData eventData) => UpdateVisual(false);
    public void OnSelect(BaseEventData eventData) => UpdateVisual(true);
    public void OnDeselect(BaseEventData eventData) => UpdateVisual(false);

    private void UpdateVisual(bool active)
    {
        if (targetImage == null) return;

        targetImage.sprite = active ? hoverSprite : normalSprite;
        targetImage.preserveAspect = active ? preserveAspect : false;
    }
}
