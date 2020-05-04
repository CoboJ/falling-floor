using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private float timeBetweenRow = 1f;
    [SerializeField] private float timeBetweenBlocks = 0.5f;
    [SerializeField] private Vector3 initBlockScale = Vector3.zero;
    [SerializeField] private Vector2 mapSize = new Vector2(7, 20);
    [SerializeField] private List<GameObject> blocks = new List<GameObject>();

    [Space(2), Header("Blocks Animation Settings")]
    [SerializeField] private float timeToComplete = 1;
    [SerializeField] private float moveToY = -1f;
    [SerializeField] private Vector3 scaleTo = Vector3.one;
    [SerializeField] private Ease moveEase = Ease.OutExpo;
    [SerializeField] private Ease scaleEase = Ease.InExpo;


    private GameObject currentMap;
    private IEnumerator generateMap;
    [SerializeField] private List<bool> Spawnable = new List<bool>();
    
    private void OnEnable() 
    {
        generateMap = GenerateMap();
        StartCoroutine(generateMap);
    }

    private void OnDisable() 
    {
        StopCoroutine(generateMap);
        Spawnable.RemoveRange(0, Spawnable.Count);
        Destroy(currentMap);
    }

    private IEnumerator GenerateMap() 
    {
        currentMap = new GameObject("_Map");
        currentMap.transform.position = Vector3.zero;
        var blockType = Random.Range(0, 2);
        
        for (int i = 0; i < mapSize.x; i++) { Spawnable.Add(true); }

        for (int z = 0; z < mapSize.y; z++)
        {
            var blockXPos = -((mapSize.x / 2) - .5f);

            for (int x = 0; x < mapSize.x; x++)
            {
                if(!Spawnable[x]) { blockXPos++; continue; }

                var block = Instantiate (
                    blocks[blockType], 
                    new Vector3(blockXPos, 
                    (moveToY - 2f), z), 
                    Quaternion.identity, 
                    currentMap.transform
                );

                block.transform.localScale = initBlockScale;
                block.transform.SetParent(currentMap.transform);

                Sequence mySequence = DOTween.Sequence();
                mySequence
                    .Append(block.transform.DOLocalMoveY(moveToY, timeToComplete).SetEase(moveEase))
                    .Join(block.transform.DOScale(scaleTo, timeToComplete).SetEase(scaleEase))
                    .SetTarget(block);
                
                mySequence.Restart();

                blockType = blockType.Equals(0) ? 1 : 0;
                blockXPos++;

                yield return new WaitForSeconds(timeBetweenBlocks);
            }

            yield return new WaitForSeconds(timeBetweenRow);
        }
    }
}
