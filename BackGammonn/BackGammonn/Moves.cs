using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input; 

namespace BackGammonn
{
    public class Moves
    {
        Player player;
        Nest prevnest;
        Nest nextnest;
        Checker checker;
        Jar jar;
        Prison prison;

        public Moves(Player _player, Nest _prevtnest, Nest _nextnest, Checker checker)
        {
            this.player = _player;
            this.nextnest = _nextnest;
            this.prevnest = _prevtnest;
            this.checker = checker;
        }
        public Moves(Player _player, Nest _prevtnest, Jar _jar, Checker checker)
        {
            this.player = _player;
            this.jar = _jar;
            this.prevnest = _prevtnest;
            this.checker = checker;
        }

        public Moves(Player _player, Nest _prevtnest, Prison _prison, Checker checker)
        {
            this.player = _player;
            this.prison = _prison;
            this.prevnest = _prevtnest;
            this.checker = checker;
        }
    }
}
