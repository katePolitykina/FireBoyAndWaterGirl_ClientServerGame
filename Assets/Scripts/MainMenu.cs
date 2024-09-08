using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetMen networkManager = null;
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private GameObject WaitingPlayersPanel = null;


    public int port;
    bool IsPortAvailable(int port)
    {
        try
        {
            // Создаем сокет и привязываем его к указанному порту
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(1);
            socket.Close();
            return true; // Если порт доступен, возвращаем true
        }
        catch (SocketException)
        {
            Debug.Log("PortIsTaken");
            return false; // Если порт занят, возвращаем false
        }
    }
    public void HostLobby()
    {
        Debug.Log("HostLobby");
        if (IsPortAvailable(port)) {
            networkManager.StartHost();
            landingPagePanel.SetActive(false);
            WaitingPlayersPanel.SetActive(true);
        }

    }
   
    public void StopHost()
    {
        networkManager.StopHost();
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void debug()
    {
        Debug.Log("Clicked");
    }

    
}
