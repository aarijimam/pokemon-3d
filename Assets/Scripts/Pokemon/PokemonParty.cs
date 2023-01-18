using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;    

    public List<Pokemon> Pokemons
    {
        get { return pokemons; }
    }


    private void Awake()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }

    public Pokemon GetHealthyPokemon()
    {
        return pokemons.Where(x => x.HP > 0).FirstOrDefault();
    }

    public void AddPokemon(Pokemon newpokemon)
    {
        if(pokemons.Count < 6)
        {
            pokemons.Add(newpokemon);
        }else
        {
            //todo pc
        }
    }
}
