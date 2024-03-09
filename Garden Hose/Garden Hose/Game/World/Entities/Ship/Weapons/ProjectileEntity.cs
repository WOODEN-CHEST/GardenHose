using GardenHose.Game.World.Entities.Physical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Ship.Weapons;

internal class ProjectileEntity : PhysicalEntity
{
    // Internal fields.
    internal float TimeLeft { get; set; } = 10f;
    internal bool IsCollided { get; set; } = false;



    // Private fields.
    private const float FADE_OUT_TIME = 1f;


    // Constructors.
    public ProjectileEntity(float timeLeft) : base(EntityType.Projectile)
    {
        CollisionHandler = new ProjectileCollisionHandler(this);
        TimeLeft = timeLeft;
        CommonMath.IsCalculated = false;
    }



    // Inherited methods.
    internal override Entity CreateClone()
    {
        throw new NotImplementedException();
    }

    internal override void Tick(GHGameTime time)
    {
        base.Tick(time);

        TimeLeft -= time.WorldTime.PassedTimeSeconds; 
        if (TimeLeft < FADE_OUT_TIME)
        {
            Delete();
            return;
        }

        float Opacity = Math.Clamp((TimeLeft + FADE_OUT_TIME) / FADE_OUT_TIME, 0f, 1f);
        foreach (PartSprite Sprite in MainPart.Sprites)
        {
            Sprite.Opacity = Opacity;
        }
    }
}