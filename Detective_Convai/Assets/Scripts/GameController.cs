using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public List<Clue> deck = new List<Clue>(); // Lista de pistas do jogo.
    public Transform npcParent; // Objeto "NPCs" que contém todos os NPCs
    public LocalInventory playerInventory; // Inventário do jogador
    private List<LocalInventory> npcInventories = new List<LocalInventory>(); // Lista de inventários dos NPCs
    public List<Clue> crimeEnvelope = new List<Clue>(); // Envelope de crime
    
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

        // Separar cartas para o envelope de crime
        PrepareCrimeEnvelope();

        // Distribui as pistas restantes para os NPCs e o jogador
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
    
    void PrepareCrimeEnvelope()
    {
        // Separar uma carta de cada tipo (suspeitos, armas, locais) para o envelope de crime
        Clue suspect = GetRandomClueByType("suspeito");
        Clue weapon = GetRandomClueByType("arma do crime");
        Clue location = GetRandomClueByType("local");

        crimeEnvelope.Add(suspect);
        crimeEnvelope.Add(weapon);
        crimeEnvelope.Add(location);
    }

    Clue GetRandomClueByType(string type)
    {
        List<Clue> cluesOfType = deck.FindAll(clue => clue.type == type);
        return cluesOfType[Random.Range(0, cluesOfType.Count)];
    }

    void DistributeClues()
    {
        int currentClueIndex = 0;
                int numPlayers = npcInventories.Count + 1; // Número de NPCs + 1 jogador
        int cluesPerPlayer = deck.Count / numPlayers; // Cartas distribuídas igualmente
        
        // Distribuir pistas para o jogador
        for (int i = 0; i < cluesPerPlayer; i++)
        {
            if (currentClueIndex < deck.Count)
            {
                playerInventory.Add(deck[currentClueIndex]);
                currentClueIndex++;
            }
        }

        // Distribuir pistas para os NPCs
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

    // Verifica se a acusação está correta
    public bool IsAccusationCorrect(List<Clue> accusation)
    {
        int correctCount = 0;
        foreach (Clue clue in accusation)
        {
            if (crimeEnvelope.Contains(clue))
            {
                correctCount++;
            }
        }

        return correctCount == 3; // Acusação correta se todas as 3 pistas corresponderem
    }

    // Função para obter uma pista pelo nome
    public Clue GetClueByName(string name)
    {
        foreach (Clue clue in deck)
        {
            if (clue.evidenceName == name)
            {
                return clue;
            }
        }
        return null;
    }

}
