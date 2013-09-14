using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MaryOnlyNeedsOranges
{
    class Snake
    {
        //members
        public Cell[] body;
        public int iSize { get; set; }
        public int iPoints { get; set; }
        public int iSpeed { get; set; }
        public enum Direction { Up, Down, Left, Right };
        private Direction dir;
        public Direction GraphicDirection;
        public bool isGrowing;
        Cell someCell;
        public bool dirChanged { get; set; }
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public int lastPoints { get; set; }
        public Color color { get; set; }

        //constructor
        public Snake()
        {
            iSize = 2;
            dirChanged = false;
            iPoints = 0;
            lastPoints = 0;
            iSpeed = 1000;
            body = new Cell[2];
            body[0] = new Cell();
            body[1] = new Cell();
            body[0].iLine = 12;
            body[0].iRow = 20;
            body[0].bVertical = false;
            body[1].iLine = 12;
            body[1].iRow = 21;
            body[1].bVertical = false;
            dir = Direction.Left;
            GraphicDirection = Direction.Left;
            isGrowing = false;
            color = Color.White;

            Red = 0;
            Green = 0;
            Blue = 0;
        }


        public void IncSpeed(int speed)
        {
            iSpeed += speed;

            if (iSpeed > 1000) iSpeed = 1000;
            else if (iSpeed < 50) iSpeed = 50;
        }

        public void IncSize(int size)
        {
            if (iSize + size < 2) return;

            Cell[] body2 = new Cell[iSize];

            for (int i = 0; i < iSize; i++)
            {
                body2[i] = body[i];
            }

            body = new Cell[iSize + size];

            if (size >= 0)
            {
                someCell = body2[iSize - 1];

                for (int i = 0; i < iSize; i++)
                {
                    body[i] = body2[i];
                }

                body[iSize + size - 1].iRow = -1;
                body[iSize + size - 1].iLine = -1;
                isGrowing = true;
            }

            else
            {
                for (int i = 0; i < iSize + size; i++)
                {
                    body[i] = body2[i];
                }
            }

            iSize = iSize + size;
        }

        public void IncPoints(int points)
        {
            iPoints += points;
        }

        public void Move(Game1.GameStage gameStage)
        {

            for (int i = iSize - 1; i > 0; i--)
            {
                body[i] = body[i - 1];
            }

            if (dir == Direction.Left)
            {
                body[0].iRow--;
            }
            if (dir == Direction.Down)
            {
                body[0].iLine++;
            }
            if (dir == Direction.Up)
            {
                body[0].iLine--;
            }
            if (dir == Direction.Right)
            {
                body[0].iRow++;
            }

            dirChanged = false;

            if (gameStage == Game1.GameStage.SinglePlayerBorderless)
            {
                if (body[0].iRow < 0)
                    body[0].iRow = 39;

                if (body[0].iRow > 39)
                    body[0].iRow = 0;

                if (body[0].iLine < 0)
                    body[0].iLine = 22;

                if (body[0].iLine > 22)
                    body[0].iLine = 0;
            }

            GraphicDirection = dir;
            isGrowing = false;
        }

        public void changeDir(Direction d)
        {
            if (dirChanged) return;

            if (dir == Direction.Up && d != Direction.Down)
            {
                dir = d;
                dirChanged = true;
            }

            else if (dir == Direction.Down && d != Direction.Up)
            {
                dir = d;
                dirChanged = true;
            }

            else if (dir == Direction.Left && d != Direction.Right)
            {
                dir = d;
                dirChanged = true;
            }

            else if (dir == Direction.Right && d != Direction.Left)
            {
                dir = d;
                dirChanged = true;
            }
        }

        public Direction getDir()
        {
            return GraphicDirection;
        }
    }

    struct Cell
    {
        public int iLine;
        public int iRow;
        public bool bVertical;
    }
}
