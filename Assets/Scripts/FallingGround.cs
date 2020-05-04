using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FallingGround : MonoBehaviour
{
    private BoxCollider boxCollider;
    private SphereCollider sphereCollider;

    private void Start() 
    {
        boxCollider = GetComponent<BoxCollider>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player") && GameManager.Instance.gameState.Equals(GameManager.GameState.Playing))
        {
            StartCoroutine(StartFalling());
        }
    }

    IEnumerator StartFalling()
    {
        yield return new WaitForSeconds(.5f);

        Sequence mySequence = DOTween.Sequence();
        
        mySequence
            .Append(transform.DOLocalMoveY(-2, 1).SetEase(Ease.OutExpo))
            .Join(transform.DOScale(Vector3.zero, 1).SetEase(Ease.InExpo))
            .SetAutoKill(false)
            .Pause();

        mySequence.Restart();

        boxCollider.enabled = false;
        sphereCollider.enabled = false;

        mySequence.WaitForCompletion();

        yield return new WaitForSeconds(2f);

        mySequence.SmoothRewind();

        yield return mySequence.WaitForRewind();

        mySequence.Kill();

        boxCollider.enabled = true;
        sphereCollider.enabled = true;
    }
}
