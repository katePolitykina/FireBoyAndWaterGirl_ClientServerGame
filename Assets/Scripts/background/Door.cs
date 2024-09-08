using System.Collections;
using System;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class Door : NetworkBehaviour
{
    public bool isBoy;
    [SyncVar]
    public bool isOpen = false;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    [Command]
    private void cmdDoorOpen(string collisionName)
    {
        rpcDoorOpen(collisionName);
    }
    [ClientRpc]
    private void rpcDoorOpen(string collisionName)
    {
        if (isBoy == true && collisionName == "Fireboy(Clone)")
        {
            animator.SetBool("Open", true);
        }
        else if (isBoy == false && collisionName == "Watergirl(Clone)")
        {
            animator.SetBool("Open", true);
        }
    }
    [Command]
    private void Cmdend()
    {
        NetworkManager.singleton.ServerChangeScene("LevelScene");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isServer) rpcDoorOpen(collision.name);
        else cmdDoorOpen(collision.name);

    }

    [Command]
    private void cmdDoorClose(string collisionName)
    {
        rpcDoorClose(collisionName);
    }

    [ClientRpc]
    private void rpcDoorClose(string collisionName)
    {
        if (isBoy == true && collisionName == "Fireboy(Clone)")
        {
            animator.SetBool("Open", false);
            animator.SetBool("Close", true);

        }
        else if (isBoy == false && collisionName == "Watergirl(Clone)")
        {
            animator.SetBool("Open", false);
            animator.SetBool("Close", true);

        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isServer) rpcDoorClose(collision.name);
        else cmdDoorClose(collision.name);

    }
}
