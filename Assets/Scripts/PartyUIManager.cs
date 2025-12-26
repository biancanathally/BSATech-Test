using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PartyUIManager : MonoBehaviour
{
    [Header("Party UI")]
    public PartyMemberSlot[] partySlots;
    public TMP_Text promptText;

    [Header("Dialogue Background Objects")]
    public GameObject defaultBackgroundObject;
    public GameObject popupBackgroundObject;

    [Header("Context Menu (Pop-up)")]
    public GameObject contextMenuPanel;
    public Button summaryButton;
    public Button switchButton;
    public Button cancelButton;

    [Header("Transition")]
    public RectTransform transitionPanel;
    public float transitionSpeed = 2.0f;

    private int _pendingSlotIndex = -1;

    void Start()
    {
        SetupParty();

        UpdatePromptText("Choose a POKEMON");

        if (defaultBackgroundObject != null)
            defaultBackgroundObject.SetActive(true);
        if (popupBackgroundObject != null)
            popupBackgroundObject.SetActive(false);

        if (contextMenuPanel != null)
            contextMenuPanel.SetActive(false);

        if (summaryButton != null)
            summaryButton.onClick.AddListener(OnSummaryAction);

        if (switchButton != null)
            switchButton.onClick.AddListener(OnSwitchAction);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelMenuAction);

        if (transitionPanel != null)
        {
            transitionPanel.anchoredPosition = Vector2.zero;
            StartCoroutine(SlideInSequence());
        }
    }

    void SetupParty()
    {
        foreach (var slot in partySlots)
            slot.Clear();

        for (int i = 0; i < GameSession.PlayerParty.Count; i++)
        {
            if (i < partySlots.Length)
                partySlots[i].Setup(GameSession.PlayerParty[i], i, this);
        }
    }

    public void OnPokemonSelected(int indexSelected)
    {
        _pendingSlotIndex = indexSelected;

        if (contextMenuPanel != null)
            contextMenuPanel.SetActive(true);

        if (defaultBackgroundObject != null)
            defaultBackgroundObject.SetActive(false);
        if (popupBackgroundObject != null)
            popupBackgroundObject.SetActive(true);

        UpdatePromptText("Do what with this PKMN?");
    }

    private void OnSummaryAction()
    {
        SceneManager.LoadScene("SummaryScene");
    }

    private void OnSwitchAction()
    {
        int indexToSwap = _pendingSlotIndex;

        OnCancelMenuAction();
        StartCoroutine(TransitionAndSwap(indexToSwap));
    }

    private void OnCancelMenuAction()
    {
        if (contextMenuPanel != null)
            contextMenuPanel.SetActive(false);
        
        if (defaultBackgroundObject != null)
            defaultBackgroundObject.SetActive(true);
        if (popupBackgroundObject != null)
            popupBackgroundObject.SetActive(false);

        UpdatePromptText("Choose a POKEMON");

        _pendingSlotIndex = -1;
    }

    public void OnCancelButton()
    {
        if (contextMenuPanel != null && contextMenuPanel.activeSelf)
        {
            OnCancelMenuAction();
            return;
        }

        StartCoroutine(TransitionAndExit());
    }

    private void UpdatePromptText(string text)
    {
        if (promptText != null)
            promptText.text = text;
    }

    private IEnumerator SlideInSequence()
    {
        yield return new WaitForSeconds(0.1f);

        float screenWidth = transitionPanel.rect.width;
        Vector2 startPos = Vector2.zero;
        Vector2 endPos = new Vector2(screenWidth, 0);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            transitionPanel.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
    }

    private IEnumerator TransitionAndSwap(int indexSelected)
    {
        if (transitionPanel != null)
        {
            float screenWidth = transitionPanel.rect.width;

            Vector2 startPos = new Vector2(-screenWidth, 0);
            Vector2 endPos = Vector2.zero;

            transitionPanel.anchoredPosition = startPos;

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * transitionSpeed;
                transitionPanel.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                yield return null;
            }
        }

        if (indexSelected > 0 && indexSelected < GameSession.PlayerParty.Count)
        {
            PokemonData previousLeader = GameSession.PlayerParty[0];
            PokemonData newLeader = GameSession.PlayerParty[indexSelected];

            GameSession.PlayerParty[0] = newLeader;
            GameSession.PlayerParty[indexSelected] = previousLeader;
        }

        SceneManager.LoadScene("BattleScene");
    }

    private IEnumerator TransitionAndExit()
    {
        if (transitionPanel != null)
        {
            float screenWidth = transitionPanel.rect.width;
            Vector2 startPos = new Vector2(-screenWidth, 0);
            Vector2 endPos = Vector2.zero;

            transitionPanel.anchoredPosition = startPos;

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * transitionSpeed;
                transitionPanel.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                yield return null;
            }
        }
        SceneManager.LoadScene("BattleScene");
    }
}
