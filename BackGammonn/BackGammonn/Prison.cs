using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input; 

namespace BackGammonn
{
    public class Prison
    {
        public int prison_x, prison_y, rightEdge, bottomEdge;
        public int width, height;
        public Vector2 asukoht, center;

        public Prison(int _width, int _height, int _X, int _Y){
            this.width = _width;
            this.height = _height;
            this.prison_x = _X;
            this.prison_y = _Y;
            this.asukoht = new Vector2(_X, _Y);
            this.bottomEdge = _Y + _height;
            this.rightEdge = _X + _width;
            this.center = new Vector2(_X + _width / 2, _Y + _height/2);
        }

        public List<Checker> checkers = new List<Checker>();

        public int addChecker(Checker checker, Nest cameFromNest)
        {
            if (checker != null && cameFromNest != null)
            {
                cameFromNest.removeChecker(checker);
                this.checkers.Add(checker);
                int numCheckers = this.numberOfCheckers();
                int ch_Ypos = (int)this.center.Y + numCheckers*checker.width;
                checker.location = new Vector2(this.center.X, ch_Ypos);
            }
            return this.numberOfCheckers();
        }

        public int removeChecker(Checker checker)
        {
            this.checkers.Remove(checker);
            return this.numberOfCheckers();
        }

        public int numberOfCheckers()
        {
            return this.checkers.Count;
        }

        public Boolean isInPrison(Checker checker)
        {
            Boolean isInPrison = false;
            foreach (Checker ch in this.checkers)
            {
                if(ch == checker){
                    isInPrison = true;
                    break;
                }
            }
            return isInPrison;
        }
    }
}
