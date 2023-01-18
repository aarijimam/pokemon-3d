using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy,PartyScreen,BattleOver, AboutToUse}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] GameObject pokeballSprite;
    [SerializeField] private TextMeshProUGUI pokemonText;
    private int pokemonsCaught;

    private void Start()
    {
        pokemonsCaught = 0;
    }

    private void Update()
    {
        if (pokemonsCaught >= 10)
        {
            SceneManager.LoadScene("EndGame");
        }
    }

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    int currentMember;

    PokemonParty playerParty;
    Pokemon wildPokemon;
    private IEnumerator coroutineA;
    public void StartBattle(PokemonParty playerParty,Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        enemyUnit.Setup(wildPokemon);
        partyScreen.Init();
        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        //pokeballSprite.transform.position
        coroutineA = (dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared."));
        yield return coroutineA;
        ActionSelection();
    }


    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action.");
        dialogBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }
    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);

    }

    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;
        var move = playerUnit.Pokemon.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);


        if (state == BattleState.PerformMove)
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;
        var move = enemyUnit.Pokemon.GetRandomMove();
        yield return RunMove(enemyUnit,playerUnit,move);
        if (state == BattleState.PerformMove)
        {
            ActionSelection();
        }
    }

    IEnumerator RunMove(BattleUnit sourceUnit,BattleUnit targetUnit,Move move)
    {
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}.");
        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        targetUnit.PlayHitAnimation();
        var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
        yield return targetUnit.Hud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);
        if (damageDetails.Fainted)
        {
            targetUnit.PlayFaintAnimation();
            yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} Fainted.");
            yield return new WaitForSeconds(2f);
            CheckForBattleOver(targetUnit);

        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                OpenPartyScreen();

            }
            else
                BattleOver(false);
        }
        else BattleOver(true);
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        OnBattleOver(won);
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {

        if (damageDetails.Critical > 1f) { 
            yield return dialogBox.TypeDialog("A critical hit.");
        }

        if (damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogBox.TypeDialog("It's super effective.");

        }

        else if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogBox.TypeDialog("It's not very effective.");

        }
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }

        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
        else if (state == BattleState.AboutToUse)
        {
            StartCoroutine(ThrowPokemon());
            pokemonText.SetText("Pokemons caught: " + ++pokemonsCaught);
        }

        if (Input.GetKeyDown(KeyCode.T)){
            StartCoroutine(ThrowPokemon());
            pokemonText.SetText("Pokemons caught: " + ++pokemonsCaught);
        }
    }

    void HandleActionSelection()

    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentAction++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentAction--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
                currentAction+=2;
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
            {
                currentAction-=2;
            }
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelector(currentAction);

        if (Input.GetKey(KeyCode.Z))
        {
            StopCoroutine(coroutineA);
            dialogBox.SetDialog("Choose an Action...");
            if (currentAction == 0)
            {
                //Fight is Selected
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                StartCoroutine(ThrowPokemon());
                pokemonText.SetText("Pokemons caught: " + ++pokemonsCaught);
                //Bag is Selected
                
            }
            else if (currentAction == 2)
            {
                //Pokemon is Selected
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                StartCoroutine(dialogBox.TypeDialog("You got away safely!"));
                OnBattleOver(true);
                //Run is Selected
            }

        }
        
        }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentMove;
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentMove;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMove += 2;
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMove -= 2;
        }
        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count -1 );
        dialogBox.UpdateMoveSelector(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StopCoroutine(coroutineA);
            StartCoroutine(PlayerMove());
        }
        if (Input.GetKey(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentMember;
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentMember;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMember += 2;
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMember-= 2;
        }
        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("You can't send out a fainted Pokemon!");
                return;
            }
            else if (selectedMember == playerUnit.Pokemon) { 
                partyScreen.SetMessageText("You can't send out a fainted Pokemon!");
                return;
            }
            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        else if (Input.GetKey(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.Pokemon.HP > 0) { 
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}!");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }
        playerUnit.Setup(newPokemon);


        dialogBox.SetMoveNames(newPokemon.Moves);

        yield return (dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!"));

        StartCoroutine(EnemyMove());
    }

    IEnumerator ThrowPokemon()
    {
        state=BattleState.Busy;

        yield return dialogBox.TypeDialog($"You used a Pokeball!");

        var pokeballObject = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var pokeball = pokeballObject.GetComponent<SpriteRenderer>();

        //Animations
        yield return pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 3), 2f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y -1.5f,0.5f).WaitForCompletion();

        for(int i = 0; i < 3; i++)
        {
           yield return new WaitForSeconds(0.5f);
           yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} was caught.");

        playerParty.AddPokemon(enemyUnit.Pokemon);
        //pokemonsCaught++;
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} has been added to your Party.");
        OnBattleOver(true);
        Destroy(pokeballObject);

    }
}
