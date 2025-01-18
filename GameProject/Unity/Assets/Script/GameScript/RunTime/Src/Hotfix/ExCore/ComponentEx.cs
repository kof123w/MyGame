namespace MyGame
{
    public static class ComponentEx  
    {
        public static void Active(this ComponentData self,bool value)
        {
            self.Enable = value;
        }
    }
}


