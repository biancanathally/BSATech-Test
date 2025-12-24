using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PartyUIManager : MonoBehaviour
{
    [Header("Party UI")]
    public PartyMemberSlot[] partySlots;

    public TMP_Text promptText;

    void Start()
    {
        SetupParty();
        if (promptText != null) promptText.text = "Choose a POKEMON";
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
        if (indexSelected > 0 && indexSelected < GameSession.PlayerParty.Count)
        {
            PokemonData previousLeader = GameSession.PlayerParty[0];
            PokemonData newLeader = GameSession.PlayerParty[indexSelected];

            GameSession.PlayerParty[0] = newLeader;
            GameSession.PlayerParty[indexSelected] = previousLeader;
        }

        SceneManager.LoadScene("BattleScene");
    }

    public void OnCancelButton()
    {
        SceneManager.LoadScene("BattleScene");
    }
}
