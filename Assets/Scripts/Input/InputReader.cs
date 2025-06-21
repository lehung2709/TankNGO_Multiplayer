using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(fileName = "NewInputReader", menuName = "Input/InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    
    public Vector2 MoveInput {  get; private set; }= Vector2.zero;
    public bool PrimaryFireInput {  get; private set; }= false;
    public Vector2 AimInput { get; private set; }= Vector2.zero ;


    private Controls controls;
    private void OnEnable()
    {
        if(controls == null)
        {
           controls=new Controls();
           controls.Player.SetCallbacks(this);
        }

        controls.Player.Enable();
        
    }
    private void OnDisable()
    {
        if(controls != null)
        {
            controls.Player.Disable();
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput=context.ReadValue<Vector2>();
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        if(context.performed) 
        PrimaryFireInput=true;
        else if(context.canceled)
        PrimaryFireInput = false;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        AimInput=context.ReadValue<Vector2>();
    }
}
