using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

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

    [Header("Transition")]
    public RectTransform transitionPanel;
    public float transitionSpeed = 2.0f;

    private void Start()
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

        if (actionButtons.Length > 2)
        {
            actionButtons[2].GetComponent<Button>().interactable = false;
        }

        if (actionButtons.Length > 3)
        {
            actionButtons[3].GetComponent<Button>().onClick.RemoveAllListeners();
            actionButtons[3].GetComponent<Button>().onClick.AddListener(OnRunButton);
        }

        if (transitionPanel != null)
        {
            transitionPanel.anchoredPosition = Vector2.zero;
            StartCoroutine(SlideInSequence());
        }
        else
        {
            ResumeBattle();
            StartCoroutine(FillPartyInBackground());
        }
    }

    private void Update()
    {
        if (movesPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            OnCloseMovesPanel();
    }

    public IEnumerator FillPartyInBackground()
    {
        if (GameSession.PlayerParty.Count == 1)
        {
            int extraMembersCount = Random.Range(1, 6);

            for (int i = 0; i < extraMembersCount; i++)
            {
                PokeApiManager.Instance.StartCoroutine(GenerateRandomPartyMember());
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private IEnumerator GenerateRandomPartyMember()
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

    private void ResumeBattle()
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

    public void OnFightButton()
    {
        actionsPanel.SetActive(false);
        movesPanel.SetActive(true);
    }

    public void OnPokemonButton()
    {
        StartCoroutine(SlideOutAndLoad("PartyScene"));
    }

    public void OnRunButton()
    {
        StopAllCoroutines();
        PokeApiManager.Instance.StopAllCoroutines();
        StartCoroutine(SlideOutAndLoad("LoadingScene"));
    }

    public void OnBackToActions()
    {
        movesPanel.SetActive(false);
        actionsPanel.SetActive(true);
    }

    public void OnCloseMovesPanel()
    {
        movesPanel.SetActive(false);
        actionsPanel.SetActive(true);

        if (actionButtons.Length > 0)
            SelectAction(actionButtons[0]);
    }

    public void SelectAction(ActionButton selectedButton)
    {
        foreach (var btn in actionButtons)
        {
            btn.SetArrowActive(btn == selectedButton);
        }
    }

    private void SetupEnemyUI(PokemonData data)
    {
        if (data.frontTexture != null)
        {
            enemyPokemonImage.texture = data.frontTexture;
        }
        else
        {
            StartCoroutine(PokeApiManager.Instance.GetSprite(data.sprites.front_default, tex =>
            {
                enemyPokemonImage.texture = tex;
                data.frontTexture = tex;
            }));
        }

        SetupPokemonData(data, false);
    }

    private void SetupAllyUI(PokemonData data)
    {
        if (data.backTexture != null)
        {
            playerPokemonImage.texture = data.backTexture;
        }
        else
        {
            StartCoroutine(PokeApiManager.Instance.GetSprite(data.sprites.back_default, tex =>
            {
                playerPokemonImage.texture = tex;
                data.backTexture = tex;
            }));
        }

        SetupPokemonData(data, true);

        if (dialogueText != null)
            dialogueText.text = $"What will {data.name.FirstCharacterToUpper()} do?";

        for (int i = 0; i < moveButtons.Length; i++)
        {
            if (i < data.moves.Length)
            {
                moveButtons[i].Setup(data.moves[i].move.name, data.moves[i].move.url, this);

                Button btn = moveButtons[i].GetComponent<Button>();
                
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners(); 

                    btn.onClick.AddListener(() => 
                    {
                        Debug.Log("Funcionalidade ainda não foi implementada");
                    });
                }

                if (i == 0)
                    SelectMove(moveButtons[i], data.moves[i].move.url);
            }
            else
            {
                moveButtons[i].Clear();
            }
        }
    }

    private void SetupPokemonData(PokemonData data, bool isPlayer)
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
            hpBar.fillAmount = (float)currentHp / maxHp;

        if (isPlayer && playerPokemonHPText != null)
            playerPokemonHPText.text = $"{currentHp}/{maxHp}";

        if (genderImg != null)
            genderImg.sprite = data.isMale ? maleIcon : femaleIcon;
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

    private void InitializePokemonStats(PokemonData data)
    {
        if (data.isInitialized)
            return;

        data.savedLevel = Random.Range(40, 100);

        int baseHp = 0;
        foreach (var s in data.stats)
        {
            if (s.stat.name == "hp")
                baseHp = s.base_stat;
        }

        data.savedMaxHp = Mathf.FloorToInt(2 * baseHp * data.savedLevel / 100f) + data.savedLevel + 10;
        data.savedCurrentHp = data.savedMaxHp;
        data.isMale = Random.value > 0.5f;
        data.isInitialized = true;
    }

    private IEnumerator SlideInSequence()
    {
        ResumeBattle();
        StartCoroutine(FillPartyInBackground());

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

    public IEnumerator SlideOutAndLoad(string sceneName)
    {
        float screenWidth = transitionPanel.rect.width;

        Vector2 startPos = new Vector2(-screenWidth, 0);
        transitionPanel.anchoredPosition = startPos;

        Vector2 endPos = Vector2.zero;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * transitionSpeed;
            transitionPanel.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
