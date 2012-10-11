using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BackGammonn
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        

        //images
        private Texture2D imageCheckerBlue { get; set; }
        private Texture2D imageCheckerRed { get; set; }
        //FONTS
        private SpriteFont font;
        
        //MOUSE
        private MouseState oldState_Left;
        private MouseState oldState_Right;
        int mouse_x = 0;
        int mouse_y = 0;

        //nest
        private Texture2D nurkall;
        private Texture2D nurkylal;
        
        //gameboard
        Board gameBoard = new Board(new Vector2(5, 5), 90, 24, 324);
        private Texture2D gameBoardTexture;

        //player
        Player pl1 = new Player("Mailis", "red");
        Player pl2 = new Player("Toomas", "blue");
        Player whosTurnToRoll;

        //announcemet: statingplayer, winner etc
        String announcement = "";

        //button
        Dice dice;
        SpriteFont myFont;
        //steps
         int stepMin = 0;
         int stepMax = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1010;
            graphics.PreferredBackBufferHeight = 612;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // add 24 nests to the board
            gameBoard.addNests("nurkall", "nurkylal", Color.Orange, Color.Blue);
            //add initial player1 checkers into Nests
            setInitialCheckerState(pl1, pl2);

            //drawing of beginning player
            whosTurnToRoll = drawingPlayer(pl1, pl2);
            announcement = whosTurnToRoll.name + " has won the dice!";
            base.Initialize();
        }
 

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            nurkall = this.Content.Load<Texture2D>("nurk_all");
            nurkylal = this.Content.Load<Texture2D>("nurk_ylal");
            gameBoardTexture = this.Content.Load<Texture2D>("board");
            imageCheckerBlue = this.Content.Load<Texture2D>("checkers/checkrBLUE");
            imageCheckerRed = this.Content.Load<Texture2D>("checkers/checkrRED");

            //Font
            font = this.Content.Load<SpriteFont>("score");

            //button
            Texture2D texture;
            texture = this.Content.Load<Texture2D>("roll");
            myFont = this.Content.Load<SpriteFont>("score");
            dice = new Dice(texture, myFont, spriteBatch); 
            
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //dice
            dice.Location(930, 270); 
            dice.Update();
            if (dice.rolledValues.Count > 0)
            {
                stepMin = dice.rolledValues.Min();
                stepMax = dice.rolledValues.Max();
            }
            //nests
            List<Nest> nests = gameBoard.Nests;
            

            //MOUSE_left
            if (whosTurnToRoll.currentMoves < 2)
            {
                MouseState newState_Left = Mouse.GetState();
                if (newState_Left.LeftButton == ButtonState.Pressed && oldState_Left.LeftButton == ButtonState.Released)
                {
                    mouse_x = newState_Left.X;
                    mouse_y = newState_Left.Y;
                    moveCheckers(whosTurnToRoll, stepMax, mouse_x, mouse_y, nests, dice);
                }
                oldState_Left = newState_Left; //// this reassigns the old state so that it is ready for next time


                //MOUSE_right
                MouseState newState_Right = Mouse.GetState();
                if (newState_Right.RightButton == ButtonState.Pressed && oldState_Right.RightButton == ButtonState.Released)
                {
                    mouse_x = newState_Right.X;
                    mouse_y = newState_Right.Y;
                    moveCheckers(whosTurnToRoll, stepMin, mouse_x, mouse_y, nests, dice);
                }
                oldState_Right = newState_Right; //// this reassigns the old state so that it is ready for next time
            }
            else
            {
                if (whosTurnToRoll == pl1)
                    whosTurnToRoll = pl2;
                else
                    whosTurnToRoll = pl1;

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
                        
            spriteBatch.Begin();
                //draw board
                spriteBatch.Draw(gameBoardTexture, gameBoard.boardLocation, Color.White);
                //draw nests 
                foreach (Nest n in gameBoard.Nests)
                {
                    if (n.nestTexture == "nurkall")
                        spriteBatch.Draw(nurkall, new Vector2(n.leftCorner, n.y_location), n.color);
                    else
                        spriteBatch.Draw(nurkylal, new Vector2(n.leftCorner, n.y_location), n.color);
                }
                //draw checkers
                foreach (Nest n in gameBoard.Nests)
                { 
                    foreach (Checker ch in n.Checkers)
                    {
                        if(ch.Color == "blue")
                            spriteBatch.Draw(imageCheckerBlue, ch.location, Color.White);
                        else
                            spriteBatch.Draw(imageCheckerRed, ch.location, Color.White);
                    }
                }
                
            spriteBatch.End();
            

            
            
            spriteBatch.Begin();
            spriteBatch.DrawString(font, announcement, new Vector2(350, 3), Color.Yellow);
            //spriteBatch.DrawString(font, "mouse" + mouse_x + " " + mouse_y + " N: " + nestCollision, new Vector2(100, 100), Color.White);
            if (dice.rolledValues.Count > 0)
                spriteBatch.DrawString(font,"dice " + dice.rolledValues[0] + " " + dice.rolledValues[1], new Vector2(100, 100), Color.White);
            spriteBatch.End();
            

            //button
            if(whosTurnToRoll.color == "red")
                dice.Draw(Color.OrangeRed);
            else
                dice.Draw(Color.RoyalBlue);

            base.Draw(gameTime);
        }

        #region DRAWING OF A STARTING PLAYER
        public Player drawingPlayer( Player pl1, Player pl2)
        {
            Player whosTurnToRoll;
            Random random = new Random();
            int firstRoll = random.Next(1, 7);
            int secondRoll = random.Next(1, 7);

            if (firstRoll == secondRoll)
            {
                drawingPlayer(pl1, pl2);
            }
            
            if (firstRoll > secondRoll)
            {
                whosTurnToRoll = pl1;
            }
            else
            {
                whosTurnToRoll = pl2;
            }
            return whosTurnToRoll;
        }
        #endregion

        #region MOVE CHECKERS- player, steps, mouse_x, mouse_y, nests, dice
        private void moveCheckers(Player player, int steps, int mouse_x, int mouse_y, List<Nest> nests, Dice dice)
        { 
            int clickedNest_playerid_plus = 0;
            Nest clickedNest = null;
            foreach (Nest n in nests)
            {
                int max_y = n.height + n.y_location;
                if (mouse_x > n.leftCorner && mouse_x < n.rightCorner && mouse_y > n.y_location && mouse_y < max_y)
                {
                    clickedNest = n;
                    break;
                }
            }
            if (clickedNest != null)
            {
                int cncc = clickedNest.Checkers.Count;
                if (cncc > 0)
                {
                    if (dice.rolledValues.Count > 0)
                    {
                        Checker remCh = clickedNest.Checkers[cncc - 1];
                        if (player.checkers.Contains(remCh))
                        {
                            clickedNest.Checkers.Remove(remCh);
                            if(player.color =="red")
                                clickedNest_playerid_plus = clickedNest.paleyerNestID + steps;
                            else
                                clickedNest_playerid_plus = clickedNest.paleyerNestID - steps;
                            Nest nextNest = gameBoard.findNest_ByPaleyerNestID(clickedNest_playerid_plus);
                            nextNest.addChecker(remCh);
                            player.currentMoves++;
                            Moves playerMove = new Moves(player, clickedNest, nextNest, remCh);
                            player.moves.Add(playerMove);
                        }
                    }
                }
            }
        }
        #endregion

        #region INITIAL STATE FOR CHECKERS

        protected void setInitialCheckerState(Player l1, Player pl2)
        {
            List<Checker> pl1Checkers = pl1.checkers;
            List<Checker> pl2Checkers = pl2.checkers;

            foreach (Nest n in gameBoard.Nests)
            {
                if (n.paleyerNestID == 1)
                {
                    for (int i = 0; i < pl1Checkers.Count; i++)
                    {
                        if (i < 2)
                        {
                            Checker myChecker = pl1Checkers[i];
                            n.addChecker(myChecker);
                        }
                    }
                }
                if (n.paleyerNestID == 12)
                {
                    for (int i = 2; i < pl1Checkers.Count; i++)
                    {
                        if (i < 7)
                        {
                            Checker myChecker = pl1Checkers[i];
                            n.addChecker(myChecker);
                        }
                    }
                }
                if (n.paleyerNestID == 17)
                {
                    for (int i = 7; i < pl1Checkers.Count; i++)
                    {
                        if (i < 10)
                        {
                            Checker myChecker = pl1Checkers[i];
                            n.addChecker(myChecker);
                        }
                    }
                }
                if (n.paleyerNestID == 19)
                {
                    for (int i = 10; i < pl1Checkers.Count; i++)
                    {
                        if (i < 15)
                        {
                            Checker myChecker = pl1Checkers[i];
                            n.addChecker(myChecker);
                        }
                    }
                }
                //OPPONENT
                if (n.opponentNestID == 1)
                {
                    for (int i = 0; i < pl2Checkers.Count; i++)
                    {
                        if (i < 2)
                        {
                            Checker myChecker = pl2Checkers[i];
                            n.addChecker(myChecker);
                        }
                    }
                }
                if (n.opponentNestID == 12)
                {
                    for (int i = 2; i < pl2Checkers.Count; i++)
                    {
                        if (i < 7)
                        {
                            Checker myChecker = pl2Checkers[i];
                            n.addChecker(myChecker);
                        }
                    }
                }
                if (n.opponentNestID == 17)
                {
                    for (int i = 7; i < pl2Checkers.Count; i++)
                    {
                        if (i < 10)
                        {
                            Checker myChecker = pl2Checkers[i];
                            n.addChecker(myChecker);
                        }
                    }
                }
                if (n.opponentNestID == 19)
                {
                    for (int i = 10; i < pl2Checkers.Count; i++)
                    {
                        if (i < 15)
                        {
                            Checker myChecker = pl2Checkers[i];
                            n.addChecker(myChecker);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
