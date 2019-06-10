using SabberStoneCore.Enums;
using SabberStoneCore.Model;
using SabberStoneCore.Model.Entities;
using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneCoreAi.Meta;
using System;
using System.Collections.Generic;

namespace SabberStoneCoreAi.src.AI_Networks.MCTS
{
	class State
	{
		private static readonly Random rnd = new Random();

		public POGame.POGame Game { get; set; }
		public int VisitCount { get; set; }
		public double WinScore { get; set; }
		public List<PlayerTask> LastMoves { get; set; }
		public bool Simulated { get; set; } = false;

		public State(POGame.POGame game, List<PlayerTask> lastMoves)
		{
			LastMoves = lastMoves;
			Game = game.getCopy(false);
			//if (LastMoves != null)
			//	foreach (PlayerTask item in lastMoves)
			//		Game.Process(item);
			VisitCount = 0;
			WinScore = 0;
		}

		public State(State other)
		{
			LastMoves = other.LastMoves;
			Game = other.Game.getCopy(false);
			VisitCount = other.VisitCount;
			WinScore = other.WinScore;
		}

		public List<State> GetAllPossibleStates()
		{
			var result = new List<State>();

			List<PlayerTask> playerTasks = Game.CurrentPlayer.Options();
			//playerTasks.RemoveAll(x => x.Source?.Card.Id == "LOEA04_31b");

			foreach (PlayerTask item in playerTasks)
			{
				var newPlayerTasks = new List<PlayerTask>();
				if (LastMoves != null)
					newPlayerTasks.AddRange(LastMoves);
				newPlayerTasks.Add(item);
				POGame.POGame game = Game.getCopy();
				game.Process(item);
				if (item != null)
					result.Add(new State(game, newPlayerTasks));
			}

			return result;
		}

		public void RandomPlay()
		{
			List<PlayerTask> playerTasks = Game.CurrentPlayer.Options();
			PlayerTask currPlayerTask = null;

			//playerTasks.RemoveAll(x => x.Source?.Card.Id == "LOEA04_31b");
			currPlayerTask = playerTasks[rnd.Next(playerTasks.Count)];

			Game.Process(currPlayerTask);

		}
	}
}
