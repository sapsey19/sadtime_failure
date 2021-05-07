using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour {
    public float lookSensitivity = 1f;

    PlayerCharacterController playerCharacterController;
    bool m_FireInputWasHeld;

    // Start is called before the first frame update
    void Start() {
        playerCharacterController = GetComponent<PlayerCharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate() {
        m_FireInputWasHeld = GetFireInputHeld();
    }

    public Vector3 GetMoveInput() {
        Vector3 move = new Vector3(Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal), 0f, Input.GetAxisRaw(GameConstants.k_AxisNameVertical));

        // constrain move input to a maximum magnitude of 1, otherwise diagonal movement might exceed the max move speed defined
        move = Vector3.ClampMagnitude(move, 1);
        return move;
    }

    public float GetLookInputHorizontal() {
        return GetMouseAxis(GameConstants.k_MouseAxisNameHorizontal);
    }

    public float GetLookInputVertical() {
        return GetMouseAxis(GameConstants.k_MouseAxisNameVertical);
    }

    public bool GetJumpInputDown() {
        return Input.GetButtonDown(GameConstants.k_ButtonNameJump);
    }

    public bool GetJumpInputHeld() {
        return Input.GetButton(GameConstants.k_ButtonNameJump);
    }

    float GetMouseAxis(string mouseInputName) {
        float i = Input.GetAxisRaw(mouseInputName);
        i *= lookSensitivity;

        i *= 0.01f;

        return i;
    }

    public bool GetFireInputDown() {
        return GetFireInputHeld() && !m_FireInputWasHeld;
    }

    public bool GetFireInputReleased() {
        return !GetFireInputHeld() && m_FireInputWasHeld;
    }

    public bool GetFireInputHeld() { 
        return Input.GetButton(GameConstants.k_ButtonNameFire);
    }
}
