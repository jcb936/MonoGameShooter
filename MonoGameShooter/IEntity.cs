using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace THClone
{
    interface IEntity
    {
        public bool Active { get; set; } 

        public void Update(GameTime gameTime);

        public void Draw(SpriteBatch spriteBatch);
    }
}
