using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    // Menu Manager
    private MainMenu MenuManager;

    [Header("Player && Characters Settings")]
    [SerializeField] public List<Player> players;
    [SerializeField] public CharactersBag charactersBag;
    [SerializeField] public int maxCharacters;
    [HideInInspector] public Player mainPlayer;

    [Header("Buttons & Panels")]
    [SerializeField] public GameObject startBtn;

    [Header("Scenes")]
    [SerializeField] private string newMatchScene;

    [Header("Misc")]
    [SerializeField] private Slider loaderBar;

    // Start is called before the first frame update
    private void Awake()
    {
        // Set Manager
        MenuManager = GetComponent<MainMenu>();

        // Hide Stuff
        loaderBar.gameObject.SetActive(false);

        foreach (Player p in players)
        {
            if (p.IsMainPlayer())
                mainPlayer = p;
        }
    }

    public void CreateNewCharacter()
    {
        MenuManager.createCharacter.CreateNewCharacter(mainPlayer);
    }

    public void LoadMatchScene()
    {
        StartCoroutine(LoadSceneAsync(newMatchScene));
    }

    IEnumerator LoadSceneAsync(string levelName)
    {
        loaderBar.gameObject.SetActive(true);
        AsyncOperation op = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);

        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            loaderBar.value = progress;
            loaderBar.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $" {(int)(progress * 100f)}%";

            yield return null;
        }
    }
}
