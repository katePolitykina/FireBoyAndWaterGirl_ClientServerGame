using Mirror;
using UnityEngine;

public class GearButton : NetworkBehaviour

{

    public MobilePlatform mobilePlatform;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void OnTriggerEnter2D()
    {
        if (isServer)
            rpcButtonDown();
        else cmdButtonDown();
    }
    [Command]
    private void cmdButtonDown()
    {
        rpcButtonDown();
    }
    [ClientRpc]
    private void rpcButtonDown()
    {
        mobilePlatform.openCount++;
        if (mobilePlatform.openCount > 0)
        {
            mobilePlatform.Open();
            spriteRenderer.enabled = false;
        }
    }
    [Command]
    private void cmdButtonUp()
    {
        rpcButtonUp();
    }
    [ClientRpc]
    private void rpcButtonUp()
    {
        mobilePlatform.openCount--;
        if (mobilePlatform.openCount == 0)
        {
            mobilePlatform.Close();
            spriteRenderer.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isServer)
            rpcButtonUp();
        else cmdButtonUp();
    }







}
