using Stride.Engine;
using Stride.Input;

namespace RampAndMapExperiment
{
    public class BasicMovement : SyncScript
    {
        public Entity entity;

        public override void Update()
        {
            if (Input.HasKeyboard)
            {
                if (Input.IsKeyDown(Keys.Up))
                {
                    entity.Transform.Position.Z -= 0.125f;
                }
                else if (Input.IsKeyDown(Keys.Down))
                {
                    entity.Transform.Position.Z += 0.125f;
                }

                if (Input.IsKeyDown(Keys.Left))
                {
                    entity.Transform.Position.X -= 0.125f;
                }
                else if (Input.IsKeyDown(Keys.Right))
                {
                    entity.Transform.Position.X += 0.125f;
                }
            }
        }
    }
}
