using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaryOnlyNeedsOranges
{
    [Serializable]
    public class HighScores
    {
        public int[] iPoints;
        public int Count;

        public HighScores(int count)
        {
            iPoints = new int[count];

            Count = count;
        }

        public static void SaveHighScores(HighScores data)
        {
            //Open the file, creating it if necessary
            StreamWriter file = new StreamWriter("highscores.txt");

            try
            {

                for (int i = 0; i < data.Count; i++)
                {
                    file.WriteLine(data.iPoints[i].ToString());
                }

            }
            finally
            {
                file.Close();
            }
        }

        public static HighScores LoadHighScores()
        {
            HighScores data = new HighScores(10);
            //open the file
            StreamReader file = new StreamReader("highscores.txt");

            try
            {
                //Read the data from the file

                for (int i = 0; i < data.Count; i++)
                {
                    data.iPoints[i] = Int32.Parse(file.ReadLine());
                }
            }


            finally
            {
                //close the file
                file.Close();
            }

            return (data);
        }

        public void Draw(SpriteBatch spriteBatch,SpriteFont font, int windowHeight, int windowWidth)
        {
            string displayString;

            MenuItemManager highscores = new MenuItemManager(11, font, windowHeight, windowWidth);

            highscores.setItem(0, "HIGHSCORES", MeniuItem.GlitterType.Always);

            for (int i = 1; i < Count + 1; i++)
            {
                displayString = iPoints[i - 1].ToString();
                highscores.setItem(i, displayString, MeniuItem.GlitterType.None);
            }

            highscores.Draw(spriteBatch);

        }

    }

}
