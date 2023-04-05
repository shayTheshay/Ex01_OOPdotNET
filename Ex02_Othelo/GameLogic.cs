using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Ex02_Othelo
{
    public class GameLogic
    {
        private eBoardLocation[,] m_Board;
        private eBoardLocation currentTurn;
        private readonly string m_WhiteName;
        private readonly string m_BlackName;
        private List<int[]> m_LegalPlays;
        // Default constructor - 8x8 grid
        public GameLogic()
        {
            m_Board = new eBoardLocation[8, 8];
            m_Board[3, 3] = eBoardLocation.White;
            m_Board[4, 4] = eBoardLocation.White;
            m_Board[3, 4] = eBoardLocation.Black;
            m_Board[4, 3] = eBoardLocation.Black;
            currentTurn = eBoardLocation.Black;
            m_WhiteName = "White";
            m_BlackName = "Black";
            m_LegalPlays = CurrentLegalPlays();
        }
        public GameLogic(int i_Size,string i_WhiteName = "White",string i_BlackName = "Black")
        {
            m_Board = new eBoardLocation[i_Size, i_Size];
            m_Board[(i_Size/2)-1, (i_Size/2)-1] = eBoardLocation.White;
            m_Board[i_Size / 2, i_Size / 2] = eBoardLocation.White;
            m_Board[(i_Size / 2) - 1, i_Size / 2] = eBoardLocation.Black;
            m_Board[i_Size / 2, (i_Size / 2) - 1] = eBoardLocation.Black;
            currentTurn = eBoardLocation.Black;
            m_WhiteName = i_WhiteName;
            m_BlackName = i_BlackName;
            m_LegalPlays = CurrentLegalPlays();
        }
        public void ResetBoard()
        {
            int size = m_Board.GetLength(0);
            m_Board = new eBoardLocation[size, size];
            m_Board[(size / 2) - 1, (size / 2) - 1] = eBoardLocation.White;
            m_Board[size / 2, size / 2] = eBoardLocation.White;
            m_Board[(size / 2) - 1, size / 2] = eBoardLocation.Black;
            m_Board[size / 2, (size / 2) - 1] = eBoardLocation.Black;
            currentTurn = eBoardLocation.Black;
        }
        public eBoardLocation[,] GetBoard()
        {
            return m_Board;
        }
        public eBoardLocation GetCurrrentTurn()
        {
            return currentTurn;
        }
        private List<int[]> CurrentLegalPlays()
        {
            // m_CurrentLegalPlays returns a list of coordinates that are legal to play,
            //each coordinate is an array where the first element
            // is the X coordinate and the second element is the Y coordinate.
            List<int[]> temp = new List<int[]>();
            for(int i = 0; i < m_Board.GetLength(0); i++)
            {
                for(int j = 0; j < m_Board.GetLength(1); j++)
                {
                    //Check each direction for legality, then if atleast one of them is legal than the whole move is legal
                    bool north = checkDirection(i, j, -1, 0);
                    bool south = checkDirection(i, j, 1, 0);
                    bool east = checkDirection(i, j, 0, 1);
                    bool west = checkDirection(i, j, 0, -1);
                    bool northEast = checkDirection(i, j, -1, 1);
                    bool northWest = checkDirection(i, j, -1, -1);
                    bool southEast = checkDirection(i, j, 1, 1);
                    bool southWest = checkDirection(i, j, 1, -1);
                    bool isFreeSpace = m_Board[i, j] == eBoardLocation.Empty;
                    if((north || south || east || west || southEast || southWest || northEast || northWest) && true == isFreeSpace)
                    {
                        int[] toAdd = { i, j };
                        temp.Add(toAdd);
                    }
                }
            }
            return temp;
        }
        private bool checkDirection(int i_X, int i_Y, int i_XOffset, int i_YOffset)
        {
            bool legality = false;
            eBoardLocation opposingLocation = eBoardLocation.Black;
            if (currentTurn == eBoardLocation.Black)
                opposingLocation = eBoardLocation.White;
            //first condition: point of direction is in bounds
            bool basicLegality = inBounds(i_X + i_XOffset, i_Y + i_YOffset);
            if (basicLegality == true)
            {
                //second condition: adjacent move in direction is actually the opposing player
                basicLegality = m_Board[i_X + i_XOffset, i_Y + i_YOffset] == opposingLocation;
            }
            int accXOffset = i_XOffset, accYOffset = i_YOffset;
            accYOffset += i_YOffset;
            accXOffset += i_XOffset;
            //If the basic legality is true, AKA there is an adjacent opposing piece, the second test is to check
            //if there is a friendly piece on the other side.
            if (basicLegality == true)
            {
                while (inBounds(i_X+accXOffset,i_Y+accYOffset) && basicLegality == true)
                {
                    //Condition for false: we reach an empty space
                    if (m_Board[i_X + accXOffset, i_Y + accYOffset] == eBoardLocation.Empty)
                    {
                        break;
                    }
                    if (m_Board[i_X + accXOffset, i_Y + accYOffset] == currentTurn)
                    {
                        legality = true;
                        break;
                    }
                    accYOffset += i_YOffset;
                    accXOffset += i_XOffset;
                }
            }
            return legality;
        }
        private void updateDirection(int i_X, int i_Y, int i_XOffset, int i_YOffset)
        {
            //Function changes every opposing piece to friendly piece until a friendly piece is reached.
            //Note: This function assumes legality of direction tested previously
            eBoardLocation opposingLocation = eBoardLocation.Black;
            if (currentTurn == eBoardLocation.Black)
                opposingLocation = eBoardLocation.White;
            int accXOffset = i_XOffset, accYOffset = i_YOffset;
            while (inBounds(i_X + accXOffset, i_Y + accYOffset))
            {
                if (m_Board[i_X + accXOffset, i_Y + accYOffset] == opposingLocation)
                {
                    m_Board[i_X + accXOffset, i_Y + accYOffset] = currentTurn;
                    accYOffset += i_YOffset;
                    accXOffset += i_XOffset;
                    System.Threading.Thread.Sleep(200);
                }
                else
                    break;
            }
        }
        private bool checkLegalityOfPlay(int i_X, int i_Y)
        {
            //Function that checks if the play is legal, first by coordinate being in legal range
            //and then by standard Othello legality rules
            bool returnValue = false;
            int[] coordinate = { i_X, i_Y };
            if (inBounds(i_X,i_Y) && m_Board[i_X,i_Y] == eBoardLocation.Empty)
            {
                // If coordinate in bounds, check if the move chosen is amongst the legal plays.
                int[] found = m_LegalPlays.Find(item => item[0] == coordinate[0] && item[1] == coordinate[1]);
                if (found !=  null)
                    returnValue = true;
            }
            return returnValue;
        }
        public bool inBounds(int i_X,int i_Y)
        {
            return i_X < m_Board.GetLength(0) && i_Y < m_Board.GetLength(1) && i_X >= 0 && i_Y >= 0;
        }
        public bool PlayMove(int i_X, int i_Y)
        {
            //Main flow: Check legality, then if the move is legal place it, update the directions coming from it 
            //and change the player turn
            if(checkLegalityOfPlay(i_X, i_Y) == false)
            {
                Console.WriteLine("Illegal move. Please insert a new move.");
                return false;
            }
            for (int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    if(i != j || i != 0)
                    {
                        if(checkDirection(i_X, i_Y, i, j) == true)
                        {
                            updateDirection(i_X,i_Y, i,j);
                        }
                    }
                }
            }
            m_Board[i_X,i_Y] = currentTurn;
            if(currentTurn == eBoardLocation.Black)
            {
                currentTurn = eBoardLocation.White;
            }
            else
            {
                currentTurn = eBoardLocation.Black;
            }
            m_LegalPlays = CurrentLegalPlays();
            return true;
        }
        public int[] ChooseRandomMove()
        {
            Random rnd = new Random();
            int[] randomValue = m_LegalPlays[rnd.Next(0, m_LegalPlays.Count())];
            return randomValue;
        }
        public string CheckWinner()
        {
            string winner = null;
            if (m_LegalPlays.Count == 0)
            {
                int blackScore = 0;
                int whiteScore = 0;
                for (int i = 0; i < m_Board.GetLength(0); i++)
                {
                    for (int j = 0; j < m_Board.GetLength(1); j++)
                    {
                        if (m_Board[i, j] == eBoardLocation.Black)
                            blackScore++;
                        if (m_Board[i, j] == eBoardLocation.White)
                            whiteScore++;
                    }
                }
                if (blackScore > whiteScore)
                    winner = m_BlackName;
                if (whiteScore > blackScore)
                    winner = m_WhiteName;
            }
            return winner;
        }
     public enum eBoardLocation
        {
            Empty,
            Black,
            White
        }
    }
}
