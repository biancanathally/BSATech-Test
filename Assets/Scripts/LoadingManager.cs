using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text loadingText;
    public Image loadingBarFill;

    void Start()
    {
        GameSession.PlayerParty.Clear();
        GameSession.CurrentEnemy = null;
        GameSession.IsBattleInProgress = false;

        // if (loadingText != null)
        //     loadingText.text = "Searching for wild Pokemon...";

        UpdateProgressBar(0f, "Initializing...");

        StartCoroutine(LoadInitialData());
    }

    void UpdateProgressBar(float progress, string message)
    {
        if (loadingBarFill != null)
            loadingBarFill.fillAmount = progress;
        
        if (loadingText != null)
            loadingText.text = message;
    }

    IEnumerator LoadInitialData()
    {
        bool enemyJsonLoaded = false;
        bool enemySpriteLoaded = false;
        PokemonData tempEnemy = null;

        int enemyId = Random.Range(1, 700);
        yield return StartCoroutine(PokeApiManager.Instance.GetPokemon(enemyId.ToString(),
            onSuccess: (data) =>
            {
                tempEnemy = data;
                enemyJsonLoaded = true;
            },
            onError: () => Debug.LogError("Erro inimigo")));

        if (!enemyJsonLoaded)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            yield break;
        }
        
        UpdateProgressBar(0.25f, "Loading enemy Pokemon...");

        if (tempEnemy != null && !string.IsNullOrEmpty(tempEnemy.sprites.front_default))
        {
            yield return StartCoroutine(PokeApiManager.Instance.GetSprite(tempEnemy.sprites.front_default,
                (tex) =>
                {
                    tempEnemy.frontTexture = tex;
                    enemySpriteLoaded = true;
                }));
        }
        else enemySpriteLoaded = true;

        InitializePokemonStats(tempEnemy);
        GameSession.CurrentEnemy = tempEnemy;

        UpdateProgressBar(0.5f, "Preparing your Pokemon...");
        // if (loadingText != null)
        //     loadingText.text = "Choosing your starter...";

        bool playerReady = false;

        while (!playerReady)
        {
            bool playerJsonLoaded = false;
            bool playerSpriteLoaded = false;
            PokemonData tempPlayer = null;

            int playerId = Random.Range(1, 700);

            yield return StartCoroutine(PokeApiManager.Instance.GetPokemon(playerId.ToString(),
                onSuccess: (data) =>
                {
                    tempPlayer = data;
                    playerJsonLoaded = true;
                }, onError: null));

            if (playerJsonLoaded && !string.IsNullOrEmpty(tempPlayer.sprites.back_default))
            {
                UpdateProgressBar(0.75f, "Loading your Pokemon...");
                
                yield return StartCoroutine(PokeApiManager.Instance.GetSprite(tempPlayer.sprites.back_default,
                    (tex) =>
                    {
                        tempPlayer.backTexture = tex;
                        playerSpriteLoaded = true;
                    }));

                if (playerSpriteLoaded)
                {
                    InitializePokemonStats(tempPlayer);
                    GameSession.PlayerParty.Add(tempPlayer);
                    playerReady = true;
                }
            }
            yield return null;
        }

        // if (loadingText != null)
        //     loadingText.text = "Battle Starting!";

        UpdateProgressBar(1f, "Battle Starting!");

        // yield return new WaitForSeconds(0.2f);
        yield return new WaitForSeconds(1.5f);

        GameSession.IsBattleInProgress = true;
        SceneManager.LoadScene("BattleScene");
    }

    void InitializePokemonStats(PokemonData data)
    {
        if (data.isInitialized) return;

        data.savedLevel = Random.Range(20, 60);

        int baseHp = 0;
        foreach (var s in data.stats) if (s.stat.name == "hp") baseHp = s.base_stat;

        data.savedMaxHp = Mathf.FloorToInt(2 * baseHp * data.savedLevel / 100f) + data.savedLevel + 10;
        data.savedCurrentHp = data.savedMaxHp;
        data.isMale = Random.value > 0.5f;
        data.isInitialized = true;
    }
}
