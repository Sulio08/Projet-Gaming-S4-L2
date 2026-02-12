using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    float _horizontalMovement;
    float speed = 3f;
    Rigidbody2D _rb;
    public Transform groundCheck; 
    public LayerMask groundLayer; 
    private bool canJump; 
    float jumpingPower = 5f;     
    float _currentAcceleration;
    float accelerationPower = 1f;   
    private bool buttonPressed = false;


    
private void Awake(){
    _rb = GetComponent<Rigidbody2D>();
}

public void Move(InputAction.CallbackContext context) {
     buttonPressed = context.performed; // Si l'action est réalisée, je passe mon booléen à vrai
    // Si un bouton est pressé, je mets à jour la direction horizontale qui ne sert plus que pour le signe désormais
    if (buttonPressed) _horizontalMovement = (context.ReadValue<Vector2>().x > 0) ? 1 : -1;
}

private void Update() {
    UpdateAcceleration();
    
    _rb.linearVelocity = new Vector2(_horizontalMovement * speed *_currentAcceleration, _rb.linearVelocity.y);
    if (IsGrounded() && _rb.linearVelocity.y <= 0) canJump = true;
}

public void Jump(InputAction.CallbackContext context) {
    if (!context.performed || !canJump) return;
    _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpingPower);
    canJump = false;
}

private bool IsGrounded()

{
  
    return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
}

private void UpdateAcceleration() {
    if (!buttonPressed && _currentAcceleration > 0) {
        
        if (IsGrounded()) _currentAcceleration -= accelerationPower * Time.deltaTime;
        else _currentAcceleration -= accelerationPower * Time.deltaTime * 0.5f;
        // pour éviter les tremblements
        if (_currentAcceleration <= 0f) {
            _currentAcceleration = 0f;
            _horizontalMovement = 0f;
        }
    } 
    else if (_currentAcceleration < 5) { // Si bouton, on accélère jusqu'à 5 au max.
        _currentAcceleration += accelerationPower * Time.deltaTime;
    }


}

}
