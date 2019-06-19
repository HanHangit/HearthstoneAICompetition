using SabberStoneCore.Enums;
using SabberStoneCore.Model;
using SabberStoneCore.Model.Entities;
using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneCoreAi.Meta;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SabberStoneCoreAi.src.AI_Networks.MCTS
{
	class State
	{
		private static readonly Random rnd = new Random();

		public POGame.POGame Game { get; set; }
		public int VisitCount { get; set; }
		public double WinScore => WinAmount / (float)AmountGames;
		public int WinAmount { get; set; }
		public int AmountGames { get; set; }
		public List<string> LastMoves { get; set; }
		public PlayerTask LastMove { get; set; }
		public bool Simulated { get; set; } = false;

		public State(POGame.POGame game, List<string> lastMoves, PlayerTask lastMove = null)
		{
			LastMoves = lastMoves;
			Game = game;
			VisitCount = 0;
			LastMove = lastMove;
		}

		public State(State other)
		{
			LastMoves = new List<string>();
			foreach (var item in other.LastMoves)
				LastMoves.Add(item);

			Game = other.Game.getCopy(false);
			VisitCount = other.VisitCount;
			AmountGames = other.AmountGames;
			WinAmount = other.WinAmount;
			LastMove = other.LastMove;
		}

		public List<State> GetAllPossibleStates()
		{
			var result = new List<State>();

			Controller player = Game.CurrentPlayer;
			var validOpts = Game.Simulate(player.Options()).Where(x => x.Value != null);

			foreach (KeyValuePair<PlayerTask, POGame.POGame> item in validOpts)
			{
				var newPlayerTasks = new List<string>();
				if (LastMoves != null)
					newPlayerTasks.AddRange(LastMoves);
				newPlayerTasks.Add(item.Key.ToString());
				result.Add(new State(item.Value, newPlayerTasks, item.Key));
			}

			return result;
		}

		public void RandomPlay()
		{
			List<PlayerTask> playerTasks = Game.CurrentPlayer.Options();
			List<(PlayerTask, float)> tasks = new List<(PlayerTask, float)>();

			foreach (var item in playerTasks)
			{
				tasks.Add((item, ScoreTask(item)));
			}

			var res = tasks.OrderByDescending(x => x.Item2);

			Game.Process(res.First().Item1);
		}

		private float ScoreTask(PlayerTask task)
		{
			float result = rnd.Next(100);

			//if (task.PlayerTaskType == PlayerTaskType.MINION_ATTACK)
			//{
			//	result += rnd.Next(1, 5);

			//	if (task.Target.Health <= task.Source.Card.ATK)
			//	{
			//		result += 10;
			//		if (task.Source.Card.Health > task.Target.AttackDamage)
			//			result += 30;
			//	}
			//}

			//if (task.PlayerTaskType == PlayerTaskType.PLAY_CARD)
			//{
			//	result += rnd.Next(1, 3);
			//}

			if (task.PlayerTaskType == PlayerTaskType.END_TURN)
				result += -1000;

			return result;
		}
	}
}
