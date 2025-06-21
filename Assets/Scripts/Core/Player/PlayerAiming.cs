using UnityEngine;
using Unity.Netcode;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform turretTransform;
    private void LateUpdate()
    {
        if(!IsOwner) return;
        Vector2 aimWordPosition = Camera.main.ScreenToWorldPoint(inputReader.AimInput);
        turretTransform.up = new Vector2(
            aimWordPosition.x-turretTransform.position.x,
            aimWordPosition.y-turretTransform.position.y).normalized;

    }
}
