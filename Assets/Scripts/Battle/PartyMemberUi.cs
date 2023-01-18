using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartyMemberUi : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] HPBar hpBar;

    [SerializeField] Color highlightedColor;

    Pokemon _pokemon;
    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.name;
        levelText.text = "Lvl. " + pokemon.Level;
        hpBar.SetHP(((float)pokemon.HP / pokemon.MaxHP));

    }

    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = highlightedColor;
        else
            nameText.color = Color.black;
    }
}
