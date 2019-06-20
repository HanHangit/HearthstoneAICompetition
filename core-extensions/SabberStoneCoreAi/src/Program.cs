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
using SabberStoneCoreAi.src.AI_Networks.EA;

namespace SabberStoneCoreAi
{
	internal class Program
	{

		private static void Main()
		{
			Console.WriteLine("Setup gameConfig");
			double[] values =
				new double[] {41.9584023938321, 20.9314780959075, 42.5709950325876, 29.2160915509873, 3.02713018633757,
					39.1902154319408, 16.5569344163206, 41.7280641359873, 25.5307942824582, 44.74108665634, 26.4395090874469,
					34.6203779911717 };

			//var gameConfig = new GameConfig()
			//{
			//	StartPlayer = 1,
			//	Player1HeroClass = CardClass.SHAMAN,
			//	Player2HeroClass = CardClass.SHAMAN,
			//	Player1Deck = Decks.MidrangeJadeShaman,
			//	Player2Deck = Decks.MidrangeJadeShaman,
			//	FillDecks = true,
			//	Shuffle = true,
			//	Logging = false,
			//	History = true
			//};

			//EA ea = new EA(100, 0.1f, 6, 10, 4, 12);
			//var result = ea.StartEA();

			//Console.WriteLine("Values");
			//foreach (var item in result)
			//{
			//	Console.WriteLine(String.Join(',', item));
			//}

			Console.WriteLine("Setup POGameHandler");
			AbstractAgent player1 = new GreedyAgent();
			AbstractAgent player2 = new KISSBot();

			Random rnd = new Random(Guid.NewGuid().GetHashCode());

			GameConfig gameConfig1 = new GameConfig()
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
			GameConfig gameConfig2 = new GameConfig()
			{
				StartPlayer = 1,
				Player1HeroClass = CardClass.MAGE,
				Player2HeroClass = CardClass.MAGE,
				Player1Deck = Decks.RenoKazakusMage,
				Player2Deck = Decks.RenoKazakusMage,
				FillDecks = true,
				Shuffle = true,
				Logging = false,
				History = true
			};
			GameConfig gameConfig3 = new GameConfig()
			{
				StartPlayer = 1,
				Player1HeroClass = CardClass.WARRIOR,
				Player2HeroClass = CardClass.WARRIOR,
				Player1Deck = Decks.AggroPirateWarrior,
				Player2Deck = Decks.AggroPirateWarrior,
				FillDecks = true,
				Shuffle = true,
				Logging = false,
				History = true
			};

			Console.WriteLine("Simulate Games");
			var gameHandler1 = new POGameHandler(gameConfig1, player2, player1, repeatDraws: false);
			var gameHandler11 = new POGameHandler(gameConfig1, player1, player2, repeatDraws: false);
			var gameHandler2 = new POGameHandler(gameConfig2, player2, player1, repeatDraws: false);
			var gameHandler21 = new POGameHandler(gameConfig2, player1, player2, repeatDraws: false);
			var gameHandler3 = new POGameHandler(gameConfig3, player2, player1, repeatDraws: false);
			var gameHandler31 = new POGameHandler(gameConfig3, player1, player2, repeatDraws: false);
			//gameHandler11.PlayGame(debug: true);
			//gameHandler11.getGameStats().printResults();
			gameHandler1.PlayGames(nr_of_games: 100, addResultToGameStats: true, debug: false);
			gameHandler1.getGameStats().printResults();

			gameHandler11.PlayGames(nr_of_games: 100, addResultToGameStats: true, debug: false);
			gameHandler11.getGameStats().printResults();

			gameHandler2.PlayGames(nr_of_games: 100, addResultToGameStats: true, debug: false);
			gameHandler2.getGameStats().printResults();

			gameHandler21.PlayGames(nr_of_games: 100, addResultToGameStats: true, debug: false);
			gameHandler21.getGameStats().printResults();

			gameHandler3.PlayGames(nr_of_games: 100, addResultToGameStats: true, debug: false);
			gameHandler3.getGameStats().printResults();

			gameHandler31.PlayGames(nr_of_games: 100, addResultToGameStats: true, debug: false);
			gameHandler31.getGameStats().printResults();

			Console.ReadLine();
		}
	}
}
