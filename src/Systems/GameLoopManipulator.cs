using GGJ2024;
using GGJ2024.Components;
using GGJ2024.Content;
using GGJ2024.Messages;
using GGJ2024.Systems;
using MoonTools.ECS;

public class GameLoopManipulator : MoonTools.ECS.Manipulator
{
	Filter ScoreFilter;
	Filter PlayerFilter;
	Filter GameTimerFilter;

	ProductSpawner ProductSpawner;

	public GameLoopManipulator(World world) : base(world)
	{
		PlayerFilter = FilterBuilder.Include<Player>().Build();
		ScoreFilter = FilterBuilder.Include<Score>().Build();
		GameTimerFilter = FilterBuilder.Include<GGJ2024.Components.GameTimer>().Build();

		ProductSpawner = new ProductSpawner(world);
	}

	public void Restart()
	{
		Set(GameTimerFilter.NthEntity(0), new GGJ2024.Components.GameTimer(90));

		var playerOne = PlayerFilter.NthEntity(0);
		var playerTwo = PlayerFilter.NthEntity(1);

		Set(playerOne, new Position(Dimensions.GAME_W * 0.47f + 0 * 48.0f, Dimensions.GAME_H * 0.25f));
		Set(playerTwo, new Position(Dimensions.GAME_W * 0.47f + 1 * 48.0f, Dimensions.GAME_H * 0.25f));

		foreach (var entity in ScoreFilter.Entities)
		{
			Set(entity, new Score(0));
			Set(entity, new Text(Fonts.KosugiID, 8, "0"));
		}

		World.Send(new PlaySongMessage());

		// respawn products

		ProductSpawner.ClearProducts();
		ProductSpawner.SpawnProducts();

		// reset orders
	}
}
