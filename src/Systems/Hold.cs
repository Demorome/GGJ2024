
using System;
using MoonWorks.Math.Float;
using MoonTools.ECS;
using MoonWorks.Graphics;
using GGJ2024.Components;

namespace GGJ2024.Systems;

public class Hold : MoonTools.ECS.System
{
    MoonTools.ECS.Filter TryHoldFilter;
    MoonTools.ECS.Filter CanHoldFilter;
    float HoldSpeed = 32.0f;

    public Hold(World world) : base(world)
    {
        TryHoldFilter =
            FilterBuilder
            .Include<Rectangle>()
            .Include<Position>()
            .Include<CanHold>()
            .Include<TryHold>()
            .Build();

        CanHoldFilter =
            FilterBuilder
            .Include<Rectangle>()
            .Include<Position>()
            .Include<CanHold>()
            .Build();
    }

    void HoldOrDrop(Entity e)
    {
        if (!HasOutRelation<Holding>(e))
        {
            bool holding = false;

            foreach (var o in OutRelations<Colliding>(e))
            {
                if (Has<CanBeHeld>(o))
                {
                    Set(o, Color.Yellow);
                    holding = true;
                    Relate(e, o, new Holding());
                }
            }

            if (!holding)
            {
                foreach (var i in InRelations<Colliding>(e))
                {
                    if (Has<CanBeHeld>(i))
                    {
                        Set(i, Color.Yellow);
                        Relate(e, i, new Holding());
                    }
                }
            }
        }
        else
        {
            var holding = OutRelationSingleton<Holding>(e);
            Remove<Color>(holding);
            Remove<Velocity>(holding);
            UnrelateAll<Holding>(e);
        }
    }

    void SetHoldVelocity(Entity e, float dt)
    {
        var holding = OutRelationSingleton<Holding>(e);
        var holdingPos = Get<Position>(holding);
        var holderPos = Get<Position>(e);

        var vel = holderPos - holdingPos;

        Set(holding, new Velocity(vel * HoldSpeed * dt));
    }

    public override void Update(TimeSpan delta)
    {
        foreach (var e in CanHoldFilter.Entities)
        {
            if (Has<TryHold>(e))
                HoldOrDrop(e);

            if (HasOutRelation<Holding>(e))
                SetHoldVelocity(e, (float)delta.TotalSeconds);

        }

    }
}
