using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class PlayerData : ScriptableObject, ISerializationCallbackReceiver
{
    [Header("Constant Data")]
    [SerializeField, Range(1, 4)] private int InitialRank = 1;
    [SerializeField] private float InitialScore = 0.00f;
    [SerializeField] private string InitialGamertag = "Player";

    [Header("Dynamic Data")]
    public int RuntimeRank;
    public float RuntimeScore;
    public string RuntimeGamertag;

    public void OnAfterDeserialize() {
        RuntimeRank = InitialRank;
        RuntimeScore = InitialScore;
        RuntimeGamertag = InitialGamertag;

        SceneManager.activeSceneChanged += OnSceneChanged;
    } 

    public void OnBeforeSerialize() { }
         
    void OnSceneChanged(Scene current, Scene next)
    {
        RuntimeRank = InitialRank;
        RuntimeScore = InitialScore;
    }
}
