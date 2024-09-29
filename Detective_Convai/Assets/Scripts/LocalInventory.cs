using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalInventory : MonoBehaviour
{
    public List<Clue> inventoryOfClues; //{ get; private set; }

    private void Awake()
    {
        inventoryOfClues = new List<Clue>();
    }    

    public void Add(Clue newClue)
    {
        inventoryOfClues.Add(newClue);
    }

    public void Remove(Clue oldClue)
    {
        inventoryOfClues.Remove(oldClue);
    }
}
