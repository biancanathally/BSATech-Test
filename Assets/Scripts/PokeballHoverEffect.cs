using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Este script pode ser colocado em QUALQUER botão do jogo
public class PokeballHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Visuals")]
    public Image targetImage;       // A imagem que vai mudar (ex: a pokébola)
    public Sprite normalSprite;     // Sprite cinza
    public Sprite hoverSprite;      // Sprite colorida
    
    [Header("Settings")]
    public bool preserveAspect = true; // Se deve manter a proporção

    private void OnEnable()
    {
        UpdateVisual(false); // Reseta ao ativar
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
