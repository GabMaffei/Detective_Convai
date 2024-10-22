using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Convai.Scripts.Runtime.Core;

public class TurnController : MonoBehaviour
{
    public ConvaiNPCManager convaiNPCManager; // Gerencia o chat
    public List<NPCAI> npcs; // Lista de NPCs que jogam
    public int currentTurnIndex = 0; // O índice do jogador atual (0 será o jogador humano)
    public bool isPlayerTurn = true; // Define se é o turno do jogador

    public GameObject playerSuggestionResultPanel; // UI para indicar o turno do jogador
    public GameObject turnResultPanel; // UI para indicar o turno dos NPCs

    void Start()
    {
        StartPlayerTurn(); // Começar com o turno do jogador
    }

    // Função para iniciar o turno do jogador
    public void StartPlayerTurn()
    {
        isPlayerTurn = true;
        turnResultPanel.SetActive(false);
        convaiNPCManager.rayLength = 4.5f;
        //playerTurnPanel.SetActive(true); // Mostra a UI de turno do jogador
        //endTurnButton.SetActive(false); // Botão de fim de turno invisível até palpite
    }

    // Função para finalizar o turno do jogador e passar para o NPC
    public void EndPlayerTurn()
    {
        isPlayerTurn = false;
        //playerTurnPanel.SetActive(false);
        StartNPCTurn(); // Chama o turno do primeiro NPC
    }

    // Função para iniciar o turno do próximo NPC
    public void StartNPCTurn()
    {
        if (currentTurnIndex < npcs.Count)
        {
            NPCAI currentNPC = npcs[currentTurnIndex];
            //npcTurnPanel.SetActive(true); // UI indicando o turno do NPC
            PlayNPCTurn(currentNPC); // Inicia o turno do NPC
        }
        else
        {
            // Volta para o jogador
            currentTurnIndex = 0;
            StartPlayerTurn();
        }
    }

    // Função que simula o turno do NPC
    void PlayNPCTurn(NPCAI npc)
    {
        // Chama a função para o NPC fazer um palpite (ou outra ação no turno dele)
        npc.PlayTurn();
    }

    // Função para finalizar o turno do NPC e passar para o próximo
    public void EndNPCTurn()
    {
        //npcTurnPanel.SetActive(false); // Oculta a UI do turno do NPC
        currentTurnIndex++; // Avança para o próximo NPC

        if (currentTurnIndex < npcs.Count)
        {
            // Chama o próximo NPC
            StartNPCTurn();
        }
        else
        {
            // Todos os NPCs jogaram, volta para o jogador
            currentTurnIndex = 0;
            StartPlayerTurn();
        }
    }

    // Função chamada pelo botão "Continuar" para avançar o turno após o jogador terminar
    public void OnNextTurnButtonPressed()
    {
        if (isPlayerTurn)
        {
            playerSuggestionResultPanel.SetActive(false);
            EndPlayerTurn(); // Finaliza o turno do jogador e passa para o NPC
        }
        else
        {
            EndNPCTurn(); // Finaliza o turno do NPC e passa para o próximo
        }
    }
}
