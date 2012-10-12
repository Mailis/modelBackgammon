using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input; 

namespace BackGammonn
{
    public class Jar
    {
        public List<Checker> checkers = new List<Checker>();
        public Player forPlayer;
        public int jar_x, jar_y;
        public Vector2 jarVector;
        public int width = 56, height = 246;
        public int lowerJar_y_position;
        public Boolean isUpper = true;

        public Jar(int _jar_x, int _jar_y, Boolean _isUpper)
        {
            this.jarVector = new Vector2(_jar_x,  _jar_y);
            this.isUpper = _isUpper;
        }

        public int addChecker(Checker checker)//SpriteBatch spriteBatch, Texture2D checkerTexture, String checkerID, String checkerColor, Color checkerTint
        {
            int checker_newlocation_Y = 0;
            if (checker != null && this.checkers.Count < 15)
            {
                int checkerYlocationPlus = checker.width * this.checkers.Count;

                checker_newlocation_Y += checkerYlocationPlus;
                 
                if (this.isUpper == false)
                {
                    //alumise jari alumine serv
                    checker_newlocation_Y = this.jar_y + this.height;
                    //y-koordinaat liigub ülesoole, kui lisatakse checker
                    checker_newlocation_Y -= checkerYlocationPlus;
                }
                Vector2 checkerLocation = new Vector2(this.jar_x + 10, checker_newlocation_Y);
                checker.location = checkerLocation;
                this.checkers.Add(checker);
            }
            return this.checkers.Count;
        }

        public Boolean jarIsFull()
        {
            return this.checkers.Count == 15;
        }

        public Boolean jarIsEmpty()
        {
            return this.checkers.Count == 0;
        }


        public int jarCheckersCount()
        {
            return this.checkers.Count;
        }

    }
}
