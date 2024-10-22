using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Convai.Scripts.Runtime.Core;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;

public class SuggestionSystem : MonoBehaviour
{
    public GameController gameController;
    public List<LocalInventory> allInventories; // Inventários dos NPCs
    public LocalInventory playerInventory;
    public TMP_Dropdown personDropdown, weaponDropdown, locationDropdown;
    public GameObject resultPanel; // Exibe a carta que o NPC vai mostrar
    public TextMeshProUGUI resultText; // Texto do resultado no painel
    public ConvaiNPCManager convaiNPCManager; // Gerencia o chat
    public GameObject suggestionPanel; // O painel de palpites
    public InterrogationController interrogationController; // Controla a troca de NPCs
    public Clue emptyClue;
    public GameObject playerCardSelectionPanel;
    public TextMeshProUGUI personSuggestedevidenceName;
    public TextMeshProUGUI weaponSuggestedevidenceName;
    public TextMeshProUGUI roomSuggestedevidenceName;
    public Button confirmPersonSuggestionButton;
    public Button confirmWeaponSuggestionButton;
    public Button confirmRoomSuggestionButton;
    public GameObject turnResultPanel; // O painel de resultados do turno
    public TextMeshProUGUI turnResultText;
    public TextMeshProUGUI personTurnResultEvidenceName;
    public TextMeshProUGUI weaponTurnResultEvidenceName;
    public TextMeshProUGUI roomTurnResultEvidenceName;

    private List<string> npcsWithoutClues = new List<string>(); // Lista de NPCs que não possuem pistas
    private int currentNPCIndex = 0; // Variável para controlar o NPC atual

    // Temporário para armazenar as pistas correspondentes
    private List<Clue> matchingClues = new List<Clue>();
    private List<Clue> lastMatchingCluesPlayer = new List<Clue>();
    private NPCAI lastMatchedNPCPlayer;

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

        matchingClues.Clear();
        npcsWithoutClues.Clear(); // Limpa a lista de NPCs sem pistas
        suggestionPanel.SetActive(false); // Oculta o painel de sugestão
        convaiNPCManager.rayLength = 0; // Desativa temporariamente a interação do Raycast com NPCs

        // Reseta o índice do NPC para começar pelo primeiro
        currentNPCIndex = 0;

        // Percorre todos os NPCs até encontrar uma pista ou esgotar as opções
        while (currentNPCIndex < allInventories.Count)
        {
            LocalInventory npcInventory = allInventories[currentNPCIndex];

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
                Clue clueToShow = matchingClues[UnityEngine.Random.Range(0, matchingClues.Count)];
                
                // Formatação do resultado
                string noClueNPCsText = npcsWithoutClues.Count > 0 
                    ? "Esses personagens não tinham pistas correspondentes: " + string.Join(", ", npcsWithoutClues) + "\n" 
                    : "";

                resultPanel.SetActive(true);
                resultText.text = noClueNPCsText + npcInventory.GetComponent<ConvaiNPC>().characterName + " lhe mostrou a pista: " + clueToShow.evidenceName;
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

    public Clue NPCMakeSuggestion(NPCAI npcAI, Clue guessedPerson, Clue guessedWeapon, Clue guessedLocation)
    {
        List<Clue> matchingClues = new List<Clue>();
        npcsWithoutClues.Clear(); // Limpa a lista de NPCs sem pistas
        currentNPCIndex = 0;

        Debug.Log(npcAI.name + " fez o palpite: " + guessedPerson.evidenceName + ", " + guessedWeapon.evidenceName + ", " + guessedLocation.evidenceName);

        // Percorre todos os NPCs até encontrar uma pista ou esgotar as opções
        while (currentNPCIndex < allInventories.Count)
        {
            LocalInventory npcInventory = allInventories[currentNPCIndex];

            // Verifica se o NPC atual é o jogador
            if (npcInventory == playerInventory)
            {
                matchingClues.Clear();
                if (npcInventory.HasClue(guessedPerson)) matchingClues.Add(guessedPerson);
                if (npcInventory.HasClue(guessedWeapon)) matchingClues.Add(guessedWeapon);
                if (npcInventory.HasClue(guessedLocation)) matchingClues.Add(guessedLocation);

                if (matchingClues.Count > 0)
                {
                    lastMatchedNPCPlayer = npcAI;
                    // Abre a UI para o jogador escolher qual carta mostrar
                    OpenPlayerCardSelectionPanel(matchingClues, guessedPerson, guessedWeapon, guessedLocation);
                    return null; // Aguardar o jogador escolher uma carta
                }
                currentNPCIndex++;
                continue;
            }

            // Pular o NPC que está fazendo a sugestão
            if (npcInventory == npcAI.npcInventory)
            {
                currentNPCIndex++;
                continue;
            }

            matchingClues.Clear();
            // Verifica se o NPC tem alguma das pistas do palpite
            if (npcInventory.HasClue(guessedPerson)) matchingClues.Add(guessedPerson);
            if (npcInventory.HasClue(guessedWeapon)) matchingClues.Add(guessedWeapon);
            if (npcInventory.HasClue(guessedLocation)) matchingClues.Add(guessedLocation);

            // Se o NPC tem pelo menos uma pista
            if (matchingClues.Count > 0)
            {
                Debug.Log("Achou um NPC que tem uma pista:" + npcInventory.GetComponent<ConvaiNPC>().characterName);
                // Seleciona aleatoriamente uma pista para mostrar ao NPC que fez o palpite
                Clue clueToShow = matchingClues[UnityEngine.Random.Range(0, matchingClues.Count)];
                // Exibe o NPC que mostrou a carta (sem revelar qual foi)
                String noClueNPCsText = npcsWithoutClues.Count > 0 
                    ? "Esses personagens não tinham pistas correspondentes: " + string.Join(", ", npcsWithoutClues) + "\n" 
                    : "";
                turnResultText.text = noClueNPCsText + npcInventory.GetComponent<ConvaiNPC>().characterName + " mostrou uma pista para " + npcAI.GetComponent<ConvaiNPC>().characterName;
                personTurnResultEvidenceName.text = guessedPerson.evidenceName;
                weaponTurnResultEvidenceName.text = guessedWeapon.evidenceName;
                roomTurnResultEvidenceName.text = guessedLocation.evidenceName;
                turnResultPanel.SetActive(true);
                return clueToShow; //Retorna pista para NPCAI
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
        Debug.Log("Nenhum personagem tinha uma carta correspondente.");
        return emptyClue; // Retorna a Clue em branco pública
    }

    public void OpenPlayerCardSelectionPanel(List <Clue> matchingCluesPlayer, Clue guessedPerson, Clue guessedWeapon, Clue guessedLocation)
    {
        // Oculta o chat de conversa durante o palpite
        convaiNPCManager.rayLength = 0;
        
        playerCardSelectionPanel.SetActive(true);

        // Exibe as sugestões no painel de seleção
        personSuggestedevidenceName.text = guessedPerson.evidenceName;
        weaponSuggestedevidenceName.text = guessedWeapon.evidenceName;
        roomSuggestedevidenceName.text = guessedLocation.evidenceName;

        // Limpa os botões
        confirmPersonSuggestionButton.interactable = false;
        confirmWeaponSuggestionButton.interactable = false;
        confirmRoomSuggestionButton.interactable = false;

        lastMatchingCluesPlayer = matchingCluesPlayer;

        foreach (Clue clue in matchingCluesPlayer)
        {
            Debug.Log("Evidencia: " + clue.evidenceName);
            Debug.Log("Tipo: " + clue.type);
            switch (clue.type)
            {
                case "suspeito":
                    confirmPersonSuggestionButton.interactable = true;
                    break;
                case "arma do crime":
                    confirmWeaponSuggestionButton.interactable = true;
                    break;
                case "local":
                    confirmRoomSuggestionButton.interactable = true;
                    break;
            }
        }
    }

    public void ConfirmPlayerCardSelection(string chosenClueType)
    {
        Clue chosenClue = null;

        // Verifica se há uma pista correspondente na lista de matchingClues
        foreach (Clue clue in lastMatchingCluesPlayer)
        {
            if (clue.type == chosenClueType)
            {
                chosenClue = clue;
                break; // Saímos do loop assim que encontramos a pista correta
            }
        }

        if (chosenClue != null)
        {
            Debug.Log("Jogador escolheu a carta: " + chosenClue.evidenceName);

            // Atualiza o cluesRevealed no NPCAI que fez o palpite
            // npcAI.cluesRevealed.Add(chosenClue);

            // Desativa o painel de seleção de cartas após a escolha
            playerCardSelectionPanel.SetActive(false);
            // Restaura o chat de conversa após o palpite
            convaiNPCManager.rayLength = 4.5f;
            matchingClues.Clear(); // Limpa as pistas após a seleção

            // Retorna a carta escolhida (esse valor pode ser passado de volta para a lógica de NPCMakeSuggestion)
            // Aqui, você pode continuar o processamento necessário para mostrar a carta ao NPC

            lastMatchedNPCPlayer.SeePlayerClue(chosenClue);
        }
        else
        {
            Debug.LogWarning("Nenhuma carta válida foi encontrada para o tipo selecionado: " + chosenClueType);
        }
    }

}
