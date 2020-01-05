using UnityEngine;

public class CameraScale : MonoBehaviour
{
    private BoardManager board;
    [SerializeField]
    private float cameraOffset;
    [SerializeField]
    private float aspectRatio;
    [SerializeField]
    private float padding;
    [SerializeField]
    private float yOffset = 1;

    // Start is called before the first frame update
    private void Start()
    {
        board = FindObjectOfType<BoardManager>();
        if (board != null)
        {
            RepositionCamera(board.xSize - 1, board.ySize - 1);
        }

    }

    private void RepositionCamera(float x, float y)
    {
        Vector3 tempPosition = new Vector3(x / 2, y / 2 + yOffset, cameraOffset);
        transform.position = tempPosition;
        if (board.xSize >= board.ySize)
        {
            Camera.main.orthographicSize = (board.xSize / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = board.ySize / 2 + padding;
        }
    }

}
