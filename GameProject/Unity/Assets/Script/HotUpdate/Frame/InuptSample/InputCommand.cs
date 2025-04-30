using FixMath.NET;

namespace MyGame
{
    public struct InputCommand 
    {
        public Fix64 Dup;
        public Fix64 Dright; 

        public InputCommand(float dup, float dright)
        {
            Dup = dup;
            Dright = dright; 
        }
    }
}