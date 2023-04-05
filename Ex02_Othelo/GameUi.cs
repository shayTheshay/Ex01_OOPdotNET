using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ex02_Othelo.GameLogic;

namespace Ex02_Othelo
{
    public class GameUi
    {
        private string m_BlackName;
        private string m_WhiteName;
        private e_GameMode m_GameMode;
        private int m_SizeOfPlaying;
        private GameLogic m_Logic;
        public void GameMainLoop()
        {
            bool wasLastPlayLegal = true;
            bool gameOver = false;
            string winner = "";
            while (false == gameOver)
            {
                Ex02.ConsoleUtils.Screen.Clear();
                if (false == wasLastPlayLegal)
                    Console.WriteLine("Illegal move, please try another");
                PrintBoard(m_Logic.GetBoard());
                wasLastPlayLegal = ExecuteTurn();
                winner = m_Logic.CheckWinner();
                if (null != winner)
                    gameOver = true;
            }
            Ex02.ConsoleUtils.Screen.Clear();
            PrintBoard(m_Logic.GetBoard());
            Console.WriteLine("Congratulations {0} for winning! Press Y to play again and any other button to exit",winner);
            string input = Console.ReadLine();
            if ("Y" == input)
            {
                m_Logic.ResetBoard();
                GameMainLoop();
            }
        }
        private String GetInputPosition()
        {
            string returnValue = Console.ReadLine();
            if(returnValue == "Q")
                System.Environment.Exit(0);
            return returnValue;
        }
        public void ChooseSettings()
        {
            //First Player Name
            Console.WriteLine("Hi please enter your name");
            m_BlackName = Console.ReadLine();
            //Game Mode (AI vs Two Players
            Console.WriteLine("If you Would like to play against another person please press 0\n" +
   "otherwise if you would like to play against the computer press any other number");
            string decideCompOrPlayer = Console.ReadLine();
            m_GameMode = e_GameMode.VersusAi;
            if (decideCompOrPlayer == "0")
            {
                m_GameMode = e_GameMode.TwoPlayer;
            }
            //Second Player Name
            m_WhiteName = "computer";
            if (m_GameMode == e_GameMode.TwoPlayer)
            {
                Console.Write("Please enter the second players's name\n");
                m_WhiteName = Console.ReadLine();
            }
            //Game Board Size
            Console.WriteLine("If you would like to play 8 size please press 8\n" +
                 "otherwise if you would like 6 size press any other number");
            string sizeStr = Console.ReadLine();
            if (sizeStr != "8")
                m_SizeOfPlaying = 6;
            else
                m_SizeOfPlaying = 8;
            //Creating GameLogic object
            Console.WriteLine("If you wish to exit while playing, please press 'Q'");
            System.Threading.Thread.Sleep(1500);
            m_Logic = new GameLogic(m_SizeOfPlaying,m_WhiteName,m_BlackName);
        }
        private bool ExecuteTurn()
        {
            int x = 0, y = 0;
            if (m_Logic.GetCurrrentTurn() == GameLogic.eBoardLocation.White && m_GameMode == e_GameMode.VersusAi)
            {
                ComputerPlayer(ref x, ref y);
            }
            else
                PersonPlayer(ref x, ref y);
            return m_Logic.PlayMove(y, x);
        }
        private void ComputerPlayer(ref int x,ref int y )
        {
            Console.WriteLine("White Player: " + "computer" + "(o)");
            Console.WriteLine("");
            int[] opponentTurn = m_Logic.ChooseRandomMove();
            x = opponentTurn[1];
            y = opponentTurn[0];
            char xChar = ((char)(x + 'A'));
            Console.WriteLine("X:" + xChar);
            Console.WriteLine("Y:" + (y+1));
            System.Threading.Thread.Sleep(1000);
        }
        private void PersonPlayer(ref int x, ref int y)
        {
            //User game input, reads input from user and then sends data to 
            //input legality check
            if(m_Logic.GetCurrrentTurn() == GameLogic.eBoardLocation.Black)
                Console.WriteLine("Black Player: " + m_BlackName + "(x)");
            else
                Console.WriteLine("White Player: " + m_WhiteName + "(o)");
            bool LegalityOfInput = false;
            string str_x="",str_y="";
            while (false == LegalityOfInput)
            {
                Console.WriteLine("Please insert an x coordinate(Enter Capital Letters)");
                str_x = GetInputPosition();
                Console.WriteLine("Please insert a y coordinate(Enter Numbers)");
                 str_y = GetInputPosition();
                LegalityOfInput = CheckLegalInputsUI(str_x, str_y);
            }
            char char_x = char.Parse(str_x);
            y = int.Parse(str_y) - 1;
            x = char_x - 'A';
        }
        public static void PrintBoard(eBoardLocation[,] m_Board)
        {
            int generalSize = m_Board.GetLength(0);
            char abcAbove = 'A';
            Console.Write("    ");
            for (int abc = 0; abc < generalSize; abc++)
                Console.Write(abcAbove++ + "   ");
            Console.WriteLine();
            int numericSide = 1;
            string signOfEqualByLength;
            if (generalSize == 8)
                signOfEqualByLength = "  =================================";
            else
                signOfEqualByLength = "  =========================";
            Console.WriteLine(signOfEqualByLength);
            for (int i = 0; i < generalSize; i++)
            {
                Console.Write(numericSide++ +" ");
                for (int j = 0; j < m_Board.GetLength(1); j++)
                {
                    PrintBoardStatus(i, j, m_Board);   
                }
                Console.WriteLine("|");
                Console.WriteLine(signOfEqualByLength);

            }
        }//ההדפס של הלוח בכל ההתאמות
        public static void PrintBoardStatus(int i_X, int i_Y, eBoardLocation[,] m_Board)
        {
            if (m_Board[i_X, i_Y] == eBoardLocation.Black)
                Console.Write("| x ");
            else
                if (m_Board[i_X, i_Y] == eBoardLocation.White)
                    Console.Write("| o ");
            else
                if (m_Board[i_X, i_Y] == eBoardLocation.Empty)
                    Console.Write("|   ");
        }//עוד חלק בהתאמת הלוח
        private bool CheckLegalInputsUI(string i_X,string i_Y)
        {
            // Note: This only relates to legality in terms of accordance to UI. Testing for
            // Logical legality takes place in GameLogic
            bool returnValue = true;
            bool YisNumeric = int.TryParse(i_Y, out _);
            bool XisChar = i_X.Length == 1 && char.IsUpper(i_X[0]);
            if (false == XisChar || false == YisNumeric)
                returnValue = false;
            if (false == returnValue)
                Console.WriteLine("Illegal input {0} {1}, please input again according to requirements",i_X,i_Y);
            return returnValue;
        }
        private enum e_GameMode
        {
            TwoPlayer,
            VersusAi
        }
    }
}
