using System;
using GGJ2024.Components;
using GGJ2024.Content;
using MoonTools.ECS;
using MoonWorks;

namespace GGJ2024.Systems;

public class GameTimer : MoonTools.ECS.System
{
	GameLoopManipulator GameLoopManipulator;
	ProductSpawner ProductSpawner;

	public GameTimer(World world) : base(world)
	{
		GameLoopManipulator = new GameLoopManipulator(world);
		ProductSpawner = new ProductSpawner(world);
	}

	public override void Update(TimeSpan delta)
	{
		var timerEntity = GetSingletonEntity<Components.GameTimer>();
		var time = Get<Components.GameTimer>(timerEntity).Time;

		var timeBefore = time;

		time -= (float)delta.TotalSeconds;

		Set(timerEntity, new Components.GameTimer(time));

		var timeSpan = new TimeSpan(0, 0, (int)time);
		var timeString = timeSpan.ToString(@"m\:ss"); // this is really bad for string memory usage but whatevsies lol -evan

		Set(timerEntity, new Text(Fonts.KosugiID, 16, timeString, MoonWorks.Graphics.Font.HorizontalAlignment.Center, MoonWorks.Graphics.Font.VerticalAlignment.Middle));

		if (time <= 0)
		{
			GameLoopManipulator.Restart();
		}

		if (OnTime(time, 0, (float)delta.TotalSeconds, 5))
		{
			// respawn products
			ProductSpawner.SpawnProducts();
		}
	}

	public static bool OnTime(float time, float triggerTime, float dt, float loopTime)
		{
			if (loopTime == 0)
			{
				return false;
			}

			var t = time % loopTime;
			return (
				(t <= triggerTime && t + dt >= triggerTime) ||
				(t <= triggerTime + loopTime && t + dt >= triggerTime + loopTime)
				);
		}
}
