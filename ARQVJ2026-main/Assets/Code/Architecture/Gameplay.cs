using Architecture.Updateable;

namespace Architecture
{
    public sealed class Gameplay : IUpdateable
    {
        private float data;
        public float Data => data;

        public Gameplay()
        {
            this.data = 0;
        }


        public void Update(float deltaTime)
        {
            data += deltaTime;
        }
    }
}