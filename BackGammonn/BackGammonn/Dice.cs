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
        private Boolean diceRolled = false;
        public List<int> rolledValues = new List<int>(2);

        public Dice(Texture2D texture, SpriteFont font, SpriteBatch sBatch) : base(texture, font, sBatch) { }

        public List<int> roll()
        {
            Random random = new Random();
            int firstRoll = random.Next(1, 7);
            int secondRoll = random.Next(1, 7);
            if (this.rolledValues.Count == 0)
            {
                this.rolledValues.Add(firstRoll);
                this.rolledValues.Add(secondRoll);
            }
            else
            {
                this.rolledValues[0] = firstRoll;
                this.rolledValues[1] = secondRoll;

            }
            this.clickText = firstRoll + " " + secondRoll;
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
                Vector2 position = new Vector2(10, 75);
                spriteBatch.DrawString(font,
                    clickText,
                    position,
                    Color.White);
            }

            spriteBatch.End();
        }


    }
}
