﻿#region copyright
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
				Player1HeroClass = CardClass.MAGE,
				Player2HeroClass = CardClass.MAGE,
				Player1Deck = Decks.RenoKazakusMage,
				Player2Deck = Decks.RenoKazakusMage,
				FillDecks = true,
				Shuffle = true,
				Logging = false,
				History = true
			};

			Console.WriteLine("Setup POGameHandler");
			AbstractAgent player1 = new HeuristicBot();
			AbstractAgent player2 = new GreedyAgent();
			var gameHandler = new POGameHandler(gameConfig, player1, player2, repeatDraws: false);

			Console.WriteLine("Simulate Games");
			gameHandler.PlayGame(debug: true);
			//gameHandler.PlayGames(nr_of_games:30, addResultToGameStats:true, debug:false);
			GameStats gameStats = gameHandler.getGameStats();
			

			gameStats.printResults();

			Console.ReadLine();
		}
	}
}
