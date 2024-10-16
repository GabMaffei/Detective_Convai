using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Convai.Scripts.Runtime.Core;

public class InventoryNotifier : MonoBehaviour
{
    // Referência ao GameObject que contém todos os NPCs
    public GameObject npcContainer;

    // Função para notificar todos os NPCs sobre os itens do seu inventário
    [ContextMenu("Notify NPCs of Inventory")]
    public void NotifyNPCsOfInventory()
    {
        // Itera por todos os NPCs filhos do GameObject npcContainer
        foreach (Transform npcTransform in npcContainer.transform)
        {
            // Obtém o script ConvaiNPC e LocalInventory de cada NPC
            ConvaiNPC npc = npcTransform.GetComponent<ConvaiNPC>();
            LocalInventory inventory = npcTransform.GetComponent<LocalInventory>();

            // Verifica se os componentes foram encontrados
            if (npc != null && inventory != null)
            {
                // Constrói a mensagem com o conteúdo do inventário
                string message = BuildInventoryMessage(inventory.inventoryOfClues);

                // Envia a mensagem para o NPC usando SendTextDataAsync
                npc.SendTextDataAsync(message);
            }
        }
    }

    // Função auxiliar para construir a mensagem de pistas a partir do inventário
    private string BuildInventoryMessage(List<Clue> clues)
    {
        if (clues.Count == 0)
        {
            return "Você não tem pistas no momento.";
        }

        string message = "As pistas que você possui são:\n";
        foreach (Clue clue in clues)
        {
            message += $"- {clue.evidenceName}\n";
        }

        return message;
    }
}
