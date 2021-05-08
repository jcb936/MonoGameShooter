using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace THClone
{
    public interface ICollidable
    {
        #region Fields
        public float BoundingRadius { get; }

        public bool FlaggedForRemoval { get; }

        public Vector2 Position { get; }

        public bool Active { get; set; }

        #endregion

        #region Member Functions
        public bool CollisionTest(ICollidable obj)
        {
            return Vector2.Distance(Position, obj.Position) <= BoundingRadius + obj.BoundingRadius;
        }

        public void OnCollision(ICollidable obj);

        #endregion


    }
}
