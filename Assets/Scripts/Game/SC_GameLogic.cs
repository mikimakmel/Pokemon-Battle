using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_GameLogic : MonoBehaviour
{
    public List<SC_BasePokemon> allPokemons = new List<SC_BasePokemon>();
    public List<SC_PokemonMove> allMoves = new List<SC_PokemonMove>();
    private Dictionary<string, GameObject> rectangles;
    private Dictionary<string, GameObject> movesSounds;
    public SC_BattleManager SC_BattleManager;
    public SC_MenuLogic SC_MenuLogic;
    public GameObject menuCamera;
    public GameObject battleCamera;
    public AudioSource battleMusic;

    private void Awake()
    {
        battleMusic = battleMusic.GetComponent<AudioSource>();
        InitRectangles();
        InitMovesSounds();
        battleCamera.SetActive(false);
        menuCamera.SetActive(true);
    }

    private void InitRectangles()
    {
        rectangles = new Dictionary<string, GameObject>();
        GameObject[] _objects = GameObject.FindGameObjectsWithTag("Rectangles");
        foreach (GameObject obj in _objects)
        {
            if (obj != null)
            {
                rectangles.Add(obj.name, obj);
                obj.SetActive(false);
            }
        }
    }

    private void InitMovesSounds()
    {
        movesSounds = new Dictionary<string, GameObject>();
        GameObject[] _objects = GameObject.FindGameObjectsWithTag("MovesSounds");
        foreach (GameObject obj in _objects)
        {
            if (obj != null)
                movesSounds.Add(obj.name, obj);
        }

        foreach (SC_PokemonMove move in allMoves)
        {
            if (movesSounds.ContainsKey(move.name))
                move.moveSound = movesSounds[move.name].GetComponent<AudioSource>();
        }
    }

    public void EnterBattle(bool _isMultiplayer = false)
    {
        battleMusic.Play();
        StartCoroutine(BattleTransition(0.1f));
        if (_isMultiplayer == false)
            SC_BattleManager.initBattle();
    }

    private IEnumerator BattleTransition(float delay)
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 1; i <= rectangles.Count; i++)
        {
            yield return new WaitForSeconds(delay);
            if (rectangles.ContainsKey("Rec_" + i))
                rectangles["Rec_" + i].SetActive(true);
            yield return new WaitForSeconds(delay);
        }
        yield return new WaitForSeconds(1f);

        menuCamera.SetActive(false);
        battleCamera.SetActive(true);

        SC_BattleManager.playerAnimator.SetTrigger("PlayerEnters");
        SC_BattleManager.foeAnimator.SetTrigger("FoeEnters");

        for (int i = 1; i <= rectangles.Count; i++)
        {
            if (rectangles.ContainsKey("Rec_" + i))
                rectangles["Rec_" + i].SetActive(false);
        }
    }

    public SC_BasePokemon getRandomPokemonFromList(List<SC_BasePokemon> _pokemonsList)
    {
        int _randomIndex = Random.Range(0, _pokemonsList.Count);
        SC_BasePokemon _randomPokemon = Instantiate(_pokemonsList[_randomIndex]);
        return _randomPokemon;
    }

    public SC_BasePokemon getPokemonByID(int _pokemonID)
    {
        foreach (SC_BasePokemon _pokemon in allPokemons)
        {
            if (_pokemon.ID == _pokemonID)
                return Instantiate(_pokemon);
        }

        return null;
    }

    public SC_PokemonMove getMoveByID(int _moveID)
    {
        foreach (SC_PokemonMove _move in allMoves)
        {
            if (_move.ID == _moveID)
                return Instantiate(_move);
        }

        return null;
    }
}
