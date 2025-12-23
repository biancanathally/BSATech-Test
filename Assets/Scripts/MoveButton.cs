using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Necessário para detectar o mouse
using TMPro;

public class MoveButton : MonoBehaviour, IPointerEnterHandler
{
    [Header("Attack Button Components")]
    public TMP_Text attackButtonText;
    public Image arrowImage;
    
    [Header("Attack Data")]
    private string _attackUrl;
    private BattleUIManager _battleUIManager;

    public void Setup(string name, string url, BattleUIManager mainManager)
    {
        attackButtonText.text = name.ToUpper();
        _attackUrl = url;
        _battleUIManager = mainManager;
        
        GetComponent<Button>().interactable = true;

        if (arrowImage != null)
            arrowImage.gameObject.SetActive(false);
    }

    public void Clear()
    {
        attackButtonText.text = "-";
        GetComponent<Button>().interactable = false;
        _attackUrl = "";

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
        if (!string.IsNullOrEmpty(_attackUrl))
            _battleUIManager.SelectMove(this, _attackUrl);
    }
}
