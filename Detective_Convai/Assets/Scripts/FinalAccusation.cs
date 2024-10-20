using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinalAccusation : MonoBehaviour
{
    public GameController gameController; // Referência ao GameController para acessar o envelope de crime
    public TMP_Dropdown  personDropdown; // Dropdown para escolher a pessoa
    public TMP_Dropdown  weaponDropdown; // Dropdown para escolher a arma
    public TMP_Dropdown  locationDropdown; // Dropdown para escolher o local
    public GameObject resultPanel; // Painel que exibe o resultado da acusação (vitória/derrota)
    public GameObject accusationPanel; // Painel que contém os Dropdowns de escolha
    public TMP_Text resultText; // Texto que exibe o resultado da acusação (dentro de Result Panel)

    public List<Clue> finalAccusation = new List<Clue>(); // Armazena as escolhas do jogador

    // Exibe o painel de acusação
    public void OpenAccusationPanel()
    {
        accusationPanel.SetActive(true);
    }

    // Função para exibir o painel de resultado
    public void ShowResultPanel(string result)
    {
        resultPanel.SetActive(true); // Ativa o painel
        resultText.text = $"Resultado: {result}"; // Substitui o texto placeholder com o resultado
    }

    // Chama ao confirmar a acusação
    public void ConfirmAccusation()
    {
        // Pega as escolhas do jogador
        Debug.Log(personDropdown.options[personDropdown.value].text);
        Debug.Log(weaponDropdown.options[weaponDropdown.value].text);
        Debug.Log(locationDropdown.options[locationDropdown.value].text);
        Clue chosenPerson = gameController.GetClueByName(personDropdown.options[personDropdown.value].text);
        Clue chosenWeapon = gameController.GetClueByName(weaponDropdown.options[weaponDropdown.value].text);
        Clue chosenLocation = gameController.GetClueByName(locationDropdown.options[locationDropdown.value].text);

        // Armazena as escolhas na lista
        finalAccusation.Clear();
        finalAccusation.Add(chosenPerson);
        finalAccusation.Add(chosenWeapon);
        finalAccusation.Add(chosenLocation);

        // Verifica se a acusação está correta
        if (gameController.IsAccusationCorrect(finalAccusation))
        {
            ShowResultPanel("Você venceu!");
        }
        else
        {
            ShowResultPanel("Acusação errada! Você perdeu.");
        }
        accusationPanel.SetActive(false); // Oculta o painel após a confirmação
    }
}
