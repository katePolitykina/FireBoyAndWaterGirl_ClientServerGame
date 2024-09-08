using UnityEngine;
using Mirror;

public class HingeBar : NetworkBehaviour
{
    private HingeJoint2D hingeJoint2D;

    [Header("Связанная мобильная платформа")]
    public MobilePlatform mobilePlatform;

    [SyncVar(hook = nameof(OnOpenStateChanged))]
    private bool isOpen;

    private float middleAngle;

    private float syncedAngle;

    private void Awake()
    {
        hingeJoint2D = GetComponent<HingeJoint2D>();
        middleAngle = (hingeJoint2D.limits.max + hingeJoint2D.limits.min) / 2f;
        Debug.Log(middleAngle);
        isOpen = false;
    }

    private void Update()
    {
        if (isServer)
        {
            syncedAngle = hingeJoint2D.jointAngle; // Синхронизируем угол с сервером

            if (isOpen && hingeJoint2D.jointAngle > middleAngle)
            {
                RpcSetHingeAngle(hingeJoint2D.limits.max);
                Debug.Log("закрываем");
                isOpen = false;
                Debug.Log("isOpen: " + isOpen + hingeJoint2D.jointAngle);
               // mobilePlatform.Close();
            }
            else if (!isOpen && hingeJoint2D.jointAngle < middleAngle)
            {
                Debug.Log("открываем");
                RpcSetHingeAngle(hingeJoint2D.limits.min);
                isOpen = true;
                Debug.Log("isOpen: " + isOpen + hingeJoint2D.jointAngle);
              //  mobilePlatform.Open();
            }
        }
    }



    public void SetHingeAngleToAngle(float targetAngle)
    {
        JointMotor2D motor = hingeJoint2D.motor;
        motor.motorSpeed = targetAngle > hingeJoint2D.jointAngle ? 300f : -300f;
        hingeJoint2D.motor = motor;
        hingeJoint2D.useMotor = true;

        StartCoroutine(StopMotorWhenAngleReached(targetAngle));
    }

    private System.Collections.IEnumerator StopMotorWhenAngleReached(float targetAngle)
    {
        while (Mathf.Abs(hingeJoint2D.jointAngle - targetAngle) > 0.1f)
        {
            yield return null;
        }

        JointMotor2D motor = hingeJoint2D.motor;
        motor.motorSpeed = 0;
        hingeJoint2D.motor = motor;
        hingeJoint2D.useMotor = false;
    }

    private void OnOpenStateChanged(bool oldVal, bool newVal)
    {
        if (isOpen)
        {
            mobilePlatform.Open();
        }
        else
        {
            mobilePlatform.Close();
        }
    }

    private void OnAngleChanged(float oldAngle, float newAngle)
    {
        if (isServer)
        {
            RpcSetHingeAngle(newAngle); // Вызов RPC на клиентах для синхронизации угла
        }
        else
        {
            CmdSetHingeAngle(newAngle);
        }
    }
    [ClientRpc]
    private void RpcSetHingeAngle(float targetAngle)
    {
        SetHingeAngleToAngle(targetAngle);
    }
    [Command]
    private void CmdSetHingeAngle(float newAngle)
    {
        RpcSetHingeAngle(newAngle);
    }
}
