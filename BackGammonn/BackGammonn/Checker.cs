using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BackGammonn
{
    public class Checker
    {
        private String color;
        private string id;
        private int y_location { get; set; }
        private int x_location { get; set; }
        public Vector2 location { get; set; }
        public Texture2D checkerTexture { get; set; }
        public int width = 38;


        public Checker(string _id, String _color)//
        {
            this.id = _id;
            this.color = _color;
            //this.checkerTexture = _checkerTexture;
        }

        public void setLocation(int y, int x)
        {
            this.location = new Vector2(x,y);
        }

        public string Id
        {
          get { return id; }
          set { id = value; }
        }
        public String Color
        {
          get { return color; }
          set { color = value; }
        }
    }
}
