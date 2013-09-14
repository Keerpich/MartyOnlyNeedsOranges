using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MaryOnlyNeedsOranges
{
    class MeniuItem
    {
        public string strName { get; set; }
        public Color color { get; set; }
        static private Random rnd;
        public Rectangle rect { get; set; }
        public int height { get; set; }
        private SpriteFont font;
        public int width { get; set; }
        public bool activated { get; set; }

        public GlitterType glitter { get; set; }        

        public enum GlitterType { None, Mouse, Always };

        static public void generateColor(MeniuItem it)
        {
            it.color = new Color(rnd.Next(255), rnd.Next(255), rnd.Next(255));
        }

        public bool insideRect(int x, int y)
        {
            if (rect.Contains(x, y)) return true;
            else return false;
        }

        public MeniuItem(SpriteFont sf)
        {
            strName = "";
            color = new Color (255, 255, 255);
            rnd = new Random();
            rect = new Rectangle(0, 0, 1, 1);
            font = sf;
        }

        public void setRect()
        {
            if (strName == "") return;

            rect = new Rectangle(width - (int)font.MeasureString(strName).X / 2, height - (int)font.MeasureString(strName).Y / 2, (int)font.MeasureString(strName).X, (int)font.MeasureString(strName).Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, strName, new Vector2(width, height), color, 0, font.MeasureString(strName) / 2, 1.0f, SpriteEffects.None, 0.5f);
        }
    }

    class MenuItemManager
    {
        public MeniuItem[] items {get; set;}

        public MenuItemManager(int size, SpriteFont font, int windowHeight, int windowWidth)
        {
            items = new MeniuItem[size];

            for (int i = 0; i < size; i++)
                items[i] = new MeniuItem(font);

            int ratio = windowHeight / (size + 1) - 10;

            foreach (MeniuItem item in items)
            {
                item.height = ratio;
                item.width = windowWidth / 2;
                ratio += windowHeight / (size + 1);
                item.activated = true;
            }
        }

        public void setItem(int index, string str, MeniuItem.GlitterType glit = MeniuItem.GlitterType.Mouse)
        {
            items[index].strName = str;
            items[index].setRect();
            items[index].glitter = glit;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (MeniuItem item in items)
            {
                item.Draw(spriteBatch);
            }
        }

        public void onMouse(int x, int y)
        {
            foreach (MeniuItem item in items)
            {
                if ((item.rect.Contains(x, y) && item.glitter == MeniuItem.GlitterType.Mouse) || (item.glitter == MeniuItem.GlitterType.Always))
                {
                    MeniuItem.generateColor(item);
                }
                else
                {
                    if (item.activated) item.color = Color.White;
                    else item.color = Color.Black;
                }
            }

        }

        public void ToggleAct(string str)
        {
            foreach (MeniuItem item in items)
            {
                if (str == item.strName)
                {
                    if (item.activated)
                        item.activated = false;
                    else item.activated = true;
                }
            }
        }

        public bool IsActivated(string str)
        {
            foreach (MeniuItem item in items)
            {
                if (str == item.strName)
                {
                    if (item.activated) return true;
                    else return false;
                }
            }

            return false;
        }

        public string onClick(int x, int y)
        {
            foreach (MeniuItem item in items)
            {
                if (item.rect.Contains(x, y))
                    return item.strName;
            }

            return "";
        }



    }
}
