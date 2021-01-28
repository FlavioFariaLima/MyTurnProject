using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private string playerName;
    [SerializeField] private int playerId;
    [SerializeField] private List<CharacterSheet> playerCharacters;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Get Player Information
    /// </summary>
    public string GetName
    {
        get { return playerName; }

        set { playerName = value; }
    }

    public int GetId()
    {
        return playerId;
    }

    public List<CharacterSheet> PlayerCharacters
    {
        get { return playerCharacters; }

        set { playerCharacters = value; }
    }
    
}
