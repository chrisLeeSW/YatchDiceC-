using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomInPlayerInfo : MonoBehaviour
{
    public TextMeshProUGUI roomInPlayerInfoText;
    public Button playerReadyButton;


    public void Awake()
    {
        playerReadyButton.onClick.AddListener(UIManager.Instance.player.GetClientsession().GameReady);
        playerReadyButton.onClick.AddListener(() =>
        {
            playerReadyButton.gameObject.SetActive(false);
            gameObject.SetActive(false);
        });
    }
    public void OnEnable()
    {
        playerReadyButton.gameObject.SetActive(true);
    }
}
