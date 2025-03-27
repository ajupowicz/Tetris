using System.Collections;
using UnityEngine;

public class BlockMovement : MonoBehaviour
{
    public GameBoard GameBoard;
    public float FallInterval = 0.3f;
    private bool isFalling = true;
    private Rigidbody2D rb;

    public static float SquareSize = 0.64f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("BlockMovement: No Rigidbody2D found!");
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            // Keep rotation frozen so that we manage rotation manually on the children.
            rb.freezeRotation = true;
        }
    }

    private void Start()
    {
        StartCoroutine(FallRoutine());
    }

    private IEnumerator FallRoutine()
    {
        while (isFalling)
        {
            yield return new WaitForSeconds(FallInterval);

            Vector2 nextPosition = rb.position + new Vector2(0, -SquareSize);

            if (!CanMove(nextPosition))
            {
                isFalling = false;

                if (GameBoard != null)
                {
                    GameBoard.RegisterBlock(transform);
                    GameBoard.CheckRows();

                }

                BlockSpawner spawner = FindObjectOfType<BlockSpawner>();
                if (spawner != null)
                {
                    spawner.OnBlockLanded(this);
                }

                PlayerController controller = FindObjectOfType<PlayerController>();
                if (controller != null)
                {
                    controller.ClearCurrentBlock();
                }

                yield break;
            }
            else
            {
                rb.MovePosition(nextPosition);
            }
        }
    }

    public void Move(Vector2 direction)
    {
        Vector2 newPosition = rb.position + direction;
        if (CanMove(newPosition))
        {
            rb.MovePosition(newPosition);
        }
    }

    public void Rotate90()
    {
        if (transform.childCount == 0)
            return;

        bool canRotate = true;

        foreach (Transform child in transform)
        {
            Vector2 oldLocal = child.localPosition;
            Vector2 newLocal = new Vector2(-oldLocal.y, oldLocal.x);
            Vector2 newWorldPos = (Vector2)transform.position + newLocal;

            if (GameBoard != null && newWorldPos.y < GameBoard.minY)
            {
                canRotate = false;
                break;
            }
            if (GameBoard != null && newWorldPos.y > GameBoard.maxY)
            {
                canRotate = false;
                break;
            }

            Vector2 boxSize = new Vector2(SquareSize, SquareSize) * 0.9f;
            int layerMask = LayerMask.GetMask("Blocks", "Default");
            Collider2D[] hits = Physics2D.OverlapBoxAll(newWorldPos, boxSize, 0, layerMask);
            foreach (var hit in hits)
            {
                if (hit.transform != transform && !hit.transform.IsChildOf(transform))
                {
                    canRotate = false;
                    break;
                }
            }

            if (!canRotate)
                break;
        }

        if (canRotate)
        {
            foreach (Transform child in transform)
            {
                Vector2 oldLocal = child.localPosition;
                child.localPosition = new Vector2(-oldLocal.y, oldLocal.x);
            }
        }
    }

    private bool CanMove(Vector2 targetPosition)
    {
        if (transform.childCount == 0)
        {
            if (GameBoard != null && targetPosition.y < GameBoard.minY)
            {
                return false;
            }
            if (GameBoard != null && (targetPosition.y - rb.position.y) > 0 && targetPosition.y > GameBoard.maxY)
            {
                return false;
            }

            Vector2 boxSize = new Vector2(SquareSize, SquareSize) * 0.9f;
            int layerMask = LayerMask.GetMask("Blocks", "Default");
            Collider2D[] hits = Physics2D.OverlapBoxAll(targetPosition, boxSize, 0, layerMask);

            foreach (var hit in hits)
            {
                if (hit.transform != transform && !hit.transform.IsChildOf(transform))
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            foreach (Transform child in transform)
            {
                Vector2 childLocalPos = child.localPosition;
                Vector2 childTargetPos = targetPosition + childLocalPos;

                if (GameBoard != null && childTargetPos.y < GameBoard.minY)
                {
                    return false;
                }
                Vector2 childCurrentPos = rb.position + childLocalPos;
                if (GameBoard != null && (childTargetPos.y - childCurrentPos.y) > 0 && childTargetPos.y > GameBoard.maxY)
                {
                    return false;
                }

                Vector2 boxSize = new Vector2(SquareSize, SquareSize) * 0.9f;
                int layerMask = LayerMask.GetMask("Blocks", "Default");
                Collider2D[] hits = Physics2D.OverlapBoxAll(childTargetPos, boxSize, 0, layerMask);

                foreach (var hit in hits)
                {
                    if (hit.transform != transform && hit.transform.parent != transform)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public bool IsFalling()
    {
        return isFalling;
    }

    public void DropDownFast(float distance)
    {
        int steps = Mathf.RoundToInt(distance / SquareSize);

        for (int i = 1; i <= steps; i++)
        {
            Vector2 stepTarget = rb.position + Vector2.down * (SquareSize * i);

            if (!CanMove(stepTarget))
            {
                rb.MovePosition(rb.position + Vector2.down * SquareSize * (i - 1));
                return;
            }
        }
        rb.MovePosition(rb.position + Vector2.down * distance);
    }

    public void SetGameBoard(GameBoard board)
    {
        GameBoard = board;
    }
}
