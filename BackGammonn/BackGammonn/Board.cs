using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BackGammonn
{
    public class Board
    {
        private List<Nest> nests = new List<Nest>(15);
        public List<Jar> jars = new List<Jar>(2);
        public Prison prison;
        public Vector2 boardLocation { get; set; }
        public int width = 795;
        public int height = 800;
        int middleGap = 43;
        //int middleGap_horizontal = 50;
        //koordinaadid, kust alates toppida pesasid (nest)
        int leftOffset { get; set; }
        int topOffset_upper { get; set; }
        int topOffset_lower { get; set; }
        

        public Board(Vector2 _boardLocation, int leftsideWidth, int topsideWidth_upper, int topsideWidth_lower)
        {
            this.boardLocation = _boardLocation;
            this.leftOffset = (int)this.boardLocation.X + leftsideWidth;
            this.topOffset_upper = (int)this.boardLocation.Y + topsideWidth_upper;
            this.topOffset_lower = (int)this.boardLocation.Y + topsideWidth_lower;
        }

        public List<Nest> addNests(String nestTexture_all, String nestTexture_ylal, Color color1, Color color2)
        {
            //upper nests
            int counter = 0;
            for (int i = 0; i < 12; i++)
            {
                Nest pesa = new Nest(counter, i, this.leftOffset, middleGap, color1, color2, nestTexture_all, this.topOffset_upper);
                this.Nests.Add(pesa);
                counter++;
            }
            //start from right upper corner
            this.Nests.Reverse();
            //lower nests
            counter++;
            for (int i = 0; i < 12; i++)
            {
                Nest pesa = new Nest(counter, i, this.leftOffset, middleGap, color1, color2, nestTexture_ylal, this.topOffset_lower);
                this.Nests.Add(pesa);
                counter++;
            }

            int nestCounterPlayer = 1;
            int nestCounterOpponent = 24;
            foreach (Nest n in this.Nests)
            {
                n.paleyerNestID = nestCounterPlayer;
                n.opponentNestID = nestCounterOpponent;
                nestCounterPlayer++;
                nestCounterOpponent--;
            }
             
            return this.Nests;
        }

        public List<Jar> addJars()
        {
            for (int i = 0; i < 2; i++)
            {
                Jar jar;
                int jar_x = (int)this.boardLocation.X + this.width + 130;
                if(i == 0)
                    jar = new Jar(jar_x, 25, true);
                else
                    jar = new Jar(jar_x, 338, false);

                this.jars.Add(jar);
            }
            return this.jars;
        }

        public Prison addPrison()
        {
            int prisonwidth = 50, prisonheight = 300;
            int prisonX = this.width / 2 - prisonwidth / 2 + this.leftOffset + (int)this.boardLocation.X -15;
            int prisonY = this.height / 2 - prisonheight + this.topOffset_upper + (int)this.boardLocation.Y;
            prison = new Prison(prisonwidth, prisonheight, prisonX, prisonY);
            return prison;
        }

 #region FIND FUNCTIONS

        public Nest findNest_ByPaleyerNestID(int playernestid){
            Nest foundNest = null;
            foreach(Nest n in this.Nests){
                if (n.paleyerNestID == playernestid)
                {
                    foundNest = n;
                    break;
                }
            }
            return foundNest;
        }

        public Nest findNest_ByOpponentNestID(int opponentnestid)
        {
            Nest foundNest = null;
            foreach (Nest n in this.Nests)
            {
                if (n.opponentNestID == opponentnestid)
                {
                    foundNest = n;
                    break;
                }
            }
            return foundNest;
        }

        public Nest findNest_ByLocation(Vector2 mouseLocation)
        {
            Nest foundNest = null;
            foreach (Nest n in this.Nests)
            {
                int nest_Y_loc_max = n.y_location + n.height;

                if (mouseLocation.X >= n.leftCorner && mouseLocation.X <= n.rightCorner)
                {
                    if (mouseLocation.Y >= n.y_location && mouseLocation.Y <= nest_Y_loc_max)
                    {
                        foundNest = n;
                        break;
                    }
                }
            }
            return foundNest;
        }

 #endregion


        //getter-setter
        public List<Nest> Nests
        {
            get { return nests; }
            set { nests = value; }
        }

    }


}
