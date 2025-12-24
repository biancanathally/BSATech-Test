using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject actionsPanel;
    public GameObject movesPanel;

    [Header("General Assets")]
    public Sprite maleIcon;
    public Sprite femaleIcon;

    [Header("Player Data UI")]
    public RawImage playerPokemonImage;
    public TMP_Text playerPokemonNameText;
    public TMP_Text playerPokemonLevelText;
    public Image playerPokemonGenderImage;
    public TMP_Text playerPokemonHPText;
    public Image playerHPBar;

    [Header("Enemy Data UI")]
    public RawImage enemyPokemonImage;
    public TMP_Text enemyPokemonNameText;
    public TMP_Text enemyPokemonLevelText;
    public Image enemyPokemonGenderImage;
    public Image enemyHPBar;

    [Header("Action Buttons (Main Menu)")]
    public ActionButton[] actionButtons;

    [Header("Moves UI")]
    public MoveButton[] moveButtons;
    public TMP_Text moveTypeText;
    public TMP_Text movePPText;

    [Header("UI Texts")]
    public TMP_Text dialogueText;

    void Start()
    {
        foreach (var btn in actionButtons)
            btn.Setup(this);

        if (actionButtons.Length > 0)
            SelectAction(actionButtons[0]);

        if (actionButtons.Length > 1)
        {
            actionButtons[1].GetComponent<Button>().onClick.RemoveAllListeners();
            actionButtons[1].GetComponent<Button>().onClick.AddListener(OnPokemonButton);
        }

        if (GameSession.IsBattleInProgress)
            ResumeBattle();
        else
            StartCoroutine(StartBattleSequence());
    }

    IEnumerator StartBattleSequence()
    {
        GameSession.IsBattleInProgress = true;

        int enemyId = Random.Range(1, 700);
        yield return StartCoroutine(PokeApiManager.Instance.GetPokemon(enemyId.ToString(),
            onSuccess: (data) =>
            {
                InitializePokemonStats(data);
                GameSession.CurrentEnemy = data;
                SetupEnemyUI(data);
            },
            onError: () => Debug.LogError("Erro Inimigo")));


        if (GameSession.PlayerParty.Count == 0)
        {
            bool playerFound = false;
            while (!playerFound)
            {
                int playerId = Random.Range(1, 700);
                yield return null;
                yield return StartCoroutine(PokeApiManager.Instance.GetPokemon(playerId.ToString(),
                    onSuccess: (data) =>
                    {
                        if (!string.IsNullOrEmpty(data.sprites.back_default))
                        {
                            InitializePokemonStats(data);
                            GameSession.PlayerParty.Add(data);
                            SetupAllyUI(data);
                            playerFound = true;

                            int extraMembersCount = Random.Range(1, 6);

                            for (int i = 0; i < extraMembersCount; i++)
                            {
                                StartCoroutine(GenerateRandomPartyMember());
                            }
                        }
                    },
                    onError: null));
            }
        }
        else
        {
            SetupAllyUI(GameSession.PlayerParty[0]);
        }
    }

    IEnumerator GenerateRandomPartyMember()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));

        int randomId = Random.Range(1, 700);
        yield return StartCoroutine(PokeApiManager.Instance.GetPokemon(randomId.ToString(),
            onSuccess: (newData) =>
            {
                if (GameSession.PlayerParty.Count < 6)
                {
                    InitializePokemonStats(newData);
                    GameSession.PlayerParty.Add(newData);
                }
            },
            onError: null));
    }

    void ResumeBattle()
    {
        if (GameSession.CurrentEnemy != null)
        {
            SetupEnemyUI(GameSession.CurrentEnemy);
        }

        if (GameSession.PlayerParty.Count > 0)
        {
            SetupAllyUI(GameSession.PlayerParty[0]);
        }
    }

    void SetupEnemyUI(PokemonData data)
    {
        StartCoroutine(PokeApiManager.Instance.GetSprite(data.sprites.front_default, tex => enemyPokemonImage.texture = tex));
        SetupPokemonData(data, false);
    }

    public void OnPokemonButton()
    {
        SceneManager.LoadScene("PartyScene");
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

    void SetupAllyUI(PokemonData data)
    {
        StartCoroutine(PokeApiManager.Instance.GetSprite(data.sprites.back_default, tex => playerPokemonImage.texture = tex));

        SetupPokemonData(data, true);

        if (dialogueText != null)
            dialogueText.text = $"What will {data.name.ToUpper()} do?";

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

    void SetupPokemonData(PokemonData data, bool isPlayer)
    {
        if (!data.isInitialized)
            InitializePokemonStats(data);

        int level = data.savedLevel; 
        int currentHp = data.savedCurrentHp;
        int maxHp = data.savedMaxHp;

        TMP_Text nameTxt = isPlayer ? playerPokemonNameText : enemyPokemonNameText;
        TMP_Text lvlTxt = isPlayer ? playerPokemonLevelText : enemyPokemonLevelText;
        Image genderImg = isPlayer ? playerPokemonGenderImage : enemyPokemonGenderImage;
        Image hpBar = isPlayer ? playerHPBar : enemyHPBar;

        if (nameTxt != null)
            nameTxt.text = data.name.ToUpper();

        if (lvlTxt != null)
            lvlTxt.text = $"Lv{level}";
        
        if (hpBar != null)
        {
            hpBar.fillAmount = (float)currentHp / maxHp;
        }

        if (isPlayer && playerPokemonHPText != null)
        {
            playerPokemonHPText.text = $"{currentHp}/{maxHp}";
        }

        if (genderImg != null)
        {
            bool isMale = Random.value > 0.5f;
            genderImg.sprite = isMale ? maleIcon : femaleIcon;
        }

        int baseHp = 0;
        foreach (var s in data.stats) if (s.stat.name == "hp") baseHp = s.base_stat;

        if (hpBar != null)
            hpBar.fillAmount = 1.0f;

        if (isPlayer && playerPokemonHPText != null)
            playerPokemonHPText.text = $"{maxHp}/{maxHp}";
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

    void InitializePokemonStats(PokemonData data)
    {
        if (data.isInitialized)
            return;

        data.savedLevel = Random.Range(40, 100);

        int baseHp = 0;
        foreach (var s in data.stats) if (s.stat.name == "hp") baseHp = s.base_stat;
        
        data.savedMaxHp = Mathf.FloorToInt(2 * baseHp * data.savedLevel / 100f) + data.savedLevel + 10;
        data.savedCurrentHp = data.savedMaxHp;
        data.isMale = Random.value > 0.5f;
        data.isInitialized = true;
    }
}
