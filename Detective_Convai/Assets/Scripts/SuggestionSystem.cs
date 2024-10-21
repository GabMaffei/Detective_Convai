using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Convai.Scripts.Runtime.Core;

public class SuggestionSystem : MonoBehaviour
{
    public GameController gameController;
    public List<LocalInventory> npcInventories; // Inventários dos NPCs
    public TMP_Dropdown personDropdown, weaponDropdown, locationDropdown;
    public GameObject resultPanel; // Exibe a carta que o NPC vai mostrar
    public TextMeshProUGUI resultText; // Texto do resultado no painel
    public ConvaiNPCManager convaiNPCManager; // Gerencia o chat
    public GameObject suggestionPanel; // O painel de palpites
    public InterrogationController interrogationController; // Controla a troca de NPCs

    private List<string> npcsWithoutClues = new List<string>(); // Lista de NPCs que não possuem pistas
    private int currentNPCIndex = 0; // Variável para controlar o NPC atual

    public void OpenSuggestionPanel()
    {
        // Oculta o chat de conversa durante o palpite
        convaiNPCManager.rayLength = 0;
        // Reseta o índice do NPC para começar pelo primeiro (índice 0)
        currentNPCIndex = 0;
        // Ativa o painel de sugestão
        suggestionPanel.SetActive(true);
    }

    // Função para fechar o painel de sugestão
    public void CloseSuggestionPanel()
    {
        // Restaura o chat de conversa após o palpite
        convaiNPCManager.rayLength = 4.5f;
        suggestionPanel.SetActive(false); // Oculta o painel
    }

    public void CloseResultPanel()
    {
        // Restaura o chat de conversa após o palpite
        convaiNPCManager.rayLength = 4.5f;
        resultPanel.SetActive(false); // Oculta o painel de resultado
    }

    // Função de palpite
    public void ConfirmSuggestion()
    {
        Clue guessedPerson = gameController.GetClueByName(personDropdown.options[personDropdown.value].text);
        Clue guessedWeapon = gameController.GetClueByName(weaponDropdown.options[weaponDropdown.value].text);
        Clue guessedLocation = gameController.GetClueByName(locationDropdown.options[locationDropdown.value].text);

        List<Clue> matchingClues = new List<Clue>();
        npcsWithoutClues.Clear(); // Limpa a lista de NPCs sem pistas
        suggestionPanel.SetActive(false); // Oculta o painel de sugestão
        convaiNPCManager.rayLength = 0; // Desativa temporariamente a interação do Raycast com NPCs

        // Reseta o índice do NPC para começar pelo primeiro
        currentNPCIndex = 0;

        // Percorre todos os NPCs até encontrar uma pista ou esgotar as opções
        while (currentNPCIndex < npcInventories.Count)
        {
            LocalInventory npcInventory = npcInventories[currentNPCIndex];

            // Ajuste para sincronizar o NPC corretamente usando o InterrogationController
            interrogationController.SetNPCByIndex(currentNPCIndex); 

            // Verifica se o NPC tem alguma das pistas do palpite
            matchingClues.Clear(); // Limpa as pistas do NPC anterior
            if (npcInventory.HasClue(guessedPerson)) matchingClues.Add(guessedPerson);
            if (npcInventory.HasClue(guessedWeapon)) matchingClues.Add(guessedWeapon);
            if (npcInventory.HasClue(guessedLocation)) matchingClues.Add(guessedLocation);

            // Se o NPC tem pelo menos uma pista
            if (matchingClues.Count > 0)
            {
                // Seleciona aleatoriamente uma pista para mostrar ao jogador
                Clue clueToShow = matchingClues[Random.Range(0, matchingClues.Count)];
                
                // Formatação do resultado
                string noClueNPCsText = npcsWithoutClues.Count > 0 
                    ? "Esses personagens não tinham cartas correspondentes: " + string.Join(", ", npcsWithoutClues) + "\n" 
                    : "";

                resultPanel.SetActive(true);
                resultText.text = noClueNPCsText + npcInventory.GetComponent<ConvaiNPC>().characterName + " lhe mostrou a carta: " + clueToShow.evidenceName;
                break; // Interrompe o loop ao encontrar um NPC com uma pista
            }
            else
            {
                // Adiciona o nome do NPC à lista de NPCs sem pistas
                npcsWithoutClues.Add(npcInventory.GetComponent<ConvaiNPC>().characterName);
            }

            // Incrementa para o próximo NPC
            currentNPCIndex++;
        }

        // Caso nenhum NPC tenha pistas
        if (matchingClues.Count == 0)
        {
            resultPanel.SetActive(true);
            string noClueNPCs = string.Join(", ", npcsWithoutClues);
            resultText.text = "Nenhum personagem tinha uma carta correspondente.\nPerguntado a: " + noClueNPCs;
        }
    }

}
