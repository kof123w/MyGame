namespace MyGame
{
    public struct InputCommand 
    {
        private float Dup;
        private float Dright; 

        public InputCommand(float dup, float dright, byte isRunning)
        {
            Dup = dup;
            Dright = dright; 
        }
    }
}