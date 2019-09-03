using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEnums : MonoBehaviour
{
    public enum PokemonType
    {
        NORMAL,
        FIRE,
        WATER,
        ELECTRIC,
        GRASS,
        ICE,
        FIGHTING,
        POISION,
        GROUND,
        FLYING,
        PSYCHIC,
        BUG,
        ROCK,
        GHOST,
        DRAGON,
        DARK,
        STEEL,
        FAIRY
    }

    public enum Rarity
    {
        VeryCommon,
        Common,
        SemiRare,
        Rare,
        VeryRare
    }

    public enum BattleMenus
    {
        Selection,
        Moves,
        Message
    }

    public enum BattleStates
    {
        Start,
        Battling,
        GameOver
    }

    public enum Turns
    {
        PlayersTurn,
        FoesTurn,
    }

    public enum MessageBoxState
    {
        EnterBattle,
        Selection,
        Attack,
        GameOver
    }
}
