using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tile : MonoBehaviour
{
    private Vector3 firstPosition;
    private Vector3 lastPosition;
    private float swipeAngle;
    private Vector3 tempPosition;

    [HideInInspector] public float xPosition;
    [HideInInspector] public float yPosition;
    [HideInInspector] public int col;
    [HideInInspector] public int row;
    [HideInInspector] public bool isMatched = false;

    private grid grid;
    private GameObject otherTile;
    private int previousCol;
    private int previousRow;

    private void Start() {
        grid = FindObjectOfType<grid>();
        xPosition = transform.position.x;
        yPosition = transform.position.y;
        col = Mathf.RoundToInt((xPosition - grid.startPosition.x)/ grid.offset.x);
        row = Mathf.RoundToInt((yPosition - grid.startPosition.y)/ grid.offset.y);
    }

    private void Update() {
        xPosition = (col * grid.offset.x) + grid.startPosition.x;
        yPosition = (row * grid.offset.y) + grid.startPosition.y;
        Swipe_Tile();
        Check_Matches();
        if(isMatched){
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = Color.grey;
        }
    }

    void OnMouseDown()
    {
        firstPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp() {
        lastPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Calculate_Angle();
    }

    void Calculate_Angle(){
        swipeAngle = Mathf.Atan2(lastPosition.y - firstPosition.y, lastPosition.x - firstPosition.x) * 180/Mathf.PI;
        Move_Tile();
    }

    void Swipe_Right_Move(){
        otherTile = grid.tiles[col + 1, row];
        otherTile.GetComponent<tile>().col -= 1;
        col += 1;
    }

    void Swipe_Left_Move(){
        otherTile = grid.tiles[col - 1, row];
        otherTile.GetComponent<tile>().col += 1;
        col -= 1;
    }    

    void Swipe_Up_Move(){
        otherTile = grid.tiles[col, row + 1];
        otherTile.GetComponent<tile>().row -= 1;
        row += 1;
    }

    void Swipe_Down_Move(){
        otherTile = grid.tiles[col, row - 1];
        otherTile.GetComponent<tile>().row += 1;
        row -= 1;
    }

    void Move_Tile(){
        previousCol = col;
        previousRow = row;
        if (swipeAngle > -45 && swipeAngle <= 45){
            // Debug.Log("Right swipe");
            Swipe_Right_Move();
        }else if(swipeAngle >45 && swipeAngle <= 135){
            // Debug.Log("Up swipe");
            Swipe_Up_Move();
        }else if (swipeAngle >135 || swipeAngle <= -135){
            // Debug.Log("Left swipe");
            Swipe_Left_Move();
        }else if (swipeAngle < -45 && swipeAngle >= -135){
            // Debug.Log("Down swipe");
            Swipe_Down_Move();
        }
    }

    void Swipe_Tile(){
        if(Mathf.Abs(xPosition - transform.position.x) > 0.1){
            tempPosition = new Vector2(xPosition, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.4f);
            StartCoroutine(Check_Move());
        }else{
            tempPosition = new Vector2(xPosition, transform.position.y);
            transform.position = tempPosition;
            grid.tiles[col, row] = this.gameObject;
        }

        if(Mathf.Abs(yPosition - transform.position.y) > 0.1){
            tempPosition = new Vector2(transform.position.x, yPosition);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.4f);
            StartCoroutine(Check_Move());
        }else{
            tempPosition = new Vector2(transform.position.x, yPosition);
            transform.position = tempPosition;
            grid.tiles[col, row] = this.gameObject;
        }   
    }

    void Check_Matches(){
        if(col > 0 && col < grid.gridSizeX -1){
            GameObject lefttile = grid.tiles[col - 1, row];
            GameObject righttile = grid.tiles[col + 1, row];
            if (lefttile && righttile){
                if (lefttile.CompareTag(gameObject.tag) && righttile.CompareTag(gameObject.tag)){
                    isMatched = true;
                    righttile.GetComponent<tile>().isMatched = true;
                    lefttile.GetComponent<tile>().isMatched = true;
                }
            }
        }
        if(row > 0 && row < grid.gridSizeY - 1){
            GameObject upTile = grid.tiles[col, row + 1];
            GameObject downTile = grid.tiles[col, row - 1];
            if (upTile && downTile){
                if (upTile.CompareTag(gameObject.tag) && downTile.CompareTag(gameObject.tag)){
                    isMatched = true;
                    downTile.GetComponent<tile>().isMatched = true;
                    upTile.GetComponent<tile>().isMatched = true;
                }
            }
        }
    }

    IEnumerator Check_Move(){
        yield return new WaitForSeconds(0.5f);
        if (otherTile != null)
        {
            if (!isMatched && !otherTile.GetComponent<tile>().isMatched)
            {
                if (gameObject.CompareTag("Special"))
                {
                    // Tugas Bonus : Special akan menghancurkan semua objek yang memiliki tag sama dengan otherTile, 
                    // lalu tile yang hancur akan diisi lagi dengan method Decrease_Row()
                    StartCoroutine(Destroy_Tile_With_Tag(otherTile.tag));
                }

                else
                {
                    otherTile.GetComponent<tile>().row = row;
                    otherTile.GetComponent<tile>().col = col;
                    row = previousRow;
                    col = previousCol;

                    grid.gameManager.scoreMultiple = 1;                    
                }
            }
            else
            {
                grid.Destroy_Matches();
            }
        }
        otherTile = null;
    }

    IEnumerator Destroy_Tile_With_Tag(string tag){
        yield return new WaitForSeconds(0.5f);
        GameObject [] willDestroyeds = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject willDestroyed in willDestroyeds)
        {
            grid.gameManager.Get_Score(1);
            Destroy(willDestroyed);
        }
        StartCoroutine(grid.Decrease_Row());
    }
}
