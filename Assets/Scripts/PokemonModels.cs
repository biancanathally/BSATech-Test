using System;
using UnityEngine;

[Serializable]
public class PokemonData
{
    public string name;
    public Sprites sprites;
    public Stat[] stats;
    public MoveWrapper[] moves;
    public Texture2D frontTexture;
    public Texture2D backTexture;
    public int savedLevel = 0; 
    public int savedMaxHp = 0;
    public int savedCurrentHp = 0;
    public bool isMale = true;
    public bool isInitialized = false;
}

[Serializable]
public class Sprites
{
    public string front_default;
    public string back_default;
}

[Serializable]
public class Stat
{
    public int base_stat;
    public StatInfo stat;
}

[Serializable]
public class StatInfo { public string name; }

[Serializable]
public class MoveWrapper { public MoveInfo move; }

[Serializable]
public class MoveInfo { public string name; public string url; }

[Serializable]
public class MoveDetailsData
{
    public int pp;
    public MoveType type;
}

[Serializable]
public class MoveType {
    public string name;
}
