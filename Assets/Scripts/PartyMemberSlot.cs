using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyMemberSlot : MonoBehaviour
{
    [Header("UI Components")]
    public RawImage pokemonIcon;
    public TMP_Text nameText;
    public TMP_Text levelText;
    public Image hpBar;
    public TMP_Text hpText;
    public Image genderImage;

    [Header("Assets")]
    public Sprite maleIcon;
    public Sprite femaleIcon;

    private int myIndex;
    private PartyUIManager _partyUIManager;
    private Button myButton;

    private void Awake()
    {
        myButton = GetComponent<Button>();
    }

    public void Setup(PokemonData data, int index, PartyUIManager uiManager)
    {
        gameObject.SetActive(true);

        myIndex = index;
        _partyUIManager = uiManager;

        if (myButton != null)
        {
            myButton.onClick.RemoveAllListeners();
            myButton.onClick.AddListener(OnSlotClicked);
        }

        nameText.text = data.name.ToUpper();
        levelText.text = $"Lv{data.savedLevel}";

        if (pokemonIcon != null && !string.IsNullOrEmpty(data.sprites.front_default))
        {
            StartCoroutine(PokeApiManager.Instance.GetSprite(data.sprites.front_default, tex => pokemonIcon.texture = tex));
        }

        if (hpText != null)
        {
            hpText.text = $"{data.savedCurrentHp}/{data.savedMaxHp}";
        }

        if (hpBar != null)
        {
            hpBar.fillAmount = (float)data.savedCurrentHp / data.savedMaxHp;
        }

        if (genderImage != null)
        {
            genderImage.sprite = data.isMale ? maleIcon : femaleIcon;
            genderImage.gameObject.SetActive(true);
        }
    }

    private void OnSlotClicked()
    {
        if (_partyUIManager != null)
            _partyUIManager.OnPokemonSelected(myIndex);
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }
}
