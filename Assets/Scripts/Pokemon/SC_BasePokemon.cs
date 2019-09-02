using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "newPokemon", menuName = "Pokemon")]
public class SC_BasePokemon : ScriptableObject
{
    public new string name;
    public Sprite frontImage;
    public Sprite backImage;
    public GlobalEnums.PokemonType type;
    public GlobalEnums.Rarity rarity;
    public Stats HpStats;
    public int attack;
    public int defense;
    public int speed;
    public int level;
    public bool canEvolve;
    public int levelToEvolve;
    public SC_BasePokemon evolveTo;
    public List<SC_PokemonMove> moves;
    public SC_HealthBar healthBar;
}

[System.Serializable]
public class Stats
{
    public float min;
    public float curr;
    public float max;
}
