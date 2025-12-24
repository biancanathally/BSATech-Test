using System.Collections.Generic;

public static class GameSession // Antigo GlobalGameState
{
    // Lista do time do jogador (Sua "Party")
    public static List<PokemonData> PlayerParty = new List<PokemonData>();

    // Inimigo atual (para não perder a referência ao mudar de cena)
    public static PokemonData CurrentEnemy;

    // Estado do jogo
    public static bool IsBattleInProgress = false; 
}
