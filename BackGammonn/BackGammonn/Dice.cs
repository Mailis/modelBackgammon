using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input; 

namespace BackGammonn
{
    class Dice : Button
    {
        public Boolean diceRolled = false;
        public List<int> rolledValues = new List<int>(4);
        public Boolean isDouble = false;

        public Dice(Texture2D texture, SpriteFont font, SpriteBatch sBatch) : base(texture, font, sBatch) { }

        public List<int> roll()
        {
            this.rolledValues = new List<int>(4);
            Random random = new Random();
            int firstRoll = random.Next(1, 7);//2;// 
            int secondRoll = random.Next(1, 7);//2;//
            if (this.rolledValues.Count == 0)
            {
                this.rolledValues.Add(firstRoll);
                this.rolledValues.Add(secondRoll);
                if (firstRoll == secondRoll)
                {
                    this.rolledValues.Add(firstRoll);
                    this.rolledValues.Add(firstRoll);
                }
            }
            //else
            //{
            //    this.rolledValues[0] = firstRoll;
            //    this.rolledValues[1] = secondRoll;
            //    if (firstRoll == secondRoll)
            //    {
            //        this.rolledValues[2] = firstRoll;
            //        this.rolledValues[3] = secondRoll;
            //    }

            //}
            //double
            if (firstRoll == secondRoll)
            {
                this.isDouble = true;
            }
            else
            {
                this.isDouble = false;
            }
            this.clickText = "";// firstRoll + " " + secondRoll;
            return this.rolledValues;
        }

       
        public override void Update()
        {
            mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Released && oldMouse.LeftButton == ButtonState.Pressed)
            {
                if (location.Contains(new Point(mouse.X, mouse.Y)))
                {
                    this.diceRolled = true;
                    this.roll();
                }
            }
            oldMouse = mouse;
        }

        public void Draw(Color diceColor)
        {
            spriteBatch.Begin();

            if (location.Contains(new Point(mouse.X, mouse.Y)))
            {
                spriteBatch.Draw(image,
                    location,
                    Color.Silver);
            }
            else
            {
                spriteBatch.Draw(image,
                    location,
                    diceColor);
            }

            if (this.diceRolled)
            {
                Vector2 position = new Vector2(400, 300);
                spriteBatch.DrawString(font,
                    clickText,
                    position,
                    Color.White);
            }

            spriteBatch.End();
        }


    }
}
