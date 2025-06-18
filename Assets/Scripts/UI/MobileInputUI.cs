using UnityEngine;
using UnityEngine.UI;

public class MobileInputUI : MonoBehaviour
{
    public FixedJoystick fixedJoystick;
    public Button jumpButton;
    public Button attackButton;

    void Start()
    {
        InputManager.joystick = fixedJoystick;

        jumpButton.onClick.AddListener(() => {
            Debug.Log("Jump button pressed");
            InputManager.isJumpPressed = true;
        });

        attackButton.onClick.AddListener(() => {
            Debug.Log("Attack button pressed");
            InputManager.isAttackPressed = true;
        });
    }
}