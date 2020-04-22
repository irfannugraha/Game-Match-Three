using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class grid : MonoBehaviour
{
    public int gridSizeX, gridSizeY;
    public GameObject tilePrefab;
    [HideInInspector] public GameObject[,] tiles;
    public GameObject[] candies;

    [HideInInspector] public Vector2 offset;
    [HideInInspector] public Vector2 startPosition;
    public GameManager gameManager;

    void Start()
    {
        Create_Grid();
    }

    void Create_Grid(){
        offset = tilePrefab.GetComponent<SpriteRenderer>().bounds.size;
        startPosition = transform.position + (Vector3.left * (offset.x * (gridSizeX / 2))) + (Vector3.down * (offset.y * (gridSizeY / 3)));
        tiles = new GameObject[gridSizeX, gridSizeY];

        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                Vector2 position = new Vector3(startPosition.x + (i * offset.x), startPosition.y + (j * offset.y));
                GameObject backgroundTile = Instantiate(tilePrefab, position, tilePrefab.transform.rotation);
                backgroundTile.transform.parent = transform;
                backgroundTile.name = "(" + i + "," + j + ")";

                int index = Random.Range(0, candies.Length);
                while (Matches_At(i ,j , candies[index]) )
                {
                    index = Random.Range(0, candies.Length);
                }

                GameObject candy = ObjectPooler.Instance.Spawn_From_Pool(index.ToString(), position, Quaternion.identity);
                candy.name = "(" + i + "," + j + ")";
                tiles[i,j] = candy;
            }
        }
    }

    bool Matches_At(int col, int row, GameObject piece){
        if(col > 1 && row > 1){
            if (tiles[col - 1, row].CompareTag(piece.tag) && tiles[col - 2, row].CompareTag(piece.tag))
            {
                return true;
            }
            if (tiles[col, row -1].CompareTag(piece.tag) && tiles[col, row-2])
            {
                return true;
            }
        }else if (col <= 1 || row <= 1)
        {
            if (col > 1)
            {
                if (tiles[col-1, row].CompareTag(piece.tag) && tiles[col-2, row].CompareTag(piece.tag))
                {
                    return true;
                }
            }
            if (row > 1)
            {
                if (tiles[col, row-1].CompareTag(piece.tag) && tiles[col, row-2].CompareTag(piece.tag))
                {
                    return true;
                }                
            }
        }
        return false;
    }

    void Destroy_Matches_At(int col, int row){
        if (tiles[col, row].GetComponent<tile>().isMatched){
            GameObject gm = tiles[col,row];
            gm.SetActive(false);
            tiles[col, row] = null;
        }
    }

    public void Destroy_Matches(){
        // Tugas 1 memanggil Get_Score untuk menambahkan score sebanyak 1
        gameManager.Get_Score(1);
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                if (tiles[i, j])
                {   
                    Destroy_Matches_At(i,j);
                }
            }
        }
        StartCoroutine(Decrease_Row());
    }

    void Refil_Board(){
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                if (!tiles[i,j])
                {
                    Vector2 tempPosition = new Vector3(startPosition.x + (i * offset.x), startPosition.y + (j * offset.y));
                    int candyToUse = Random.Range(0, candies.Length);
                    GameObject tileToRefill = ObjectPooler.Instance.Spawn_From_Pool(candyToUse.ToString(), tempPosition, Quaternion.identity);
                    tiles[i,j] = tileToRefill;
                }
            }
        }
    }

    bool Matches_On_Board(){
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                if (tiles[i, j])
                {
                    if (tiles[i, j].GetComponent<tile>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public IEnumerator Decrease_Row(){
        int nullCount = 0;
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                if (!tiles[i, j])
                {
                    nullCount++;
                }else if(nullCount > 0)
                {
                    tiles[i, j].GetComponent<tile>().row -= nullCount;
                    tiles[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Fill_Board());
    }

    IEnumerator Fill_Board(){
        Refil_Board();
        yield return new WaitForSeconds(0.5f);
        while (Matches_On_Board()){
            // Tugas 2 scoreMultiple ditambah 1 setiap ada yang match
            gameManager.scoreMultiple += 1;
            Destroy_Matches();
        }
    }

}