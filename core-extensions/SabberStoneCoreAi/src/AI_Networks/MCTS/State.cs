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
			List<PlayerTask> playerTasks = player.Options();
			var validOpts = Game.Simulate(playerTasks).Where(x => x.Value != null).ToList();

			//if(validOpts.Count() > 1)
			//{
			//	PlayerTask t = playerTasks.Find(x => x.PlayerTaskType == PlayerTaskType.END_TURN);
			//	validOpts.RemoveAll(n => n.Key == t);
			//}

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

			var simGames = Game.Simulate(playerTasks).Where(x => x.Value != null).ToList();
			simGames.OrderByDescending(x => ScoreGame(x.Value, Game.CurrentPlayer.PlayerId));

			Game = simGames.First().Value;
		}

		public static float ScoreGame(POGame.POGame game, int playerId)
		{
			float result = 0;
			Controller player = game.CurrentPlayer.PlayerId == playerId ? game.CurrentPlayer : game.CurrentOpponent;
			Controller enemy = game.CurrentPlayer.PlayerId == playerId ? game.CurrentOpponent : game.CurrentPlayer;

			result += player.Hero.Health * Consts.PlayerHeroHealth;
			result -= enemy.Hero.Health * Consts.EnemyHeroHealth;

			result += MinionAttackOnBoard(player) * Consts.PlayerMinionAttack;
			result += MinionHealthOnBoard(player) * Consts.PlayerMinionHealth;
			result -= MinionAttackOnBoard(enemy) * Consts.EnemyMinionAttack;
			result -= MinionHealthOnBoard(enemy) * Consts.EnemyMinionHealth;

			return result;
		}

		private static int MinionAttackOnBoard(Controller ctrl)
		{
			int result = 0;

			foreach (var item in ctrl.BoardZone)
				result += item.AttackDamage;

			return result;
		}

		private static int MinionHealthOnBoard(Controller ctrl)
		{
			int result = 0;

			foreach (var item in ctrl.BoardZone)
				result += item.Health;

			return result;
		}
	}
}
