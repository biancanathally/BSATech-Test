using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Necessário para detectar o mouse
using TMPro;

public class MoveButton : MonoBehaviour, IPointerEnterHandler
{
    [Header("Moves Button Components")]
    public TMP_Text moveButtonText;
    public Image arrowImage;
    
    [Header("Move Data")]
    private string _moveUrl;
    private BattleUIManager _battleUIManager;

    public void Setup(string name, string url, BattleUIManager mainManager)
    {
        moveButtonText.text = name.ToUpper();
        _moveUrl = url;
        _battleUIManager = mainManager;
        
        GetComponent<Button>().interactable = true;

        if (arrowImage != null)
            arrowImage.gameObject.SetActive(false);
    }

    public void Clear()
    {
        moveButtonText.text = "-";
        GetComponent<Button>().interactable = false;
        _moveUrl = "";

        if (arrowImage != null)
            arrowImage.gameObject.SetActive(false);
    }

    public void SetArrowActive(bool isActive)
    {
        if (arrowImage != null)
            arrowImage.gameObject.SetActive(isActive);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(_moveUrl))
            _battleUIManager.SelectMove(this, _moveUrl);
    }
}
