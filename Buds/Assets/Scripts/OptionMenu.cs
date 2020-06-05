using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    public Button optionsExitButton;

    public void HideMenu() {
        if (optionsExitButton.IsActive()) {
            optionsExitButton.onClick.Invoke();
        }
        
    }
}
