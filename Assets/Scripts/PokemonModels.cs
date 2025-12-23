using System;

[Serializable]
public class PokemonData
{
    public string name;
    public Sprites sprites;
    public Stat[] stats;
    public MoveWrapper[] moves;
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
public class MoveType { public string name; }
