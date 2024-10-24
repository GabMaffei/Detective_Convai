using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Convai.Scripts.Runtime.Core;
using TMPro; // Certifique-se de incluir esta linha para o TextMeshPro

public class InterrogationController : MonoBehaviour
{
    public List<CinemachineVirtualCamera> characterCameras; // Lista de todas as VCams
    public List<Transform> characterPositions; // Posições dos personagens (NPCs)
    // public List<ConvaiNPC> convaiNPCs; // Lista de NPCs correspondentes
    public Transform player; // O jogador a ser movido
    public float distanceOffset = 2.0f; // Distância segura para evitar colisão
    public GameObject npcContainer; // Referência ao GameObject que contém os NPCs
    public TextMeshProUGUI characterNameText; // Referência ao TextMeshPro para o nome do NPC

    private int currentIndex = 0; // Índice do personagem atual

    void Start()
    {
        // Ativa apenas a câmera do primeiro personagem inicialmente
        SetActiveCamera(currentIndex);
    }

    // Função para navegar pelos personagens e sincronizar o inventário de NPCs
    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    [ContextMenu("Next Character")]
    public void NextCharacter()
    {
        currentIndex = (currentIndex + 1) % characterCameras.Count;
        SetActiveCamera(currentIndex);
    }

    [ContextMenu("Previous Character")]
    public void PreviousCharacter()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = characterCameras.Count - 1;
        }
        SetActiveCamera(currentIndex);
    }

    public void SetNPCByIndex(int index)
    {
        if (index >= 0 && index < characterCameras.Count)
        {
            currentIndex = index; // Atualiza o índice do NPC
            SetActiveCamera(currentIndex); // Sincroniza a câmera e a posição do jogador com o NPC atual
        }
    }


    void SetActiveCamera(int index)
    {
        // Desativa todas as câmeras
        foreach (var cam in characterCameras)
        {
            cam.gameObject.SetActive(false);
        }

        // Ativa a câmera do personagem selecionado
        characterCameras[index].gameObject.SetActive(true);

        // Move o jogador para uma posição ajustada próxima ao NPC, sem colidir com ele
        Vector3 directionToNPC = (characterPositions[index].position - player.position).normalized; // Direção do jogador até o NPC
        Vector3 adjustedPosition = characterPositions[index].position - directionToNPC * distanceOffset; // Aplica o offset

        player.position = adjustedPosition; // Move o jogador para a posição ajustada

        // Atualiza o nome do NPC na interface
        UpdateCharacterName(index);
    }

    // Função para atualizar o nome do personagem no TextMeshPro
    void UpdateCharacterName(int index)
    {
        Transform npcTransform = npcContainer.transform.GetChild(index);
        ConvaiNPC npc = npcTransform.GetComponent<ConvaiNPC>();

        if (npc != null && characterNameText != null)
        {
            characterNameText.text = $"Interrogando: {npc.characterName}"; // Aqui você pode personalizar a mensagem
        }
    }
}
