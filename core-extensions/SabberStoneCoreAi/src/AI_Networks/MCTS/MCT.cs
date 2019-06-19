using SabberStoneCore.Model;
using SabberStoneCore.Model.Entities;
using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneCoreAi.Meta;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SabberStoneCoreAi.src.AI_Networks.MCTS
{
	class MCT
	{
		private readonly int WIN_SCORE = 1;
		private readonly float calcTime = 5000;
		private readonly int NUM_GAMES = 1;
		private readonly Random rnd = new Random();
		private double MAX_VALUE = 0;

		public PlayerTask FindNextMove(POGame.POGame game)
		{
			PlayerTask result = null;
			var tree = new Tree(game);
			Node rootNode = tree.Root;
			rootNode.ExpandFull();

			//Console.ForegroundColor = ConsoleColor.Yellow;
			//Console.WriteLine("Available Moves");
			//foreach (Node item in rootNode.Children)
			//	Console.WriteLine($"{item.State.LastMoves.First()}");
			//Console.ResetColor();

			var watch = new Stopwatch();
			watch.Start();
			while (watch.ElapsedMilliseconds <= calcTime)
			{
				Node nextNode = SelectNextPromisingNode(rootNode);
				while (!nextNode.IsLeaf && nextNode.FullyExpanded)
					nextNode = SelectNextPromisingNode(nextNode);

				nextNode = ExpandNode(nextNode);

				double playResult = SimulateRandomPlayout(nextNode, game.CurrentPlayer.PlayerId);
				BackPropagation(nextNode, playResult);
			}

			var bestMoves = rootNode.Children.OrderByDescending(n => n.State.WinScore).ToList();
			//if(bestMoves.Count > 1)
			//{
			//	bestMoves.RemoveAll(x => x.State.LastMove.PlayerTaskType == PlayerTaskType.END_TURN);
			//}
			Console.ForegroundColor = ConsoleColor.Cyan;
			foreach (Node item in bestMoves)
				Console.WriteLine($"[{item.State.WinScore}] - {item.State.LastMoves.First()}");
			Console.ResetColor();

			//Console.ReadLine();
			Node winnerNode = bestMoves.First();
			tree.Root = winnerNode;
			result = winnerNode.State.LastMove;

			return result;
		}

		private void BackPropagation(Node node, double score)
		{
			Node helpNode = node;
			while (helpNode != null)
			{
				helpNode.State.VisitCount++;
				helpNode.State.WinAmount += (int)score;
				helpNode.State.AmountGames += NUM_GAMES;

				helpNode = helpNode.Parent;
			}
		}

		private Node ExpandNode(Node node)
		{
			return node.Expand() ?? node;
		}

		private Node SelectNextPromisingNode(Node rootNode)
		{
			Node node = UCT.FindBestNodeUCT(rootNode);
			return node;
		}

		private double SimulateRandomPlayout(Node node, int playerID)
		{
			double result = 0;
			var simNode = new Node(node);
			float wins = 0;

			for (int i = 0; i < NUM_GAMES; i++)
			{
				simNode = new Node(node);

				while (simNode.State.Game.State != SabberStoneCore.Enums.State.COMPLETE)
				{
					try
					{
						simNode.State.RandomPlay();
					}
					catch (Exception)
					{
						break;
					}
				}
				if (simNode.State.Game.CurrentPlayer.PlayState == SabberStoneCore.Enums.PlayState.WON
					&& node.State.Game.CurrentPlayer.PlayerId == playerID)
				{
					wins++;
				}
				else if (simNode.State.Game.CurrentOpponent.PlayState == SabberStoneCore.Enums.PlayState.WON
						&& node.State.Game.CurrentOpponent.PlayerId == playerID)
				{
					wins++;
				}
			}
			result = wins;

			return result;
		}

	}
}
