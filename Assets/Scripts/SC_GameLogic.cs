using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SC_GameLogic : MonoBehaviour
{
    private Screens currentScreen;
    public List<SC_BasePokemon> allPokemons = new List<SC_BasePokemon>();
    public List<SC_PokemonMove> allMoves = new List<SC_PokemonMove>();
    public GameObject menuCamera;
    public GameObject battleCamera;
    public SC_BattleManager SC_BattleManager;
    public AudioSource battleMusic;
    private Button Button_play;
    private Dictionary<string, GameObject> rectangles;

    private void Awake()
    {
        battleMusic = battleMusic.GetComponent<AudioSource>();
        Button_play = GameObject.Find("Button_play").GetComponent<Button>();
        InitRectangles();
        battleCamera.SetActive(false);
        menuCamera.SetActive(true);
    }

    private void InitRectangles()
    {
        rectangles = new Dictionary<string, GameObject>();
        GameObject[] _objects = GameObject.FindGameObjectsWithTag("Rectangles");
        foreach (GameObject obj in _objects)
        {
            rectangles.Add(obj.name, obj);
            obj.SetActive(false);
        }
    }

    public void EnterBattle()
    {
        Button_play.interactable = false;
        battleMusic.Play();
        SC_BattleManager.initBattle();
        StartCoroutine(BattleTransition(0.1f));
    }

    private IEnumerator BattleTransition(float delay)
    {
        yield return new WaitForSeconds(1f);
        for (int i = 1; i <= rectangles.Count; i++)
        {
            yield return new WaitForSeconds(delay);
            if (rectangles.ContainsKey("Rec_" + i))
                rectangles["Rec_" + i].SetActive(true);
            yield return new WaitForSeconds(delay);
        }
        menuCamera.SetActive(false);
        battleCamera.SetActive(true);
        for (int i = 1; i <= rectangles.Count; i++)
        {
            if (rectangles.ContainsKey("Rec_" + i))
                rectangles["Rec_" + i].SetActive(false);
        }
    }

    public List<SC_BasePokemon> GetPokemonByRarity(GlobalEnums.Rarity rarity)
    {
        List<SC_BasePokemon> pokemonsList = new List<SC_BasePokemon>();

        foreach(SC_BasePokemon pokemon in allPokemons)
        {
            if (pokemon.rarity == rarity)
                pokemonsList.Add(pokemon);
        }

        return pokemonsList;
    }

    public SC_BasePokemon getRandomPokemonFromList(List<SC_BasePokemon> pokemonsList)
    {
        int randomIndex = Random.Range(0, pokemonsList.Count);
        SC_BasePokemon randomPokemon = Instantiate(pokemonsList[randomIndex]);
        //allPokemons.Remove(pokemonsList[randomIndex]);
        return randomPokemon;
    }
}
