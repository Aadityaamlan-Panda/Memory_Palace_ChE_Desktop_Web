using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DropdownLoader : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    void Start()
    {
        dropdown.ClearOptions();

        List<string> options = new List<string>
        {
            "Chair",
            "Table",
            "Door",
            "Lamp"
        };

        dropdown.AddOptions(options);
    }
}