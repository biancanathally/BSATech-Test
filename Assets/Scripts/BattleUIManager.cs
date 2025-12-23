using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject actionsPanel;
    public GameObject movesPanel;

    [Header("Action Buttons (Main Menu)")]
    public ActionButton[] actionButtons;

    [Header("Pokemon Player")]
    public RawImage pokemonPlayerImage;
    // public Text pokemonPlayerNameText;
    // public Text pokemonPlayerHPText;

    [Header("Pokemon Enemy")]
    public RawImage pokemonEnemyImage;
    // public Text pokemonEnemyNameText;

    [Header("Moves UI")]
    public MoveButton[] moveButtons;
    public TMP_Text moveTypeText;
    public TMP_Text movePPText;

    void Start()
    {
        // moveTypeText.text = "-";
        // movePPText.text = "-/-";

        foreach (var btn in actionButtons)
        {
            btn.Setup(this);
        }

        // Fight button is selected by default
        if (actionButtons.Length > 0)
            SelectAction(actionButtons[0]);

        StartCoroutine(StartBattleSequence());
    }

    public void SelectAction(ActionButton selectedButton)
    {
        foreach (var btn in actionButtons)
        {
            btn.SetArrowActive(btn == selectedButton);
        }
    }

    public void OnFightButton()
    {
        actionsPanel.SetActive(false);
        movesPanel.SetActive(true);
    }

    public void OnBackToActions()
    {
        movesPanel.SetActive(false);
        actionsPanel.SetActive(true);
    }

    IEnumerator StartBattleSequence()
    {
        int enemyRandomId = Random.Range(1, 700);
        yield return StartCoroutine(PokeApiManager.Instance.GetPokemon(enemyRandomId.ToString(),
            onSuccess: (data) =>
            {
                StartCoroutine(PokeApiManager.Instance.GetSprite(data.sprites.front_default, tex => pokemonEnemyImage.texture = tex));
            },
            onError: () => Debug.LogError("Falha ao carregar inimigo")));

        bool playerPokemonFound = false;
        while (!playerPokemonFound)
        {
            int playerRandomId = Random.Range(1, 700);
            yield return null;

            yield return StartCoroutine(PokeApiManager.Instance.GetPokemon(playerRandomId.ToString(),
                onSuccess: (data) =>
                {
                    if (!string.IsNullOrEmpty(data.sprites.back_default))
                    {
                        SetupAllyUI(data);
                        playerPokemonFound = true;
                    }
                },
                onError: null));
        }
    }

    void SetupAllyUI(PokemonData data)
    {
        // pokemonPlayerNameText.text = data.name.ToUpper();
        StartCoroutine(PokeApiManager.Instance.GetSprite(data.sprites.back_default, tex => pokemonPlayerImage.texture = tex));

        int baseHp = 0;
        foreach (var s in data.stats) if (s.stat.name == "hp") baseHp = s.base_stat;
        int lvl = 50;
        int hpTotal = Mathf.FloorToInt(((2 * baseHp) * lvl) / 100f) + lvl + 10;
        // pokemonPlayerHPText.text = $"{hpTotal}/{hpTotal}";

        for (int i = 0; i < moveButtons.Length; i++)
        {
            if (i < data.moves.Length)
            {
                moveButtons[i].Setup(data.moves[i].move.name, data.moves[i].move.url, this);

                if (i == 0)
                    SelectMove(moveButtons[i], data.moves[i].move.url);
            }
            else
            {
                moveButtons[i].Clear();
            }
        }
    }

    public void SelectMove(MoveButton selectedButton, string url)
    {
        foreach (var btn in moveButtons)
        {
            bool isTarget = btn == selectedButton;
            btn.SetArrowActive(isTarget);
        }

        UpdateMoveDetails(url);
    }

    public void UpdateMoveDetails(string url)
    {
        StartCoroutine(PokeApiManager.Instance.GetMoveDetails(url, (details) =>
        {
            moveTypeText.text = details.type.name.ToUpper();
            movePPText.text = $"{details.pp}/{details.pp}";
        }));
    }

    public void OnCloseMovesPanel()
    {
        movesPanel.SetActive(false);
        actionsPanel.SetActive(true);

        if (actionButtons.Length > 0)
            SelectAction(actionButtons[0]); 
    }

    // To handle ESC key to close moves panel
    void Update()
    {
        if (movesPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            OnCloseMovesPanel();
    }
}
