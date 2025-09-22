using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    public override bool MakeMove(GameBoard board)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("GameBoard"))
                {
                    int column = board.GetColumnFromHitPoint(hit.point);

                    if (board.IsValidMove(column))
                    {
                        board.MakeMove(column, ID);
                        return true;
                    }
                    else
                        Debug.Log("Column full");
                        
                }
            }
        }

        return false;
    }
}
