using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BackGammonn
{
    public class Player
    {
        public String name { get; set; }
        public String color { get; set; }
        public Boolean isTurnToRoll { get; set; }
        public Boolean isWinner { get; set; }
        public List<Checker> checkers = new List<Checker>(15);
        public List<Moves> moves = new List<Moves>();
        public int currentMoves = 0;

        public Player(String _name, String _color)//, Boolean _isTurnToRoll, Texture2D _checkerTexture
        {
            this.color = _color;
            this.name = _name;
            addCheckersToPlayer();
        }

        public void addCheckersToPlayer()
        {

            for (int i = 1; i <= 15; i++)
            {
                Checker ch = new Checker(this.color + i, this.color);//
                this.checkers.Add(ch);
            }
        }


        public Checker findCheckerByID(string checkerid)
        {
            Checker foundChecker = null;
            foreach (Checker ch in this.checkers)
            {
                if (ch.Id == checkerid)
                {
                    foundChecker = ch;
                    break;
                }
            }
            return foundChecker;
        }
    }
}
