using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BackGammonn
{
    public class Nest
    {
        public int nestId { get; set; }
        public int leftCorner { get; set; }
        public int rightCorner { get; set; }
        public int y_location { get; set; }
        public String nestTexture { get; set; }
        public int width = 65;
        public int height = 254;
        public Color color { get; set; }
        public int paleyerNestID = 0;
        public int opponentNestID = 0;
        public Boolean isBlot = false;

        private List<Checker> checkers = new List<Checker>();

        //public Boolean isHomeArea { get; set; }


        public Nest(int counter, int incrementer, int boardLeftSide, int middleGap, Color color1, Color color2, String nestTexture, int _y_location)
        {
            //texture
            this.nestTexture = nestTexture;
            this.y_location = _y_location;

            if (incrementer == 0 )
            {
                this.leftCorner = boardLeftSide;
            }
            else if (incrementer >= 6 )
            {
                this.leftCorner = incrementer * this.width + middleGap + boardLeftSide;
            }
            else
            {
                this.leftCorner = incrementer * this.width + boardLeftSide;
            }

            //right corner
            this.rightCorner = this.leftCorner + this.width;


            //color
            if (counter % 2 == 0)
            {
                this.color = color1;
            }
            else
            {
                this.color = color2;
            }
        }


        public int addChecker(Checker checker)//SpriteBatch spriteBatch, Texture2D checkerTexture, String checkerID, String checkerColor, Color checkerTint
        {
            int checker_location_Y = 0;
            if(this.paleyerNestID < 13)
              checker_location_Y = this.y_location;
            else
              checker_location_Y = this.y_location+this.height-45;

            if (checker != null)
            {
                int checkerWidth = checker.width;
                int checkersInThisNest = this.Checkers.Count;
                int checkerYlocationPlus = checkerWidth * checkersInThisNest;

                if (this.paleyerNestID < 13)
                    checker_location_Y += checkerYlocationPlus;
                else
                { 
                    checker_location_Y -= checkerYlocationPlus;
                }

                Vector2 checkerLocation = new Vector2(this.leftCorner+10, checker_location_Y);
                checker.location = checkerLocation;
                this.checkers.Add(checker);
            }
            return this.Checkers.Count;
        }



        public int removeChecker(Checker checker)
        {
            this.Checkers.Remove(checker);
            return this.Checkers.Count;
        }


        public List<Checker> Checkers
        {
            get { return checkers; }
            set { checkers = value; }
        }
    }
}
