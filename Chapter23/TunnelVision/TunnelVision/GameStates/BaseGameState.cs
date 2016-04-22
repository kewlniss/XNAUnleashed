using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using XELibrary;

namespace TunnelVision
{
    public partial class BaseGameState : GameState
    {
        protected TunnelVision OurGame;
        protected ContentManager Content;

        public BaseGameState(Game game)
            : base(game)
        {
            Content = game.Content;
            OurGame = (TunnelVision)game;
        }
    }
}
