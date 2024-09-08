using Mirror;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class JoinLobbyMenu : MonoBehaviour
    {
        [SerializeField] private NetMen networkManager = null;

        [Header("UI")]
        [SerializeField] private GameObject landingPagePanel = null;
        [SerializeField] private TMP_InputField ipAddressInputField = null;
        [SerializeField] private Button joinButton = null;

        private void OnEnable()
        {
            Debug.Log("OnEnable");
            NetMen.OnClientConnected += HandleClientConnected;
            NetMen.OnClientDisconnected += HandleClientDisconnected;
        }

        private void OnDisable()
        {
            Debug.Log("OnDisable");
            NetMen.OnClientConnected -= HandleClientConnected;
            NetMen.OnClientDisconnected -= HandleClientDisconnected;
        }

        public void JoinLobby() { 
      
            string ipAddress = ipAddressInputField.text;
            networkManager.networkAddress = ipAddress;
            Debug.Log("JoinLobby_StartsClient");
            networkManager.StartClient();
        }

        private void HandleClientConnected()
        {
            Debug.Log("HandleClientConnected");
            joinButton.interactable = true;

            gameObject.SetActive(false);
            landingPagePanel.SetActive(false);

        }

        private void HandleClientDisconnected()
        {
            Debug.Log("HandleClientDisconnected");
            joinButton.interactable = true;
        }
    }
}