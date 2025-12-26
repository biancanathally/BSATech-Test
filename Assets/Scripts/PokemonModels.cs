using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class PokemonData
{
    public int id;
    public string name;
    public Sprites sprites;
    public Stat[] stats;
    public MoveWrapper[] moves;

    public TypeWrapper[] types;
    public AbilityWrapper[] abilities;

    public Texture2D frontTexture;
    public Texture2D backTexture;

    public int savedLevel = 0;
    public int savedMaxHp = 0;
    public int savedCurrentHp = 0;
    public bool isMale = true;
    public bool isInitialized = false;

    public string flavorText;
    public string natureName;
    public string locationMet;
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
public class MoveType
{
    public string name;
}

[Serializable]
public class TypeWrapper
{
    public TypeInfo type;
}

[Serializable]
public class TypeInfo
{
    public string name;
}

[Serializable]
public class AbilityWrapper
{
    public AbilityInfo ability;
    public bool is_hidden;
}

[Serializable]
public class AbilityInfo
{
    public string name;
    public string url;
}

[Serializable]
public class AbilityDetailData
{
    public FlavorTextEntry[] flavor_text_entries;
}

[Serializable]
public class PokemonSpeciesData
{
    public FlavorTextEntry[] flavor_text_entries;
}

[Serializable]
public class FlavorTextEntry
{
    public string flavor_text;
    public LanguageInfo language;
}

[Serializable]
public class NatureData
{
    public string name;
}

[Serializable]
public class LanguageInfo
{
    public string name;
}
