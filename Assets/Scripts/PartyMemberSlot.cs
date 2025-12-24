using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyMemberSlot : MonoBehaviour
{
    [Header("UI Components")]
    public RawImage icon;
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

        int level = 50;
        levelText.text = $"Lv{level}";

        if (icon != null && !string.IsNullOrEmpty(data.sprites.front_default))
        {
            StartCoroutine(PokeApiManager.Instance.GetSprite(data.sprites.front_default, tex => icon.texture = tex));
        }

        int baseHp = 0;
        foreach (var s in data.stats) if (s.stat.name == "hp") baseHp = s.base_stat;
        int maxHp = Mathf.FloorToInt(2 * baseHp * level / 100f) + level + 10;
        
        if (hpText != null)
            hpText.text = $"{maxHp}/{maxHp}";
        if (hpBar != null)
            hpBar.fillAmount = 1.0f;

        if (genderImage != null)
        {
            bool isMale = Random.value > 0.5f;
            genderImage.sprite = isMale ? maleIcon : femaleIcon;
            genderImage.gameObject.SetActive(true);
        }
    }

    void OnSlotClicked()
    {
        if (_partyUIManager != null)
            _partyUIManager.OnPokemonSelected(myIndex);
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }
}
