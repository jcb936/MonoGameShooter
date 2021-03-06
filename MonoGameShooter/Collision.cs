using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THClone
{
    public class CollisionComparer : IEqualityComparer<Collision>
    {
        public bool Equals(Collision a, Collision b)
        {
            if ((a == null) || (b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public int GetHashCode(Collision a)
        {
            return a.GetHashCode();
        }
    }

    public class Collision
    {
		public ICollidable A;
        public ICollidable B;

        public Collision(ICollidable a, ICollidable b)
        {
            A = a;
            B = b;
        }

        public bool Equals(Collision other)
        {
            if (other == null) return false;

            if ((this.A.Equals(other.A) && this.B.Equals(other.B)))
            {
                return true;
            }

            return false;
        }

        public void Resolve()
        {
            if (this.A.Active && this.B.Active)
                this.A.OnCollision(this.B);
        }

    }
}
