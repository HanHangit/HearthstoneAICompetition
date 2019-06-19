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
			int wins = 0;

			for (int i = 0; i < NUM_GAMES; i++)
			{
				simNode = new Node(node);

				while (simNode.State.Game.State != SabberStoneCore.Enums.State.COMPLETE)
				{
					if (simNode.State.Game.CurrentPlayer.PlayerId != playerID)
					{
						foreach (var item in simNode.State.Game.CurrentPlayer.HandZone)
						{
							item.Card = Decks.RenoKazakusMage[rnd.Next(Decks.RenoKazakusMage.Count)];
						}
					}
					else
					{

					}

					try
					{
						simNode.State.RandomPlay();
						//simNode.State.Game.FullPrint();
					}
					catch (Exception)
					{
						break;
					}
				}
				if (simNode.State.Game.State == SabberStoneCore.Enums.State.COMPLETE
					&& node.State.Game.CurrentPlayer.PlayerId == playerID)
				{
					wins++;
				}
			}
			result = wins;

			Controller simEnemy = simNode.State.Game.CurrentPlayer.PlayerId == playerID ? simNode.State.Game.CurrentOpponent : simNode.State.Game.CurrentPlayer;
			Controller simPlayer = simNode.State.Game.CurrentPlayer.PlayerId == playerID ? simNode.State.Game.CurrentPlayer : simNode.State.Game.CurrentOpponent;
			//result += simEnemy.Hero.Health - simPlayer.Hero.Health;

			//if(simEnemy.BoardZone.Count == 0)
			//	result += 10000;

			//foreach (var item in simPlayer.BoardZone)
			//	result += item.Damage;

			//result += (simPlayer.Hero.Health - simEnemy.Hero.Health) * 1000;




			return result;
		}

	}
}
