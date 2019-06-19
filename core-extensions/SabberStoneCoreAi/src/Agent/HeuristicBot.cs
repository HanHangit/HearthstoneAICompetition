using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneCoreAi.Agent;
using SabberStoneCoreAi.POGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SabberStoneCoreAi.src.Agent
{
	class HeuristicBot : AbstractAgent
	{
		public override void FinalizeAgent()
		{
		}

		public override void FinalizeGame()
		{
		}

		public override PlayerTask GetMove(POGame.POGame poGame)
		{
			PlayerTask result = null;

			List<PlayerTask> allTasks = poGame.CurrentPlayer.Options();

			List<(PlayerTask, float)> scoreTasks = new List<(PlayerTask, float)>();

			var simGames = poGame.Simulate(allTasks).Where(x => x.Value != null);

			foreach (var item in simGames)
				scoreTasks.Add((item.Key, ScoreGame(item.Value, poGame.CurrentPlayer.PlayerId)));

			scoreTasks = scoreTasks.OrderByDescending(x => x.Item2).ToList();

			Console.ForegroundColor = ConsoleColor.Cyan;
			foreach (var item in scoreTasks)
				Console.WriteLine($"[{item.Item2}] " + item.Item1.ToString());
			Console.ResetColor();

			result = scoreTasks.First().Item1;

			return result;
		}

		public override void InitializeAgent()
		{
		}

		public override void InitializeGame()
		{
		}

		#region Helpers

		private float ScoreGame(POGame.POGame game, int playerId)
		{
			float result = 0;

			return result;
		}

		#endregion
	}
}
