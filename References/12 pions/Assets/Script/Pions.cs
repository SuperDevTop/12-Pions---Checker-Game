using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pions : MonoBehaviour
{
    public bool isRed;
    public bool isBlack;
    public bool isKing;
    public bool isForceToMove(Pions[,] board, int x, int y)
    {
        if (isRed || isKing)
        {
            // right
            if (x <= 2)
            {
                Pions p = board[x + 1, y];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isRed != isRed)
                {
                    // check if it's possible to land after the jump
                    if (board[x + 2, y] == null)
                        return true;
                }
            }

            // left
            if (x >= 2)
            {
                Pions p = board[x - 1, y];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isRed != isRed)
                {
                    // check if it's possible to land after the jump
                    if (board[x - 2, y] == null)
                        return true;
                }
            }

            // top 
            if (y <= 2)
            {
                Pions p = board[x, y + 1];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isRed != isRed)
                {
                    // check if it's possible to land after the jump
                    if (board[x, y + 2] == null)
                        return true;
                }
            }
        }
        if (isBlack || isKing)
        {
            // right
            if (x <= 2)
            {
                Pions p = board[x + 1, y];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isBlack != isBlack)
                {
                    // check if it's possible to land after the jump
                    if (board[x + 2, y] == null)
                        return true;
                }
            }

            // left
            if (x >= 2)
            {
                Pions p = board[x - 1, y];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isBlack != isBlack)
                {
                    // check if it's possible to land after the jump
                    if (board[x - 2, y] == null)
                        return true;
                }
            }

            // Bot 
            if (y >= 2)
            {
                Pions p = board[x, y - 1];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isBlack != isBlack)
                {
                    // check if it's possible to land after the jump
                    if (board[x, y - 2] == null)
                        return true;
                }
            }
        }
        if (isRed && isKing)
        {
            // right
            if (x == 1)
            {
                Pions p = board[x + 1, y];
                Pions q = board[x + 2, y];
                Pions a = board[x + 3, y];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isRed != isRed && q == null && a != null || p != null && p.isRed != isRed && q == null && a == null || p == null && q != null && q.isRed != isRed && a == null)
                    return true;
            }
            if (x == 0)
            {
                Pions p = board[x + 1, y];
                Pions q = board[x + 2, y];
                Pions a = board[x + 3, y];
                Pions b = board[x + 4, y];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isRed != isRed && q == null && a != null && b != null || p != null && p.isRed != isRed && q == null && a == null && b != null || p != null && p.isRed != isRed && q == null && a == null && b == null || p == null && q != null && q.isRed != isRed && a == null && b != null || p == null && q != null && q.isRed != isRed && a == null && b == null || p == null && q == null && a != null && a.isRed != isRed && b == null)
                    return true;
            }
            if (x == 2)
            {
                Pions p = board[x + 1, y];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isRed != isRed)
                {
                    // check if it's possible to land after the jump
                    if (board[x + 2, y] == null)
                        return true;
                }
                Pions q = board[x - 1, y];
                // if there is a pions, and it is not the same color as ours
                if (q != null && q.isRed != isRed)
                {
                    // check if it's possible to land after the jump
                    if (board[x - 2, y] == null)
                        return true;
                }
            }

            // left
            if (x == 3)
            {
                Pions p = board[x - 1, y];
                Pions q = board[x - 2, y];
                Pions a = board[x - 3, y];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isRed != isRed && q == null && a != null || p != null && p.isRed != isRed && q == null && a == null || p == null && q != null && q.isRed != isRed && a == null)
                    return true;
            }
            if (x == 4)
            {
                Pions p = board[x - 1, y];
                Pions q = board[x - 2, y];
                Pions a = board[x - 3, y];
                Pions b = board[x - 4, y];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isRed != isRed && q == null && a != null && b != null || p != null && p.isRed != isRed && q == null && a == null && b != null || p != null && p.isRed != isRed && q == null && a == null && b == null || p == null && q != null && q.isRed != isRed && a == null && b != null || p == null && q != null && q.isRed != isRed && a == null && b == null || p == null && q == null && a != null && a.isRed != isRed && b == null)
                    return true;
            }

            // top 
            if (y == 1)
            {
                Pions p = board[x, y + 1];
                Pions q = board[x, y + 2];
                Pions a = board[x, y + 3];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isRed != isRed && q == null && a != null || p != null && p.isRed != isRed && q == null && a == null || p == null && q != null && q.isRed != isRed && a == null)
                    return true;
            }
            if (y == 0)
            {
                Pions p = board[x, y + 1];
                Pions q = board[x, y + 2];
                Pions a = board[x, y + 3];
                Pions b = board[x, y + 4];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isRed != isRed && q == null && a != null && b != null || p != null && p.isRed != isRed && q == null && a == null && b != null || p != null && p.isRed != isRed && q == null && a == null && b == null || p == null && q != null && q.isRed != isRed && a == null && b != null || p == null && q != null && q.isRed != isRed && a == null && b == null || p == null && q == null && a != null && a.isRed != isRed && b == null)
                    return true;
            }
            if (y == 2)
            {
                Pions p = board[x, y + 1];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isRed != isRed)
                {
                    // check if it's possible to land after the jump
                    if (board[x, y + 2] == null)
                        return true;
                }
                Pions q = board[x, y - 1];
                // if there is a pions, and it is not the same color as ours
                if (q != null && q.isRed != isRed)
                {
                    // check if it's possible to land after the jump
                    if (board[x, y - 2] == null)
                        return true;
                }
            }
            if (y == 3)
            {
                Pions p = board[x, y - 1];
                Pions q = board[x, y - 2];
                Pions a = board[x, y - 3];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isRed != isRed && q == null && a != null || p != null && p.isRed != isRed && q == null && a == null || p == null && q != null && q.isRed != isRed && a == null)
                    return true;
            }
            if (y == 4)
            {
                Pions p = board[x, y - 1];
                Pions q = board[x, y - 2];
                Pions a = board[x, y - 3];
                Pions b = board[x, y - 4];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isRed != isRed && q == null && a != null && b != null || p != null && p.isRed != isRed && q == null && a == null && b != null || p != null && p.isRed != isRed && q == null && a == null && b == null || p == null && q != null && q.isRed != isRed && a == null && b != null || p == null && q != null && q.isRed != isRed && a == null && b == null || p == null && q == null && a != null && a.isRed != isRed && b == null)
                    return true;
            }
        }
        if (isBlack && isKing)
        {
            // right
            if (x == 1)
            {
                Pions p = board[x + 1, y];
                Pions q = board[x + 2, y];
                Pions a = board[x + 3, y];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isBlack != isBlack && q == null && a != null || p != null && p.isBlack != isBlack && q == null && a == null || p == null && q != null && q.isBlack != isBlack && a == null)
                    return true;
            }
            if (x == 0)
            {
                Pions p = board[x + 1, y];
                Pions q = board[x + 2, y];
                Pions a = board[x + 3, y];
                Pions b = board[x + 4, y];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isBlack != isBlack && q == null && a != null && b != null || p != null && p.isBlack != isBlack && q == null && a == null && b != null || p != null && p.isBlack != isBlack && q == null && a == null && b == null || p == null && q != null && q.isBlack != isBlack && a == null && b != null || p == null && q != null && q.isBlack != isBlack && a == null && b == null || p == null && q == null && a != null && a.isBlack != isBlack && b == null)
                    return true;
            }
            if (x == 2)
            {
                Pions p = board[x + 1, y];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isBlack != isBlack)
                {
                    // check if it's possible to land after the jump
                    if (board[x + 2, y] == null)
                        return true;
                }
                Pions q = board[x - 1, y];
                // if there is a pions, and it is not the same color as ours
                if (q != null && q.isBlack != isBlack)
                {
                    // check if it's possible to land after the jump
                    if (board[x - 2, y] == null)
                        return true;
                }
            }

            // left
            if (x == 3)
            {
                Pions p = board[x - 1, y];
                Pions q = board[x - 2, y];
                Pions a = board[x - 3, y];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isBlack != isBlack && q == null && a != null || p != null && p.isBlack != isBlack && q == null && a == null || p == null && q != null && q.isBlack != isBlack && a == null)
                    return true;
            }
            if (x == 4)
            {
                Pions p = board[x - 1, y];
                Pions q = board[x - 2, y];
                Pions a = board[x - 3, y];
                Pions b = board[x - 4, y];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isBlack != isBlack && q == null && a != null && b != null || p != null && p.isBlack != isBlack && q == null && a == null && b != null || p != null && p.isBlack != isBlack && q == null && a == null && b == null || p == null && q != null && q.isBlack != isBlack && a == null && b != null || p == null && q != null && q.isBlack != isBlack && a == null && b == null || p == null && q == null && a != null && a.isBlack != isBlack && b == null)
                    return true;
            }

            // top 
            if (y == 1)
            {
                Pions p = board[x, y + 1];
                Pions q = board[x, y + 2];
                Pions a = board[x, y + 3];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isBlack != isBlack && q == null && a != null || p != null && p.isBlack != isBlack && q == null && a == null || p == null && q != null && q.isBlack != isBlack && a == null)
                    return true;
            }
            if (y == 0)
            {
                Pions p = board[x, y + 1];
                Pions q = board[x, y + 2];
                Pions a = board[x, y + 3];
                Pions b = board[x, y + 4];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isBlack != isBlack && q == null && a != null && b != null || p != null && p.isBlack != isBlack && q == null && a == null && b != null || p != null && p.isBlack != isBlack && q == null && a == null && b == null || p == null && q != null && q.isBlack != isBlack && a == null && b != null || p == null && q != null && q.isBlack != isBlack && a == null && b == null || p == null && q == null && a != null && a.isBlack != isBlack && b == null)
                    return true;
            }
            if (y == 2)
            {
                Pions p = board[x, y + 1];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isBlack != isBlack)
                {
                    // check if it's possible to land after the jump
                    if (board[x, y + 2] == null)
                        return true;
                }
                Pions q = board[x, y - 1];
                // if there is a pions, and it is not the same color as ours
                if (q != null && q.isBlack != isBlack)
                {
                    // check if it's possible to land after the jump
                    if (board[x, y - 2] == null)
                        return true;
                }
            }
            if (y == 3)
            {
                Pions p = board[x, y - 1];
                Pions q = board[x, y - 2];
                Pions a = board[x, y - 3];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isBlack != isBlack && q == null && a != null || p != null && p.isBlack != isBlack && q == null && a == null || p == null && q != null && q.isBlack != isBlack && a == null)
                    return true;
            }
            if (y == 4)
            {
                Pions p = board[x, y - 1];
                Pions q = board[x, y - 2];
                Pions a = board[x, y - 3];
                Pions b = board[x, y - 4];
                // if there is a pions, and it is not the same color as ours
                if (p != null && p.isBlack != isBlack && q == null && a != null && b != null || p != null && p.isBlack != isBlack && q == null && a == null && b != null || p != null && p.isBlack != isBlack && q == null && a == null && b == null || p == null && q != null && q.isBlack != isBlack && a == null && b != null || p == null && q != null && q.isBlack != isBlack && a == null && b == null || p == null && q == null && a != null && a.isBlack != isBlack && b == null)
                    return true;
            }
        }

        return false;
    }
    public bool ValidMove(Pions[,] board, int x1, int y1, int x2, int y2)
    {
        // if you are moving on top of another pions
        if (board[x2, y2] != null)
            return false;

        int deltaMove = Mathf.Abs(x2 - x1);
        int deltaMoveY = y2 - y1;

        if (isRed || isKing)
        {
            if (deltaMove == 1)
            {
                if (deltaMoveY == 0)
                    return true;
            }
            else if (deltaMove == 0)
            {
                if (deltaMoveY == 1)
                    return true;
            }
            else if (deltaMove == 2)
            {
                if (deltaMoveY == 0)
                {
                    Pions p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isRed != isRed)
                    {
                        return true;
                    }
                }
            }
            if (deltaMove == 0)
            {
                if (deltaMoveY == 2)
                {
                    Pions p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isRed != isRed)
                    {
                        return true;
                    }
                }
            }
        }


        if (isBlack || isKing)
        {
            if (deltaMove == 1)
            {
                if (deltaMoveY == 0)
                    return true;
            }
            else if (deltaMove == 0)
            {
                if (deltaMoveY == -1)
                    return true;
            }
            else if (deltaMove == 2)
            {
                if (deltaMoveY == 0)
                {
                    Pions p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isBlack != isBlack)
                    {
                        return true;
                    }
                }
            }
            if (deltaMove == 0)
            {
                if (deltaMoveY == -2)
                {
                    Pions p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isBlack != isBlack)
                    {
                        return true;
                    }
                }
            }

        }
        if (isKing && isBlack)
        {
            if (deltaMove == 2)
            {
                if (deltaMoveY == 0)
                {
                    Pions p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isBlack != isBlack || p == null)
                        return true;
                }
            }
            else if (x2 - x1 == 3)
            {
                if (deltaMoveY == 0)
                {
                    Pions p = board[x1 + 1, y1];
                    Pions q = board[x2 - 1, y1];
                    if (p != null && p.isBlack != isBlack && q == null || p == null && q == null || p == null && q != null && q.isBlack != isBlack)
                        return true;
                }

            }
            else if (x2 - x1 == -3)
            {
                if (deltaMoveY == 0)
                {
                    Pions p = board[x1 - 1, y1];
                    Pions q = board[x2 + 1, y1];
                    if (p != null && p.isBlack != isBlack && q == null || p == null && q == null || p == null && q != null && q.isBlack != isBlack)
                        return true;
                }

            }
            else if (x2 - x1 == 4)
            {
                if (deltaMoveY == 0)
                {
                    Pions p = board[x1 + 1, y1];
                    Pions q = board[x2 - 1, y1];
                    Pions a = board[x2 - 2, y1];
                    if (p != null && p.isBlack != isBlack && q == null && a == null || p == null && q == null && a == null || p == null && q != null && q.isBlack != isBlack && a == null || p == null && q == null && a != null && a.isBlack != isBlack)
                        return true;
                }

            }
            else if (x2 - x1 == -4)
            {
                if (deltaMoveY == 0)
                {
                    Pions p = board[x1 - 1, y1];
                    Pions q = board[x2 + 1, y1];
                    Pions a = board[x2 + 2, y1];
                    if (p != null && p.isBlack != isBlack && q == null && a == null || p == null && q == null && a == null || p == null && q != null && q.isBlack != isBlack && a == null || p == null && q == null && a != null && a.isBlack != isBlack)
                        return true;
                }

            }
            if (deltaMove == 0)
            {
                if (deltaMoveY == -2)
                {
                    Pions p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isBlack != isBlack || p == null)
                        return true;
                }
            }
            if (deltaMove == 0)
            {
                if (deltaMoveY == 2)
                {
                    Pions p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isBlack != isBlack || p == null)
                        return true;
                }
            }
            if (deltaMove == 0)
            {
                if (deltaMoveY == -3)
                {
                    Pions p = board[x1, y1 - 1];
                    Pions q = board[x1, y2 + 1];
                    if ((p != null && p.isBlack != isBlack && q == null) || (p == null && q == null) || (p == null && q != null && q.isBlack != isBlack))
                        return true;
                }
            }
            if (deltaMove == 0)
            {
                if (deltaMoveY == 3)
                {
                    Pions p = board[x1, y1 + 1];
                    Pions q = board[x1, y2 - 1];
                    if ((p != null && p.isBlack != isBlack && q == null) || (p == null && q == null) || (p == null && q != null && q.isBlack != isBlack))
                        return true;
                }
            }
            if (deltaMove == 0)
            {
                if (deltaMoveY == -4)
                {
                    Pions p = board[x1, y1 - 1];
                    Pions q = board[x1, y2 + 1];
                    Pions a = board[x1, y2 + 2];
                    if (p != null && p.isBlack != isBlack && q == null && a == null || p == null && q == null && a == null || p == null && q != null && q.isBlack != isBlack && a == null || p == null && q == null && a != null && a.isBlack != isBlack)
                        return true;
                }
            }
            if (deltaMove == 0)
            {
                if (deltaMoveY == 4)
                {
                    Pions p = board[x1, y1 + 1];
                    Pions q = board[x1, y2 - 1];
                    Pions a = board[x1, y2 - 2];
                    if (p != null && p.isRed != isRed && q == null && a == null || p == null && q == null && a == null || p == null && q != null && q.isRed != isRed && a == null || p == null && q == null && a != null && a.isRed != isRed)
                        return true;
                }
            }
        }
        if (isKing && isRed)
        {
            if (deltaMove == 2)
            {
                if (deltaMoveY == 0)
                {
                    Pions p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isRed != isRed || p == null)
                        return true;
                }
            }
            else if (x2 - x1 == 3)
            {
                if (deltaMoveY == 0)
                {
                    Pions p = board[x1 + 1, y1];
                    Pions q = board[x2 - 1, y1];
                    if (p != null && p.isRed != isRed && q == null || p == null && q == null || p == null && q != null && q.isRed != isRed)
                        return true;
                }

            }
            else if (x2 - x1 == -3)
            {
                if (deltaMoveY == 0)
                {
                    Pions p = board[x1 - 1, y1];
                    Pions q = board[x2 + 1, y1];
                    if (p != null && p.isRed != isRed && q == null || p == null && q == null || p == null && q != null && q.isRed != isRed)
                        return true;
                }

            }
            else if (x2 - x1 == 4)
            {
                if (deltaMoveY == 0)
                {
                    Pions p = board[x1 + 1, y1];
                    Pions q = board[x2 - 1, y1];
                    Pions a = board[x2 - 2, y1];
                    if (p != null && p.isRed != isRed && q == null && a == null || p == null && q == null && a == null || p == null && q != null && q.isRed != isRed && a == null || p == null && q == null && a != null && a.isRed != isRed)
                        return true;
                }

            }
            else if (x2 - x1 == -4)
            {
                if (deltaMoveY == 0)
                {
                    Pions p = board[x1 - 1, y1];
                    Pions q = board[x2 + 1, y1];
                    Pions a = board[x2 + 2, y1];
                    if (p != null && p.isRed != isRed && q == null && a == null || p == null && q == null && a == null || p == null && q != null && q.isRed != isRed && a == null || p == null && q == null && a != null && a.isRed != isRed)
                        return true;
                }

            }
            if (deltaMove == 0)
            {
                if (deltaMoveY == 2)
                {
                    Pions p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isRed != isRed || p == null)
                        return true;
                }
            }
            if (deltaMove == 0)
            {
                if (deltaMoveY == -2)
                {
                    Pions p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isRed != isRed || p == null)
                        return true;
                }
            }
            if (deltaMove == 0)
            {
                if (deltaMoveY == 3)
                {
                    Pions p = board[x1, y1 + 1];
                    Pions q = board[x1, y2 - 1];
                    if (p != null && p.isRed != isRed && q == null || p == null && q == null || p == null && q != null && q.isRed != isRed)
                        return true;
                }
            }
            if (deltaMove == 0)
            {
                if (deltaMoveY == -3)
                {
                    Pions p = board[x1, y1 - 1];
                    Pions q = board[x1, y2 + 1];
                    if ((p != null && p.isBlack != isBlack && q == null) || (p == null && q == null) || (p == null && q != null && q.isBlack != isBlack))
                        return true;
                }
            }
            if (deltaMove == 0)
            {
                if (deltaMoveY == 4)
                {
                    Pions p = board[x1, y1 + 1];
                    Pions q = board[x1, y2 - 1];
                    Pions a = board[x1, y2 - 2];
                    if (p != null && p.isRed != isRed && q == null && a == null || p == null && q == null && a == null || p == null && q != null && q.isRed != isRed && a == null || p == null && q == null && a != null && a.isRed != isRed)
                        return true;
                }
            }
            if (deltaMove == 0)
            {
                if (deltaMoveY == -4)
                {
                    Pions p = board[x1, y1 - 1];
                    Pions q = board[x1, y2 + 1];
                    Pions a = board[x1, y2 + 2];
                    if (p != null && p.isBlack != isBlack && q == null && a == null || p == null && q == null && a == null || p == null && q != null && q.isBlack != isBlack && a == null || p == null && q == null && a != null && a.isBlack != isBlack)
                        return true;
                }
            }
        }

        return false;

    }
}
