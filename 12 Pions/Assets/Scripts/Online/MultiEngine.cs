using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

public class MultiEngine : MonoBehaviourPunCallbacks, IOnEventCallback, IPointerDownHandler, IPointerUpHandler
{
    enum WhoseTurn
    {
        Me = 0,
        Opponent = 1
    }

    enum GameStyle
    {
        Offline = 0,
        Online = 1
    }

    enum PawnStyle
    {
        normal = 0,
        king = 1
    }

    enum GameState
    {
        Start = 0,
        Play = 1,
        End = 2,
        Peace = 3
    }

    public GameObject[] mePawns;
    public GameObject[] oppPawns;
    public int[] availablePawns;
    public GameObject[] cages;
    public GameObject[] allPawns;
    public GameObject[] playerAvatars;
    public GameObject[] boards;
    public List<GameObject> removeObjects;
    public GameObject movePawn;
    public GameObject doubleMovePawn;
    public GameObject origincage;
    public GameObject targetCage;
    public GameObject filpPawnForKing;
    public GameObject endUI;
    public Text endText;
    public Text player1Name;
    public Text player2Name;

    public Sprite[] normalPawnSprites;
    public Sprite[] kingPawnSprites;

    public int[] mePawnStyles;
    public int[] oppPawnStyles;
    public int gameStateIndex;
    public int whoseTurnIndex;
    public int gameStyleIndex;
    public int movePawnStyle;
    public int oIndex;
    public int tIndex;
    public int kingIndex;
    public int gameState;

    public bool isMovePawn;
    public bool isFlipEnemy;
    public bool isDoublePlay;
    public bool isKing;
    public bool isSkip;

    public string removeObjectsInfo;    

    #region MonoBehaviourCallBacks    
    void Start()
    {
        for (int i = 0; i < oppPawns.Length; i++)
        {
            oppPawns[i].transform.position = cages[i].transform.position;
            oppPawns[i].GetComponent<Image>().sprite = normalPawnSprites[1];
            oppPawnStyles[i] = (int)PawnStyle.normal;
        }

        for (int i = 0; i < mePawns.Length; i++)
        {
            mePawns[i].transform.position = cages[i + 13].transform.position;
            mePawns[i].GetComponent<Image>().sprite = normalPawnSprites[0];
            mePawnStyles[i] = (int)PawnStyle.normal;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            SendBoardInfo(new object[] { MultiPhoton.multiBoard });
            
            if(UnityEngine.Random.Range(0, 1f) > 0.5f)
            {                
                SendTurn(new object[] { 0 });
            }
            else
            {
                SendTurn(new object[] { 1 });
            }
        }

        gameState = (int)GameState.Play;

        foreach (var player in PhotonNetwork.PlayerList) // 2 Players Room
        {
            
        }

        player1Name.text = PhotonNetwork.PlayerList[0].NickName;
        player2Name.text = PhotonNetwork.PlayerList[1].NickName;
    }
   
    void Update()
    {
        if (isMovePawn)
        {
            movePawn.transform.position = Input.mousePosition;
        }        
    }

    public void GameEvaluation()
    {
        availablePawns[0] = 0;
        availablePawns[1] = 0;

        for (int i = 0; i < mePawns.Length; i++)
        {
            for (int j = 0; j < cages.Length; j++)
            {
                if (Vector3.Distance(cages[j].transform.position, mePawns[i].transform.position) < 0.1f)
                {
                    availablePawns[0]++;
                }
            }
        }

        for (int i = 0; i < oppPawns.Length; i++)
        {
            for (int j = 0; j < cages.Length; j++)
            {
                if (Vector3.Distance(cages[j].transform.position, oppPawns[i].transform.position) < 0.1f)
                {
                    availablePawns[1]++;
                }
            }
        }

        if (availablePawns[0] == 0 || availablePawns[1] == 0)
        {
            UpdateEvaluation(new object[] { availablePawns[0], availablePawns[1] });
        }
    }

    IEnumerator DelayGoToMainScene(float waitSec)
    {
        PhotonNetwork.Disconnect();
        yield return new WaitForSeconds(waitSec);
        
        SceneManager.LoadScene("Main");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(gameState == (int)GameState.Play)
        {
            if (isDoublePlay)
            {
                if (whoseTurnIndex == (int)WhoseTurn.Me && PhotonNetwork.IsMasterClient)
                {
                    if (eventData.pointerCurrentRaycast.gameObject == doubleMovePawn)
                    {
                        isMovePawn = true;
                        movePawn = eventData.pointerCurrentRaycast.gameObject;
                        movePawn.transform.SetParent(GameObject.FindGameObjectWithTag("Dynamic").transform, false);

                        for (int i = 0; i < mePawns.Length; i++)
                        {
                            if (movePawn == mePawns[i])
                            {
                                movePawnStyle = mePawnStyles[i];
                            }
                        }

                        SetOriginCage();
                    }
                }
                else if (whoseTurnIndex == (int)WhoseTurn.Opponent && !PhotonNetwork.IsMasterClient)
                {
                    if (eventData.pointerCurrentRaycast.gameObject == doubleMovePawn)
                    {
                        isMovePawn = true;
                        movePawn = eventData.pointerCurrentRaycast.gameObject;
                        movePawn.transform.SetParent(GameObject.FindGameObjectWithTag("Dynamic").transform, false);

                        for (int i = 0; i < oppPawns.Length; i++)
                        {
                            if (movePawn == oppPawns[i])
                            {
                                movePawnStyle = oppPawnStyles[i];
                            }
                        }

                        SetOriginCage();
                    }
                }
            }
            else
            {
                if (whoseTurnIndex == (int)WhoseTurn.Me && PhotonNetwork.IsMasterClient)
                {
                    if (eventData.pointerCurrentRaycast.gameObject.tag == "My Ball")
                    {
                        isMovePawn = true;
                        movePawn = eventData.pointerCurrentRaycast.gameObject;
                        movePawn.transform.SetParent(GameObject.FindGameObjectWithTag("Dynamic").transform, false);

                        for (int i = 0; i < mePawns.Length; i++)
                        {
                            if (movePawn == mePawns[i])
                            {
                                movePawnStyle = mePawnStyles[i];
                            }
                        }

                        SetOriginCage();
                    }
                }
                else if (whoseTurnIndex == (int)WhoseTurn.Opponent && !PhotonNetwork.IsMasterClient)
                {
                    if (eventData.pointerCurrentRaycast.gameObject.tag == "Opp Ball")
                    {
                        isMovePawn = true;
                        movePawn = eventData.pointerCurrentRaycast.gameObject;
                        movePawn.transform.SetParent(GameObject.FindGameObjectWithTag("Dynamic").transform, false);

                        for (int i = 0; i < oppPawns.Length; i++)
                        {
                            if (movePawn == oppPawns[i])
                            {
                                movePawnStyle = oppPawnStyles[i];
                            }
                        }

                        SetOriginCage();
                    }
                }
            }            
        }                
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(gameState == (int)GameState.Play)
        {
            if (isMovePawn)
            {
                isMovePawn = false;
                movePawn.transform.SetParent(GameObject.FindGameObjectWithTag("Static").transform, false);
                TargetCage(Input.mousePosition);

                if (targetCage != null && targetCage != origincage && IsEmptyCage(targetCage))
                {
                    if (IsAvailableMoveCage())
                    {
                        movePawn.transform.position = targetCage.transform.position;
                        SyncPawns();

                        if (IsKing(targetCage))
                        {
                            SetKingPawn();
                        }

                        if (whoseTurnIndex == (int)WhoseTurn.Me && PhotonNetwork.IsMasterClient)
                        {
                            if (removeObjects.Count != 0)
                            {
                                LostMyPawn(1);
                            }
                   
                            SendTurn(new object[] {1});                                                 
                        }
                        else if (whoseTurnIndex == (int)WhoseTurn.Opponent && !PhotonNetwork.IsMasterClient)
                        {
                            if (removeObjects.Count != 0)
                            {
                                LostMyPawn(0);
                            }
                                                    
                            SendTurn(new object[] { 0 });
                        }
                    }
                    else if (IsAvailableFlipCage() && IsAvailableFlipPawn())
                    {
                        movePawn.transform.position = targetCage.transform.position;
                        SyncPawns();

                        KillEnemyPawn();

                        if (IsKing(targetCage))
                        {
                            SetKingPawn();
                        }

                        if (IsPlayTwice(movePawn))
                        {
                            doubleMovePawn = movePawn;
                        }
                        else
                        {
                            if (whoseTurnIndex == (int)WhoseTurn.Me && PhotonNetwork.IsMasterClient)
                            {                          
                                SendTurn(new object[] { 1 });                               
                            }
                            else if (whoseTurnIndex == (int)WhoseTurn.Opponent && !PhotonNetwork.IsMasterClient)
                            {                           
                                SendTurn(new object[] { 0 });                                
                            }
                        }
                    }
                    else
                    {
                        movePawn.transform.position = origincage.transform.position;
                    }
                }
                else
                {
                    movePawn.transform.position = origincage.transform.position;
                }                
            }
        }
    }

    public void SyncPawns()
    {
        int moveIndex = 0;

        for (int i = 0; i < allPawns.Length; i++)
        {
            if (movePawn == allPawns[i])
            {
                moveIndex = i;
                break;
            }
        }

        SendPosition(new object[] { movePawn.transform.position, 2, moveIndex });
    }

    public bool IsEmptyCage(GameObject cage)
    {
        bool isEmpty;

        isEmpty = true;

        for (int j = 0; j < allPawns.Length; j++)
        {
            if (Vector3.Distance(cage.transform.position, allPawns[j].transform.position) < 0.1f)
            {
                isEmpty = false;
            }
        }

        return isEmpty;
    }

    public bool IsAvailableMoveCage()
    {
        for (int i = 0; i < cages.Length; i++)
        {
            if (Vector3.Distance(cages[i].transform.position, origincage.transform.position) < 0.1f)
            {
                oIndex = i;
            }

            if (Vector3.Distance(cages[i].transform.position, targetCage.transform.position) < 0.1f)
            {
                tIndex = i;
            }
        }

        if (whoseTurnIndex == (int)WhoseTurn.Me && movePawnStyle == (int)PawnStyle.normal && PhotonNetwork.IsMasterClient)
        {
            if (oIndex % 5 != 0 && tIndex == oIndex - 1)
            {
                return true;
            }
            else if (oIndex % 5 != 4 && tIndex == oIndex + 1)
            {
                return true;
            }
            else if (tIndex == oIndex - 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Opponent && movePawnStyle == (int)PawnStyle.normal && !PhotonNetwork.IsMasterClient)
        {
            if (oIndex % 5 != 0 && tIndex == oIndex - 1)
            {
                return true;
            }
            else if (oIndex % 5 != 4 && tIndex == oIndex + 1)
            {
                return true;
            }
            else if (tIndex == oIndex + 5)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Me && movePawnStyle == (int)PawnStyle.king && PhotonNetwork.IsMasterClient)
        {
            return IsAvailableMoveForKing(oIndex, tIndex);
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Opponent && movePawnStyle == (int)PawnStyle.king && !PhotonNetwork.IsMasterClient)
        {
            return IsAvailableMoveForKing(oIndex, tIndex);
        }
        else
        {
            return false;
        }
    }

    public bool IsAvailableMoveForKing(int origin, int target)
    {
        bool isAvailable;
        isAvailable = true;

        if (origin / 5 == target / 5)
        {
            if (origin % 5 < target % 5)
            {
                for (int i = origin % 5 + 1; i < target % 5; i++)
                {
                    if (IsEmptyCage(cages[(origin / 5) * 5 + i]) == false)
                    {
                        isAvailable = false;
                    }
                }
            }
            else if (origin % 5 > target % 5)
            {
                for (int i = target % 5 + 1; i < origin % 5; i++)
                {
                    if (IsEmptyCage(cages[(target / 5) * 5 + i]) == false)
                    {
                        isAvailable = false;
                    }
                }
            }
        }
        else if (origin % 5 == target % 5)
        {
            if (origin / 5 < target / 5)
            {
                for (int i = origin / 5 + 1; i < target / 5; i++)
                {
                    if (IsEmptyCage(cages[i * 5 + origin % 5]) == false)
                    {
                        isAvailable = false;
                    }
                }
            }
            else if (origin / 5 > target / 5)
            {
                for (int i = target / 5 + 1; i < origin / 5; i++)
                {
                    if (IsEmptyCage(cages[i * 5 + origin % 5]) == false)
                    {
                        isAvailable = false;
                    }
                }
            }
        }

        return isAvailable;
    }

    public bool IsAvailableFlipCage()
    {
        for (int i = 0; i < cages.Length; i++)
        {
            if (Vector3.Distance(cages[i].transform.position, origincage.transform.position) < 0.1f)
            {
                oIndex = i;
            }

            if (Vector3.Distance(cages[i].transform.position, targetCage.transform.position) < 0.1f)
            {
                tIndex = i;
            }
        }

        if (whoseTurnIndex == (int)WhoseTurn.Me && movePawnStyle == (int)PawnStyle.normal && PhotonNetwork.IsMasterClient)
        {
            if ((oIndex % 5 == 2 || oIndex % 5 == 3 || oIndex % 5 == 4) && tIndex == oIndex - 2)
            {
                return true;
            }
            else if ((oIndex % 5 == 0 || oIndex % 5 == 1 || oIndex % 5 == 2) && tIndex == oIndex + 2)
            {
                return true;
            }
            else if (tIndex == oIndex - 10 && (oIndex / 5 != 1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Opponent && movePawnStyle == (int)PawnStyle.normal && !PhotonNetwork.IsMasterClient)
        {
            if ((oIndex % 5 == 2 || oIndex % 5 == 3 || oIndex % 5 == 4) && tIndex == oIndex - 2)
            {
                return true;
            }
            else if ((oIndex % 5 == 0 || oIndex % 5 == 1 || oIndex % 5 == 2) && tIndex == oIndex + 2)
            {
                return true;
            }
            else if (tIndex == oIndex + 10 && (oIndex / 5 != 3))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Me && movePawnStyle == (int)PawnStyle.king)
        {
            return IsAvailableFlipForKing(oIndex, tIndex);
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Opponent && movePawnStyle == (int)PawnStyle.king)
        {
            return IsAvailableFlipForKing(oIndex, tIndex);
        }
        else
        {
            return false;
        }
    }

    public bool IsAvailableFlipPawn()
    {
        for (int i = 0; i < cages.Length; i++)
        {
            if (Vector3.Distance(cages[i].transform.position, origincage.transform.position) < 0.1f)
            {
                oIndex = i;
            }
        }

        for (int i = 0; i < cages.Length; i++)
        {
            if (Vector3.Distance(cages[i].transform.position, targetCage.transform.position) < 0.1f)
            {
                tIndex = i;
            }
        }

        if (whoseTurnIndex == (int)WhoseTurn.Me && movePawnStyle == (int)PawnStyle.normal && PhotonNetwork.IsMasterClient)
        {
            isFlipEnemy = false;

            for (int i = 0; i < oppPawns.Length; i++)
            {
                if (oIndex % 5 != 0)
                {
                    if (Vector3.Distance(cages[oIndex - 1].transform.position, oppPawns[i].transform.position) < 0.1f && tIndex == oIndex - 2)
                    {
                        isFlipEnemy = true;
                    }
                }

                if (oIndex % 5 != 4)
                {
                    if (Vector3.Distance(cages[oIndex + 1].transform.position, oppPawns[i].transform.position) < 0.1f && tIndex == oIndex + 2)
                    {
                        isFlipEnemy = true;
                    }
                }

                if (oIndex >= 5)
                {
                    if (Vector3.Distance(cages[oIndex - 5].transform.position, oppPawns[i].transform.position) < 0.1f && tIndex == oIndex - 10)
                    {
                        isFlipEnemy = true;
                    }
                }
            }

            return isFlipEnemy;
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Opponent && movePawnStyle == (int)PawnStyle.normal && !PhotonNetwork.IsMasterClient)
        {
            isFlipEnemy = false;

            for (int i = 0; i < mePawns.Length; i++)
            {
                if (oIndex % 5 != 0)
                {
                    if ((Vector3.Distance(cages[oIndex - 1].transform.position, mePawns[i].transform.position) < 0.1f) && tIndex == oIndex - 2)
                    {
                        isFlipEnemy = true;
                    }
                }

                if (oIndex % 5 != 4)
                {
                    if (Vector3.Distance(cages[oIndex + 1].transform.position, mePawns[i].transform.position) < 0.1f && tIndex == oIndex + 2)
                    {
                        isFlipEnemy = true;
                    }
                }

                if (oIndex <= 19)
                {
                    if (Vector3.Distance(cages[oIndex + 5].transform.position, mePawns[i].transform.position) < 0.1f && tIndex == oIndex + 10)
                    {
                        isFlipEnemy = true;
                    }
                }
            }

            return isFlipEnemy;
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Me && movePawnStyle == (int)PawnStyle.king && PhotonNetwork.IsMasterClient)
        {
            return IsAvailableFlipForKing(oIndex, tIndex);
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Opponent && movePawnStyle == (int)PawnStyle.king && !PhotonNetwork.IsMasterClient)
        {
            return IsAvailableFlipForKing(oIndex, tIndex);
        }
        else
        {
            return false;
        }
    }

    public bool IsAvailableFlipForKing(int origin, int target)
    {
        bool isAvailable;
        int flipPawnCount;

        flipPawnCount = 0;
        isAvailable = true;

        if (origin / 5 == target / 5)
        {
            if (origin % 5 < target % 5)
            {
                for (int i = origin % 5 + 1; i < target % 5; i++)
                {
                    if (IsEmptyCage(cages[(origin / 5) * 5 + i]) == false)
                    {
                        flipPawnCount++;
                        CageToPawn(cages[(origin / 5) * 5 + i]);
                    }
                }


                if (flipPawnCount > 1)
                {
                    isAvailable = false;
                }
            }
            else if (origin % 5 > target % 5)
            {
                for (int i = target % 5 + 1; i < origin % 5; i++)
                {
                    if (IsEmptyCage(cages[(target / 5) * 5 + i]) == false)
                    {
                        flipPawnCount++;
                        CageToPawn(cages[(target / 5) * 5 + i]);
                    }
                }


                if (flipPawnCount > 1)
                {
                    isAvailable = false;
                }
            }
        }
        else if (origin % 5 == target % 5)
        {
            if (origin / 5 < target / 5)
            {
                for (int i = origin / 5 + 1; i < target / 5; i++)
                {
                    if (IsEmptyCage(cages[i * 5 + origin % 5]) == false)
                    {
                        flipPawnCount++;
                        CageToPawn(cages[i * 5 + origin % 5]);
                    }
                }

                if (flipPawnCount > 1)
                {
                    isAvailable = false;
                }
            }
            else if (origin / 5 > target / 5)
            {
                for (int i = target / 5 + 1; i < origin / 5; i++)
                {
                    if (IsEmptyCage(cages[i * 5 + origin % 5]) == false)
                    {
                        flipPawnCount++;
                        CageToPawn(cages[i * 5 + origin % 5]);
                    }
                }

                if (flipPawnCount > 1)
                {
                    isAvailable = false;
                }
            }
        }

        return isAvailable;
    }

    public void CageToPawn(GameObject cage)
    {
        for (int i = 0; i < mePawns.Length; i++)
        {
            if (Vector3.Distance(mePawns[i].transform.position, cage.transform.position) < 0.1f)
            {
                filpPawnForKing = mePawns[i];
            }
        }

        for (int i = 0; i < oppPawns.Length; i++)
        {
            if (Vector3.Distance(oppPawns[i].transform.position, cage.transform.position) < 0.1f)
            {
                filpPawnForKing = oppPawns[i];
            }
        }
    }

    public void FindAvailableFlipPawn(GameObject origin)
    {
        for (int i = 0; i < cages.Length; i++)
        {
            if (Vector3.Distance(cages[i].transform.position, origin.transform.position) < 0.1f)
            {
                oIndex = i;
            }
        }

        if (whoseTurnIndex == (int)WhoseTurn.Me)
        {
            for (int i = 0; i < oppPawns.Length; i++)
            {
                if (oIndex % 5 == 2 || oIndex % 5 == 3 || oIndex % 5 == 4)
                {
                    if (Vector3.Distance(cages[oIndex - 1].transform.position, oppPawns[i].transform.position) < 0.1f && IsEmptyCage(cages[oIndex - 2]) && origin.transform.position != new Vector3(10000f, 10000f, 0))
                    {
                        removeObjects.Add(origin);
                    }
                }

                if (oIndex % 5 == 0 || oIndex % 5 == 1 || oIndex % 5 == 2)
                {
                    if (Vector3.Distance(cages[oIndex + 1].transform.position, oppPawns[i].transform.position) < 0.1f && IsEmptyCage(cages[oIndex + 2]) && origin.transform.position != new Vector3(10000f, 10000f, 0))
                    {
                        removeObjects.Add(origin);
                    }
                }

                if (oIndex / 5 == 2 || oIndex / 5 == 3 || oIndex / 5 == 4)
                {
                    if (Vector3.Distance(cages[oIndex - 5].transform.position, oppPawns[i].transform.position) < 0.1f && IsEmptyCage(cages[oIndex - 10]) && origin.transform.position != new Vector3(10000f, 10000f, 0))
                    {
                        removeObjects.Add(origin);
                    }
                }
            }
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Opponent)
        {
            for (int i = 0; i < mePawns.Length; i++)
            {
                if (oIndex % 5 == 2 || oIndex % 5 == 3 || oIndex % 5 == 4)
                {
                    if (Vector3.Distance(cages[oIndex - 1].transform.position, mePawns[i].transform.position) < 0.1f && IsEmptyCage(cages[oIndex - 2]) && origin.transform.position != new Vector3(10000f, 10000f, 0))
                    {
                        removeObjects.Add(origin);
                    }
                }

                if (oIndex % 5 == 0 || oIndex % 5 == 1 || oIndex % 5 == 2)
                {
                    if (Vector3.Distance(cages[oIndex + 1].transform.position, mePawns[i].transform.position) < 0.1f && IsEmptyCage(cages[oIndex + 2]) && origin.transform.position != new Vector3(10000f, 10000f, 0))
                    {
                        removeObjects.Add(origin);
                    }
                }

                if (oIndex / 5 == 0 || oIndex / 5 == 1 || oIndex / 5 == 2)
                {
                    if (Vector3.Distance(cages[oIndex + 5].transform.position, mePawns[i].transform.position) < 0.1f && IsEmptyCage(cages[oIndex + 10]) && origin.transform.position != new Vector3(10000f, 10000f, 0))
                    {
                        removeObjects.Add(origin);
                    }
                }
            }
        }
    }

    public void FindAvailableFlipKingPawn(GameObject origin)
    {
        for (int i = 0; i < cages.Length; i++)
        {
            if (Vector3.Distance(cages[i].transform.position, origin.transform.position) < 0.1f)
            {
                oIndex = i;
            }
        }

        if (whoseTurnIndex == (int)WhoseTurn.Me)
        {
            for (int i = 0; i < mePawns.Length; i++)
            {
                if (Vector3.Distance(mePawns[i].transform.position, origin.transform.position) < 0.1f)
                {
                    if (mePawnStyles[i] == (int)PawnStyle.king)
                    {
                        isSkip = false;

                        if (oIndex % 5 == 2 || oIndex % 5 == 3 || oIndex % 5 == 4)
                        {
                            // Check mePawns
                            for (int j = 1; j < oIndex % 5; j++)
                            {
                                if (oIndex % 5 == 4 && j == 3)
                                {
                                    continue;
                                }

                                for (int k = 0; k < mePawns.Length; k++)
                                {
                                    if (Vector3.Distance(cages[oIndex - j].transform.position, mePawns[k].transform.position) < 0.1f)
                                    {
                                        isSkip = true;
                                        break;
                                    }
                                }

                                if (!isSkip && !IsEmptyCage(cages[oIndex - j]) && !IsEmptyCage(cages[oIndex - j - 1]))
                                {
                                    isSkip = true;
                                    break;
                                }
                            }

                            // Find oppPawns
                            if (!isSkip)
                            {
                                for (int j = 1; j < oIndex % 5; j++)
                                {
                                    for (int k = 0; k < oppPawns.Length; k++)
                                    {
                                        if (Vector3.Distance(cages[oIndex - j].transform.position, oppPawns[k].transform.position) < 0.1f && IsEmptyCage(cages[oIndex - j - 1]) && origin.transform.position != new Vector3(10000f, 10000f, 0))
                                        {
                                            removeObjects.Add(origin);
                                        }
                                    }
                                }
                            }
                        }

                        if (oIndex % 5 == 0 || oIndex % 5 == 1 || oIndex % 5 == 2)
                        {
                            // Check mePawns
                            for (int j = 1; j < 4 - oIndex % 5; j++)
                            {
                                if (oIndex % 5 == 0 && j == 3)
                                {
                                    continue;
                                }

                                for (int k = 0; k < mePawns.Length; k++)
                                {
                                    if (Vector3.Distance(cages[oIndex + j].transform.position, mePawns[k].transform.position) < 0.1f)
                                    {
                                        isSkip = true;
                                        break;
                                    }
                                }

                                if (!isSkip && !IsEmptyCage(cages[oIndex + j]) && !IsEmptyCage(cages[oIndex + j + 1]))
                                {
                                    isSkip = true;
                                    break;
                                }
                            }

                            // Find oppPawns
                            if (!isSkip)
                            {
                                for (int j = 1; j < 4 - oIndex % 5; j++)
                                {
                                    for (int k = 0; k < oppPawns.Length; k++)
                                    {
                                        if (Vector3.Distance(cages[oIndex + j].transform.position, oppPawns[k].transform.position) < 0.1f && IsEmptyCage(cages[oIndex + j + 1]) && origin.transform.position != new Vector3(10000f, 10000f, 0))
                                        {
                                            removeObjects.Add(origin);
                                        }
                                    }
                                }
                            }
                        }

                        if (oIndex / 5 == 2 || oIndex / 5 == 3 || oIndex / 5 == 4)
                        {
                            // Check mePawns
                            for (int j = 1; j < oIndex / 5; j++)
                            {
                                if (oIndex / 5 == 4 && j == 3)
                                {
                                    continue;
                                }

                                for (int k = 0; k < mePawns.Length; k++)
                                {
                                    if (Vector3.Distance(cages[oIndex - j * 5].transform.position, mePawns[k].transform.position) < 0.1f)
                                    {
                                        isSkip = true;
                                        break;
                                    }
                                }

                                if (!isSkip && !IsEmptyCage(cages[oIndex - j * 5]) && !IsEmptyCage(cages[oIndex - (j + 1) * 5]))
                                {
                                    isSkip = true;
                                    break;
                                }
                            }

                            // Find oppPawns
                            if (!isSkip)
                            {
                                for (int j = 1; j < oIndex / 5; j++)
                                {
                                    for (int k = 0; k < oppPawns.Length; k++)
                                    {
                                        if (Vector3.Distance(cages[oIndex - j * 5].transform.position, oppPawns[k].transform.position) < 0.1f && IsEmptyCage(cages[oIndex - (j + 1) * 5]) && origin.transform.position != new Vector3(10000f, 10000f, 0))
                                        {
                                            removeObjects.Add(origin);
                                        }
                                    }
                                }
                            }
                        }

                        if (oIndex / 5 == 0 || oIndex / 5 == 1 || oIndex / 5 == 2)
                        {
                            // Check mePawns
                            for (int j = 1; j < 4 - oIndex / 5; j++)
                            {
                                if (oIndex / 5 == 0 && j == 3)
                                {
                                    continue;
                                }

                                for (int k = 0; k < mePawns.Length; k++)
                                {
                                    if (Vector3.Distance(cages[oIndex + j * 5].transform.position, mePawns[k].transform.position) < 0.1f)
                                    {
                                        isSkip = true;
                                        break;
                                    }
                                }

                                if (!isSkip && !IsEmptyCage(cages[oIndex + j * 5]) && !IsEmptyCage(cages[oIndex + (j + 1) * 5]))
                                {
                                    isSkip = true;
                                    break;
                                }
                            }

                            // Find oppPawns
                            if (!isSkip)
                            {
                                for (int j = 1; j < 4 - oIndex / 5; j++)
                                {
                                    for (int k = 0; k < oppPawns.Length; k++)
                                    {
                                        if (Vector3.Distance(cages[oIndex + j * 5].transform.position, oppPawns[k].transform.position) < 0.1f && IsEmptyCage(cages[oIndex + (j + 1) * 5]) && origin.transform.position != new Vector3(10000f, 10000f, 0))
                                        {
                                            removeObjects.Add(origin);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Opponent)
        {
            for (int i = 0; i < oppPawns.Length; i++)
            {
                if (Vector3.Distance(oppPawns[i].transform.position, origin.transform.position) < 0.1f)
                {
                    if (oppPawnStyles[i] == (int)PawnStyle.king)
                    {
                        isSkip = false;

                        if (oIndex % 5 == 2 || oIndex % 5 == 3 || oIndex % 5 == 4)
                        {
                            // Check mePawns
                            for (int j = 1; j < oIndex % 5; j++)
                            {
                                if (oIndex % 5 == 4 && j == 3)
                                {
                                    continue;
                                }

                                for (int k = 0; k < oppPawns.Length; k++)
                                {
                                    if (Vector3.Distance(cages[oIndex - j].transform.position, oppPawns[k].transform.position) < 0.1f)
                                    {
                                        isSkip = true;
                                        break;
                                    }
                                }

                                if (!isSkip && !IsEmptyCage(cages[oIndex - j]) && !IsEmptyCage(cages[oIndex - j - 1]))
                                {
                                    isSkip = true;
                                    break;
                                }
                            }

                            // Find oppPawns
                            if (!isSkip)
                            {
                                for (int j = 1; j < oIndex % 5; j++)
                                {
                                    for (int k = 0; k < mePawns.Length; k++)
                                    {
                                        if (Vector3.Distance(cages[oIndex - j].transform.position, mePawns[k].transform.position) < 0.1f && IsEmptyCage(cages[oIndex - j - 1]) && origin.transform.position != new Vector3(10000f, 10000f, 0))
                                        {
                                            removeObjects.Add(origin);
                                        }
                                    }
                                }
                            }
                        }

                        if (oIndex % 5 == 0 || oIndex % 5 == 1 || oIndex % 5 == 2)
                        {
                            // Check mePawns
                            for (int j = 1; j < 4 - oIndex % 5; j++)
                            {
                                if (oIndex % 5 == 0 && j == 3)
                                {
                                    continue;
                                }

                                for (int k = 0; k < oppPawns.Length; k++)
                                {
                                    if (Vector3.Distance(cages[oIndex + j].transform.position, oppPawns[k].transform.position) < 0.1f)
                                    {
                                        isSkip = true;
                                        break;
                                    }
                                }

                                if (!isSkip && !IsEmptyCage(cages[oIndex + j]) && !IsEmptyCage(cages[oIndex + j + 1]))
                                {
                                    isSkip = true;
                                    break;
                                }
                            }

                            // Find oppPawns
                            if (!isSkip)
                            {
                                for (int j = 1; j < 4 - oIndex % 5; j++)
                                {
                                    for (int k = 0; k < mePawns.Length; k++)
                                    {
                                        if (Vector3.Distance(cages[oIndex + j].transform.position, mePawns[k].transform.position) < 0.1f && IsEmptyCage(cages[oIndex + j + 1]) && origin.transform.position != new Vector3(10000f, 10000f, 0))
                                        {
                                            removeObjects.Add(origin);
                                        }
                                    }
                                }
                            }
                        }

                        if (oIndex / 5 == 2 || oIndex / 5 == 3 || oIndex / 5 == 4)
                        {
                            // Check mePawns
                            for (int j = 1; j < oIndex / 5; j++)
                            {
                                if (oIndex / 5 == 4 && j == 3)
                                {
                                    continue;
                                }

                                for (int k = 0; k < oppPawns.Length; k++)
                                {
                                    if (Vector3.Distance(cages[oIndex - j * 5].transform.position, oppPawns[k].transform.position) < 0.1f)
                                    {
                                        isSkip = true;
                                        break;
                                    }
                                }

                                if (!isSkip && !IsEmptyCage(cages[oIndex - j * 5]) && !IsEmptyCage(cages[oIndex - (j + 1) * 5]))
                                {
                                    isSkip = true;
                                    break;
                                }
                            }

                            // Find oppPawns
                            if (!isSkip)
                            {
                                for (int j = 1; j < oIndex / 5; j++)
                                {
                                    for (int k = 0; k < mePawns.Length; k++)
                                    {
                                        if (Vector3.Distance(cages[oIndex - j * 5].transform.position, mePawns[k].transform.position) < 0.1f && IsEmptyCage(cages[oIndex - (j + 1) * 5]) && origin.transform.position != new Vector3(10000f, 10000f, 0))
                                        {
                                            removeObjects.Add(origin);
                                        }
                                    }
                                }
                            }
                        }

                        if (oIndex / 5 == 0 || oIndex / 5 == 1 || oIndex / 5 == 2)
                        {
                            // Check mePawns
                            for (int j = 1; j < 4 - oIndex / 5; j++)
                            {
                                if (oIndex / 5 == 0 && j == 3)
                                {
                                    continue;
                                }

                                for (int k = 0; k < oppPawns.Length; k++)
                                {
                                    if (Vector3.Distance(cages[oIndex + j * 5].transform.position, oppPawns[k].transform.position) < 0.1f)
                                    {
                                        isSkip = true;
                                        break;
                                    }
                                }

                                if (!isSkip && !IsEmptyCage(cages[oIndex + j * 5]) && !IsEmptyCage(cages[oIndex + (j + 1) * 5]))
                                {
                                    isSkip = true;
                                    break;
                                }
                            }

                            // Find oppPawns
                            if (!isSkip)
                            {
                                for (int j = 1; j < 4 - oIndex / 5; j++)
                                {
                                    for (int k = 0; k < mePawns.Length; k++)
                                    {
                                        if (Vector3.Distance(cages[oIndex + j * 5].transform.position, mePawns[k].transform.position) < 0.1f && IsEmptyCage(cages[oIndex + (j + 1) * 5]) && origin.transform.position != new Vector3(10000f, 10000f, 0))
                                        {
                                            removeObjects.Add(origin);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public bool IsPlayTwice(GameObject origin)
    {
        isDoublePlay = false;

        for (int i = 0; i < cages.Length; i++)
        {
            if (Vector3.Distance(cages[i].transform.position, origin.transform.position) < 0.1f)
            {
                oIndex = i;
            }
        }

        if (whoseTurnIndex == (int)WhoseTurn.Me && PhotonNetwork.IsMasterClient)
        {
            isDoublePlay = false;

            for (int j = 0; j < mePawns.Length; j++)
            {
                if (Vector3.Distance(mePawns[j].transform.position, origin.transform.position) < 0.1f)
                {
                    if (mePawnStyles[j] == (int)PawnStyle.normal)
                    {
                        for (int i = 0; i < oppPawns.Length; i++)
                        {
                            if (oIndex % 5 == 2 || oIndex % 5 == 3 || oIndex % 5 == 4)
                            {
                                if (Vector3.Distance(cages[oIndex - 1].transform.position, oppPawns[i].transform.position) < 0.1f && IsEmptyCage(cages[oIndex - 2]))
                                {
                                    isDoublePlay = true;
                                }
                            }

                            if (oIndex % 5 == 0 || oIndex % 5 == 1 || oIndex % 5 == 2)
                            {
                                if (Vector3.Distance(cages[oIndex + 1].transform.position, oppPawns[i].transform.position) < 0.1f && IsEmptyCage(cages[oIndex + 2]))
                                {
                                    isDoublePlay = true;
                                }
                            }

                            if (oIndex / 5 == 2 || oIndex / 5 == 3 || oIndex / 5 == 4)
                            {
                                if (Vector3.Distance(cages[oIndex - 5].transform.position, oppPawns[i].transform.position) < 0.1f && IsEmptyCage(cages[oIndex - 10]))
                                {
                                    isDoublePlay = true;
                                }
                            }
                        }
                    }
                    else if (mePawnStyles[j] == (int)PawnStyle.king)
                    {
                        for (int i = 0; i < cages.Length; i++)
                        {
                            if (Vector3.Distance(cages[i].transform.position, targetCage.transform.position) < 0.1f)
                            {
                                kingIndex = i;
                            }
                        }

                        if (kingIndex % 5 == 2 || kingIndex % 5 == 3 || kingIndex % 5 == 4)
                        {
                            isSkip = false;

                            // Check mePawns
                            for (int i = 1; i < kingIndex % 5; i++)
                            {
                                if (kingIndex % 5 == 4 && i == 3)
                                {
                                    continue;
                                }

                                for (int k = 0; k < mePawns.Length; k++)
                                {
                                    if (Vector3.Distance(cages[kingIndex - i].transform.position, mePawns[k].transform.position) < 0.1f)
                                    {
                                        isSkip = true;
                                        break;
                                    }
                                }

                                if (!isSkip && !IsEmptyCage(cages[kingIndex - i]) && !IsEmptyCage(cages[kingIndex - i - 1]))
                                {
                                    isSkip = true;
                                    break;
                                }
                            }

                            // Find oppPawns
                            if (!isSkip)
                            {
                                for (int i = 1; i < kingIndex % 5; i++)
                                {
                                    for (int k = 0; k < oppPawns.Length; k++)
                                    {
                                        if (Vector3.Distance(cages[kingIndex - i].transform.position, oppPawns[k].transform.position) < 0.1f && IsEmptyCage(cages[kingIndex - i - 1]))
                                        {
                                            isDoublePlay = true;
                                        }
                                    }
                                }
                            }
                        }

                        if (kingIndex % 5 == 0 || kingIndex % 5 == 1 || kingIndex % 5 == 2)
                        {
                            isSkip = false;

                            // Check mePawns
                            for (int i = 1; i < 4 - kingIndex % 5; i++)
                            {
                                if (kingIndex % 5 == 0 && i == 3)
                                {
                                    continue;
                                }

                                for (int k = 0; k < mePawns.Length; k++)
                                {
                                    if (Vector3.Distance(cages[kingIndex + i].transform.position, mePawns[k].transform.position) < 0.1f)
                                    {
                                        isSkip = true;
                                        break;
                                    }
                                }

                                if (!isSkip && !IsEmptyCage(cages[kingIndex + i]) && !IsEmptyCage(cages[kingIndex + i + 1]))
                                {
                                    isSkip = true;
                                    break;
                                }
                            }

                            // Find oppPawns
                            if (!isSkip)
                            {
                                for (int i = 1; i < 4 - kingIndex % 5; i++)
                                {
                                    for (int k = 0; k < oppPawns.Length; k++)
                                    {
                                        if (Vector3.Distance(cages[kingIndex + i].transform.position, oppPawns[k].transform.position) < 0.1f && IsEmptyCage(cages[kingIndex + i + 1]))
                                        {
                                            isDoublePlay = true;
                                        }
                                    }
                                }
                            }
                        }

                        if (kingIndex / 5 == 2 || kingIndex / 5 == 3 || kingIndex / 5 == 4)
                        {
                            isSkip = false;

                            // Check mePawns
                            for (int i = 1; i < kingIndex / 5; i++)
                            {
                                if (kingIndex / 5 == 4 && i == 3)
                                {
                                    continue;
                                }

                                for (int k = 0; k < mePawns.Length; k++)
                                {
                                    if (Vector3.Distance(cages[kingIndex - i * 5].transform.position, mePawns[k].transform.position) < 0.1f)
                                    {
                                        isSkip = true;
                                        break;
                                    }
                                }

                                if (!isSkip && !IsEmptyCage(cages[kingIndex - i * 5]) && !IsEmptyCage(cages[kingIndex - (i + 1) * 5]))
                                {
                                    isSkip = true;
                                    break;
                                }
                            }

                            // Find oppPawns
                            if (!isSkip)
                            {
                                for (int i = 1; i < kingIndex / 5; i++)
                                {
                                    for (int k = 0; k < oppPawns.Length; k++)
                                    {
                                        if (Vector3.Distance(cages[kingIndex - i * 5].transform.position, oppPawns[k].transform.position) < 0.1f && IsEmptyCage(cages[kingIndex - (i + 1) * 5]))
                                        {
                                            isDoublePlay = true;
                                        }
                                    }
                                }
                            }
                        }

                        if (kingIndex / 5 == 0 || kingIndex / 5 == 1 || kingIndex / 5 == 2)
                        {
                            // Check mePawns
                            for (int i = 1; i < 4 - kingIndex / 5; i++)
                            {
                                if (kingIndex / 5 == 0 && i == 3)
                                {
                                    continue;
                                }

                                for (int k = 0; k < mePawns.Length; k++)
                                {
                                    if (Vector3.Distance(cages[kingIndex + i * 5].transform.position, mePawns[k].transform.position) < 0.1f)
                                    {
                                        isSkip = true;
                                        break;
                                    }
                                }

                                if (!isSkip && !IsEmptyCage(cages[kingIndex + i * 5]) && !IsEmptyCage(cages[kingIndex + (i + 1) * 5]))
                                {
                                    isSkip = true;
                                    break;
                                }
                            }

                            // Find oppPawns
                            if (!isSkip)
                            {
                                for (int i = 1; i < 4 - kingIndex / 5; i++)
                                {
                                    for (int k = 0; k < oppPawns.Length; k++)
                                    {
                                        if (Vector3.Distance(cages[kingIndex + i * 5].transform.position, oppPawns[k].transform.position) < 0.1f && IsEmptyCage(cages[kingIndex + (i + 1) * 5]))
                                        {
                                            isDoublePlay = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Opponent && !PhotonNetwork.IsMasterClient)
        {
            isDoublePlay = false;

            for (int j = 0; j < oppPawns.Length; j++)
            {
                if (Vector3.Distance(oppPawns[j].transform.position, origin.transform.position) < 0.1f)
                {
                    if (oppPawnStyles[j] == (int)PawnStyle.normal)
                    {
                        for (int i = 0; i < mePawns.Length; i++)
                        {
                            if (oIndex % 5 == 2 || oIndex % 5 == 3 || oIndex % 5 == 4)
                            {
                                if (Vector3.Distance(cages[oIndex - 1].transform.position, mePawns[i].transform.position) < 0.1f && IsEmptyCage(cages[oIndex - 2]))
                                {
                                    isDoublePlay = true;
                                }
                            }

                            if (oIndex % 5 == 0 || oIndex % 5 == 1 || oIndex % 5 == 2)
                            {
                                if (Vector3.Distance(cages[oIndex + 1].transform.position, mePawns[i].transform.position) < 0.1f && IsEmptyCage(cages[oIndex + 2]))
                                {
                                    isDoublePlay = true;
                                }
                            }

                            if (oIndex / 5 == 0 || oIndex / 5 == 1 || oIndex / 5 == 2)
                            {
                                if (Vector3.Distance(cages[oIndex + 5].transform.position, mePawns[i].transform.position) < 0.1f && IsEmptyCage(cages[oIndex + 10]))
                                {
                                    isDoublePlay = true;
                                }
                            }
                        }
                    }
                    else if (oppPawnStyles[j] == (int)PawnStyle.king)
                    {
                        for (int i = 0; i < cages.Length; i++)
                        {
                            if (Vector3.Distance(cages[i].transform.position, targetCage.transform.position) < 0.1f)
                            {
                                kingIndex = i;
                            }
                        }

                        if (kingIndex % 5 == 2 || kingIndex % 5 == 3 || kingIndex % 5 == 4)
                        {
                            isSkip = false;

                            // Check mePawns
                            for (int i = 1; i < kingIndex % 5; i++)
                            {
                                if (kingIndex % 5 == 4 && i == 3)
                                {
                                    continue;
                                }

                                for (int k = 0; k < oppPawns.Length; k++)
                                {
                                    if (Vector3.Distance(cages[kingIndex - i].transform.position, oppPawns[k].transform.position) < 0.1f)
                                    {
                                        isSkip = true;
                                        break;
                                    }
                                }

                                if (!isSkip && !IsEmptyCage(cages[kingIndex - i]) && !IsEmptyCage(cages[kingIndex - i - 1]))
                                {
                                    isSkip = true;
                                    break;
                                }
                            }

                            // Find oppPawns
                            if (!isSkip)
                            {
                                for (int i = 1; i < kingIndex % 5; i++)
                                {
                                    for (int k = 0; k < mePawns.Length; k++)
                                    {
                                        if (Vector3.Distance(cages[kingIndex - i].transform.position, mePawns[k].transform.position) < 0.1f && IsEmptyCage(cages[kingIndex - i - 1]))
                                        {
                                            isDoublePlay = true;
                                        }
                                    }
                                }
                            }
                        }

                        if (kingIndex % 5 == 0 || kingIndex % 5 == 1 || kingIndex % 5 == 2)
                        {
                            isSkip = false;

                            // Check mePawns
                            for (int i = 1; i < 4 - kingIndex % 5; i++)
                            {
                                if (kingIndex % 5 == 0 && i == 3)
                                {
                                    continue;
                                }

                                for (int k = 0; k < oppPawns.Length; k++)
                                {
                                    if (Vector3.Distance(cages[kingIndex + i].transform.position, oppPawns[k].transform.position) < 0.1f)
                                    {
                                        isSkip = true;
                                        break;
                                    }
                                }

                                if (!isSkip && !IsEmptyCage(cages[kingIndex + i]) && !IsEmptyCage(cages[kingIndex + i + 1]))
                                {
                                    isSkip = true;
                                    break;
                                }
                            }

                            // Find oppPawns
                            if (!isSkip)
                            {
                                for (int i = 1; i < 4 - kingIndex % 5; i++)
                                {
                                    for (int k = 0; k < mePawns.Length; k++)
                                    {
                                        if (Vector3.Distance(cages[kingIndex + i].transform.position, mePawns[k].transform.position) < 0.1f && IsEmptyCage(cages[kingIndex + i + 1]))
                                        {
                                            isDoublePlay = true;
                                        }
                                    }
                                }
                            }
                        }

                        if (kingIndex / 5 == 2 || kingIndex / 5 == 3 || kingIndex / 5 == 4)
                        {
                            isSkip = false;

                            // Check mePawns
                            for (int i = 1; i < kingIndex / 5; i++)
                            {
                                if (kingIndex / 5 == 4 && i == 3)
                                {
                                    continue;
                                }

                                for (int k = 0; k < oppPawns.Length; k++)
                                {
                                    if (Vector3.Distance(cages[kingIndex - i * 5].transform.position, oppPawns[k].transform.position) < 0.1f)
                                    {
                                        isSkip = true;
                                        break;
                                    }
                                }

                                if (!isSkip && !IsEmptyCage(cages[kingIndex - i * 5]) && !IsEmptyCage(cages[kingIndex - (i + 1) * 5]))
                                {
                                    isSkip = true;
                                    break;
                                }
                            }

                            // Find oppPawns
                            if (!isSkip)
                            {
                                for (int i = 1; i < kingIndex / 5; i++)
                                {
                                    for (int k = 0; k < mePawns.Length; k++)
                                    {
                                        if (Vector3.Distance(cages[kingIndex - i * 5].transform.position, mePawns[k].transform.position) < 0.1f && IsEmptyCage(cages[kingIndex - (i + 1) * 5]))
                                        {
                                            isDoublePlay = true;
                                        }
                                    }
                                }
                            }
                        }

                        if (kingIndex / 5 == 0 || kingIndex / 5 == 1 || kingIndex / 5 == 2)
                        {
                            isSkip = false;

                            // Check mePawns
                            for (int i = 1; i < 4 - kingIndex / 5; i++)
                            {
                                if (kingIndex / 5 == 0 && i == 3)
                                {
                                    continue;
                                }

                                for (int k = 0; k < oppPawns.Length; k++)
                                {
                                    if (Vector3.Distance(cages[kingIndex + i * 5].transform.position, oppPawns[k].transform.position) < 0.1f)
                                    {
                                        isSkip = true;
                                        break;
                                    }
                                }

                                if (!isSkip && !IsEmptyCage(cages[kingIndex + i * 5]) && !IsEmptyCage(cages[kingIndex + (i + 1) * 5]))
                                {
                                    isSkip = true;
                                    break;
                                }
                            }

                            // Find oppPawns
                            if (!isSkip)
                            {
                                for (int i = 1; i < 4 - kingIndex / 5; i++)
                                {
                                    for (int k = 0; k < mePawns.Length; k++)
                                    {
                                        if (Vector3.Distance(cages[kingIndex + i * 5].transform.position, mePawns[k].transform.position) < 0.1f && IsEmptyCage(cages[kingIndex + (i + 1) * 5]))
                                        {
                                            isDoublePlay = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return isDoublePlay;
    }

    public void KillEnemyPawn()
    {
        for (int i = 0; i < cages.Length; i++)
        {
            if (Vector3.Distance(cages[i].transform.position, origincage.transform.position) < 0.1f)
            {
                oIndex = i;
            }

            if (Vector3.Distance(cages[i].transform.position, targetCage.transform.position) < 0.1f)
            {
                tIndex = i;
            }
        }

        if (whoseTurnIndex == (int)WhoseTurn.Me && movePawnStyle == (int)PawnStyle.normal && PhotonNetwork.IsMasterClient)
        {
            if (tIndex == oIndex - 2)
            {
                for (int i = 0; i < oppPawns.Length; i++)
                {
                    if (Vector3.Distance(cages[oIndex - 1].transform.position, oppPawns[i].transform.position) < 0.1f)
                    {
                        oppPawns[i].transform.position = new Vector3(100000f, 100000f, 0);
                        SendPosition(new object[] { new Vector3(10000f, 10000f, 0), 1, i });
                    }
                }
            }
            else if (tIndex == oIndex + 2)
            {
                for (int i = 0; i < oppPawns.Length; i++)
                {
                    if (Vector3.Distance(cages[oIndex + 1].transform.position, oppPawns[i].transform.position) < 0.1f)
                    {
                        oppPawns[i].transform.position = new Vector3(100000f, 100000f, 0);
                        SendPosition(new object[] { new Vector3(10000f, 10000f, 0), 1, i });
                    }
                }
            }
            else if (tIndex == oIndex - 10)
            {
                for (int i = 0; i < oppPawns.Length; i++)
                {
                    if (Vector3.Distance(cages[oIndex - 5].transform.position, oppPawns[i].transform.position) < 0.1f)
                    {
                        oppPawns[i].transform.position = new Vector3(100000f, 100000f, 0);
                        SendPosition(new object[] { new Vector3(10000f, 10000f, 0), 1, i });
                    }
                }
            }
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Opponent && movePawnStyle == (int)PawnStyle.normal && !PhotonNetwork.IsMasterClient)
        {
            if (tIndex == oIndex - 2)
            {
                for (int i = 0; i < mePawns.Length; i++)
                {
                    if (Vector3.Distance(cages[oIndex - 1].transform.position, mePawns[i].transform.position) < 0.1f)
                    {
                        mePawns[i].transform.position = new Vector3(100000f, 100000f, 0);
                        SendPosition(new object[] { new Vector3(10000f, 10000f, 0), 0, i });
                    }
                }
            }
            else if (tIndex == oIndex + 2)
            {
                for (int i = 0; i < mePawns.Length; i++)
                {
                    if (Vector3.Distance(cages[oIndex + 1].transform.position, mePawns[i].transform.position) < 0.1f)
                    {
                        mePawns[i].transform.position = new Vector3(100000f, 100000f, 0);
                        SendPosition(new object[] { new Vector3(10000f, 10000f, 0), 0, i });
                    }
                }
            }
            else if (tIndex == oIndex + 10)
            {
                for (int i = 0; i < mePawns.Length; i++)
                {
                    if (Vector3.Distance(cages[oIndex + 5].transform.position, mePawns[i].transform.position) < 0.1f)
                    {
                        mePawns[i].transform.position = new Vector3(100000f, 100000f, 0);
                        SendPosition(new object[] { new Vector3(10000f, 10000f, 0), 0, i });
                    }
                }
            }
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Me && movePawnStyle == (int)PawnStyle.king && PhotonNetwork.IsMasterClient)
        {
            int moveIndex = 0;

            for (int i = 0; i < 24; i++)
            {
                if (filpPawnForKing == allPawns[i])
                {
                    moveIndex = i;
                    break;
                }
            }

            allPawns[moveIndex].transform.position = new Vector3(100000f, 100000f, 0);
            SendPosition(new object[] { new Vector3(10000f, 10000f, 0), 2, moveIndex });
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Opponent && movePawnStyle == (int)PawnStyle.king && !PhotonNetwork.IsMasterClient)
        {
            int moveIndex = 0;

            for (int i = 0; i < 24; i++)
            {
                if (filpPawnForKing == allPawns[i])
                {
                    moveIndex = i;
                    break;
                }
            }

            allPawns[moveIndex].transform.position = new Vector3(100000f, 100000f, 0);
            SendPosition(new object[] { new Vector3(10000f, 10000f, 0), 2, moveIndex });
        }
    }

    public void LostMyPawn(int turnIndex)
    {
        removeObjectsInfo = "";

        for (int i = 0; i < removeObjects.Count; i++)
        {                 
            for(int j = 0; j < allPawns.Length; j++)
            {
                if (Vector3.Distance(removeObjects[i].transform.position, allPawns[j].transform.position) < 0.1f)
                {
                    if (i == 0)
                    {
                        removeObjectsInfo = "" + j;
                    }
                    else
                    {
                        removeObjectsInfo += "-" + j;
                    }                                    
                }               
            }
        }

        SendRemoveObjectsPos(new object[] {removeObjectsInfo, turnIndex });
    }

    public void SendRemoveObj()
    {
        removeObjectsInfo = "";

        for (int i = 0; i < removeObjects.Count; i++)
        {
            for (int j = 0; j < allPawns.Length; j++)
            {
                if (Vector3.Distance(removeObjects[i].transform.position, allPawns[j].transform.position) < 0.1f)
                {
                    if (i == 0)
                    {
                        removeObjectsInfo = "" + j;
                    }
                    else
                    {
                        removeObjectsInfo += "-" + j;
                    }
                }
            }
        }

        UpdateRemoveObjectsPos(new object[] { removeObjectsInfo });
    }

    public void TargetCage(Vector3 mousePosition)
    {
        for (int i = 0; i < cages.Length; i++)
        {
            if (Vector3.Distance(cages[i].transform.position, mousePosition) < 50f)
            {
                targetCage = cages[i];
            }
        }
    }

    public bool IsKing(GameObject cage)
    {
        isKing = false;

        if (whoseTurnIndex == (int)WhoseTurn.Me && movePawnStyle == (int)PawnStyle.normal && PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < 5; i++)
            {
                if (Vector3.Distance(cages[i].transform.position, cage.transform.position) < 0.1f)
                {
                    isKing = true;
                }
            }
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Opponent && movePawnStyle == (int)PawnStyle.normal && !PhotonNetwork.IsMasterClient)
        {
            for (int i = 20; i < 25; i++)
            {
                if (Vector3.Distance(cages[i].transform.position, cage.transform.position) < 0.1f)
                {
                    isKing = true;
                }
            }
        }

        return isKing;
    }

    public void SetKingPawn()
    {
        if (whoseTurnIndex == (int)WhoseTurn.Me && PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < mePawns.Length; i++)
            {
                if (Vector3.Distance(mePawns[i].transform.position, movePawn.transform.position) < 0.1f)
                {
                    SendKingPawn(new object[] { 0, i });
                }
            }
        }
        else if (whoseTurnIndex == (int)WhoseTurn.Opponent && !PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < oppPawns.Length; i++)
            {
                if (Vector3.Distance(oppPawns[i].transform.position, movePawn.transform.position) < 0.1f)
                {
                    SendKingPawn(new object[] { 1, i });
                }
            }
        }
    }

    public void BackBtnClick()
    {
        StartCoroutine(DelayGoToMainScene(0.5f));
    }

    #endregion

    #region Extra

    public void SendTurn(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(2, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendPosition(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(3, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendKingPawn(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(4, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendRemoveObjectsPos(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(5, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void UpdateRemoveObjectsPos(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(6, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void UpdateEvaluation(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(7, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendBoardInfo(object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(8, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SetOriginCage()
    {
        for (int i = 0; i < cages.Length; i++)
        {
            if (Vector3.Distance(cages[i].transform.position, movePawn.transform.position) < 0.1f)
            {
                origincage = cages[i];
            }
        }
    }

    #endregion


    #region MonoBehaviourPunCallbacks
    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected.");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SceneManager.LoadScene("Main");
    }    

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == 2)
        {
            object[] pos = (object[])photonEvent.CustomData;     

            whoseTurnIndex = (int)pos[0];       

            if(whoseTurnIndex == 0)
            {
                playerAvatars[0].GetComponent<Animator>().enabled = true;
                playerAvatars[1].GetComponent<Animator>().enabled = false;
            }
            else
            {
                playerAvatars[0].GetComponent<Animator>().enabled = false;
                playerAvatars[1].GetComponent<Animator>().enabled = true;                        
            }

            removeObjects = new List<GameObject>();
            if (whoseTurnIndex == 0 && !PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < mePawns.Length; i++)
                {
                    FindAvailableFlipPawn(mePawns[i]);
                    FindAvailableFlipKingPawn(mePawns[i]);
                }

                if (removeObjects.Count > 0)
                {
                    SendRemoveObj();
                }               
            }
            else if (whoseTurnIndex == 1 && PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < oppPawns.Length; i++)
                {
                    FindAvailableFlipPawn(oppPawns[i]);
                    FindAvailableFlipKingPawn(oppPawns[i]);
                }

                if (removeObjects.Count > 0)
                {
                    SendRemoveObj();
                }
            }
        }  
        else if(eventCode == 3)
        {
            object[] pos = (object[])photonEvent.CustomData;
            Vector3 targetPosition = (Vector3)pos[0];

            if ((int)pos[1] == 0)
            {
                mePawns[(int)pos[2]].transform.position = targetPosition;
            }
            else if ((int)pos[1] == 1)
            {
                oppPawns[(int)pos[2]].transform.position = targetPosition;
            }
            else if ((int)pos[1] == 2)
            {
                allPawns[(int)pos[2]].transform.position = targetPosition;
            }

            GameEvaluation();
        }
        else if(eventCode == 4)
        {
            object[] king = (object[])photonEvent.CustomData;

            if((int)king[0] == 0)
            {
                mePawnStyles[(int)king[1]] = (int)PawnStyle.king;
                mePawns[(int)king[1]].GetComponent<Image>().sprite = kingPawnSprites[0];
            }
            else if((int)king[0] == 1)
            {
                oppPawnStyles[(int)king[1]] = (int)PawnStyle.king;
                oppPawns[(int)king[1]].GetComponent<Image>().sprite = kingPawnSprites[1];
            }
        }
        else if(eventCode == 5)
        {
            object[] removeObjInfo = (object[])photonEvent.CustomData;

            string[] array = removeObjInfo[0].ToString().Split("-");

            for (int i = 0; i < array.Length; i++)
            {
               
            }

            if(array.Length > 0)
            {
                allPawns[int.Parse(array[0])].transform.position = new Vector3(10000f, 10000f, 0);
            }

            GameEvaluation();
        }
        else if (eventCode == 6)
        {
            removeObjects = new List<GameObject>();
            object[] removeObjInfo = (object[])photonEvent.CustomData;

            print(removeObjInfo[0].ToString());
            string[] array = removeObjInfo[0].ToString().Split("-");

            for (int i = 0; i < array.Length; i++)
            {
                removeObjects.Add(allPawns[int.Parse(array[i])]);                
            }
        }
        else if (eventCode == 7)
        {
            object[] evaluationInfo = (object[])photonEvent.CustomData;

            if ((int)evaluationInfo[0] == 0)
            {
                gameState = (int)GameState.End;
                endText.text = "player2 wins!";

                endUI.SetActive(true);
                playerAvatars[0].GetComponent<Animator>().enabled = false;
                playerAvatars[1].GetComponent<Animator>().enabled = false;

                StartCoroutine(DelayGoToMainScene(3f));
            }
            else if ((int)evaluationInfo[1] == 0)
            {
                gameState = (int)GameState.End;
                endText.text = "player1 wins!";

                endUI.SetActive(true);
                playerAvatars[0].GetComponent<Animator>().enabled = false;
                playerAvatars[1].GetComponent<Animator>().enabled = false;

                StartCoroutine(DelayGoToMainScene(3f));
            }
            else if ((int)evaluationInfo[0] == 1 && (int)evaluationInfo[1] == 1)
            {
                gameState = (int)GameState.Peace;
                endText.text = "Draw!";

                endUI.SetActive(true);
                playerAvatars[0].GetComponent<Animator>().enabled = false;
                playerAvatars[1].GetComponent<Animator>().enabled = false;

                StartCoroutine(DelayGoToMainScene(3f));
            }
        }
        else if(eventCode == 8)
        {
            object[] boardInfo = (object[])photonEvent.CustomData;

            for(int i = 0; i < 4; i++)
            {
                boards[i].SetActive(false);
            }
            
            boards[(int)boardInfo[0]].SetActive(true);
        }
    }
    #endregion
}
