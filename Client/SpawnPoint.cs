namespace lsrplogin.Client
{
    public class SpawnPoint
    {

        public float x;
        public float y;
        public float z;
        public float heading;
        public string model;
        public bool skipFade = true;

        public SpawnPoint()
        {            
        }

        public SpawnPoint(float x, float y, float z, float heading, string model, bool skipFade = true)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.heading = heading;
            this.model = model;
            this.skipFade = skipFade;
        }
    }
}