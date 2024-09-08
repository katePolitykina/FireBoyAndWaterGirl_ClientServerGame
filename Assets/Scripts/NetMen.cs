using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class NetMen : NetworkManager { 

    public GameObject waterGirlPrefab;
    public GameObject fireBoyPrefab;
    

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;


    public override void OnClientConnect()
    {
        Debug.Log("ClientConnected_ClientsVersion");
        base.OnClientConnect();

        Debug.Log("ClientConnected_ClientsVersion_AddPlayer");
        NetworkClient.AddPlayer();
        OnClientConnected?.Invoke();
    }
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        NetworkManager.singleton.ServerChangeScene("Waiting");
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        OnClientDisconnected?.Invoke();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
   }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        Debug.Log("ClientConnected_ServerVersion_conCount: "+NetworkServer.connections.Count);
        base.OnServerConnect(conn);

        if (SceneManager.GetActiveScene().name != "Lobby" && SceneManager.GetActiveScene().name != "Waiting")
        {
            Debug.Log("ClientConnected_ServerVersion_Disconnect" + conn.identity);
            conn.Disconnect();
            return;
        }
        
        if (NetworkServer.connections.Count > maxConnections)
        {
            Debug.Log("ClientConnected_ServerVersion_Disconnect" + conn.identity);
            conn.Disconnect();
            return;
        }
       // FirstLevelStart();
    }
    
    public void FirstLevelStart()
    {
        Debug.Log("FirstLevel");
        if (NetworkServer.connections.Count == maxConnections)
        {
           NetworkManager.singleton.ServerChangeScene("LevelScene");
        }
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
       
        
        Debug.Log("OnServerAddPlayer connection id: " + conn.connectionId);
        // add player at correct spawn position
        Debug.Log("OnServerAddPlayer NumberOfPlayers0: " + numPlayers);
        GameObject player;
      
        if (numPlayers == 0)
        {
            player = Instantiate(fireBoyPrefab);
            player.GetComponent<Player>().isBoy = true;
        }
        else
        {
            player = Instantiate(waterGirlPrefab);
            player.GetComponent<Player>().isBoy = false;
        }
        DontDestroyOnLoad(player);
        NetworkServer.AddPlayerForConnection(conn, player);
        Debug.Log("OnServerAddPlayer NumberOfPlayers1: " + numPlayers);
        FirstLevelStart();
    }

    public GameObject FindInactiveChildByName(GameObject parent, string childName)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.gameObject.name == childName && !child.gameObject.activeSelf)
            {
                return child.gameObject;
            }
        }
        return null;
    }
    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
    }
    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log($"Scene changed to: {sceneName}, current numPlayers: {numPlayers}");
        base.OnServerSceneChanged(sceneName);
        if (sceneName == "Lobby" && NetworkServer.connections.Count < maxConnections)
        {
            Debug.Log("OnClientSceneChanged");
            GameObject Canvas_MainMenu = GameObject.Find("Canvas_MainMenu");
            if (Canvas_MainMenu != null)
            {
                Debug.Log("Found Canvas_MainMenu");
                GameObject Panel_waiting = FindInactiveChildByName(Canvas_MainMenu, "Panel_waiting");
                if (Panel_waiting != null)
                {
                    Debug.Log("Found Panel_waiting");
                    Panel_waiting.SetActive(true);
                }
                else
                {
                    Debug.Log("Panel_waiting not found");
                }

            }
            else
            {
                Debug.Log("Not Found Canvas_MainMenu");
            }
            GameObject Panel_LandingPage = GameObject.Find("Panel_LandingPage");
            if (Panel_LandingPage != null)
            {
                Panel_LandingPage.SetActive(false);
            }


        }
        //TODO: добавить проверку что сцена игровая
       
        if (sceneName == "LevelScene" || sceneName == "LevelScene1")
        {
            // Reassign players to their positions
            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                if (conn.identity != null)
                {
                    Player player = conn.identity.GetComponent<Player>();
                    if (player.isBoy)
                    {
                        //  conn.identity.transform.position = start.GetChild(0).position;
                        conn.identity.transform.position = GameObject.Find("BoySpawn").transform.position;
                        Debug.Log("SpawnpositionBoy");
                       
                    }
                    else
                    {
                        //  conn.identity.transform.position = start.GetChild(1).position;
                        conn.identity.transform.position = GameObject.Find("GirlSpawn").transform.position;
                        Debug.Log("SpawnpositionGirl");
                    }

                    conn.identity.gameObject.SetActive(true);
                }
            }
        }
        
    }
    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);

    }

}

