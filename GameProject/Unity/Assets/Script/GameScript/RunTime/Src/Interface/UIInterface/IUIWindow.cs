namespace MyGame
{
     public interface IUIWindow
     {
          public void Show();
          public void Hide();
          public void OnUpdate(float deltaTime);
          public void OnAwake();
          public void OnDestroy();
          public void OnStart();
     }
}