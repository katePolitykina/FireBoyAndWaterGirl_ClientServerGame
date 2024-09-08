using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class n : NetworkBehaviour
{
    [SyncVar]
    public bool isBoy = false;
    [SyncVar]
    public bool isGirl = false;

 
    [Server]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        Debug.Log("OnTriggerEnter2D not Server");
        if (collision.name == "Fireboy(Clone)")
        {
            Debug.Log("BoyCollides");
            isBoy = true;
        }
        if (collision.name == "Watergirl(Clone)")
        {
            Debug.Log("GirlCollides");
            isGirl = true;
        }
        Debug.Log("Boy: " + isBoy + " Girl: " + isGirl);
        if (isBoy && isGirl)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.name == "LevelScene")
                NetworkManager.singleton.ServerChangeScene("LevelScene1");
            else if (activeScene.name == "LevelScene1")
                NetworkManager.singleton.ServerChangeScene("EndGame");
        }


    }


    [Server]
    private void OnTriggerExit2D(Collider2D collision) {

        if (collision.name == "Fireboy(Clone)")
        {

            isBoy = false;
        }
        else if (collision.name == "Watergirl(Clone)")
        {
            isGirl = false;
        }

    }





}
