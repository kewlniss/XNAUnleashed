using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XELibrary
{
    public class FirstPersonCamera : Camera
    {

        public FirstPersonCamera(Game game)
            : base(game)
        {
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            //reset movement vector
            movement = Vector3.Zero;

            if (input.KeyboardState.IsKeyDown(Keys.A) ||
                input.GamePads[playerIndex].IsButtonDown(Buttons.LeftThumbstickLeft))
            {
                movement.X--;
            }
            if (input.KeyboardState.IsKeyDown(Keys.D) ||
                input.GamePads[playerIndex].IsButtonDown(Buttons.LeftThumbstickRight))
            {
                movement.X++;
            }

            if (input.KeyboardState.IsKeyDown(Keys.S) ||
                input.GamePads[playerIndex].IsButtonDown(Buttons.LeftThumbstickDown))
            {
                movement.Z++;
            }
            if (input.KeyboardState.IsKeyDown(Keys.W) ||
                input.GamePads[playerIndex].IsButtonDown(Buttons.LeftThumbstickUp))
            {
                movement.Z--;
            }

            //make sure we don’t increase speed if pushing up and over (diagonal)
            if (movement.LengthSquared() != 0)
                movement.Normalize();

            base.Update(gameTime);
        }

    }
}
