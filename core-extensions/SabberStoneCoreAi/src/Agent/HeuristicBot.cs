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

			foreach (var item in allTasks)
				scoreTasks.Add((item, ScoreTask(item, poGame)));

			scoreTasks.OrderByDescending(x => x.Item2);

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

		private float ScoreTask(PlayerTask task, POGame.POGame game)
		{
			float result = 0;

			switch (task)
			{
				case PlayerTask t when task.PlayerTaskType == PlayerTaskType.PLAY_CARD:
					result = ScorePlayTask(task, game);
					break;
				case PlayerTask t when task.PlayerTaskType == PlayerTaskType.PLAY_CARD:
					result = ScorePlayTask(task, game);
					break;
				default:
					break;
			}

			return result;
		}

		private float ScorePlayTask(PlayerTask task, POGame.POGame game)
		{
			float result = 0;



			return result;
		}

		#endregion
	}
}
