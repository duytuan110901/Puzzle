using UnityEngine.Tilemaps;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board mainBoard { get; private set; }
    public Piece trackingPiece { get; private set; }
    public Vector3Int[] cells { get; set; }
    public Vector3Int position { get; set; }
    public void Initialized(Board mainBoard, Vector3Int position, Piece trackingPiece, Tile tile)
    {
        this.mainBoard = mainBoard;
        this.tile = tile;
        this.trackingPiece = trackingPiece;
        this.position = position;
        if (cells == null)
        {
            this.cells = new Vector3Int[trackingPiece.cells.Length];
        }
        for (int i = 0; i < trackingPiece.cells.Length; i++)
        {
            this.cells[i] = trackingPiece.cells[i];
        }
    }
   
    public void Rotate(int direction)
    {
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cell = this.cells[i]; ;
            int x, y;
            switch (trackingPiece.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // "I" and "O" are rotated from an offset center point
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }
            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }


}
