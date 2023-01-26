using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plateau12Pions : MonoBehaviour
{
    public Pions[,] pions = new Pions[5, 5];
    public GameObject PionNoirePrefabs;
    public GameObject PionRougePrefabs;
    public int RedPions = 0;
    private GameObject[] RedTab;
    public int BlackPions = 0;
    private GameObject[] BlackTab;

    private Vector3 boardOffset = new Vector3(0.5f, 1 / 8f, 0.5f);
    private Vector3 pionsOffset = new Vector3(0f, 0f, 0f);

    public bool isRed;
    public bool isBlack;
    private bool isRedTurn;
    private bool haskilled;

    private Pions selectedPions;
    private List<Pions> forcedPions;

    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;

    private void Start()
    {
        isRedTurn = true;
        GenerateBoard();
        //count pions
        RedTab = GameObject.FindGameObjectsWithTag("PionRouge");

        foreach (GameObject Red in RedTab)
        {
            RedPions += 1;
        }
        BlackTab = GameObject.FindGameObjectsWithTag("PionNoire");

        foreach (GameObject Black in BlackTab)
        {
            BlackPions += 1;
        }
    }

    private void Update()
    {
        UpdateMouseOver();
        RedPions = 0;
        RedTab = GameObject.FindGameObjectsWithTag("PionRouge");

        foreach (GameObject Red in RedTab)
        {
            RedPions += 1;
        }
        if (RedPions == 1)
        {
            foreach (GameObject Red in RedTab)
            {
                Red.GetComponent<Pions>().isKing = true;
                Red.transform.Rotate(Vector3.right * 180);
            }
        }
        BlackPions = 0;
        BlackTab = GameObject.FindGameObjectsWithTag("PionNoire");

        foreach (GameObject Black in BlackTab)
        {
            BlackPions += 1;
        }
        if (BlackPions == 1)
        {
            foreach (GameObject Black in BlackTab)
            {
                Black.GetComponent<Pions>().isKing = true;
                Black.transform.Rotate(Vector3.right * 180);
            }
        }

        if ((isRed) ? isRedTurn : !isRedTurn)
        {
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            if (selectedPions != null)
                UpdatePionsDrag(selectedPions);

            if (Input.GetMouseButtonDown(0))
                SelectPions(x, y);

            if (Input.GetMouseButtonUp(0))
                TryMove((int)startDrag.x, (int)startDrag.y, x, y);
        }
    }

    private void UpdateMouseOver()
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int)(hit.point.x);
            mouseOver.y = (int)(hit.point.z);
        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }

    private void UpdatePionsDrag(Pions p)
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            p.transform.position = hit.point + (Vector3.up * 0.23529412f);
        }
    }
    private void SelectPions(int x, int y)
    {
        // out of bounds
        if (x < 0 || x >= 5 || y < 0 || y >= 5)
            return;

        Pions p = pions[x, y];
        if (p != null && p.isRed == isRed || p != null && p.isBlack == isBlack)
        {
            selectedPions = p;
            startDrag = mouseOver;
        }
    }
    private void TryMove(int x1, int y1, int x2, int y2)
    {
        forcedPions = ScanForPossibleToMove();
        // multiplayer Support
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPions = pions[x1, y1];

        //out of bounds
        if (x2 < 0 || x2 >= 5 || y2 < 0 || y2 >= 5)
        {
            if (selectedPions != null)
                MovePions(selectedPions, x1, y1);

            startDrag = Vector2.zero;
            selectedPions = null;
            return;
        }

        if (selectedPions != null)
        {
            // if it has not moved
            if (endDrag == startDrag)
            {
                MovePions(selectedPions, x1, y1);
                startDrag = Vector2.zero;
                selectedPions = null;
                return;
            }

            // check if it's valid move 
            if (selectedPions.ValidMove(pions, x1, y1, x2, y2))
            {

                //Did we will anything
                // if this is a jump
                if (Mathf.Abs(x2 - x1) == 2 || Mathf.Abs(y2 - y1) == 2)
                {
                    Pions p = pions[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null)
                    {
                        pions[(x1 + x2) / 2, (y1 + y2) / 2] = null;
                        DestroyImmediate(p.gameObject);
                        haskilled = true;
                    }
                }
                else if (x2 - x1 == 3)
                {
                    Pions p = pions[x1 + 1, y1];
                    Pions q = pions[x1 + 2, y1];
                    if (p != null)
                    {
                        pions[x1 + 1, y1] = null;
                        DestroyImmediate(p.gameObject);
                        haskilled = true;
                    }
                    if (q != null)
                    {
                        pions[x1 + 2, y1] = null;
                        DestroyImmediate(q.gameObject);
                        haskilled = true;
                    }

                }
                else if (x2 - x1 == -3)
                {
                    Pions p = pions[x1 - 1, y1];
                    Pions q = pions[x1 - 2, y1];
                    if (p != null)
                    {
                        pions[x1 - 1, y1] = null;
                        DestroyImmediate(p.gameObject);
                        haskilled = true;
                    }
                    if (q != null)
                    {
                        pions[x1 - 2, y1] = null;
                        DestroyImmediate(q.gameObject);
                        haskilled = true;
                    }

                }
                else if (x2 - x1 == 4)
                {
                    Pions p = pions[x1 + 1, y1];
                    Pions q = pions[x1 + 2, y1];
                    Pions a = pions[x1 + 3, y1];
                    if (p != null)
                    {
                        pions[x1 + 1, y1] = null;
                        DestroyImmediate(p.gameObject);
                        haskilled = true;
                    }
                    if (q != null)
                    {
                        pions[x1 + 2, y1] = null;
                        DestroyImmediate(q.gameObject);
                        haskilled = true;
                    }
                    if (a != null)
                    {
                        pions[x1 + 3, y1] = null;
                        DestroyImmediate(a.gameObject);
                        haskilled = true;
                    }

                }
                else if (x2 - x1 == -4)
                {
                    Pions p = pions[x1 - 1, y1];
                    Pions q = pions[x1 - 2, y1];
                    Pions a = pions[x1 - 3, y1];
                    if (p != null)
                    {
                        pions[x1 - 1, y1] = null;
                        DestroyImmediate(p.gameObject);
                        haskilled = true;
                    }
                    if (q != null)
                    {
                        pions[x1 - 2, y1] = null;
                        DestroyImmediate(q.gameObject);
                        haskilled = true;
                    }
                    if (a != null)
                    {
                        pions[x1 - 3, y1] = null;
                        DestroyImmediate(a.gameObject);
                        haskilled = true;
                    }

                }
                else if (y2 - y1 == 3)
                {
                    Pions p = pions[x1, y1 + 1];
                    Pions q = pions[x1, y1 + 2];
                    if (p != null)
                    {
                        pions[x1, y1 + 1] = null;
                        DestroyImmediate(p.gameObject);
                        haskilled = true;
                    }
                    if (q != null)
                    {
                        pions[x1, y1 + 2] = null;
                        DestroyImmediate(q.gameObject);
                        haskilled = true;
                    }

                }
                else if (y2 - y1 == -3)
                {
                    Pions p = pions[x1, y1 - 1];
                    Pions q = pions[x1, y1 - 2];
                    if (p != null)
                    {
                        pions[x1, y1 - 1] = null;
                        DestroyImmediate(p.gameObject);
                        haskilled = true;
                    }
                    if (q != null)
                    {
                        pions[x1, y1 - 2] = null;
                        DestroyImmediate(q.gameObject);
                        haskilled = true;
                    }

                }
                else if (y2 - y1 == 4)
                {
                    Pions p = pions[x1, y1 + 1];
                    Pions q = pions[x1, y1 + 2];
                    Pions a = pions[x1, y1 + 3];
                    if (p != null)
                    {
                        pions[x1, y1 + 1] = null;
                        DestroyImmediate(p.gameObject);
                        haskilled = true;
                    }
                    if (q != null)
                    {
                        pions[x1, y1 + 2] = null;
                        DestroyImmediate(q.gameObject);
                        haskilled = true;
                    }
                    if (a != null)
                    {
                        pions[x1, y1 + 3] = null;
                        DestroyImmediate(a.gameObject);
                        haskilled = true;
                    }

                }
                else if (y2 - y1 == -4)
                {
                    Pions p = pions[x1, y1 - 1];
                    Pions q = pions[x1, y1 - 2];
                    Pions a = pions[x1, y1 - 3];
                    if (p != null)
                    {
                        pions[x1, y1 - 1] = null;
                        DestroyImmediate(p.gameObject);
                        haskilled = true;
                    }
                    if (q != null)
                    {
                        pions[x1, y1 - 2] = null;
                        DestroyImmediate(q.gameObject);
                        haskilled = true;
                    }
                    if (a != null)
                    {
                        pions[x1, y1 - 3] = null;
                        DestroyImmediate(a.gameObject);
                        haskilled = true;
                    }

                }



                // if he don't kill
                if (isRed)
                {
                    if (forcedPions.Count != 0 && selectedPions.isRed != haskilled)
                    {
                        destroyPions();
                    }
                }
                if (isBlack)
                {
                    if (forcedPions.Count != 0 && selectedPions.isBlack != haskilled)
                    {
                        destroyPions();
                    }
                }


                pions[x2, y2] = selectedPions;
                pions[x1, y1] = null;
                MovePions(selectedPions, x2, y2);

                EndTurn();
            }
            else
            {
                MovePions(selectedPions, x1, y1);
                startDrag = Vector2.zero;
                selectedPions = null;
                return;
            }

        }
    }

    private void EndTurn()
    {
        int x = (int)endDrag.x;
        int y = (int)endDrag.y;

        // Promotions
        if (selectedPions != null)
        {
            if (selectedPions.isRed && !selectedPions.isKing && y == 4)
            {
                selectedPions.isKing = true;
                selectedPions.transform.Rotate(Vector3.right * 180);
            }
            else if (selectedPions.isBlack && !selectedPions.isKing && y == 0)
            {
                selectedPions.isKing = true;
                selectedPions.transform.Rotate(Vector3.right * 180);
            }
        }

        selectedPions = null;
        startDrag = Vector2.zero;

        if (ScanForPossibleToMove(selectedPions, x, y).Count != 0 && haskilled)
            return;

        isRedTurn = !isRedTurn;
        isRed = !isRed;
        isBlack = !isBlack;
        haskilled = false;
        CheckVictory();
    }

    private void CheckVictory()
    {
        var ps = FindObjectsOfType<Pions>();
        bool hasRed = false, hasBlack = false;
        for (int i = 0; i < ps.Length; i++)
        {
            if (ps[i].isRed)
                hasRed = true;
            else
                hasBlack = true;
        }
        if (!hasRed)
            Victory(false);
        if (!hasBlack)
            Victory(true);
    }

    private void Victory(bool isRed)
    {
        if (isRed)
            Debug.Log("Red team has won ");
        else
            Debug.Log("Black team has won ");
    }

    private List<Pions> ScanForPossibleToMove(Pions p, int x, int y)
    {
        forcedPions = new List<Pions>();

        if (pions[x, y].isForceToMove(pions, x, y))
            forcedPions.Add(pions[x, y]);

        return forcedPions;
    }

    private List<Pions> ScanForPossibleToMove()
    {
        forcedPions = new List<Pions>();
        // check all the pions 
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                if (pions[i, j] != null && pions[i, j].isRed == isRedTurn || pions[i, j] != null && pions[i, j].isBlack == !isRedTurn)
                    if (pions[i, j].isForceToMove(pions, i, j))
                        forcedPions.Add(pions[i, j]);
        return forcedPions;
    }

    private void destroyPions()
    {
        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
                if (pions[i, j] != null && pions[i, j].isRed == isRedTurn || pions[i, j] != null && pions[i, j].isBlack == !isRedTurn)
                    if (pions[i, j].isForceToMove(pions, i, j))
                    {
                        forcedPions.Add(pions[i, j]);
                        Destroy(pions[i, j].gameObject);
                    }


        return;

    }

    private void GenerateBoard()
    {
        // Generate red team
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                // Generate our PionsNoire
                GeneratePions(x, y);
            }
        }
        for (int y = 2; y == 2; y++)
        {
            for (int x = 3; x < 5; x++)
            {
                // Generate pionsRouge lign 3
                GeneratePions(x, y);
            }
        }

        // Generate black team
        for (int y = 4; y > 2; y--)
        {
            for (int x = 0; x < 5; x++)
            {
                // Generate our PionsNoire
                GeneratePions(x, y);
            }
        }
        for (int y = 2; y == 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                // Generate pionsNoire lign 3
                GeneratePions(x, y);
            }
        }
    }

    private void GeneratePions(int x, int y)
    {
        bool isPieceRed = (y > 2 || y == 2 && x < 2) ? false : true;
        GameObject go = Instantiate((isPieceRed) ? PionRougePrefabs : PionNoirePrefabs) as GameObject;
        go.transform.SetParent(transform);
        Pions p = go.GetComponent<Pions>();
        pions[x, y] = p;
        MovePions(p, x, y);

    }

    private void MovePions(Pions p, int x, int y)
    {
        p.transform.position = (Vector3.right * x) + (Vector3.forward * y) + boardOffset + pionsOffset;
    }
}
