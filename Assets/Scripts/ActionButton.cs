using UnityEngine;
using UnityEngine.EventSystems; // Para detectar o mouse
using UnityEngine.UI;

public class ActionButton : MonoBehaviour, IPointerEnterHandler
{
    public GameObject arrowObject;   
    private BattleUIManager _battleUIManager;

    public void Setup(BattleUIManager mainManager)
    {
        _battleUIManager = mainManager;
        
        if (arrowObject != null)
            arrowObject.SetActive(false);
    }

    public void SetArrowActive(bool isActive)
    {
        if (arrowObject != null)
            arrowObject.SetActive(isActive);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _battleUIManager.SelectAction(this);
    }
}
