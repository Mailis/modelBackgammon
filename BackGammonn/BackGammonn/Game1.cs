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
        private Texture2D imageCheckerBlueInJar { get; set; }
        private Texture2D imageCheckerRedInJar { get; set; }

        //FONTS
        private SpriteFont font;
        SpriteFont myFont;
        
        //MOUSE
        private MouseState oldState_Left;
        private MouseState oldState_Right;
        int mouse_x = 0;
        int mouse_y = 0;

        //nest
        private Texture2D nurkall;
        private Texture2D nurkylal;
        //jar
        private Texture2D jarTexture;
        
        //gameboard
        Board gameBoard = new Board(new Vector2(5, 5), 90, 24, 324);
        private Texture2D gameBoardTexture;

        //player
        Player player1 = new Player("Red", "red");
        Player player2 = new Player("Blue", "blue");
        Player whosTurnToRoll;

        //announcemet: statingplayer, winner etc
        String announcement = "";

        //button:dice
        Dice dice;
        String diceAnnouncement;
        //button:done
        //Done doneButton;
        //private Texture2D imageDone { get; set; }

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
            player1.isOpponent = false;
            player2.isOpponent = true;
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
            gameBoard.addJars();
            foreach (Jar jar in gameBoard.jars)
            {
                if (jar.isUpper)
                {
                    player2.jar = jar;
                }
                else
                {
                    player1.jar = jar;
                }
            }
            gameBoard.addPrison();
            //add initial player1 checkers into Nests
            setInitialCheckerState(player1, player2);

            //drawing of beginning player
            whosTurnToRoll = drawingPlayer(player1, player2);
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
            jarTexture = this.Content.Load<Texture2D>("jar");
            imageCheckerRedInJar = this.Content.Load<Texture2D>("checkers/checkrREDinJar");
            imageCheckerBlueInJar = this.Content.Load<Texture2D>("checkers/checkrBLUEinJar");

            //Font
            font = this.Content.Load<SpriteFont>("score");

            //button:dice
            Texture2D texture;
            texture = this.Content.Load<Texture2D>("roll");
            myFont = this.Content.Load<SpriteFont>("score");
            dice = new Dice(texture, myFont, spriteBatch);
            //button:done
            //imageDone = this.Content.Load<Texture2D>("done");
            //this.doneButton = new Done(imageDone, myFont, spriteBatch);
            
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

            //done
            //this.doneButton.Location(15, 210);
            //this.doneButton.Update();
            //dice
            if (dice.diceRolled)
            {
                this.announcement = "";//kustuta teade, kes sai m‰ngu alustada
            }
            dice.Location(19, 273);
            dice.Update();
            if (dice.rolledValues.Count > 0)
            {
                stepMin = dice.rolledValues.Min();
                stepMax = dice.rolledValues.Max();
            
                //diceAnnouncement
                if (dice.isDouble)
                {
                    diceAnnouncement = "dice: " + dice.rolledValues[0] + " " + dice.rolledValues[1] + " "
                                                + dice.rolledValues[2] + " " + dice.rolledValues[3];
                }
                else
                {
                    diceAnnouncement = "dice: " + dice.rolledValues[0] + " " + dice.rolledValues[1];
                }
            }
            //nests
            List<Nest> nests = gameBoard.Nests;
            
            bool hasMoreMoves = (whosTurnToRoll.currentMoves < 2 && dice.isDouble == false) || (whosTurnToRoll.currentMoves < 4 && dice.isDouble == true);

            //bool doNotChangePlayers = true;
            //kui nupp on vangis
            Boolean nuppOnVangis = false;
            List<Checker> playerCheckersInPrison = new List<Checker>(15);

            foreach (Checker ch in whosTurnToRoll.checkers)
            {
                foreach (Checker chInPrison in gameBoard.prison.checkers)
                {
                    if (ch == chInPrison)
                    {
                        playerCheckersInPrison.Add(ch);
                        nuppOnVangis = true;
                    }
                }
            }
            if (nuppOnVangis)
            {
                List<Nest> availableNests = new List<Nest>();
                List<Nest> all_AvailableNests = new List<Nest>();
                foreach (int step in dice.rolledValues)
                {
                    availableNests = findTargetNestsInOpponentsHome(whosTurnToRoll, step);
                }
                foreach (Nest stepN in availableNests)
                {
                    all_AvailableNests.Add(stepN);
                }
                //all_AvailableNests = new List<Nest>();test
                if (nuppOnVangis && all_AvailableNests.Count == 0)
                {
                    //m‰ngija j‰‰b vahele
                    hasMoreMoves = false;
                }
            }


            if (hasMoreMoves)
            {
                
                    //MOUSE_left
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
                whosTurnToRoll.currentMoves = 0;
                //m‰‰ra m‰ngijate vahetus, ehk kelle kord on veeretada
                if (whosTurnToRoll == player1)
                    whosTurnToRoll = player2;
                else
                    whosTurnToRoll = player1;
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
                //draw jars
                int jarcounter = 0;
                foreach (Jar j in gameBoard.jars)
                {
                    if (jarcounter == 0)
                    {
                        spriteBatch.Draw(jarTexture, j.jarVector, Color.White);//new Vector2(j.jarVector.X, 25)
                        foreach (Checker ch in j.checkers)
                            spriteBatch.Draw(imageCheckerBlueInJar, ch.location, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(jarTexture, j.jarVector, Color.White); //new Vector2(j.jarVector.X, 350)
                        foreach(Checker ch in j.checkers)
                            spriteBatch.Draw(imageCheckerRedInJar, ch.location, Color.White);
                    }
                    jarcounter++;
                }
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
                
                if (gameBoard.prison.numberOfCheckers() > 0)
                {
                    foreach (Checker ch in gameBoard.prison.checkers)
                    {
                        if (ch.Color == "blue")
                            spriteBatch.Draw(imageCheckerBlue, ch.location, Color.White);
                        else
                            spriteBatch.Draw(imageCheckerRed, ch.location, Color.White);
                    }
                }
            spriteBatch.End();
            

            
            //draw upper yellow announcement
            spriteBatch.Begin();
            spriteBatch.DrawString(font, announcement, new Vector2(350, 3), Color.Yellow);

            //spriteBatch.DrawString(font, "mouse" + mouse_x + " " + mouse_y + " N: " + nestCollision, new Vector2(100, 100), Color.White);

            //draw diceAnnouncement
            if (dice.rolledValues.Count > 0)
                spriteBatch.DrawString(font, diceAnnouncement, new Vector2(330, 285), Color.White);
            spriteBatch.End();
            

            //button
            //this.doneButton.Draw();
            if(whosTurnToRoll.isOpponent == false)
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
                //pl2.isTurnToRoll = false;
            }
            else
            {
                whosTurnToRoll = pl2;
                //pl1.isTurnToRoll = false;
            }
            whosTurnToRoll.isTurnToRoll = true;
            return whosTurnToRoll;
        }
        #endregion



        #region MOVE CHECKERS- player, steps, mouse_x, mouse_y, nests, dice

        private Nest findTargetNest(Nest clickedNest, Player player, int steps)
        {
            int clickedNest_playerid_plusStep = 0;
            if (player.isOpponent == false)
                clickedNest_playerid_plusStep = clickedNest.paleyerNestID + steps;
            else
                clickedNest_playerid_plusStep = clickedNest.paleyerNestID - steps;

            Nest nextNest = gameBoard.findNest_ByPaleyerNestID(clickedNest_playerid_plusStep);
            return nextNest;
        }

        private void moveCheckerIntoFreeNest(Nest clickedNest, Checker remCh, Player player, Nest nextNest)
        {

            if (player.checkers.Contains(remCh))
            {
                if(clickedNest != null)
                    clickedNest.Checkers.Remove(remCh);
                

                if (targetNestContainsMoreThanOneOpponentsChecker(nextNest, player))
                {
                    return;
                }
                else if (targetNestContainsNOopponentsChecker(nextNest, player))
                {
                    nextNest.addChecker(remCh);
                    if (clickedNest != null)
                        player.checkers_addTo_removeFrom_Home(clickedNest, remCh, gameBoard.Nests);
                }


            }
        }                


        // P’HILINE MEETOD, MIS JUHIB NUPPUDE LIIKUMIST
        private void moveCheckers(Player player, int steps, int mouse_x, int mouse_y, List<Nest> nests, Dice dice)
        {
            Prison prison = this.gameBoard.prison;
            if (steps == 0 && dice.rolledValues.Count>0)
            {
                steps = dice.rolledValues.Max();
            }

            //kui nupp on vangis
            Boolean nuppOnVangis = false;
            List<Checker> playerCheckersInPrison = new List<Checker>(15);
            foreach (Checker ch in player.checkers)
            {
                foreach (Checker chInPrison in prison.checkers)
                {
                    if (ch == chInPrison)
                    {
                        playerCheckersInPrison.Add(ch);
                        nuppOnVangis = true;
                    }
                }
            }


            //FIND CLICKED NEST OR PRISON
            Nest clickedNest = null;
            List<Nest> availableNests = findTargetNestsInOpponentsHome(player, steps);
            if (nuppOnVangis && availableNests.Count == 0)
            {
                    //m‰ngija j‰‰b vahele
                    player.currentMoves = 2;
                    dice.isDouble = false;
                    return;
            }

            if (!nuppOnVangis)
            {
                foreach (Nest n in nests)
                {
                    int max_y = n.height + n.y_location;
                    if (mouse_x > n.leftCorner && mouse_x < n.rightCorner && mouse_y > n.y_location && mouse_y < max_y)
                    {
                        clickedNest = n;
                        break;
                    }
                }
            }
            else
            {
                if (mouse_x > prison.prison_x && mouse_x < prison.rightEdge && mouse_y > prison.prison_y && mouse_y < prison.bottomEdge)
                {
                    if (nuppOnVangis && availableNests.Count == 0)
                    {
                        //m‰ngija j‰‰b vahele
                        player.currentMoves = 2;
                        dice.isDouble = false;
                        return;
                    }
                    if (playerCheckersInPrison.Count > 0)
                    {
                        Checker checkerInPrison = playerCheckersInPrison[0];
                        Nest nestToMoveTo = null;
                        foreach (Nest n in availableNests)
                        {
                            if (player.isOpponent && n.opponentNestID == steps)
                            {
                                nestToMoveTo = n;
                            }
                            else if (!player.isOpponent && n.paleyerNestID == steps)
                            {
                                nestToMoveTo = n; //moveCheckerIntoFreeNest(null, checkerInPrison, player, n);
                            }
                        }

                        if (nestToMoveTo != null && checkerInPrison != null)
                            if (dice.rolledValues.Count > 0)
                            {
                                if (targetNestContainsNOopponentsChecker(nestToMoveTo, player))
                                {
                                    moveCheckerIntoFreeNest(clickedNest, checkerInPrison, player, nestToMoveTo);
                                    prison.checkers.Remove(checkerInPrison);
                                }
                                else if (targetNestContainsONEopponentsChecker(nestToMoveTo, player))
                                {
                                    Checker opponentsOneChecker = nestToMoveTo.Checkers[0];
                                    nestToMoveTo.Checkers.Remove(opponentsOneChecker);
                                    moveCheckerIntoFreeNest(null, checkerInPrison, player, nestToMoveTo);
                                    prison.checkers.Remove(checkerInPrison);
                                    prison.addChecker(opponentsOneChecker, nestToMoveTo);
                                }

                                playerCheckersInPrison.Remove(checkerInPrison);
                                ///////////////////// 
                                //kasutatud t‰ringuv‰‰rtus nullida:
                                if (player.checkers.Contains(checkerInPrison))
                                {
                                    for (int i = 0; i < dice.rolledValues.Count; i++)
                                    {
                                        if (dice.rolledValues[i] == steps)
                                        {
                                            dice.rolledValues[i] = 0;
                                            break;
                                        }
                                    }

                                    //lisa playerile k‰ikudelogisse
                                    player.currentMoves++;
                                    Moves playerMove = new Moves(player, null, nestToMoveTo, checkerInPrison);
                                    player.moves.Add(playerMove);
                                }
                            }


                    }
                }
            }






            //FIND NESTs, WHERE PLAYER HAS checkers currently
            List<Nest> palyerHasCheckersInNests = findPlayerCurrentNests(gameBoard, player);
            //find all available target nests
            List<Nest> targetNests = findTargetNests(player, steps, palyerHasCheckersInNests);
            
            //kas on vabu Nest-e, kuhu k‰ia
            //is there any available target Nests for the player
            if (targetNests.Count == 0)
            {
                //m‰ngija j‰‰b vahele
                player.currentMoves = 2;
                dice.isDouble = false;
                return;
            }

            
            
            //MANAGE OTHER STATES
            // player has some free Nests where to go and defenitely goes somewhere
            if (clickedNest != null)
            {
                int countCheckersInCurrentNest = clickedNest.Checkers.Count;
                //kui klikitud nest ei olnud t¸hi
                if (countCheckersInCurrentNest > 0)
                {
                    if (dice.rolledValues.Count > 0)
                    {
                        Checker remCh = clickedNest.Checkers[countCheckersInCurrentNest - 1];
                        Nest nextNest = findTargetNest(clickedNest, player, steps);


                        /**/
                        //kui Player-i kıik nupud on tema kodus
                        if (player.allCheckersAreInHomeArea() == true)
                        {
                            int maxNestPositionForChecker = 0;
                            List<Nest> filledHomeNests = new List<Nest>(6);
                            Nest maxNest = null;
                            Checker movable = null;                         

                            foreach (Nest n in gameBoard.Nests)
                            {
                                foreach (Checker ch_in_homeNest in player.checkersInHome)
                                {
                                    if (n.Checkers.Contains(ch_in_homeNest))
                                    {
                                        filledHomeNests.Add(n);
                                    }
                                }
                            }

                            foreach (Nest n in filledHomeNests)
                            {
                                if (!player.isOpponent && n.paleyerNestID > maxNestPositionForChecker)
                                {
                                    maxNestPositionForChecker = n.paleyerNestID;
                                    maxNest = n;
                                }
                                else if (player.isOpponent && n.opponentNestID > maxNestPositionForChecker)
                                {
                                    maxNestPositionForChecker = n.opponentNestID;
                                    maxNest = n;
                                }
                            }
                            
                            int new_stepMax = dice.rolledValues.Max();
                            if (maxNestPositionForChecker <= new_stepMax)
                            {
                                movable = maxNest.Checkers[0];
                                player.jar.addChecker(maxNest.Checkers[0]);
                                for (int i = 0; i < dice.rolledValues.Count; i++)
                                {
                                    if (dice.rolledValues[i] == new_stepMax)
                                    {
                                        dice.rolledValues[i] = 0;
                                        break;
                                    }
                                }
                                //lisa playerile k‰ikudelogisse
                                player.currentMoves++;
                                if (movable != null)
                                {
                                    Moves playerMove = new Moves(player, clickedNest, player.jar, movable);
                                    player.moves.Add(playerMove);
                                }
                                return;
                            }

                        }
                       /* */
                        //kui target pole null!!
                        if (nextNest != null)
                        {
                            if (targetNestContainsNOopponentsChecker(nextNest, player))
                            {
                                moveCheckerIntoFreeNest(clickedNest, remCh, player, nextNest);
                            }
                            else if (targetNestContainsONEopponentsChecker(nextNest, player))
                            {
                                Checker opponentsOneChecker = nextNest.Checkers[0];
                                nextNest.Checkers.Remove(opponentsOneChecker);
                                moveCheckerIntoFreeNest(clickedNest, remCh, player, nextNest);
                                gameBoard.prison.addChecker(opponentsOneChecker, nextNest);
                            }


                            ///////////////////// 
                            //kasutatud t‰ringuv‰‰rtus nullida:
                            if (player.checkers.Contains(remCh) && targetNests.Contains(nextNest))
                            {
                                for (int i = 0; i < dice.rolledValues.Count; i++)
                                {
                                    if (dice.rolledValues[i] == steps)
                                    {
                                        dice.rolledValues[i] = 0;
                                        break;
                                    }
                                }

                                //lisa playerile k‰ikudelogisse
                                player.currentMoves++;
                                Moves playerMove = new Moves(player, clickedNest, nextNest, remCh);
                                player.moves.Add(playerMove);
                            }
                        }
                    }
                }
            }
             
        }
        #endregion

        #region INITIAL STATE FOR CHECKERS

        protected void setInitialCheckerState(Player l1, Player pl2)
        {
            List<Checker> pl1Checkers = player1.checkers;
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
                            l1.checkers_addTo_removeFrom_Home(n, myChecker, gameBoard.Nests);
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
                            l1.checkers_addTo_removeFrom_Home(n, myChecker, gameBoard.Nests);
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
                            l1.checkers_addTo_removeFrom_Home(n, myChecker, gameBoard.Nests);
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
                            l1.checkers_addTo_removeFrom_Home(n, myChecker, gameBoard.Nests);
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
                            pl2.checkers_addTo_removeFrom_Home(n, myChecker, gameBoard.Nests);
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
                            pl2.checkers_addTo_removeFrom_Home(n, myChecker, gameBoard.Nests);
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
                            pl2.checkers_addTo_removeFrom_Home(n, myChecker, gameBoard.Nests);
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
                            pl2.checkers_addTo_removeFrom_Home(n, myChecker, gameBoard.Nests);
                        }
                    }
                }
            }
        }
        #endregion


        #region GAME STATES

        /*0.
         * Kas m‰ngija kıik nupud on Kodus?------kontrollida eraldi enne tavalist moveCheckerit???
         *   JAH:
         *      2.1. Kas tal on mıni nupp Nestis, mille m‰ngijaID vastab t‰ringuviske numbrile
         *           vıi kui selle Nest-i m‰ngijaID on v‰iksem kui t‰ringuviske number?
         *           JAH:
         *               k‰i alguses suurema v‰‰rtusega t‰ringu j‰rgi suurema v‰‰rtusega NestID-lt nupp Jar-i
         *               ...korda sammu '2.1. JAH'
         *           EI:
         *              vt. tavaline moveChecker
         *************************************** tavaline moveChecker:
         * 1.
         * Kas m‰ngija mıni nupp on Prison-is?
         *   JAH:
         *            Kas ta saab selle t‰ringuviskega k‰ia lauale?
         *            1.1
         *               Kui saab:
         *                    k‰i lauale
         *                    lisa see nupp t‰ringu nr-le vastavasse Nest-i(paremKlikk: suurem, vasakKlikk: v‰iksem)
         *                    eemalda see nupp Prison-ist
         *            1.2
         *               Kui ei saa: 
         *                    veeretamise kord l‰heb vastasm‰ngijale
         * 
         *ELSE
         *2.
         *Kas m‰ngijal on ruumi k‰ia vastavalt t‰ringuviskele?
         *2.1 EI:
         *       veeretamise kord l‰heb vastasm‰ngijale
         *2.2 JAH:
         *       Kas selle konkreetse nupuga on k‰imisruumi?
         *       2.2.1 JAH:
         *                 Kas selle nupu siht-Nest sisaldab vastase ¸htainsat nuppu:
         *                 JAH: lisa vastase nupp Prison-isse
         *                 EI: k‰i vastavasse Nest-i
         *       2.2.1 EI: ‰ra tee midagi
         *       
         *           
         */

        //0.
        //leia kıik nestid, kus on palyer-i nupud
        public List<Nest> findPlayerCurrentNests(Board gameBoard, Player player)
        {
            List<Checker> playerCheckers = player.checkers;
            List<Nest> palyerHasCheckersInNests = new List<Nest>(24);
            List<Nest> targetNests = new List<Nest>(24);
            foreach (Nest n in gameBoard.Nests)
            {
                foreach (Checker ch in playerCheckers)
                {
                    if (n.Checkers.Contains(ch))
                    {
                        if (!palyerHasCheckersInNests.Contains(n))
                            palyerHasCheckersInNests.Add(n);
                    }
                }
            }
            return palyerHasCheckersInNests;
        }
        //player-i siht-NEST-ide  leidmine
        public List<Nest> findTargetNests(Player player, int step, List<Nest> playerCurrentNests)
        {
            int targetNestID = 0;
            Nest targetNest = null;
            List<Nest> targetNests = new List<Nest>(24);

            foreach (Nest currentNest in playerCurrentNests)
            {
                if (!player.isOpponent)
                {
                    targetNestID = currentNest.paleyerNestID + step;
                    targetNest = this.gameBoard.findNest_ByPaleyerNestID(targetNestID);
                }
                else
                {
                    targetNestID = currentNest.opponentNestID + step;
                    targetNest = this.gameBoard.findNest_ByOpponentNestID(targetNestID);
                }

                if (targetNestContainsNOopponentsChecker(targetNest, player) || targetNestContainsONEopponentsChecker(targetNest, player))
                {
                    targetNests.Add(targetNest);
                }

            }

            return targetNests;
        }
        //player-i siht-NEST-ide  leidmine, kui nupp on vangis
        public List<Nest> findTargetNestsInOpponentsHome(Player player, int step)
        { 
            List<Nest> availabletNests = new List<Nest>(6);
            List<Nest> possibleNests = new List<Nest>(6);
            foreach (Nest n in this.gameBoard.Nests)
            {
                if (!player.isOpponent && n.paleyerNestID< 7)
                {
                    possibleNests.Add(n);// targetNest = this.gameBoard.findNest_ByOpponentNestID(targetNestID);
                }
                else if (player.isOpponent && n.opponentNestID  < 7)
                {
                    possibleNests.Add(n); //targetNest = this.gameBoard.findNest_ByPaleyerNestID(targetNestID);
                }
            }

            foreach (Nest n in possibleNests)
            {
                if (targetNestContainsNOopponentsChecker(n, player) || targetNestContainsONEopponentsChecker(n, player))
                {
                    availabletNests.Add(n);
                }

            }

            return availabletNests;
        }               
                


        
        //MORE THAN ONE OPPONENT'S
        public Boolean targetNestContainsMoreThanOneOpponentsChecker(Nest targetNest, Player player)
        { 
            int numberOfOpponentCheckerInTargetNest = 0;
            if (targetNest != null)
            if (targetNest.Checkers.Count > 0)
            {
                foreach (Checker ch in targetNest.Checkers)
                {
                    if (!player.checkers.Contains(ch))//kui sisalduvad nupud ei ole player-i omad
                    {
                        numberOfOpponentCheckerInTargetNest++;
                    }
                }
            }

            return numberOfOpponentCheckerInTargetNest > 1;
        }
        //ONE OPPONENT'S
        public Boolean targetNestContainsONEopponentsChecker(Nest targetNest, Player player)
        {
            int numberOfOpponentCheckerInTargetNest = 0;
            if (targetNest != null)
            if (targetNest.Checkers.Count > 0)
            {
                foreach (Checker ch in targetNest.Checkers)
                {
                    if (!player.checkers.Contains(ch))//kui sisalduvad nupud ei ole player-i omad
                    {
                        numberOfOpponentCheckerInTargetNest++;
                    }
                }
            }

            return numberOfOpponentCheckerInTargetNest == 1;
        }
        //TARGET NEST IS FREE FROM ENEMY :)
        public Boolean targetNestContainsNOopponentsChecker(Nest targetNest, Player player)
        {
            int numberOfOpponentCheckerInTargetNest = 0;
            if (targetNest != null)
            if (targetNest.Checkers.Count > 0)
            {
                foreach (Checker ch in targetNest.Checkers)
                {
                    if (!player.checkers.Contains(ch))//kui sisalduvad nupud ei ole player-i omad
                    {
                        numberOfOpponentCheckerInTargetNest++;
                    }
                }
            }

            return numberOfOpponentCheckerInTargetNest == 0;
        }
        #endregion
    }
}
