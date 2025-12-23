using UnityEngine;
using UnityEngine.EventSystems; // Para detectar o mouse
using UnityEngine.UI;

public class ActionButton : MonoBehaviour, IPointerEnterHandler
{
    [Header("Configuração Visual")]
    public GameObject arrowObject; // A seta (Image ou GameObject)
    
    private BattleUIManager manager;

    // Função chamada pelo Gerente no Start
    public void Setup(BattleUIManager mainManager)
    {
        manager = mainManager;
        
        // Garante que começa desligado para não ficarem todos acesos
        if (arrowObject != null)
            arrowObject.SetActive(false);
    }

    // Função para ligar/desligar a seta
    public void SetArrowActive(bool isActive)
    {
        if (arrowObject != null)
            arrowObject.SetActive(isActive);
    }

    // Quando o mouse passa por cima
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Avisa o gerente: "Eu fui selecionado, desligue os outros!"
        manager.SelectAction(this);
    }
}
