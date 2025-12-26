using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class SummaryUIManager : MonoBehaviour
{
    [Header("Basic Info")]
    public RawImage pokemonIcon;
    public TMP_Text pokemonNameText;
    public TMP_Text levelText;
    public TMP_Text idNoText;
    public Image genderImage;
    public Sprite maleSprite;
    public Sprite femaleSprite;

    [Header("Profile Details")]
    public TMP_Text type1Text;
    public Image type1Bg;
    public TMP_Text type2Text;
    public Image type2Bg;

    [Header("Ability")]
    public TMP_Text abilityNameText;
    public TMP_Text abilityDescText;

    [Header("Buttons")]
    public Button cancelButton;

    private PokemonData _currentData;

    void Start()
    {
        if (GameSession.PokemonIndexToView < 0 || GameSession.PokemonIndexToView >= GameSession.PlayerParty.Count)
        {
            Debug.LogError("Nenhum Pokemon selecionado para Summary!");
            SceneManager.LoadScene("PartyScene");
            return;
        }

        _currentData = GameSession.PlayerParty[GameSession.PokemonIndexToView];

        SetupBasicInfo();

        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelButton);
    }

    void SetupBasicInfo()
    {
        if (pokemonNameText != null)
            pokemonNameText.text = _currentData.name.FirstCharacterToUpper();
        if (levelText != null)
            levelText.text = $"Lv{_currentData.savedLevel}";

        if (idNoText != null)
            idNoText.text = _currentData.id.ToString("00000");

        if (pokemonIcon != null)
        {
            if (_currentData.frontTexture != null)
                pokemonIcon.texture = _currentData.frontTexture;
            else if (_currentData.sprites != null)
                StartCoroutine(PokeApiManager.Instance.GetSprite(_currentData.sprites.front_default, tex => pokemonIcon.texture = tex));
        }

        if (genderImage != null)
        {
            genderImage.sprite = _currentData.isMale ? maleSprite : femaleSprite;
            genderImage.gameObject.SetActive(true);
        }

        // Types
        if (_currentData.types != null && _currentData.types.Length > 0)
        {
            // Type 1
            if (type1Text != null)
                type1Text.text = _currentData.types[0].type.name.ToUpper();
            if (type1Bg != null)
            {
                type1Bg.gameObject.SetActive(true);
                type1Bg.color = GetTypeColor(_currentData.types[0].type.name);
            }

            // Type 2
            if (_currentData.types.Length > 1)
            {
                if (type2Text != null) type2Text.text = _currentData.types[1].type.name.ToUpper();
                if (type2Bg != null)
                {
                    type2Bg.gameObject.SetActive(true);
                    type2Bg.color = GetTypeColor(_currentData.types[1].type.name);
                }
            }
            else
            {
                if (type2Bg != null) type2Bg.gameObject.SetActive(false);
            }
        }

        // if (_currentData.abilities != null && _currentData.abilities.Length > 0 && abilityNameText != null)
        // {
        //     abilityNameText.text = _currentData.abilities[0].ability.name.ToUpper();
        // }

        if (_currentData.abilities != null && _currentData.abilities.Length > 0)
        {
            // 1. Define o Nome
            if (abilityNameText != null)
                abilityNameText.text = _currentData.abilities[0].ability.name.ToUpper();

            // 2. Busca a Descrição (NOVO)
            if (abilityDescText != null)
            {
                abilityDescText.text = "Loading...";
                string abilityUrl = _currentData.abilities[0].ability.url;

                if (!string.IsNullOrEmpty(abilityUrl))
                {
                    StartCoroutine(PokeApiManager.Instance.GetAbilityDescription(abilityUrl, (desc) =>
                    {
                        abilityDescText.text = desc;
                    }));
                }
                else
                {
                    abilityDescText.text = "-";
                }
            }
        }
    }

    Color GetTypeColor(string typeName)
    {
        switch (typeName.ToLower())
        {
            case "fire": return new Color(1f, 0.5f, 0f); // Laranja
            case "water": return new Color(0f, 0.5f, 1f); // Azul
            case "grass": return new Color(0.3f, 0.8f, 0.3f); // Verde
            case "electric": return new Color(1f, 1f, 0f); // Amarelo
            case "psychic": return new Color(1f, 0.4f, 0.7f); // Rosa
            case "ice": return new Color(0.5f, 0.9f, 1f); // Ciano
            case "dragon": return new Color(0.4f, 0.3f, 1f); // Roxo
            case "dark": return new Color(0.4f, 0.4f, 0.4f); // Cinza Escuro
            case "fairy": return new Color(1f, 0.7f, 0.8f); // Rosa Claro
            default: return Color.gray;
        }
    }

    void OnCancelButton()
    {
        SceneManager.LoadScene("PartyScene");
    }
}
