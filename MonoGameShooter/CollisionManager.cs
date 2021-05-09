using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THClone
{
    class CollisionManager
    {
        public static CollisionManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new CollisionManager();
                return instance;
            }
        }

        private static CollisionManager instance;

        private List<ICollidable> m_Collidables = new List<ICollidable>();
        private HashSet<Collision> m_Collisions = new HashSet<Collision>(new CollisionComparer());

        private CollisionManager()
        {}

        public void AddCollidable(ICollidable c)
        {
            m_Collidables.Add(c);
        }

        public void RemoveCollidable(ICollidable c)
        {
            m_Collidables.Remove(c);
        }
           

        public void Update()
        {
            UpdateCollisions();
            ResolveCollisions();
        }

        private void UpdateCollisions()
        {
            if (m_Collisions.Count > 0)
            {
                m_Collisions.Clear();
            }

            // Iterate through collidable objects and test for collisions between each one
            for (int i = 0; i < m_Collidables.Count; i++)
            {
                for (int j = 0; j < m_Collidables.Count; j++)
                {
                    ICollidable collidable1 = m_Collidables[i];
                    ICollidable collidable2 = m_Collidables[j];

                    // Make sure we're not checking an object with itself
                    if (!collidable1.Equals(collidable2))
                    {
                        // If the two objects are colliding then add them to the set
                        if (collidable1.Active && collidable2.Active && collidable1.CollisionTest(collidable2))
                        {
                            m_Collisions.Add(new Collision(collidable1, collidable2));
                        }
                    }
                }
            }

            // Iterate through collidable objects and test for collisions between each one
            for (int i = m_Collidables.Count - 1; i >= 0; --i)
            {
                if (!m_Collidables[i].Active)
                    RemoveCollidable(m_Collidables[i]);

            }
        }


        private void ResolveCollisions()
        {
            foreach (Collision c in m_Collisions)
            {
                c.Resolve();
            }
        }
    }
}
