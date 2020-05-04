using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine;

public class SpawnsManager : MonoBehaviour
{
    List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    Vector3[] initPos = new Vector3[] { 
        new Vector3(-5, 4, 4), 
        new Vector3(5, 4, 4), 
        new Vector3(-5, 4, -4), 
        new Vector3(5, 4, -4)
    };

    [SerializeField] private GameObject spawnpointPrefab;

    private void Awake() 
    {
        for (int i = 0; i < initPos.Length; i++)
        {
            var currentSpawnpoint = Instantiate(spawnpointPrefab, initPos[i], Quaternion.identity);
            currentSpawnpoint.transform.SetParent(transform);
            spawnPoints.Add(currentSpawnpoint.GetComponent<SpawnPoint>());
        }
    }
    
    private void OnEnable() {
        Message.AddListener<GameEventMessage>(OnMessage);
    }

    private void OnDisable() {
        Message.RemoveListener<GameEventMessage>(OnMessage);
    }

    private void OnMessage(GameEventMessage message) {
        if(message == null || message.Source == null) return;

        if(message.EventName.Equals("Respawn"))
        {
            message.Source.SetActive(false);
            var customCharacterController = message.Source.GetComponent<CustomCharacterController>();
            Respawn(customCharacterController);
        }
    }

    private void Respawn(CustomCharacterController x)
    {
        foreach (var spawnPoint in spawnPoints)
        {
            if(spawnPoint.IsAvailable)
            {
                spawnPoint.myCoroutine = spawnPoint.Spawn(x);
                StartCoroutine(spawnPoint.myCoroutine);

                return;
            }
        }
    }
}
