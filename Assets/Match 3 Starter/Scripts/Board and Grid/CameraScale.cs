using UnityEngine;

public class CameraScale : MonoBehaviour
{
    BoardManager board;
    [SerializeField]
    float cameraOffset;
    [SerializeField]
    float aspectRatio;
    [SerializeField]
    float padding;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<BoardManager>();
        if (board != null)
        {
            RepositionCamera(board.xSize - 1, board.ySize - 1);
        }

    }

    void RepositionCamera(float x, float y)
    {
        Vector3 tempPosition = new Vector3(x / 2, y / 2, cameraOffset);
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
