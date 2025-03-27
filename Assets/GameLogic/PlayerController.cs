using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private BlockMovement blockMovement;
    
    [Header("Fast Falling")]
    public float fastDropDistance = 4f;
    private string playerTag;
    private float minX, maxX;

    private void Start()
    {
        playerTag = gameObject.CompareTag("Player1") ? "Player1" : "Player2";

        if (playerTag == "Player1")
        {
            minX = -7.84f;
            maxX = -1.44f;
        }
        else
        {
            minX = 1.44f;
            maxX = 7.84f;
        }

        AssignCurrentBlock();
    }

    private void Update()
    {
        if (Keyboard.current == null || blockMovement == null) return;

        float moveDirection = 0f;

        if (playerTag == "Player1")
        {
            if (Keyboard.current.aKey.wasPressedThisFrame)
                moveDirection = -BlockMovement.SquareSize;
            if (Keyboard.current.dKey.wasPressedThisFrame)
                moveDirection = BlockMovement.SquareSize;
        }
        else if (playerTag == "Player2")
        {
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
                moveDirection = -BlockMovement.SquareSize;
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
                moveDirection = BlockMovement.SquareSize;
        }
        if (playerTag == "Player1" && Keyboard.current.sKey.wasPressedThisFrame)
        {
            blockMovement?.DropDownFast(fastDropDistance * BlockMovement.SquareSize);
        }
        else if (playerTag == "Player2" && Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            blockMovement?.DropDownFast(fastDropDistance * BlockMovement.SquareSize);
        }


        if (moveDirection != 0)
        {
            float minBlockX = float.MaxValue;
            float maxBlockX = float.MinValue;
            foreach (Transform child in blockMovement.transform)
            {
                float childX = child.position.x;
                if (childX < minBlockX)
                    minBlockX = childX;
                if (childX > maxBlockX)
                    maxBlockX = childX;
            }

            float newMinBlockX = minBlockX + moveDirection;
            float newMaxBlockX = maxBlockX + moveDirection;

            if (newMinBlockX >= minX && newMaxBlockX <= maxX)
            {
                blockMovement.Move(new Vector2(moveDirection, 0));
            }
        }


        if (playerTag == "Player1" && Keyboard.current.wKey.wasPressedThisFrame)
        {
            blockMovement.Rotate90();
        }
        else if (playerTag == "Player2" && Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            blockMovement.Rotate90();
        }
    }

    public void AssignCurrentBlock()
    {
        BlockMovement[] blocks = FindObjectsOfType<BlockMovement>();
        if (blocks.Length == 0) return;

        foreach (var block in blocks)
        {
            bool belongsToPlayer1 = (playerTag == "Player1" && block.transform.position.x < 0);
            bool belongsToPlayer2 = (playerTag == "Player2" && block.transform.position.x >= 0);

            if ((belongsToPlayer1 || belongsToPlayer2) && block.IsFalling())
            {
                blockMovement = block;
                Debug.Log($"{playerTag} assigned block {block.name} at {block.transform.position}");
                return;
            }
        }
    }

    public void ClearCurrentBlock()
    {
        blockMovement = null;
    }
}
