using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Board : MonoBehaviour
{
    public TetrominoData[] tetrominos;
    public Tilemap tilemap { get; private set; }
    private Vector3Int spawPiecePosition = new Vector3Int(-1, 6, 0);
    private Vector3Int[] spawListGhostPosition = new Vector3Int[512];
    private Vector2Int boardSize = new Vector2Int(8, 16);
    public Piece activePiece { get; private set; }
    public Ghost activeGhost { get; private set; }
    private Queue<int> lists = new Queue<int>();
    private int[] countContact = new int[512];
    private Queue<int> contacts = new Queue<int>();
    private int[] status = {0, 1, 2, 3};
    public int getPosition { get; private set; }
    private bool isRun = false;
    public RectInt Bounds
    {
        get
        {
            Vector2Int position =  new Vector2Int(-boardSize.x/2,-boardSize.y/2);
            return new RectInt(position, boardSize);
        }
    }
    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();
        this.activeGhost = GetComponentInChildren<Ghost>();

        for (int i = 0; i < this.tetrominos.Length; i++)
        {
            this.tetrominos[i].Initialize();
        }
    }
    private void Start()
    {
        DemToPosition(spawListGhostPosition);
        SpawnPiece(spawPiecePosition, 0);
        SpawnListGhost(activePiece, countContact,contacts, status);
        SpawnGhost();
        
    }
    IEnumerator MyCoroutine()
    {
        isRun = true;
        yield return new WaitForSeconds(3f);
        isRun = false;
        ClickSelect();
    }
    public void Update()
    {
        
       
    }

    public void ClickSelect()
    {
        ClearPiece(activePiece);
        ClearGhost(activeGhost);
        SpawnPiece(spawListGhostPosition[getPosition], getPosition % 4);
        ClearLines();
        lists.Dequeue();
        SpawnPiece(spawPiecePosition, 0);
        contacts.Clear();
        SpawnListGhost(this.activePiece, countContact,contacts, status);
        SpawnGhost();
        if(isRun)
        {
            StopAllCoroutines();
        }
        StartCoroutine(MyCoroutine());
        
    }
    public void ClickCycle()
    {
        if(activePiece.data.tetromino == Tetromino.O)
        {
            for(int i = 0;i<4; i++)
            {
                ClearGhost(activeGhost);
                SpawnGhost();
            }
        }
        ClearGhost(activeGhost);
        SpawnGhost();
    }
    public void SpawnPiece(Vector3Int position,int status)
    {
        while(lists.Count < 4)
        {
            int random = Random.Range(0, this.tetrominos.Length);
            lists.Enqueue(random);

        }
        TetrominoData data = tetrominos[lists.Peek()];
        this.activePiece.Initialized(this, position, data);
        if (IsValidPosition(activePiece, spawPiecePosition, 0))
        {
            SetPiece(this.activePiece, status);
        }
        else
        {
            GameOver();
            SceneManager.LoadScene(2);
        }
        
    }
    public void GameOver()
    {
        tilemap.ClearAllTiles();

    }

    public void SpawnGhost()
        {
            
            int dem = contacts.Dequeue();
            contacts.Enqueue(dem);
            getPosition = dem;
            this.activeGhost.Initialized(this, spawListGhostPosition[dem], activePiece, activeGhost.tile);
            SetGhost(activeGhost, dem%4);
        }
        
    public void DemToPosition(Vector3Int[] spawListGhostPosition)
    {
        int dem = 0;
        for (int i = Bounds.xMin; i < Bounds.xMax; i++)
        {
            for (int j = Bounds.yMin; j < Bounds.yMax - 2; j++)
            {

                for (int k = 0; k < status.Length; k++)
                {

                    Vector3Int spawPosition = new Vector3Int(i, j, 0);
                    spawListGhostPosition[dem] = spawPosition;
                    dem++;

                }

            }
        }
    }
    public void SpawnListGhost(Piece piece, int[] countContact, Queue<int> contact, int[] status)
    {
        int dem = -1;
        for (int i = Bounds.xMin; i < Bounds.xMax; i++)
        {
            for(int j = Bounds.yMin; j < Bounds.yMax - 2; j++)
            {
                
                for(int k=0; k<status.Length; k++)
                {
                    dem++;
                    countContact[dem] = -1;
                    Vector3Int spawPosition = new Vector3Int(i, j, 0);
                    Vector3Int checkGround = new Vector3Int(i, j - 1, 0);

                    if (IsValidPosition(piece, spawPosition, k) && !IsValidPosition(piece, checkGround, k))
                    {
                        countContact[dem] = CountContactPiece(piece, spawPosition, k);
                    }
                    
                    
                }
                
            }
        }
        SortArrayToQueue(countContact, contact);
    }
    public int CountContactPiece(Piece piece, Vector3Int position, int status)
    {
        int dem = 0;
        Ghost ghost1 = GetComponentInChildren<Ghost>();
        ghost1.Initialized(this, position, piece, ghost1.tile);
        
        if (status > 0)
        {
            for (int i = 0; i < status; i++)
            {
                ghost1.Rotate(1);
            }
        }

        for (int i = 0; i < ghost1.cells.Length; i++)
            {
                Vector3Int vt = new Vector3Int(1,0,0);
                Vector3Int tilePosition = ghost1.cells[i] + ghost1.position + vt;
                if (!Bounds.Contains((Vector2Int)tilePosition))
                {
                    dem++;
                continue;
            }
                if (this.tilemap.HasTile(tilePosition))
                {
                    dem++;
                }
            }
            for (int i = 0; i < ghost1.cells.Length; i++)
            {
            Vector3Int vt = new Vector3Int(-1, 0, 0);
            Vector3Int tilePosition = ghost1.cells[i] + ghost1.position + vt;
            if (!Bounds.Contains((Vector2Int)tilePosition))
                {
                    dem++;
                continue;
                }
                if (this.tilemap.HasTile(tilePosition))
                {
                    dem++;
                }
            }
        

        for (int i = 0; i < ghost1.cells.Length; i++)
            {
            Vector3Int vt = new Vector3Int(0, -1, 0);
            Vector3Int tilePosition = ghost1.cells[i] + ghost1.position + vt;
            if (!Bounds.Contains((Vector2Int)tilePosition))
                {
                    dem++;
                continue;
            }
                if (this.tilemap.HasTile(tilePosition))
                {
                    dem++;
                }
            }


        return dem;
        
    }

    public bool IsValidPosition(Piece piece, Vector3Int position, int status)
    {
        Ghost ghost1 = GetComponentInChildren<Ghost>();
        ghost1.Initialized(this, position, piece, ghost1.tile);

        
        if(status > 0)
        {
            for(int i = 0; i<status; i++)
            {
                ghost1.Rotate(1);
            }
        }
        for (int i = 0; i < ghost1.cells.Length; i++)
        {
            Vector3Int tilePosition = ghost1.cells[i] +ghost1.position;
            if (!Bounds.Contains((Vector2Int)tilePosition))
            {
                
                return false;
            }
            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }
        
        return true;
    }

    public void SetPiece(Piece piece, int status)
    {
        if (status > 0)
        {
            for (int i = 0; i < status; i++)
            {
                piece.Rotate(1);
            }
        }
       
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    public void ClearPiece(Piece piece)
    {
       
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }

    }
    public void SetGhost(Ghost ghost,int status)
    {

        if (status > 0)
        {
            for (int i = 0; i < status; i++)
            {
                ghost.Rotate(1);
            }
        }

        for (int i = 0; i < ghost.cells.Length; i++)
        {
            Vector3Int tilePosition = ghost.cells[i] + ghost.position;
            this.tilemap.SetTile(tilePosition, ghost.tile);
        }
    }
    public void ClearGhost(Ghost ghost)
    {
        
        for (int i = 0; i < ghost.cells.Length; i++)
        {
            Vector3Int tilePosition = ghost.cells[i] + ghost.position;
            this.tilemap.SetTile(tilePosition, null);
        }

    }
    public void SortArrayToQueue(int[]countContact, Queue<int> contact)
    {

        for(int i=0;i<10;i++)
        {
            int max = 0;
            int position = 0;
            for(int j =0;j< countContact.Length; j++)
            {
                if (countContact[j] > max)
                {
                    max = countContact[j];
                    position = j;
                }
            }
            contact.Enqueue(position);
            countContact[position] = 0;
        }
    }


    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {

            if (IsLineFull(row))
            {
                LineClear(row);
            }
            else
            {
                row++;
            }
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position) || tilemap.GetTile(position) == activeGhost.tile)
            {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }

   
}
