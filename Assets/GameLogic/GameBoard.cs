using UnityEngine;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
    public string playerTag;
    public event System.Action<string> OnGameOverTriggered;
    public event System.Action<string, int> OnScoreEarned; // string = playerTag, int = points


    public int width = 10;
    public int height = 20;

    public float minY = -5f;

    public float maxY = 15f;

    public float offsetX = 0f;

    private List<Transform> blocks = new List<Transform>();

    private bool gameOverAlreadyTriggered = false;

    private void Update()
    {
        if (!gameOverAlreadyTriggered && IsGameOver())
        {
            gameOverAlreadyTriggered = true;
            Debug.Log($"{playerTag} lost!");
            OnGameOverTriggered?.Invoke(playerTag);
        }
    }

    public void RegisterBlock(Transform block)
    {
        if (block.childCount == 0)
        {
            if (!blocks.Contains(block))
            {
                blocks.Add(block);
            }
        }
        else
        {
            foreach (Transform child in block)
            {
                if (!blocks.Contains(child))
                {
                    blocks.Add(child);
                }
            }
        }
    }

    public void CheckRows()
    {
        List<int> fullRows = new List<int>();

        for (int y = 0; y < height; y++)
        {
            if (IsRowFull(y))
            {
                fullRows.Add(y);
            }
        }

       
        fullRows.Sort();       
        fullRows.Reverse();    
        int rowsToClear = Mathf.Min(fullRows.Count, 4);
        fullRows = fullRows.GetRange(0, rowsToClear);


        foreach (int y in fullRows)
        {
            ClearRow(y);
        }

        for (int i = 0; i < fullRows.Count; i++)
        {
            int clearedRow = fullRows[i];

            foreach (Transform block in blocks)
            {
                int blockGridX = Mathf.RoundToInt((block.position.x - offsetX) / BlockMovement.SquareSize);
                int blockGridY = Mathf.RoundToInt(block.position.y / BlockMovement.SquareSize);

                if (blockGridY > clearedRow && blockGridX >= 0 && blockGridX < width)
                {
                    block.position += Vector3.down * BlockMovement.SquareSize;
                }
            }
        }

        if (rowsToClear > 0)
        {
            int totalPoints = rowsToClear * 100;
            OnScoreEarned?.Invoke(playerTag, totalPoints);
        }
    }


    private bool IsRowFull(int gridY)
    {
        bool[] occupied = new bool[width];

        foreach (Transform block in blocks)
        {
            int blockGridX = Mathf.RoundToInt((block.position.x - offsetX) / BlockMovement.SquareSize);
            int blockGridY = Mathf.RoundToInt(block.position.y / BlockMovement.SquareSize);

            if (blockGridY == gridY && blockGridX >= 0 && blockGridX < width)
            {
                occupied[blockGridX] = true;
            }
        }

        for (int x = 0; x < width; x++)
        {
            if (!occupied[x])
                return false;
        }
        return true;
    }

    private void ClearRow(int gridY)
    {
        List<Transform> rowBlocks = new List<Transform>();

        foreach (Transform block in blocks)
        {
            int blockGridX = Mathf.RoundToInt((block.position.x - offsetX) / BlockMovement.SquareSize);
            int blockGridY = Mathf.RoundToInt(block.position.y / BlockMovement.SquareSize);

            if (blockGridY == gridY && blockGridX >= 0 && blockGridX < width)
            {
                rowBlocks.Add(block);
            }
        }

        foreach (Transform b in rowBlocks)
        {
            blocks.Remove(b);
            Destroy(b.gameObject);
        }

    }

    private void MoveRowsDown(int clearedRow)
    {
        foreach (Transform block in blocks)
        {
            int blockGridX = Mathf.RoundToInt((block.position.x - offsetX) / BlockMovement.SquareSize);
            int blockGridY = Mathf.RoundToInt(block.position.y / BlockMovement.SquareSize);

            if (blockGridY > clearedRow && blockGridX >= 0 && blockGridX < width)
            {
                block.position += Vector3.down * BlockMovement.SquareSize;
            }
        }
    }

    public bool IsGameOver()
    {
        foreach (Transform t in blocks)
        {
            if (t.position.y > maxY)
            {

                return true;
            }
        }
        return false;
    }
}
