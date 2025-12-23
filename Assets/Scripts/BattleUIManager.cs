using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    [Header("Pokemon Player")]
    public RawImage allyImage;
    // public Text allyNameText;
    // public Text allyHPText;

    [Header("Pokemon Enemy")]
    public RawImage enemyImage;
    // public Text enemyNameText;

    [Header("Attacks UI")]
    public MoveButton[] moveButtons; 
    
    // --- ALTERAÇÃO 1: Criamos duas variáveis separadas ---
    public TMP_Text moveTypeText; // Arraste o texto que mostra "TYPE" aqui
    public TMP_Text movePPText;   // Arraste o texto que mostra "PP" aqui

    void Start()
    {
        // moveTypeText.text = "-";
        // movePPText.text = "-/-";

        StartCoroutine(StartBattleSequence());
    }

    IEnumerator StartBattleSequence()
    {
        // 1. Configurar Inimigo (Gengar)
        int enemyRandomId = Random.Range(1, 700);
        yield return StartCoroutine(PokeApiManager.Instance.GetPokemon(enemyRandomId.ToString(),
            onSuccess: (data) =>
            {
                // enemyNameText.text = data.name.ToUpper();
                StartCoroutine(PokeApiManager.Instance.GetSprite(data.sprites.front_default, tex => enemyImage.texture = tex));
            },
            onError: () => Debug.LogError("Falha ao carregar inimigo")));

        // 2. Configurar Aliado 
        bool allyFound = false;
        while (!allyFound)
        {
            int playerRandomId = Random.Range(1, 700);
            yield return null;

            yield return StartCoroutine(PokeApiManager.Instance.GetPokemon(playerRandomId.ToString(),
                onSuccess: (data) =>
                {
                    if (!string.IsNullOrEmpty(data.sprites.back_default))
                    {
                        SetupAllyUI(data);
                        allyFound = true;
                    }
                },
                onError: null));
        }
    }

    void SetupAllyUI(PokemonData data)
    {
        // allyNameText.text = data.name.ToUpper();
        StartCoroutine(PokeApiManager.Instance.GetSprite(data.sprites.back_default, tex => allyImage.texture = tex));

        int baseHp = 0;
        foreach (var s in data.stats) if (s.stat.name == "hp") baseHp = s.base_stat;
        int lvl = 50;
        int hpTotal = Mathf.FloorToInt(((2 * baseHp) * lvl) / 100f) + lvl + 10;
        // allyHPText.text = $"{hpTotal}/{hpTotal}";

        // Botões de Ataque
        for (int i = 0; i < moveButtons.Length; i++)
        {
            if (i < data.moves.Length)
            {
                // 1. Configura o botão normalmente
                moveButtons[i].Setup(data.moves[i].move.name, data.moves[i].move.url, this);

                // --- ALTERAÇÃO AQUI ---
                // 2. Se for o PRIMEIRO ataque (índice 0), forçamos a atualização dos detalhes imediatamente
                if (i == 0)
                {
                    // // Chama a mesma função que o clique do botão chamaria
                    // UpdateMoveDetails(data.moves[i].move.url);
                    
                    // // Opcional: Se seus botões tiverem um visual de "selecionado",
                    // // você pode ativar o visual do botão 0 aqui também.

                    SelectMove(moveButtons[i], data.moves[i].move.url);
                }
            }
            else
            {
                moveButtons[i].Clear();
            }
        }

        // for (int i = 0; i < moveButtons.Length; i++)
        // {
        //     if (i < data.moves.Length)
        //     {
        //         moveButtons[i].Setup(data.moves[i].move.name, data.moves[i].move.url, this);
        //     }
        //     else
        //     {
        //         moveButtons[i].Clear();
        //     }
        // }
    }

    // --- NOVA FUNÇÃO CENTRALIZADA ---
    // Chamada tanto pelo OnPointerEnter do botão quanto pelo Setup inicial
    public void SelectMove(MoveButton selectedButton, string url)
    {
        // 1. Parte Visual: Liga a seta só do selecionado, desliga dos outros
        foreach (var btn in moveButtons)
        {
            // Se o botão do loop for igual ao botão selecionado, true. Senão, false.
            bool isTarget = btn == selectedButton;
            btn.SetArrowActive(isTarget);
        }

        // 2. Parte de Dados: Chama a atualização dos textos
        UpdateMoveDetails(url);
    }

    // --- ALTERAÇÃO 2: Atualizamos os dois textos separadamente ---
    public void UpdateMoveDetails(string url)
    {
        // // Colocamos um texto temporário enquanto carrega
        // moveTypeText.text = "TYPE: ...";
        // movePPText.text = "PP: ...";

        StartCoroutine(PokeApiManager.Instance.GetMoveDetails(url, (details) => {
            // Aqui dividimos a informação
            moveTypeText.text = details.type.name.ToUpper();
            movePPText.text = $"{details.pp}/{details.pp}";

            // ativar o asset da seta
            
        }));
    }
}
