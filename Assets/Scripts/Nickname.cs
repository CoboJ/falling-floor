using UnityEngine;
using TMPro;

public class Nickname : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    private void Awake() {
        if(PlayerPrefs.HasKey("PlayerName"))
            inputField.text = PlayerPrefs.GetString("PlayerName");
    }

    public void SetNickname(string nickname)
    {
        PlayerPrefs.SetString("PlayerName", nickname);
    }
}
