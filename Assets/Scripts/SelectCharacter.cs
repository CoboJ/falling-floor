using UnityEngine;
using Doozy.Engine.UI;
using TMPro;

public class SelectCharacter : MonoBehaviour
{
    public void SetCharacter(GameObject character)
    {
        PlayerPrefs.SetString("Character", character.name);

        UIPopup popup = UIPopup.GetPopup("Selected");
        if(popup == null) { return; }

        popup.Data.Labels[0].GetComponent<TextMeshProUGUI>().SetText(character.name);
        popup.Show();
    }
}
