using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SpawnPoint : MonoBehaviour
{
    private BoxCollider boxCollider;
    private bool isAvailable = true;

    public IEnumerator myCoroutine;
    public bool IsAvailable { get { return isAvailable; } }

    private void Awake() 
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
    }

    public IEnumerator Spawn(CustomCharacterController player)
    {
        isAvailable = false;
        
        Sequence mySequence = DOTween.Sequence();
        mySequence
            .Append(transform.DOLocalMoveY(5, .75f).SetEase(Ease.OutExpo))
            .Join(transform.DOScale(Vector3.one, .75f).SetEase(Ease.InExpo))
            .SetAutoKill(false)
            .Pause();

        mySequence.Restart();

        yield return mySequence.WaitForCompletion();

        boxCollider.enabled = true;

        player.tPersonCam.playerInactive = true;
        player.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        player.gameObject.SetActive(true);
        player.tPersonCam.CenterInCharacter();

        yield return new WaitForSeconds(2f);
        if(GameManager.Instance.gameState.Equals(GameManager.GameState.GameOver)) StopCoroutine(myCoroutine);

        player.myRigidbody.constraints = RigidbodyConstraints.FreezeRotation | ~RigidbodyConstraints.FreezePosition;
        mySequence.SmoothRewind();

        yield return mySequence.WaitForRewind();

        mySequence.Kill();

        boxCollider.enabled = false;
        isAvailable = true;

        if(GameManager.Instance.gameState.Equals(GameManager.GameState.GameOver)) { player.GameOver(); }
    }
}
