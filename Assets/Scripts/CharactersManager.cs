using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class CharactersManager : MonoBehaviour
{
    // General Privates Vars.
    private List<CustomCharacterController> players = new List<CustomCharacterController>();

    List<Vector3> initPos = new List<Vector3>() { 
        new Vector3(-5, 0, 4), 
        new Vector3(5, 0, 4), 
        new Vector3(-5, 0, -4), 
        new Vector3(5, 0, -4)
    };

    List<Vector3> initRot = new List<Vector3>() { 
        new Vector3(0, 90, 0), 
        new Vector3(0, -90, 0), 
        new Vector3(0, 90, 0), 
        new Vector3(0, -90, 0)
    };

    [Header("Player Vars")]
    [SerializeField] private GameObject[] playerPrefabs;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject playerCamera;
    private Character charSelected = new Character();
    public enum Character { Cat, Bear, Bunny }

    [Header("Agents Vars")]
    [SerializeField] private GameObject agentCamera;
    [SerializeField] private GamertagsList gamertagsList;
    private List<string> availableGamertags = new List<string>();
    [SerializeField] private List<GameObject> agentsPrefabs;
    [SerializeField] private List<PlayerData> agentsData;
    
    [Header("General Vars")]
    [SerializeField] private GameObject prefabCrown;
    private Transform tCrown;

    private void Awake()
    {
        availableGamertags = PlayerPrefsX.GetStringArray("AvailableGamertags").ToList();
        if (!PlayerPrefs.HasKey("AvailableGamertags") || availableGamertags.Count < 3)
        {
            PlayerPrefsX.SetStringArray("AvailableGamertags", gamertagsList.gamertags.ToArray());
            availableGamertags = PlayerPrefsX.GetStringArray("AvailableGamertags").ToList();
        }

        DOTween.SetTweensCapacity(1250, 780);

        SpawnPlayer();
        SpawnAgents();
        tCrown = Instantiate(prefabCrown, new Vector3(0, 50, 0), Quaternion.identity).transform;
    }

    private void SpawnPlayer()
    {
        if(PlayerPrefs.HasKey("Character"))
            charSelected = (Character)System.Enum.Parse(typeof(Character), PlayerPrefs.GetString("Character"));
        GameObject player = Instantiate(playerPrefabs[(int)charSelected], initPos[0], Quaternion.Euler(initRot[0]));
        var xCharController = player.GetComponent<CustomCharacterController>();
        players.Add(xCharController);

        // Set Gamertag;
        if(PlayerPrefs.HasKey("PlayerName"))
            playerData.RuntimeGamertag = PlayerPrefs.GetString("PlayerName");

        // Set Data
        xCharController.Data = playerData;

        // Set Camera
        xCharController.myCamera = Instantiate(playerCamera, initPos[0], Quaternion.Euler(initRot[0])).transform;
        initPos.RemoveAt(0); initRot.RemoveAt(0);
    }

    private void SpawnAgents()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject agent = Instantiate(agentsPrefabs[i], initPos[i], Quaternion.Euler(initRot[i]));
            var xAgentController = agent.GetComponent<CustomCharacterController>();
            players.Add(xAgentController);

            // Set Gamertag;
            int nGamertag = UnityEngine.Random.Range(0, availableGamertags.Count);
            agentsData[i].RuntimeGamertag = availableGamertags[nGamertag];
            availableGamertags.RemoveAt(nGamertag);
            PlayerPrefsX.SetStringArray("AvailableGamertags", availableGamertags.ToArray());

            // Se Data
            xAgentController.Data = agentsData[i];

            // Set Camera
            xAgentController.myCamera = Instantiate(agentCamera, initPos[i], Quaternion.Euler(initRot[i])).transform;
        }
    }

    private void LateUpdate() {
        SetCrown();
    }

    private void SetCrown()
    {
        foreach (var playerData in players.OrderByDescending(x => x.Score))
        {
            if(playerData.myHead.childCount.Equals(0))
            {
                tCrown.SetParent(playerData.myHead);
                tCrown.localPosition = new Vector3(0, .1f, 0);
                tCrown.localEulerAngles = new Vector3(0, 0, 0);
                tCrown.localScale = new Vector3(0, 0, 0);

                tCrown.DOLocalMoveY(.3f, .5f).SetEase(Ease.OutBack);
                tCrown.DOLocalRotate(new Vector3(0, 180, 0), 1f).SetEase(Ease.OutBack);
                tCrown.DOScale(new Vector3(.25f, .25f, .25f), .5f).SetEase(Ease.OutBack);
            }

            return;
        }
    }
}   