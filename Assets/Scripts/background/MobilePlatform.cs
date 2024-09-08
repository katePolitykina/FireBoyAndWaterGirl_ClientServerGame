using Mirror;
using UnityEngine;

public class MobilePlatform : NetworkBehaviour
{
    private float changeSpeed = 2.5f;
    public Vector2 offset;

    private Vector3 closePosition;
    private Vector3 openPosition;
    [SyncVar]
    public int openCount = 0;

    private enum PlatformState
    {
        None,
        Close, MoveToClose, Open, MoveToOpen,
    }
    PlatformState platformState;

    private void Awake()
    {
        platformState = PlatformState.Close;
        closePosition = transform.position;
        openPosition = transform.position + (Vector3)offset;
    }


    private void Update()
    {
        switch(platformState)
        {
            case PlatformState.MoveToClose:
                if ((transform.position - openPosition).sqrMagnitude < 0.01f
                    || Vector3.Dot((transform.position - closePosition).normalized, (transform.position - openPosition).normalized) < -0.9f)
                {
                    Vector3 direction = (closePosition - transform.position).normalized;
                    transform.position += direction * changeSpeed * Time.deltaTime;
                }
                else
                {
                    platformState = PlatformState.Close;
                }
                break;
            case PlatformState.MoveToOpen:
                if ((transform.position - closePosition).sqrMagnitude < 0.01f
                    || Vector3.Dot((transform.position - closePosition).normalized, (transform.position - openPosition).normalized) < -0.9f)
                {
                    Vector3 direction = (openPosition - transform.position).normalized;
                    transform.position += direction * changeSpeed * Time.deltaTime;
                }
                else
                {
                    platformState = PlatformState.Open;
                }
                break;
        }
    }

    public void Open()
    {
        Debug.Log("Opens");
        if (platformState != PlatformState.Open)
        {
            platformState = PlatformState.MoveToOpen;
        }
    }

    public void Close()
    {
        if (platformState != PlatformState.Close)
        {
            platformState = PlatformState.MoveToClose;
        }
    }


}
