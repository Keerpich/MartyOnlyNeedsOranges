#region Using Statements
using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
#endregion

namespace MaryOnlyNeedsOranges
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private enum MapItems { Nothing, Tree, Food, SnakeCell };
        public enum GameStage { SplashScreen, Meniu, SinglePlayerWBorders, SinglePlayerBorderless, Credits, Options, Paused, Highscores, Askname };

        private Snake snake;
        private Food food;
        private Random rnd;
        public GameStage gameStage;
        TimeSpan timeClick;
        GameStage gameType;

        float scoreY;
        float volBg;
        float volSfx;
        bool bMovingScore;
        bool bClickAvailable;
        bool bTongueOut;

        HighScores highScore;
        string actualName;

        MenuItemManager mainMenu;
        MenuItemManager optionsMenu;

        Texture2D imgTree;
        Texture2D imgApple;
        Texture2D imgLemon;
        Texture2D imgBlueB;
        Texture2D imgOrange;
        Texture2D imgSnakeHead;
        Texture2D imgSnakeHead2;
        Texture2D imgSnakeBody;
        Texture2D imgSnakeTurn;
        Texture2D imgSnakeTail;
        Texture2D imgNormal;
        Texture2D imgSplashScreen;
        Texture2D imgGrass;
        Texture2D imgGrapes;

        SoundEffect sfxBite;
        SoundEffect sfxHit;

        SoundEffect meniuMusic;
        SoundEffect playMusic;

        SoundEffectInstance instance;

        SpriteFont fontArcade;

        TimeSpan lastTime;
        TimeSpan foodTime;
        TimeSpan scoreTime;
        TimeSpan pauseTime;
        TimeSpan tongueTime;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //to be changed
            gameStage = GameStage.SplashScreen;
            gameType = GameStage.SinglePlayerWBorders;
            snake = new Snake();
            food = null;
            rnd = new Random();
            lastTime = new TimeSpan(0, 0, 3);
            bClickAvailable = false;
            volSfx = 1f;
            volBg = 1f;
            scoreY = 0f;
            bMovingScore = true;
            bTongueOut = false;
            actualName = "";
            scoreTime = new TimeSpan(0, 0, 0);
            pauseTime = new TimeSpan(0, 0, 0);
            tongueTime = new TimeSpan(0, 0, 0);

            base.Initialize();
        }

        private void Reinitialize()
        {
            snake = new Snake();
            scoreTime = new TimeSpan(0, 0, 0);
            scoreY = 0f;
            highScore = null;
            CreateFood();
            highScore = new HighScores(10);
            BackgroundMusic(meniuMusic);
            gameStage = GameStage.Meniu;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //GFX Content
            imgApple = Content.Load<Texture2D>("Graphics/apple-block");
            imgLemon = Content.Load<Texture2D>("Graphics/lemon-block");
            imgOrange = Content.Load<Texture2D>("Graphics/orange-block");
            imgNormal = Content.Load<Texture2D>("Graphics/normal-block");
            imgTree = Content.Load<Texture2D>("Graphics/tree-block");
            imgBlueB = Content.Load<Texture2D>("Graphics/blueberries-block");
            imgSnakeBody = Content.Load<Texture2D>("Graphics/snake-body");
            imgSnakeHead = Content.Load<Texture2D>("Graphics/snake-head");
            imgSnakeHead2 = Content.Load<Texture2D>("Graphics/snake-head2");
            imgSnakeTail = Content.Load<Texture2D>("Graphics/snake-tail");
            imgSnakeTurn = Content.Load<Texture2D>("Graphics/snake-corner");
            imgSplashScreen = Content.Load<Texture2D>("Graphics/splashscreen");
            imgGrass = Content.Load<Texture2D>("Graphics/grass");
            imgGrapes = Content.Load <Texture2D>("Graphics/grapes");

            //SFX Content
            sfxBite = Content.Load<SoundEffect>("Sounds/bite");
            sfxHit = Content.Load<SoundEffect>("Sounds/hit");

            //Songs
            meniuMusic = Content.Load<SoundEffect>("Sounds/meniusfx");
            playMusic = Content.Load<SoundEffect>("Sounds/gamesfx");

            //Font
            fontArcade = Content.Load<SpriteFont>("Fonts/arcade");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (gameStage == GameStage.SplashScreen)
            {
                if (gameTime.TotalGameTime.CompareTo(lastTime) >= 0)
                {
                    BackgroundMusic(meniuMusic);
                    timeClick = gameTime.TotalGameTime.Subtract(new TimeSpan(0, 0, 1));
                    setupMenu();
                    gameStage = GameStage.Meniu;
                }
            }

            else if (gameStage == GameStage.Meniu)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && (timeClick.Add(new TimeSpan(0, 0, 0, 0, 250)).CompareTo(gameTime.TotalGameTime) <= 0))
                {
                    timeClick = gameTime.TotalGameTime;
                    MeniuClick(mainMenu);
                }
                MeniuUpdate(mainMenu);
            }

            else if (gameStage == GameStage.Options)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && (timeClick.Add(new TimeSpan(0, 0, 0, 0, 250)).CompareTo(gameTime.TotalGameTime) <= 0))
                {
                    timeClick = gameTime.TotalGameTime;
                    MeniuClick(optionsMenu);
                }
                MeniuUpdate(optionsMenu);
            }

            else if (gameStage == GameStage.Credits)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    gameStage = GameStage.Meniu;
            }

            else if (gameStage == GameStage.Paused)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.P) && pauseTime.Add(new TimeSpan(0, 0, 0, 0, 200)).CompareTo(gameTime.TotalGameTime) <= 0)
                {
                    gameStage = gameType;
                    pauseTime = gameTime.TotalGameTime;
                }
            }

            //only if we play single player
            else if (gameStage == GameStage.SinglePlayerWBorders || gameStage == GameStage.SinglePlayerBorderless)
            {
                SinglePlayerUpdate(gameTime);

                if (Keyboard.GetState().IsKeyDown(Keys.P) && pauseTime.Add(new TimeSpan(0, 0, 0, 0, 200)).CompareTo(gameTime.TotalGameTime) <= 0)
                {
                    gameStage = GameStage.Paused;
                    pauseTime = gameTime.TotalGameTime;
                }
            }

            else if (gameStage == GameStage.Highscores)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    gameStage = GameStage.Meniu;
                    highScore = null;
                }
            }


            base.Update(gameTime);
        }

        private void BackgroundMusic(SoundEffect eff)
        {
            if (instance != null)
                instance.Stop();

            instance = eff.CreateInstance();
            instance.IsLooped = true;
            instance.Volume = volBg;
            instance.Play();
        }

        private void isDead()
        {
            LoadHighScore();
            SaveHighScore();

            Reinitialize();
        }

        private void LoadHighScore()
        {
            highScore = HighScores.LoadHighScores();
        }

        private void SaveHighScore()
        {
            bool saved = false;
            int pos = highScore.Count - 2;

            for (int i = highScore.Count - 2; i >= 0; i--)
            {
                if (highScore.iPoints[i] < snake.iPoints)
                {
                    highScore.iPoints[i + 1] = highScore.iPoints[i];
                    saved = true;
                    pos = i;
                }
            }

            if (saved)
                highScore.iPoints[pos] = snake.iPoints;

            if (!saved && snake.iPoints > highScore.iPoints[highScore.Count - 1])
                highScore.iPoints[highScore.Count - 1] = snake.iPoints;

            HighScores.SaveHighScores(highScore);
        }

        private void setupMenu()
        {
            mainMenu = new MenuItemManager(5, fontArcade, GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width);
            mainMenu.setItem(0, "Play");
            mainMenu.setItem(1, "Highscores");
            mainMenu.setItem(2, "Options");
            mainMenu.setItem(3, "Credits");
            mainMenu.setItem(4, "Exit");
        }

        private void setupOptionsMenu()
        {
            optionsMenu = new MenuItemManager(5, fontArcade, GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width);
            optionsMenu.setItem(0, "Borders");
            optionsMenu.setItem(1, "Sound FX");
            optionsMenu.setItem(2, "Background Music");
            optionsMenu.setItem(3, "Moving Score");
            optionsMenu.setItem(4, "Back");
        }

        private void MeniuUpdate(MenuItemManager menu)
        {
            menu.onMouse(Mouse.GetState().X, Mouse.GetState().Y);

        }

        private void MeniuClick(MenuItemManager menu)
        {
            string strResult = menu.onClick(Mouse.GetState().X, Mouse.GetState().Y);

            if (strResult == "Play")
            {
                BackgroundMusic(playMusic);
                gameStage = gameType;
            }
            else if (strResult == "Exit")
                End();
            else if (strResult == "Options")
            {
                setupOptionsMenu();
                gameStage = GameStage.Options;
            }
            else if (strResult == "Credits")
                gameStage = GameStage.Credits;

            else if (strResult == "Back")
            {
                setupMenu();
                gameStage = GameStage.Meniu;
            }

            else if (strResult == "Sound FX")
            {
                optionsMenu.ToggleAct("Sound FX");
                if (optionsMenu.IsActivated("Sound FX"))
                    volSfx = 1f;
                else volSfx = 0f;
            }

            else if (strResult == "Background Music")
            {
                optionsMenu.ToggleAct("Background Music");
                if (optionsMenu.IsActivated("Background Music"))
                {
                    volBg = 1f;
                    BackgroundMusic(meniuMusic);
                }
                else
                {
                    volBg = 0f;
                    BackgroundMusic(meniuMusic);
                }
            }

            else if (strResult == "Borders")
            {
                optionsMenu.ToggleAct("Borders");
                if(optionsMenu.IsActivated("Borders"))
                    gameType = GameStage.SinglePlayerWBorders;
                else
                    gameType = GameStage.SinglePlayerBorderless;
            }

            else if (strResult == "Moving Score")
            {
                optionsMenu.ToggleAct("Moving Score");
                if (optionsMenu.IsActivated("Moving Score"))
                {
                    bMovingScore = true;
                }
                else
                {
                    bMovingScore = false;
                }
            }

            else if (strResult == "Highscores")
            {
                gameStage = GameStage.Highscores;
                highScore = HighScores.LoadHighScores();
            }
        }

        private void SinglePlayerUpdate(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Reinitialize();


            if (scoreTime.Add(new TimeSpan(0, 0, 0, 0, 333)).CompareTo(gameTime.TotalGameTime) <= 0)
            {
                scoreTime = gameTime.TotalGameTime;
                scoreY += 10f;
                if (scoreY >= 810)
                    scoreY = -10;
            }

            TongueTimeElapse(gameTime);

            FoodTimeElapse(gameTime);

            Collision();

            if (food == null)
                CreateFood(gameTime);

            Movement();

            SnakeMove(gameTime);
        }

        private void TongueTimeElapse(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Subtract(tongueTime) > new TimeSpan(0, 0, 0, 0, 1500))
            {
                if (bTongueOut)
                    bTongueOut = false;
                else bTongueOut = true;

                tongueTime = gameTime.TotalGameTime;
            }
        }

        private void FoodTimeElapse(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (food != null && food.type == Food.Type.Grapes)
                    if (gameTime.TotalGameTime.Subtract(foodTime) > new TimeSpan(0, 0, 3))
                        food = null;
            }
            else
            {
                if (food != null && food.type == Food.Type.Grapes)
                    if (gameTime.TotalGameTime.Subtract(foodTime) > new TimeSpan(0, 0, 0, 12))
                        food = null;
            }
        }

        private void SnakeMove(GameTime gameTime)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (gameTime.TotalGameTime.Subtract(lastTime) > new TimeSpan(0, 0, 0, 0, snake.iSpeed / 4))
                {
                    snake.Move(gameStage);
                    lastTime = gameTime.TotalGameTime;
                }
            }
            else
            {
                if (gameTime.TotalGameTime.Subtract(lastTime) > new TimeSpan(0, 0, 0, 0, snake.iSpeed))
                {
                    snake.Move(gameStage);
                    lastTime = gameTime.TotalGameTime;
                }
            }
        }

        private void End()
        {
            Exit(); //TODO: add proper ending
        }

        private void Collision()
        {

            //with walls
            if (gameStage == GameStage.SinglePlayerWBorders)
                if (snake.body[0].iLine == 0 || snake.body[0].iLine == 22 || snake.body[0].iRow == 0 || snake.body[0].iRow == 39)
                {
                    sfxHit.Play(volSfx, 0f, 0f);
                    isDead();
                }

            //with cells
            for (int i = 1; i < snake.iSize; i++)
            {
                if (snake.body[0].iLine == snake.body[i].iLine && snake.body[0].iRow == snake.body[i].iRow)
                {
                    sfxHit.Play(volSfx, 0f, 0f);
                    isDead();
                }
            }

            //with food
            if (food != null)
            {
                if (snake.body[0].iRow == food.row && snake.body[0].iLine == food.line)
                {
                    
                    snake.IncSpeed(food.speed);
                    snake.IncSize(food.size);
                    snake.IncPoints(food.points);

                    snake.Green += food.green;
                    snake.Red += food.red;
                    snake.Blue += food.blue;
                    GenerateColor();

                    sfxBite.Play(volSfx, 0f, 0f);

                    food = null;
                }
            }

        }

        private void CreateFood()
        {
            int chance = rnd.Next(100);
            food = new Food();

            if (snake.iPoints - snake.lastPoints >= 15)
            {
                food.size = 0;
                food.speed = 0;
                food.points = snake.iPoints;

                food.blue = 8;
                food.red = 3;
                food.green = 0;

                food.type = Food.Type.Grapes;

                snake.lastPoints = snake.iPoints;
            }
            else if (chance < 5)
            {
                food.size = 0;
                food.speed = 0;
                food.points = 1;

                food.green = 7;
                food.blue = 0;
                food.red = 0;

                food.type = Food.Type.Apple;
            }

            else if (chance < 10)
            {
                food.size = 0;
                food.speed = 30;
                food.points = 0;

                food.green = 0;
                food.blue = 12;
                food.red = 0;

                food.type = Food.Type.Blueberry;
            }

            else if (chance < 15)
            {
                food.size = -1;
                food.points = 0;
                food.speed = 0;

                food.green = 6;
                food.red = 3;
                food.blue = 0;

                food.type = Food.Type.Lemon;
            }

            else if (chance < 18)
            {
                food.size = -1;
                food.speed = 20;
                food.points = 1;

                food.green = 3;
                food.red = 3;
                food.blue = 0;

                food.type = Food.Type.Orange;
            }

            else
            {
                food.size = 1;
                food.speed = -15;
                food.points = 1;

                food.red = 1;
                food.blue = 0;
                food.green = 0;

                food.type = Food.Type.Normal;
            }

            if (gameStage == GameStage.SinglePlayerBorderless)
            {
                food.line = rnd.Next(0, 22);
                food.row = rnd.Next(0, 39);
            }
            else if (gameStage == GameStage.SinglePlayerWBorders)
            {
                food.line = rnd.Next(1, 21);
                food.row = rnd.Next(1, 38);
            }

            for (int i = 0; i < snake.iSize; i++)
            {
                if (snake.body[i].iLine == food.line && snake.body[i].iRow == food.row)
                {
                    food = null;
                    return;
                }
            }
        }

        private void CreateFood(GameTime gameTime)
        {
            int chance = rnd.Next(100);
            food = new Food();

            if (snake.iPoints - snake.lastPoints >= 15)
            {
                food.size = 0;
                food.speed = 0;
                food.points = snake.iPoints;

                food.blue = 8;
                food.red = 3;
                food.green = 0;

                food.type = Food.Type.Grapes;

                snake.lastPoints = snake.iPoints;
            }
            else if (chance < 5)
            {
                food.size = 0;
                food.speed = 0;
                food.points = 1;

                food.green = 7;
                food.blue = 0;
                food.red = 0;

                food.type = Food.Type.Apple;
            }

            else if (chance < 10)
            {
                food.size = 0;
                food.speed = 30;
                food.points = 0;

                food.green = 0;
                food.blue = 12;
                food.red = 0;

                food.type = Food.Type.Blueberry;
            }

            else if (chance < 15)
            {
                food.size = -1;
                food.points = 0;
                food.speed = 0;

                food.green = 6;
                food.red = 3;
                food.blue = 0;

                food.type = Food.Type.Lemon;
            }

            else if (chance < 18)
            {
                food.size = -1;
                food.speed = 20;
                food.points = 1;

                food.green = 3;
                food.red = 3;
                food.blue = 0;

                food.type = Food.Type.Orange;
            }

            else
            {
                food.size = 1;
                food.speed = -15;
                food.points = 1;

                food.red = 1;
                food.blue = 0;
                food.green = 0;

                food.type = Food.Type.Normal;
            }

            if (gameStage == GameStage.SinglePlayerBorderless)
            {
                food.line = rnd.Next(0, 22);
                food.row = rnd.Next(0, 39);
            }
            else if (gameStage == GameStage.SinglePlayerWBorders)
            {
                food.line = rnd.Next(1, 21);
                food.row = rnd.Next(1, 38);
            }

            for (int i = 0; i < snake.iSize; i++)
            {
                if (snake.body[i].iLine == food.line && snake.body[i].iRow == food.row)
                {
                    food = null;
                    return;
                }
            }

            if (food != null && food.type == Food.Type.Grapes)
            {
                foodTime = gameTime.TotalGameTime;
            }
        }

        //movement
        private void Movement()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                snake.changeDir(Snake.Direction.Up);
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                snake.changeDir(Snake.Direction.Down);
            else if (Keyboard.GetState().IsKeyDown(Keys.Left))
                snake.changeDir(Snake.Direction.Left);
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                snake.changeDir(Snake.Direction.Right);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (gameStage == GameStage.SplashScreen)
                DrawSplashScreen();

            if (gameStage == GameStage.Meniu)
                DrawMeniu(mainMenu);

            if (gameStage == GameStage.Options)
                DrawMeniu(optionsMenu);

            if (gameStage == GameStage.Credits)
                DrawCredits();

            //only if we play single player
            if (gameStage == GameStage.SinglePlayerBorderless || gameStage == GameStage.SinglePlayerWBorders)
                SinglePlayerDraw (gameTime);

            if (gameStage == GameStage.Highscores)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);

                spriteBatch.Begin();
                highScore.Draw(spriteBatch, fontArcade, GraphicsDevice.Viewport.Height, GraphicsDevice.Viewport.Width);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void DrawMeniu(MenuItemManager menu)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();


            menu.Draw(spriteBatch);
            /*
            spriteBatch.DrawString(fontArcade, "Play", new Vector2(400, 100), Color.White, 0, fontArcade.MeasureString("Play") / 2, 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(fontArcade, "Options", new Vector2(400, 260), Color.White, 0, fontArcade.MeasureString("Options") / 2, 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(fontArcade, "Exit", new Vector2(400, 420), Color.White, 0, fontArcade.MeasureString("Exit") / 2, 1.0f, SpriteEffects.None, 0.5f);
            */

            spriteBatch.End();
        }

        private void DrawSplashScreen()
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(imgSplashScreen, new Vector2(0, 0), Color.White);
            spriteBatch.End();
        }

        private Texture2D rectangle()
        {
            Texture2D texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            texture.SetData<Color>(new Color[] { Color.Black });
            return texture;
        }

        private void SinglePlayerDraw(GameTime gameTime)
        {
            //if (pointMove > 800 * 3) pointMove = 0; (if you want point display to move)

            spriteBatch.Begin();

            DrawStage();
            DrawScore();

            if (gameStage == GameStage.SinglePlayerWBorders)
                DrawTrees();

            DrawSnake();

            DrawFood();

            spriteBatch.End();
        }

        private void DrawStage()
        {
            if (gameStage == GameStage.SinglePlayerWBorders)
                GraphicsDevice.Clear(Color.LawnGreen);
            else if (gameStage == GameStage.SinglePlayerBorderless)
                //spriteBatch.Draw(imgGrass, new Vector2(0, 0), Color.White);
                GraphicsDevice.Clear(Color.LawnGreen);
        }

        private void DrawScore()
        {
            spriteBatch.Draw(rectangle(), new Rectangle(0, 460, 800, 30), Color.Black);

            if (bMovingScore == false)
            {
                spriteBatch.DrawString(fontArcade, "Points: " + snake.iPoints.ToString(), new Vector2(400, 480), Color.Orange, 0, fontArcade.MeasureString("Points: " + snake.iPoints.ToString()) / 2, 1.0f, SpriteEffects.None, 0.5f);
            }

            else
            {
                spriteBatch.DrawString(fontArcade, "Points: " + snake.iPoints.ToString(), new Vector2(scoreY, 480), Color.Orange, 0, fontArcade.MeasureString("Points: " + snake.iPoints.ToString()) / 2, 1.0f, SpriteEffects.None, 0.5f);
            }
        }

        public void DrawCredits()
        {
            string[] strCredits = new string[7];
            strCredits[0] = "This game was created by";
            strCredits[1] = "Daniel Alexandru Radu";
            strCredits[2] = "a.k.a. TwoOfDiamonds";
            strCredits[3] = "You can visit my blog at ";
            strCredits[4] = "tinyprojects.blogspot.com";

            spriteBatch.Begin();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            for (int i = 0; i < 5; i++)
            {
                spriteBatch.DrawString(fontArcade, strCredits[i], new Vector2(GraphicsDevice.Viewport.Width / 2,
                    GraphicsDevice.Viewport.Height / 5 - 10 + i * (GraphicsDevice.Viewport.Height / 5 - 10)), Color.White, 0,
                    fontArcade.MeasureString(strCredits[i]) / 2, 1.0f, SpriteEffects.None, 0.5f);

            }

            spriteBatch.End();

        }

        public void DrawTrees()
        {
            //draw trees
            for (int i = 0 ; i < 40; i++)
            {
                spriteBatch.Draw(imgTree, new Vector2(i * 20, 0), Color.White);
                spriteBatch.Draw(imgTree, new Vector2(i * 20, 440), Color.White);
            }

            for (int i = 0; i < 23; i++)
            {
                spriteBatch.Draw(imgTree, new Vector2(0, i * 20), Color.White);
                spriteBatch.Draw(imgTree, new Vector2(780, i * 20), Color.White);
            }
        }

        public void DrawSnake()
        {
            DrawSnakeHead();
            DrawSnakeBody();
            DrawSnakeTail();

        }

        private void DrawSnakeHead()
        {
            float rotationAngle = 0;
            Texture2D imgsnakehead;

            if (bTongueOut)
                imgsnakehead = imgSnakeHead;
            else
                imgsnakehead = imgSnakeHead2;

            if (snake.getDir() == Snake.Direction.Down)
                rotationAngle = 3 * MathHelper.Pi / 2;
            else if (snake.getDir() == Snake.Direction.Up)
                rotationAngle = MathHelper.Pi / 2;
            else if (snake.getDir() == Snake.Direction.Right)
                rotationAngle = MathHelper.Pi;
            else if (snake.getDir() == Snake.Direction.Left)
                rotationAngle = 0;

            if (snake.getDir() == Snake.Direction.Right)
                spriteBatch.Draw(imgsnakehead, new Rectangle(snake.body[0].iRow * 20 + 10, snake.body[0].iLine * 20 + 10, 20, 20), null, snake.color,
                    rotationAngle, new Vector2(10f, 10f), SpriteEffects.FlipVertically, 0f);
            else if (snake.getDir() == Snake.Direction.Right)
                spriteBatch.Draw(imgsnakehead, new Rectangle(snake.body[0].iRow * 20 + 10, snake.body[0].iLine * 20 + 10, 20, 20), null, snake.color,
                    rotationAngle, new Vector2(10f, 10f), SpriteEffects.FlipHorizontally, 0f);
            else
                spriteBatch.Draw(imgsnakehead, new Rectangle(snake.body[0].iRow * 20 + 10, snake.body[0].iLine * 20 + 10, 20, 20), null, snake.color,
                    rotationAngle, new Vector2(10f, 10f), SpriteEffects.None, 0f);
        }

        //generate color of snake
        private void GenerateColor()
        {
            float total;
            total = snake.Green + snake.Red + snake.Blue;

            float greenPer = snake.Green / total;
            float redPer = snake.Red / total;
            float bluePer = snake.Blue / total;

            Color toRet = new Color((int)(redPer * 255), (int)(greenPer * 255), (int)(bluePer * 255));

            /*if ((snake.Green == 0 && snake.Red == 0) || (snake.Green == 0 && snake.Blue == 0) || (snake.Red == 0 && snake.Blue == 0))
                toRet = Color.White;*/

            snake.color = toRet;

        }

        private void DrawSnakeBody()
        {

            for (int i = 1; i < snake.iSize - 1; i++)
            {
                //do not draw if the last cell hasn't been added to the snake due to recent food
                if (snake.body[snake.iSize - 1].iRow == -1 && snake.body[snake.iSize - 1].iLine == -1 && i == snake.iSize - 2) continue;

                //if the three cells are on the same line simply draw the body sprite (horizontally)
                if (snake.body[i-1].iLine == snake.body[i+1].iLine)
                {
                    spriteBatch.Draw(imgSnakeBody, new Rectangle(snake.body[i].iRow * 20 + 10, snake.body[i].iLine * 20 + 10, 20, 20), null, snake.color,
                0f, new Vector2(10f, 10f), SpriteEffects.None, 0f);
                }

                //if the three cells are on the same row rotate the sprite 90 degrees (draw it vertically)
                else if (snake.body[i - 1].iRow == snake.body[i + 1].iRow)
                {
                    spriteBatch.Draw(imgSnakeBody, new Rectangle(snake.body[i].iRow * 20 + 10, snake.body[i].iLine * 20 + 10, 20, 20), null, snake.color,
                MathHelper.Pi / 2, new Vector2(10f, 10f), SpriteEffects.None, 0f);
                }

                //if the last cell is on the left and the next one is above (or vice versa) rotate the corner sprite by 90 degrees
                else if ((snake.body[i - 1].iRow < snake.body[i].iRow && snake.body[i + 1].iLine < snake.body[i].iLine) ||
                    (snake.body[i-1].iLine < snake.body[i].iLine && snake.body[i+1].iRow < snake.body[i].iRow))
                {
                    spriteBatch.Draw(imgSnakeTurn, new Rectangle(snake.body[i].iRow * 20 + 10, snake.body[i].iLine * 20 + 10, 20, 20), null, snake.color,
                MathHelper.Pi / 2, new Vector2(10f, 10f), SpriteEffects.None, 0f);
                }


                //if the last cell is on the left and the next one is below (or vice versa) simply draw the sprite
                else if ((snake.body[i - 1].iRow < snake.body[i].iRow && snake.body[i + 1].iLine > snake.body[i].iLine) ||
                    (snake.body[i + 1].iRow < snake.body[i].iRow && snake.body[i - 1].iLine > snake.body[i].iLine))
                {
                    spriteBatch.Draw(imgSnakeTurn, new Rectangle(snake.body[i].iRow * 20 + 10, snake.body[i].iLine * 20 + 10, 20, 20), null, snake.color,
                0f, new Vector2(10f, 10f), SpriteEffects.None, 0f);
                }

                //if the last cell is on the right and the next one is above (or vice versa) rotate the sprite by 180 degrees
                else if ((snake.body[i - 1].iRow > snake.body[i].iRow && snake.body[i + 1].iLine < snake.body[i].iLine) ||
                    (snake.body[i + 1].iRow > snake.body[i].iRow && snake.body[i - 1].iLine < snake.body[i].iLine))
                {
                    spriteBatch.Draw(imgSnakeTurn, new Rectangle(snake.body[i].iRow * 20 + 10, snake.body[i].iLine * 20 + 10, 20, 20), null, snake.color,
                MathHelper.Pi, new Vector2(10f, 10f), SpriteEffects.None, 0f);
                }

                //if the last cell is on the right and the next one is below (or vice versa) rotate the sprite by 270 degrees
                else if ((snake.body[i - 1].iRow > snake.body[i].iRow && snake.body[i + 1].iLine > snake.body[i].iLine) ||
                    (snake.body[i + 1].iRow > snake.body[i].iRow && snake.body[i - 1].iLine > snake.body[i].iLine))
                {
                    spriteBatch.Draw(imgSnakeTurn, new Rectangle(snake.body[i].iRow * 20 + 10, snake.body[i].iLine * 20 + 10, 20, 20), null, snake.color,
                3 * MathHelper.Pi / 2, new Vector2(10f, 10f), SpriteEffects.None, 0f);
                }
            }
        }

        //tail drawing
        private void DrawSnakeTail()
        {
            float rotationAngle = 0;
            int x = 1;

            //if the snakes has to grow by one cell this makes sure that the tail will be drawn normally
            if (snake.body[snake.iSize - 1].iLine == -1 && snake.body[snake.iSize - 1].iRow == -1 && snake.iSize > 2)
                x = 2;

            //calculate the rotation angle of the sprite for the tail
            if (snake.body[snake.iSize - x - 1].iLine < snake.body[snake.iSize - x].iLine)
                rotationAngle = MathHelper.Pi / 2;
            else if (snake.body[snake.iSize - x - 1].iLine > snake.body[snake.iSize - x].iLine)
                rotationAngle = 3 * MathHelper.Pi / 2;
            else if (snake.body[snake.iSize - x - 1].iRow > snake.body[snake.iSize - x].iRow)
                rotationAngle = MathHelper.Pi;
            else if (snake.body[snake.iSize - x - 1].iRow < snake.body[snake.iSize - x].iRow)
                rotationAngle = 0;

            //draw the tail
            spriteBatch.Draw(imgSnakeTail, new Rectangle(snake.body[snake.iSize - x].iRow * 20 + 10, snake.body[snake.iSize - x].iLine * 20 + 10, 20, 20), null, snake.color,
                rotationAngle, new Vector2(10f, 10f), SpriteEffects.None, 0f);
        }

        //food drawing
        private void DrawFood()
        {
            //draw food
            if (food != null)
            {
                if (food.type == Food.Type.Normal)
                {
                    spriteBatch.Draw(imgNormal, new Rectangle(food.row * 20, food.line * 20, 20, 20), Color.White);
                }

                else if (food.type == Food.Type.Apple)
                {
                    spriteBatch.Draw(imgApple, new Rectangle(food.row * 20, food.line * 20, 20, 20), Color.DarkGreen);
                }

                else if (food.type == Food.Type.Blueberry)
                {
                    spriteBatch.Draw(imgBlueB, new Rectangle(food.row * 20, food.line * 20, 20, 20), Color.White);
                }

                else if (food.type == Food.Type.Lemon)
                {
                    spriteBatch.Draw(imgLemon, new Rectangle(food.row * 20, food.line * 20, 20, 20), Color.White);
                }

                else if (food.type == Food.Type.Orange)
                {
                    spriteBatch.Draw(imgOrange, new Rectangle(food.row * 20, food.line * 20, 20, 20), Color.White);
                }
                else if (food.type == Food.Type.Grapes)
                {
                    spriteBatch.Draw(imgGrapes, new Rectangle(food.row * 20, food.line * 20, 20, 20), Color.White);
                }
            }
        }
    }

}
