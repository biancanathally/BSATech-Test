using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PartyUIManager : MonoBehaviour
{
    [Header("Party UI")]
    public PartyMemberSlot[] partySlots;
    public TMP_Text promptText;

    [Header("Transition")]
    public RectTransform transitionPanel;
    public float transitionSpeed = 2.0f;

    void Start()
    {
        SetupParty();
        if (promptText != null)
            promptText.text = "Choose a POKEMON";

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
        StartCoroutine(TransitionAndSwap(indexSelected));
    }

    public void OnCancelButton()
    {
        StartCoroutine(TransitionAndExit());
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
