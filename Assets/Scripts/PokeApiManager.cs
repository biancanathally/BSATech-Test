using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PokeApiManager : MonoBehaviour
{
    private static PokeApiManager _instance;

    public static PokeApiManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PokeApiManager>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject("PokeApiService_Auto");
                    _instance = obj.AddComponent<PokeApiManager>();
                }
            }
            return _instance;
        }
    }

    public IEnumerator GetPokemon(string idOrName, System.Action<PokemonData> onSuccess, System.Action onError)
    {
        string url = $"https://pokeapi.co/api/v2/pokemon/{idOrName}";
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                PokemonData data = JsonUtility.FromJson<PokemonData>(req.downloadHandler.text);
                onSuccess?.Invoke(data);
            }
            else
            {
                Debug.LogError($"Erro ao buscar Pokemon: {req.error}");
                onError?.Invoke();
            }
        }
    }

    public IEnumerator GetSprite(string url, System.Action<Texture2D> onSuccess)
    {
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(url))
        {
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                Texture2D tex = DownloadHandlerTexture.GetContent(req);
                tex.filterMode = FilterMode.Point;
                onSuccess?.Invoke(tex);
            }
        }
    }

    public IEnumerator GetMoveDetails(string url, System.Action<MoveDetailsData> onSuccess)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                MoveDetailsData data = JsonUtility.FromJson<MoveDetailsData>(req.downloadHandler.text);
                onSuccess?.Invoke(data);
            }
        }
    }

    public IEnumerator GetAbilityDescription(string url, System.Action<string> onSuccess)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                AbilityDetailData data = JsonUtility.FromJson<AbilityDetailData>(req.downloadHandler.text);
                string desc = "No description.";

                if (data.flavor_text_entries != null)
                {
                    foreach (var entry in data.flavor_text_entries)
                    {
                        if (entry.language.name == "en")
                        {
                            desc = entry.flavor_text.Replace("\n", " ").Replace("\f", " ");
                            break;
                        }
                    }
                }
                onSuccess?.Invoke(desc);
            }
            else
            {
                Debug.LogWarning("Erro ability: " + req.error);
                onSuccess?.Invoke("Could not load description.");
            }
        }
    }
}
