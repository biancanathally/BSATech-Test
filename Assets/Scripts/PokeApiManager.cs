using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PokeApiManager : MonoBehaviour
{
    // Variável privada para guardar a instância
    private static PokeApiManager _instance;

    // Propriedade Pública INTELIGENTE
    public static PokeApiManager Instance
    {
        get
        {
            // Se a instância ainda não existe...
            if (_instance == null)
            {
                // 1. Tenta achar na cena (caso você tenha colocado e esquecido)
                _instance = FindFirstObjectByType<PokeApiManager>();

                // 2. Se realmente não existir, cria via código
                if (_instance == null)
                {
                    GameObject obj = new GameObject("PokeApiService_Auto"); // Cria objeto vazio
                    _instance = obj.AddComponent<PokeApiManager>(); // Adiciona este script nele
                }
            }
            return _instance;
        }
    }

    // --- O RESTO DO CÓDIGO CONTINUA IGUAL ---

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
}