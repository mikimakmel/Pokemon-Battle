using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newMove", menuName = "Move")]
public class SC_PokemonMove : ScriptableObject
{
    public int ID;
    public new string name;
    public int maxPP;
    public int currPP;
    public int power;
    public int accuracy;
    public GlobalEnums.PokemonType type;
    public AudioSource moveSound;
}
