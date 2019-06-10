using SabberStoneCore.Model.Entities;
using SabberStoneCore.Tasks.PlayerTasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SabberStoneCoreAi.src.AI_Networks.MCTS
{
	class MCT
	{
		private readonly int WIN_SCORE = 3000;
		private readonly float calcTime = 5000f;
		private readonly int NUM_GAMES = 10;
		private readonly Random rnd = new Random();
		private bool _debug = false;

		public MCT(bool debug = false)
		{
			_debug = debug;
		}

		public PlayerTask FindNextMove(POGame.POGame game)
		{
			PlayerTask result = null;
			var tree = new Tree(game);
			Node rootNode = tree.Root;

			var watch = new Stopwatch();
			watch.Start();
			while (watch.ElapsedMilliseconds <= calcTime)
			{
				Node nextNode = SelectNextPromisingNode(rootNode);
				if (nextNode.State.Game.State == SabberStoneCore.Enums.State.RUNNING)
					ExpandNode(nextNode);

				if (nextNode.Children.Count > 0)
					nextNode = nextNode.Children[rnd.Next(nextNode.Children.Count)];

				double playResult = SimulateRandomPlayout(nextNode, game.CurrentPlayer.PlayerId);
				BackPropagation(nextNode, playResult);
			}

			var bestMoves = rootNode.Children.OrderByDescending(n => n.State.WinScore).ToList();
			if (_debug)
			{
				Console.ForegroundColor = ConsoleColor.Cyan;
				foreach (Node item in bestMoves)
					Console.WriteLine($"[{item.State.WinScore}] - {item.State.LastMoves.First()}");
				Console.ResetColor();
			}

			//Console.ReadLine();
			Node winnerNode = bestMoves.First();
			tree.Root = winnerNode;
			result = winnerNode.State.LastMoves.First();

			return result;
		}

		private void BackPropagation(Node node, double score)
		{
			Node helpNode = node;
			while (helpNode != null)
			{
				helpNode.State.VisitCount++;
				helpNode.State.WinScore += score;

				helpNode = helpNode.Parent;
			}
		}

		private void ExpandNode(Node node)
		{
			List<State> possibleStates = node.State.GetAllPossibleStates();

			foreach (State item in possibleStates)
			{
				var newNode = new Node(item) { Parent = node };

				node.Children.Add(newNode);
			}
		}

		private Node SelectNextPromisingNode(Node rootNode)
		{
			Node node = rootNode;
			while (node.Children.Count != 0)
			{
				node = UCT.FindBestNodeUCT(node);
			}
			return node;
		}

		private double SimulateRandomPlayout(Node node, int playerID)
		{
			double result = 0;
			Controller enemy = node.State.Game.CurrentPlayer.PlayerId == playerID ? node.State.Game.CurrentOpponent : node.State.Game.CurrentPlayer;

			var simNode = new Node(node);

			int maxRounds = 10;
			int round = 0;

			while (simNode.State.Game.State != SabberStoneCore.Enums.State.COMPLETE && round < maxRounds)
			{
				round++;
				try
				{
					simNode.State.RandomPlay();
				}
				catch (Exception ex)
				{
					break;
				}
			}

			if (node.State.Game.State == SabberStoneCore.Enums.State.COMPLETE
				&& node.State.Game.CurrentPlayer.PlayerId == playerID)
			{
				result += WIN_SCORE;
			}
			Controller simEnemy = simNode.State.Game.CurrentPlayer.PlayerId == playerID ? simNode.State.Game.CurrentOpponent : simNode.State.Game.CurrentPlayer;
			Controller simPlayer = simNode.State.Game.CurrentPlayer.PlayerId == playerID ? simNode.State.Game.CurrentPlayer : simNode.State.Game.CurrentOpponent;
			result += simPlayer.Hero.Health - simEnemy.Hero.Health;
			result += simPlayer.BoardZone.Count() - simEnemy.BoardZone.Count();

			return result;
		}

	}
}
