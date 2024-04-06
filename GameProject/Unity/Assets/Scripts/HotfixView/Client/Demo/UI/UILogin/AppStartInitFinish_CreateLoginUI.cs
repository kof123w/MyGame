namespace ET.Client
{
	[Event(SceneType.Demo)]
	public class AppStartInitFinish_CreateLoginUI: AEvent<Scene, AppStartInitFinish>
	{
		protected override async ETTask Run(Scene root, AppStartInitFinish args)
		{
			await UIHelper.Create(root, UIType.UILogin, UILayer.Mid);
			/*Computer computer = root.GetComponent<ComputersComponent>().AddChild<Computer>();
			computer.AddComponent<PCCaseComponent>();
			computer.AddComponent<MonitorComponent,int>(10);
			computer.Open();
			computer.GetComponent<MonitorComponent>().ChangeBrightness(5);
			await root.GetComponent<TimerComponent>().WaitAsync(3000);
			computer?.Dispose();*/
			 
			/*Log.Debug("Start Test!");
			await TestAsync(root);
			int testVal = await TestAsync2(root);
			Log.Debug($"testVal{testVal}");
			Log.Debug("End Test!");*/
		}

		public async ETTask TestAsync(Scene root)
		{ 
			Log.Debug("Enter TestAsync!");
			await root.GetComponent<TimerComponent>().WaitAsync(3000);
			Log.Debug("Leave TestAsync!");
		}

		public async ETTask<int> TestAsync2(Scene root)
		{
			Log.Debug("Enter TestAsync2!");
			await root.GetComponent<TimerComponent>().WaitAsync(3000);
			Log.Debug("Leave TestAsync2!");

			return 10;
		}
	}
}
