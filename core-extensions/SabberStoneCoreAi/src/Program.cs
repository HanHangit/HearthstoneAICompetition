#region copyright
// SabberStone, Hearthstone Simulator in C# .NET Core
// Copyright (C) 2017-2019 SabberStone Team, darkfriend77 & rnilva
//
// SabberStone is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License.
// SabberStone is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SabberStoneCore.Config;
using SabberStoneCore.Enums;
using SabberStoneCoreAi.POGame;
using SabberStoneCoreAi.Agent.ExampleAgents;
using SabberStoneCoreAi.Agent;
using SabberStoneCoreAi.src.Agent;
using SabberStoneCoreAi.Meta;
using SabberStoneCoreAi.src.AI_Networks.MCTS;

namespace SabberStoneCoreAi
{
	internal class Program
	{

		private static void Main()
		{
			Console.WriteLine("Setup gameConfig");

			var gameConfig = new GameConfig()
			{
				StartPlayer = 1,
				Player1HeroClass = CardClass.SHAMAN,
				Player2HeroClass = CardClass.SHAMAN,
				Player1Deck = Decks.MidrangeJadeShaman,
				Player2Deck = Decks.MidrangeJadeShaman,
				FillDecks = true,
				Shuffle = true,
				Logging = false,
				History = true
			};

			Console.WriteLine("Setup POGameHandler");
			AbstractAgent player1 = new GreedyAgent();
			AbstractAgent player2 = new HeuristicBot();

			Random rnd = new Random(Guid.NewGuid().GetHashCode());


			var gameHandler = new POGameHandler(gameConfig, player1, player2, repeatDraws: false);

			Console.WriteLine("Simulate Games");
			//gameHandler.PlayGame(debug: true);
			for (int i = 0; i < 100; i++)
			{
				Consts.EnemyHeroHealth = (float)rnd.NextDouble() * 100;
				Consts.EnemyMinionAttack = (float)rnd.NextDouble() * 100;
				Consts.EnemyMinionHealth = (float)rnd.NextDouble() * 100;
				Consts.PlayerHeroHealth = (float)rnd.NextDouble() * 100;
				Consts.PlayerMinionAttack = (float)rnd.NextDouble() * 100;
				Consts.PlayerMinionHealth = (float)rnd.NextDouble() * 100;
				gameHandler.PlayGames(nr_of_games: 100, addResultToGameStats: true, debug: false);
				GameStats gameStats = gameHandler.getGameStats();
				Console.WriteLine(Consts.EnemyHeroHealth);
				Console.WriteLine(Consts.EnemyMinionAttack);
				Console.WriteLine(Consts.EnemyMinionHealth);
				Console.WriteLine(Consts.PlayerHeroHealth);
				Console.WriteLine(Consts.PlayerMinionAttack);
				Console.WriteLine(Consts.PlayerMinionHealth);
				gameStats.printResults();
			}



			Console.ReadLine();
		}
	}
}
