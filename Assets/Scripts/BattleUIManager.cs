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

        // Opcional: Já deixa o primeiro botão (Fight) selecionado visualmente
        if (actionButtons.Length > 0)
        {
            SelectAction(actionButtons[0]);
        }

        StartCoroutine(StartBattleSequence());
    }

    public void SelectAction(ActionButton selectedButton)
    {
        foreach (var btn in actionButtons)
        {
            // Liga a seta se for o botão selecionado, desliga se não for
            btn.SetArrowActive(btn == selectedButton);
        }
    }

    public void OnFightButton()
    {
        actionsPanel.SetActive(false); // Esconde o menu inicial
        movesPanel.SetActive(true);    // Mostra os ataques

        // Opcional: Se quiser garantir que o primeiro ataque já venha selecionado visualmente
        // quando abrir o menu, você pode re-chamar a seleção do primeiro botão aqui,
        // mas a lógica do SetupAllyUI já deve ter cuidado disso.
    }

    // --- NOVA FUNÇÃO (Opcional) ---
    // Para voltar (Botão de cancelar/voltar)
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
        // 1. Troca os painéis
        movesPanel.SetActive(false);
        actionsPanel.SetActive(true);

        // 2. (Opcional) Reseta a seleção visual do Menu Principal
        // Isso garante que a seta volte para o "Fight" ou para onde estava
        if (actionButtons.Length > 0)
        {
            // Se quiser que volte sempre para o Fight (índice 0):
            SelectAction(actionButtons[0]); 
        }
    }

    // DICA EXTRA: Para funcionar com a tecla ESC do teclado
    void Update()
    {
        // Se o painel de ataques estiver aberto e apertar ESC
        if (movesPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            OnCloseMovesPanel();
        }
    }
}
