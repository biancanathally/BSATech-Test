using System.Collections.Generic;

public static class GameSession
{
    public static List<PokemonData> PlayerParty = new List<PokemonData>();
    public static PokemonData CurrentEnemy;
    public static bool IsBattleInProgress = false;
}
