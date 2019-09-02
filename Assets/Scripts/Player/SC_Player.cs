using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Player : MonoBehaviour
{
    public List<SC_BasePokemon> ownedPokemon = new List<SC_BasePokemon>();

    public void AddToOwnedPokemons(SC_BasePokemon pokemon)
    {
        if (pokemon != null)
            ownedPokemon.Add(pokemon);
        else Debug.Log("ERROR: Adding null pokemon");
    }
}

