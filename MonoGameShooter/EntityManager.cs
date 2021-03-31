using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace THClone
{
    class EntityManager
    {
        public static EntityManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new EntityManager();
                return instance;
            }
        }

        private static EntityManager instance;

        private List<IEntity> entities;

        private Queue<IEntity> entitiesToAdd;

        private EntityManager()
        {
            entities = new List<IEntity>();
            entitiesToAdd = new Queue<IEntity>();
        }

        public void Update(GameTime gameTime)
        {
            // update entities
            foreach (var entity in entities)
                entity.Update(gameTime);

            for (int i = entities.Count - 1; i >= 0; --i)
            {
                if (!entities[i].Active)
                    entities.RemoveAt(i);
            }

            // add entities
            while (entitiesToAdd.Count != 0)
            {
                entities.Add(entitiesToAdd.Dequeue());
            }
        }

        public void DrawEntities(SpriteBatch spriteBatch)
        {
            foreach (var entity in entities)
                entity.Draw(spriteBatch);
        }

        public void AddEntity(IEntity entity)
        {
            entitiesToAdd.Enqueue(entity);
        }

    }
}
