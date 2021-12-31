using UnityEngine;

public class WeaponSwitching : MonoBehaviour {
    //weapon index is based on Unity Hierachy
    //first weapon in hierachy has index 0, second has 1, etc 

    public int selectedWeapon = 0;

    private int previousWeapon = 1;

    void Start() {
        SelectWeapon();
    }

    void Update() {
        int previousSelectedWeapon = selectedWeapon;
        if(Input.GetAxis("Mouse ScrollWheel") > 0f) {
            if (selectedWeapon >= transform.childCount - 1) {
                selectedWeapon = 0;
            }
            else {
                selectedWeapon++;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f) {
            if (selectedWeapon <= 0)
                selectedWeapon = transform.childCount - 1;
            else
                selectedWeapon--;
        }

        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            selectedWeapon = 0;
            previousWeapon = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            selectedWeapon = 1;
            previousWeapon = 0;
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            int temp = selectedWeapon;
            selectedWeapon = previousWeapon;
            previousWeapon = temp;
            SelectWeapon();
        }

        if (previousSelectedWeapon != selectedWeapon) {
            SelectWeapon();
        }        
    }

    void SelectWeapon() {
        int i = 0;
        foreach(Transform weapon in transform) {
            if(i == selectedWeapon) {
                weapon.gameObject.SetActive(true);
            }
            else {
                weapon.gameObject.SetActive(false);
                //if(weapon.gameObject.transform.name == "lawnchair")
                //    weapon.gameObject.GetComponent<Animation>().Play("WeaponSwitching");
            }
            i++;
        }
    }
}
