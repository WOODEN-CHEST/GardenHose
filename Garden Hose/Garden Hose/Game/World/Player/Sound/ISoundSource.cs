using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Player.Sound;

internal interface ISoundSource
{
    // Fields.
    public Vector2 SoundSourcePosition { get; }
    public Vector2 SoundSourceMotion { get; }
}