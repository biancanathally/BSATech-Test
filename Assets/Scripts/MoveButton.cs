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
    private string _attackUrl; // Guarda a URL para buscar detalhes (PP/Type)
    private BattleUIManager _battleUIManager; // Referência ao gerente para avisar sobre o mouse

    public void Setup(string name, string url, BattleUIManager mainManager)
    {
        attackButtonText.text = name.ToUpper();
        _attackUrl = url;
        _battleUIManager = mainManager;
        
        // Torna o botão clicável/interativo
        GetComponent<Button>().interactable = true;

        // if (arrowImage != null)
        //     arrowImage.enabled = false;
        if (arrowImage != null) 
        {
            // MUDANÇA: Desativa o Objeto inteiro, não só a imagem
            arrowImage.gameObject.SetActive(false); 
        }
    }

    public void Clear()
    {
        attackButtonText.text = "-";
        GetComponent<Button>().interactable = false;
        _attackUrl = "";

        if(arrowImage != null) arrowImage.enabled = false;
    }

    public void SetArrowActive(bool isActive)
    {
        if (arrowImage != null)
            // arrowImage.enabled = isActive;
            arrowImage.gameObject.SetActive(isActive);
    }

    // Detecta automaticamente quando o mouse entra no botão
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(_attackUrl))
        {
            // // Avisa o gerente para atualizar o texto de detalhes
            // _battleUIManager.UpdateMoveDetails(_attackUrl);

            _battleUIManager.SelectMove(this, _attackUrl);
        }
    }
}
