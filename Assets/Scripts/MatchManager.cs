using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MatchManager : MonoBehaviour
{
    [Header("Temporary")]
    [SerializeField] private List<Player> tempPlayers = new List<Player>();
    [SerializeField] private int playersCount; // How many players in this match
    public Player playerTurn;

    [Header("Players and Characters")]
    private Dictionary<int, Player> matchPlayers = new Dictionary<int, Player>(); // Identify the players
    public Dictionary<int, Player> MatchPlayers
    {
        get { return matchPlayers; }
    }

    [SerializeField] private List<CharacterSheet> inGameCharacters; // All Characters in the game, players and IA
    private List<int> iniciativeResults;

    [Header("Turn Information")]
    [SerializeField] private int turnDuration = 100; // In Seconds
    [SerializeField] private int waitUntilStartTurn = 3; // In Seconds
    private float globalTime;
    private float turnTime;
    private int turnOwnerIndex;

    public int GetTurnOwnerIndex()
    {
        return turnOwnerIndex;
    }

    public int TurnOwnerId
    {
        get { return turnOwnerIndex;}
    }
    private int turnsCount;
    public int CurrentTurn
    {
        get { return turnsCount + 1; }
    }
    private CharacterSheet characterRound;

    private bool matchEnd = false;

    [Header("Match Control")]
    [SerializeField] private Player mainPlayer;

    // Get/Set Variables
    public List<CharacterSheet> InGameCharacters()
    {
        return inGameCharacters;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set Players For The Match!
        playersCount = tempPlayers.Count;

        Task startMatch = FindMatchPlayers();
        startMatch.Wait();

        SetPlayerCharactersShotCut(mainPlayer);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Task CheckForMatchEnd()
    {
        bool[] hasCharacters = new bool[playersCount];
        int pCount = 0;
        int aliveCount = 0;

        if (!matchEnd)
        {
            foreach (Player p in matchPlayers.Values)
            {
                foreach (CharacterSheet c in p.PlayerCharacters)
                {
                    if (c.IsAlive())
                        hasCharacters[pCount] = true;
                }

                pCount++;
            }
        }

        foreach (bool b in hasCharacters)
        {
            if (b == true)
                aliveCount++;
        }

        if (aliveCount == 0)
        {
            EndMatch();
            Debug.Log($"Match Drawn.");
        }
        else if (aliveCount == 1)
        {
            EndMatch();
            Debug.Log($"Someone Win the Match");
        }

        return null;
    }

    private void SetupCharacterController()
    {
        inGameCharacters[turnOwnerIndex].controller.ResetActions();
        Global.UI.DisablActionsBtns();
        if (inGameCharacters[turnOwnerIndex].controller.IsAi() && inGameCharacters[turnOwnerIndex].controller.GetPlayerAI() == null)
        {
            Global.UI.pathLine.gameObject.SetActive(false);
            StartCoroutine(inGameCharacters[turnOwnerIndex].controller.StartAI());
        }

        if (inGameCharacters[turnOwnerIndex].controller.IsAi())
        {
            inGameCharacters[turnOwnerIndex].controller.GetPlayerAI().CanEndTurn(true);
        }

        inGameCharacters[turnOwnerIndex].controller.SelectedForAct(true);

        if (inGameCharacters[turnOwnerIndex].GetPlayerId() == mainPlayer.GetId())
        {
            Global.UI.SetupActionsOwner(inGameCharacters[turnOwnerIndex].controller);
        }

        playerTurn = matchPlayers[inGameCharacters[turnOwnerIndex].GetPlayerId()];
        Global.UI.SetupActionsOwner(inGameCharacters[turnOwnerIndex].controller);

        Global.UI.characterSelectedToUI = inGameCharacters[turnOwnerIndex];
        Global.UI.SelectCharacterForUI(inGameCharacters[turnOwnerIndex].GetId());

        UpdateIconStatus(inGameCharacters[turnOwnerIndex], true);
    }

    /// <summary>
    /// Lets set the players and the characters in the match
    /// </summary>
    async Task<Dictionary<int, Player>> FindMatchPlayers()
    {
        //Debug.Log("Step 1 - Setting Players..."); 

        Task findAllPlayers = Task.Run(() =>
        {
            matchPlayers = new Dictionary<int, Player>();
            inGameCharacters = new List<CharacterSheet>();

            for ( int p = 0; p < tempPlayers.Count; p++)
            {
                // Add Players to the Match
                matchPlayers.Add(tempPlayers[p].GetId(), tempPlayers[p]);

                foreach(CharacterSheet c in tempPlayers[p].PlayerCharacters)
                {
                    inGameCharacters.Add(c);
                }
            }
        });
        findAllPlayers.Wait();
        //Debug.Log("Done!");

        // Setup First Round
        //Debug.Log("Setting First Round...");
        SettingFirstRound();

        return matchPlayers;
    }

    /// <summary>
    /// Control each player turn
    /// </summary>
    /// 
    private void SettingFirstRound()
    {        
        turnsCount = 0;
        turnOwnerIndex = 0;
        SetupCharactersOrderAndIcons();
        characterRound = inGameCharacters[turnOwnerIndex]; // Set the first player to play

        // Show Character Information
        Global.UI.UpdateCharacterInfoPanel(characterRound);

        //Debug.Log("Starting Match...");
        // Starting Match
        StartCoroutine(TurnStart());
    }

    public void SetupCharactersOrderAndIcons()
    {
        inGameCharacters = MatchEvents.RollIniciatives(inGameCharacters);

        for (int position = 0; position < inGameCharacters.Count; position++)
        {
            GameObject charIcon = Instantiate(Global.UI.CharacterIconsPrefab, Global.UI.CharacterSheetIcons.transform);
            charIcon.name = inGameCharacters[position].GetId().ToString();

            int p = position;

            charIcon.GetComponent<Button>().onClick.AddListener(delegate { 
                CharacterIconClick(p); 
            });

            charIcon.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = inGameCharacters[position].GetName();
            charIcon.transform.Find("Portrait").GetComponent<Image>().sprite = inGameCharacters[position].Portrait();
            charIcon.transform.Find("Health").GetComponent<TextMeshProUGUI>().text = inGameCharacters[position].GetCurrrentHelth().ToString();
            charIcon.transform.Find("HealthBar").GetComponent<Slider>().maxValue = inGameCharacters[position].GetHealth();
            charIcon.transform.Find("HealthBar").GetComponent<Slider>().value = inGameCharacters[position].GetCurrrentHelth();
            charIcon.transform.Find("Iniciative").GetComponent<TextMeshProUGUI>().text = inGameCharacters[position].MatchIniciative.ToString();
            charIcon.transform.Find("AC").GetComponent<TextMeshProUGUI>().text = inGameCharacters[position].GetArmour().ToString();

            inGameCharacters[position].CharIcon = charIcon;
        }
    }

    public void UpdateCharactersIcons()
    {
        for (int i = 0; i < Global.UI.CharacterSheetIcons.transform.childCount; i++)
        {
            Global.UI.CharacterSheetIcons.transform.GetChild(i).Find("Name").GetComponent<TextMeshProUGUI>().text = inGameCharacters[i].GetName();
            Global.UI.CharacterSheetIcons.transform.GetChild(i).Find("Portrait").GetComponent<Image>().sprite = inGameCharacters[i].Portrait();
            Global.UI.CharacterSheetIcons.transform.GetChild(i).Find("Health").GetComponent<TextMeshProUGUI>().text = inGameCharacters[i].GetCurrrentHelth().ToString();
            Global.UI.CharacterSheetIcons.transform.GetChild(i).Find("HealthBar").GetComponent<Slider>().maxValue = inGameCharacters[i].GetHealth();
            Global.UI.CharacterSheetIcons.transform.GetChild(i).Find("HealthBar").GetComponent<Slider>().value = inGameCharacters[i].GetCurrrentHelth();
            Global.UI.CharacterSheetIcons.transform.GetChild(i).Find("Iniciative").GetComponent<TextMeshProUGUI>().text = inGameCharacters[i].MatchIniciative.ToString();
            Global.UI.CharacterSheetIcons.transform.GetChild(i).Find("AC").GetComponent<TextMeshProUGUI>().text = inGameCharacters[i].GetArmour().ToString();
        }
    }

    public void SetPlayerCharactersShotCut(Player player)
    {
        for (int position = 0; position < player.PlayerCharacters.Count; position++)
        {
            CharacterSheet c = player.PlayerCharacters[position];

            GameObject charIcon = Instantiate(c.CharIcon, Global.UI.playerCharactersShotcut.transform);
            charIcon.name = c.GetId().ToString();
            c.shotcutIcon = charIcon.gameObject;
            c.shotcutIcon.GetComponent<Button>().onClick.AddListener(delegate { c.controller.SelectedForUI(true);});
        }
    }

    public void UpdateIconHealthBar(CharacterSheet character)
    {
        character.CharIcon.transform.Find("HealthBar").GetComponent<Slider>().value = character.GetCurrrentHelth();
        character.CharIcon.transform.Find("Health").GetComponent<TextMeshProUGUI>().text = character.GetCurrrentHelth().ToString();

        foreach (Transform child in Global.UI.playerCharactersShotcut.transform)
        {
            if (child.name == character.GetId().ToString())
            {
                child.Find("HealthBar").GetComponent<Slider>().value = character.GetCurrrentHelth();
                child.Find("Health").GetComponent<TextMeshProUGUI>().text = character.GetCurrrentHelth().ToString();
            }
        }
    }

    public void UpdateIconStatus(CharacterSheet character, bool active)
    {
        if (active)
        {
            character.CharIcon.transform.GetComponent<Image>().color = new Color(0.5f, 0.8f, 0.5f, 1);
        }
        else
        {
            character.CharIcon.transform.GetComponent<Image>().color = new Color(0, 0, 0, 1);
        }


        foreach (Transform child in Global.UI.playerCharactersShotcut.transform)
        {
            if (child.name == character.GetId().ToString())
            {
                child.GetComponent<Image>().color = character.CharIcon.transform.GetComponent<Image>().color;
            }
        }
    }

    public void CharacterIconClick(int index)
    {
        Vector3 p = inGameCharacters[index].controller.characterPosition;
        StartCoroutine(Global.UI.mainCamera.GoToCharacter(p));

        // Show Character Information
        Global.UI.UpdateCharacterInfoPanel(inGameCharacters[index]);
        inGameCharacters[index].controller.SelectedForUI(true);

        Debug.Log($"Go to Position: {p}");
    }

    IEnumerator TurnStart()
    {
        SetupCharacterController();
        inGameCharacters[turnOwnerIndex].controller.AllowToMove(true);
        Global.UI.SetCursor(Global.UI.cursorDefault, false);

        // Change UI Indicator
        Global.UI.TurnPanel.transform.Find("TurnCounter").GetComponent<TextMeshProUGUI>().text = $"{turnsCount + 1}";

        float totalTime = waitUntilStartTurn;
        float timeAlertRest = 3;

        while (totalTime > timeAlertRest)
        {
            //countdownImage.fillAmount = totalTime / duration;
            totalTime -= Time.deltaTime;
            var integer = (int)totalTime; /* choose how to quantize this */

            Global.UI.TurnPanel.transform.Find("Timer").GetComponent<TextMeshProUGUI>().text = integer.ToString();

            yield return null;
        }

        while (totalTime >= 0 && totalTime <= timeAlertRest)
        {
            //countdownImage.fillAmount = totalTime / duration;
            totalTime -= Time.deltaTime;
            var integer = (int)totalTime; /* choose how to quantize this */

            Global.UI.TurnPanel.transform.Find("Timer").GetComponent<TextMeshProUGUI>().text = integer.ToString();

            yield return null;

        }

        if (totalTime <= 2)
        {
            Global.UI.TurnPanel.transform.Find("Timer").GetComponent<TextMeshProUGUI>().text = "Go!";

            yield return StartCoroutine(TurnRolling());
        }
    }

    IEnumerator TurnRolling()
    {
        Global.UI.TurnPanel.transform.Find("TurnCounter").GetComponent<TextMeshProUGUI>().text = $"{turnsCount + 1}";
        characterRound.isMyTurn = true;

        float totalTime = turnDuration;
        while (totalTime >= 0)
        {
            //countdownImage.fillAmount = totalTime / duration;
            totalTime -= Time.deltaTime;
            var integer = (int)totalTime; /* choose how to quantize this */

            Global.UI.TurnPanel.transform.Find("Timer").GetComponent<TextMeshProUGUI>().text = integer.ToString();

            if (totalTime <= turnDuration / 3)
                Global.UI.TurnPanel.transform.Find("Timer").GetComponent<TextMeshProUGUI>().color = new Color(1, 0, 0, 1f);
            else
                Global.UI.TurnPanel.transform.Find("Timer").GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 10, 1f);

            yield return null;
        }

        if (totalTime <= 0)
        {
            TurnEnd();
            yield return null;
        }       
    }

    public void PassTurn()
    {
        Global.Commands.playerIsAttacking = false;
        Debug.Log($"Character Pass!");
        TurnEnd();
    }

    public void TurnEnd()
    {
        Debug.Log($"Turn is End!");
        StopAllCoroutines();

        // Change Player
        if (turnOwnerIndex >= inGameCharacters.Count - 1)
            turnOwnerIndex = 0;
        else
            turnOwnerIndex++;

        if (!inGameCharacters[turnOwnerIndex].IsAlive())
        {
            TurnEnd();
            return;
        }

        UpdateIconStatus(characterRound, false);
        characterRound.isMyTurn = false; // Its not this character turn anymore
        characterRound.controller.AllowToMove(false);
        characterRound = inGameCharacters[turnOwnerIndex]; // Change to next character

        // Counting Rounds
        turnsCount++;

        StartCoroutine(TurnStart());
    }

    public void EndMatch()
    {
        Global.PauseGame();
        Global.UI.EndMatch.SetActive(true);
    }
}
