using UnityEngine;
using Unity.Netcode;


public class PlayerMovement : NetworkBehaviour
{
   
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Rigidbody2D rb;
    
    private PlayerStats playerStats;



    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsOwner)
        playerStats = GetComponent<PlayerStats>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        Vector2 moveInput = inputReader.MoveInput;
        Vector2 moveDir = moveInput.normalized;

        rb.linearVelocity = moveDir * playerStats.movementSpeed.Value;

        if (moveDir != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

            bodyTransform.rotation = Quaternion.RotateTowards(
                bodyTransform.rotation,
                targetRotation,
                playerStats.turningRate * Time.fixedDeltaTime
            );
        }
    }

   
}
