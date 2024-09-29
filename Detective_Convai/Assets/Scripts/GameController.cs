using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<Clue> deck = new List<Clue>(); // Lista de pistas do jogo.
    public Transform npcParent; // Objeto "NPCs" que contém todos os NPCs
    private List<LocalInventory> npcInventories = new List<LocalInventory>(); // Lista de inventários dos NPCs
    
    // Start is called before the first frame update
    void Start()
    {
        // Carrega todos os NPCs da cena
        foreach (Transform npc in npcParent)
        {
            LocalInventory inventory = npc.GetComponent<LocalInventory>();
            if (inventory != null)
            {
                npcInventories.Add(inventory);
            }
        }

        // Embaralha o deck de pistas
        ShuffleDeck();

        // Distribui as pistas para os NPCs
        DistributeClues();
    }

    void ShuffleDeck()
    {
        // Método para embaralhar o deck
        for (int i = 0; i < deck.Count; i++)
        {
            Clue temp = deck[i];
            int randomIndex = Random.Range(0, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void DistributeClues()
    {
        int currentClueIndex = 0;
        
        // Distribui as pistas igualmente entre os NPCs
        while (currentClueIndex < deck.Count)
        {
            foreach (LocalInventory npcInventory in npcInventories)
            {
                if (currentClueIndex < deck.Count)
                {
                    npcInventory.Add(deck[currentClueIndex]);
                    currentClueIndex++;
                }
                else
                {
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Pode adicionar lógica de turnos aqui, se necessário
    }

}
