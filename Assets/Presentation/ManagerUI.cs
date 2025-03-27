using UnityEngine;
using TMPro;

public class ManagerUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject player1NewNameContainer;
    public GameObject player2NewNameContainer;

    [Header("Input")]
    public TMP_InputField inputFieldP1;
    public TMP_Text player1Name;

    public TMP_InputField inputFieldP2;
    public TMP_Text player2Name;

    [Header("Block Spawner")]
    public BlockSpawner blockSpawner;

    private bool player1Confirmed = false;
    private bool player2Confirmed = false;

    public void SetBlockSpawner(BlockSpawner spawner)
    {
        blockSpawner = spawner;
    }

    public void OnConfirmButtonClickedPlayer1()
    {
        if (inputFieldP1 != null && player1Name != null)
        {
            string enteredName = inputFieldP1.text;
            player1Name.text = enteredName;
        }

        if (player1NewNameContainer != null)
        {
            player1NewNameContainer.SetActive(false);
        }

        player1Confirmed = true;
        TryStartGame();
    }

    public void OnConfirmButtonClickedPlayer2()
    {
        if (inputFieldP2 != null && player2Name != null)
        {
            string enteredName = inputFieldP2.text;
            player2Name.text = enteredName;
        }

        if (player2NewNameContainer != null)
        {
            player2NewNameContainer.SetActive(false);
        }

        player2Confirmed = true;
        TryStartGame();
    }

    private void TryStartGame()
    {
        if (player1Confirmed && player2Confirmed && blockSpawner != null)
        {
            blockSpawner.StartGame();
        }
    }
}
