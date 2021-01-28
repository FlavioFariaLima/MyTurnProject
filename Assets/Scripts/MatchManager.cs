using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : MonoBehaviour
{
    [Header("Temporary")]
    [SerializeField] private List<Player> tempPlayers = new List<Player>();
    [SerializeField] private int playersCount; // How many players in this match
    public Player playerTurn;

    [Header("Players and Characters")]
    private Dictionary<int, MatchPlayer> matchPlayers = new Dictionary<int, MatchPlayer>(); // Identify the players
    public Dictionary<int, MatchPlayer> MatchPlayers
    {
        get { return matchPlayers; }
    }

    private List<MatchCharacter> inGameCharacters; // All Characters in the game, players and IA
    private List<int> iniciativeResults;

    [Header("Turn Information")]
    [SerializeField] private int turnDuration = 240; // In Seconds
    [SerializeField] private int waitUntilStartTurn = 2; // In Seconds
    private float globalTime;
    private float turnTime;
    private int turnOwnerId;
    private int turnsCount;
    public int CurrentTurn
    {
        get { return turnsCount + 1; }
    }
    private MatchCharacter characterRound;

    private bool matchEnd = false;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set Players For The Match!
        playersCount = tempPlayers.Count;

        FindMatchPlayers();

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
            foreach (MatchPlayer p in matchPlayers.Values)
            {
                foreach (CharacterSheet c in p.characters)
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
        inGameCharacters[turnOwnerId].character.controller.ResetAllForNewTurn();
        Global.UI.SetupActionsOwner(inGameCharacters[turnOwnerId].character.controller);

        if (inGameCharacters[turnOwnerId].character.controller.IsAi() && inGameCharacters[turnOwnerId].character.controller.GetPlayerAI() == null)
        {
            StartCoroutine(inGameCharacters[turnOwnerId].character.controller.StartAI());
        }

        if (!inGameCharacters[turnOwnerId].character.controller.IsAi())
        {
            inGameCharacters[turnOwnerId].character.controller.SetSelectState(true);
        }
        else
        {
            inGameCharacters[turnOwnerId].character.controller.GetPlayerAI().CanEndTurn(true);
            inGameCharacters[turnOwnerId].character.controller.SetSelectState(true);
        }

        playerTurn = matchPlayers[inGameCharacters[turnOwnerId].character.GetPlayerId()].player;

        UpdateIconStatus(inGameCharacters[turnOwnerId].character, true);
        //MoveCameraToCharacter(turnOwnerId);
    }

    /// <summary>
    /// Lets set the players and the characters in the match
    /// </summary>
    async Task<Dictionary<int, MatchPlayer>> FindMatchPlayers()
    {
        //Debug.Log("Step 1 - Setting Players..."); 

        Task findAllPlayers = Task.Run(() =>
        {
            matchPlayers = new Dictionary<int, MatchPlayer>();

            foreach (Player p in tempPlayers)
            {                
                // Add Players to the Match
                matchPlayers.Add(p.GetId(), new MatchPlayer(p, p.PlayerCharacters));
            }

        });
        findAllPlayers.Wait();

        //Debug.Log("Step 2 - Setting Characters...");
        // Find Players Characters
        Task findPlayersCharacters = Task.Run(() =>
        {
            //Debug.Log("Number of Players: " + matchPlayers.Count);
            inGameCharacters = new List<MatchCharacter>();

            foreach (MatchPlayer mp in matchPlayers.Values)
            {
                //Debug.Log(mp.player.GetName + ", has #" + mp.characters.Count);


                foreach (CharacterSheet c in mp.characters)
                {
                    MatchCharacter newChar = new MatchCharacter(mp, c, c.GetId(), 0);

                    // Define if character is AI
                    if (newChar.player.player.GetName == "DM")
                    {
                        newChar.character.controller.CharacterIsAI(true);
                    }
                    else
                    {
                        newChar.character.controller.CharacterIsAI(false);
                    }

                    inGameCharacters.Add(newChar);
                }
            }

            //Debug.Log($"Char Count = {inGameCharacters.Count}");
        });

        findPlayersCharacters.Wait();
        //Debug.Log("Done!");

        //Debug.Log("Setting First Round...");
        // Setup First Round
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
        turnOwnerId = 0;
        inGameCharacters = MatchEvents.RollIniciatives(inGameCharacters);

        for (int position = 0; position < inGameCharacters.Count; position++)
        {
            GameObject charIcon = Instantiate(Global.UI.CharacterIconsPrefab, Global.UI.MatchCharacterIcons.transform);

            int p = position;

            charIcon.GetComponent<Button>().onClick.AddListener(delegate { MoveCameraToCharacter(p); });

            charIcon.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inGameCharacters[position].character.GetName();
            charIcon.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = inGameCharacters[position].iniciative.ToString();
            charIcon.transform.GetChild(2).GetComponent<Slider>().maxValue = inGameCharacters[position].character.GetHealth();
            charIcon.transform.GetChild(2).GetComponent<Slider>().value = inGameCharacters[position].character.GetHealth();

            inGameCharacters[position].character.CharIcon = charIcon;
        }

        characterRound = inGameCharacters[turnOwnerId]; // Set the first player to play

        // Show Character Information
        Global.UI.ShowCharacterInfo(characterRound.character);

        //Debug.Log("Starting Match...");
        // Starting Match
        StartCoroutine(TurnStart());
    }

    public void UpdateIconHealthBar(CharacterSheet character)
    {
        character.CharIcon.transform.GetChild(2).GetComponent<Slider>().value = character.GetCurrrentHelth();
    }

    public void UpdateIconStatus(CharacterSheet character, bool active)
    {
        if (active)
        {
            character.CharIcon.transform.GetComponent<Image>().color = new Color(0.5f, 0.8f, 0.5f, 1);
        }
        else
        {
            character.CharIcon.transform.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }

    public void MoveCameraToCharacter(int index)
    {
        Vector3 p = inGameCharacters[index].character.controller.characterPosition;
        StartCoroutine(Global.UI.mainCamera.GoToCharacter(p));

        // Show Character Information
        Global.UI.ShowCharacterInfo(inGameCharacters[index].character);

        Debug.Log($"Go to Position: {p}");
    }

    IEnumerator TurnStart()
    {
        SetupCharacterController();

        // Change UI Indicator
        Global.UI.TurnPanel.transform.Find("PlayerTurn").GetComponent<TextMeshProUGUI>().text = $"Starting Round\n{characterRound.character.GetName()}";      

        float totalTime = waitUntilStartTurn;

        while (totalTime > 3)
        {
            //countdownImage.fillAmount = totalTime / duration;
            totalTime -= Time.deltaTime;
            var integer = (int)totalTime; /* choose how to quantize this */

            Global.UI.TurnPanel.transform.Find("Timer").GetComponent<TextMeshProUGUI>().text = integer.ToString();
            Global.UI.TurnPanel.transform.Find("Bkg").GetComponent<Image>().color = new Color(1, 0, 0, 0.3f);

            yield return null;
        }

        while (totalTime >= 0 && totalTime <= 3)
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
            Global.UI.TurnPanel.transform.Find("Bkg").GetComponent<Image>().color = new Color(0, 0, 0, 0.3f);

            yield return StartCoroutine(TurnRolling());
        }
    }

    IEnumerator TurnRolling()
    {
        Global.UI.TurnPanel.transform.Find("PlayerTurn").GetComponent<TextMeshProUGUI>().text = $"Playing Round {turnsCount + 1} \n {characterRound.character.GetName()} ";
        characterRound.character.isMyTurn = true;

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
        if (turnOwnerId >= inGameCharacters.Count - 1)
            turnOwnerId = 0;
        else
            turnOwnerId++;

        if (!inGameCharacters[turnOwnerId].character.IsAlive())
        {
            TurnEnd();
            return;
        }

        UpdateIconStatus(characterRound.character, false);
        characterRound.character.isMyTurn = false; // Its not this character turn anymore
        characterRound = inGameCharacters[turnOwnerId]; // Change to next character

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

public class MatchPlayer
{
    public int id;
    public Player player;
    public List<CharacterSheet> characters;

    public MatchPlayer (Player player, List<CharacterSheet> playerChars)
    {
        this.id = player.GetId();
        this.player = player;
        this.characters = playerChars;
    }
}

public class MatchCharacter
{
    public MatchPlayer player;
    public CharacterSheet character;
    public int id;
    public int iniciative;

    public MatchCharacter(MatchPlayer p, CharacterSheet c, int charId, int ini)
    {
        this.player = p;
        this.character = c;
        this.id = charId;
        this.iniciative = ini;
    }

}


