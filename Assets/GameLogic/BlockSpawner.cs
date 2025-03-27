using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> blocks;

    [SerializeField] private Transform player1SpawnArea;
    [SerializeField] private Transform player2SpawnArea;

    [SerializeField] private GameObject player1BoardObject;
    [SerializeField] private GameObject player2BoardObject;

    private GameBoard player1Board;
    private GameBoard player2Board;

    private void Start()
    {
        player1Board = player1BoardObject?.GetComponent<GameBoard>();
        player2Board = player2BoardObject?.GetComponent<GameBoard>();

        if (player1Board == null || player2Board == null)
        {
            Debug.LogError("BlockSpawner: Missing GameBoard reference!");
        }

    }

    public void StartGame()
    {
        SpawnBlockForPlayer("Player1");
        SpawnBlockForPlayer("Player2");
    }

    public void OnBlockLanded(BlockMovement block)
    {
        string blockTag = block.gameObject.tag; // "Player1" or "Player2"
        GameBoard board = (blockTag == "Player1") ? player1Board : player2Board;

        if (board != null && board.IsGameOver())
        {
            Debug.Log("GAME OVER for " + blockTag);
            return;
        }

        StartCoroutine(SpawnNextBlockAfterDelay(blockTag, 0.3f));
    }

    private IEnumerator SpawnNextBlockAfterDelay(string playerTag, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnBlockForPlayer(playerTag);
    }

    private void SpawnBlockForPlayer(string playerTag)
    {
        if (blocks == null || blocks.Count == 0)
        {
            Debug.LogError("BlockSpawner: No available blocks to spawn!");
            return;
        }

        GameObject prefab = blocks[Random.Range(0, blocks.Count)];
        GameObject block = Instantiate(prefab);
        BlockMovement movement = block.GetComponent<BlockMovement>();

        if (playerTag == "Player1")
        {
            if (player1Board != null)
            {
                float xPos = player1Board.offsetX + (player1Board.width * BlockMovement.SquareSize) / 2;
                float yPos = player1SpawnArea.position.y;

                block.transform.position = new Vector3(xPos, yPos, 0);
                block.tag = "Player1";

                movement?.SetGameBoard(player1Board);
            }
        }
        else
        {
            if (player2Board != null)
            {
                float xPos = player2Board.offsetX + (player2Board.width * BlockMovement.SquareSize) / 2;
                float yPos = player2SpawnArea.position.y;

                block.transform.position = new Vector3(xPos, yPos, 0);
                block.tag = "Player2";

                movement?.SetGameBoard(player2Board);
            }
        }

        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (var player in players)
        {
            player.AssignCurrentBlock();
        }
    }
}
