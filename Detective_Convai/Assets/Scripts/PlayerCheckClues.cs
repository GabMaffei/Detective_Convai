using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using Convai.Scripts.Runtime.Core;

public class PlayerCheckClues : MonoBehaviour
{
    public GameObject playerCluesPanel;
    public TMP_Text personEvidenceName;
    public TMP_Text weaponEvidenceName;
    public TMP_Text roomEvidenceName;
    public LocalInventory playerInventory;
    public ConvaiNPCManager convaiNPCManager;

    public void OpenPlayerCluesPanel()
    {
        convaiNPCManager.rayLength = 0f;

        string personEvidenceTemp = "", weaponEvidenceTemp = "", roomEvidenceTemp = "";
        foreach (Clue clue in playerInventory.GetAllClues())
        {
            switch (clue.type)
            {
                case "suspeito":
                    personEvidenceTemp += clue.evidenceName + "\n"; 
                    break;
                case "arma do crime":
                    weaponEvidenceTemp += clue.evidenceName + "\n"; 
                    break;
                case "local":
                    roomEvidenceTemp += clue.evidenceName + "\n"; 
                    break;
            }
        }

        personEvidenceName.text = personEvidenceTemp;
        weaponEvidenceName.text = weaponEvidenceTemp;
        roomEvidenceName.text = roomEvidenceTemp;
        
        playerCluesPanel.SetActive(true);
    }

    public void ClosePlayerCluesPanel()
    {
        convaiNPCManager.rayLength = 4.5f;

        playerCluesPanel.SetActive(false);
    }

}
