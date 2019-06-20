using SabberStoneCore.Config;
using SabberStoneCore.Enums;
using SabberStoneCoreAi.Agent;
using SabberStoneCoreAi.Agent.ExampleAgents;
using SabberStoneCoreAi.Meta;
using SabberStoneCoreAi.POGame;
using SabberStoneCoreAi.src.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SabberStoneCoreAi.src.AI_Networks.EA
{
	class EA
	{
		private int _iterations = 0;
		private int _popSize = 0;
		private int _numGames = 0;
		private int _selectSize = 0;
		private int _valueSize = 0;
		private double _mutChance = 0;
		private double _mutRange = 1;
		private Random _rnd = new Random(Guid.NewGuid().GetHashCode());

		#region Configs

		private GameConfig gameConfig1 = new GameConfig()
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
		private GameConfig gameConfig2 = new GameConfig()
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
		private GameConfig gameConfig3 = new GameConfig()
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

		#endregion

		public EA(int iterations, float mutChance, int popSize, int numGames, int selectSize, int valueSize)
		{
			_mutChance = mutChance;
			_iterations = iterations;
			_popSize = popSize;
			_numGames = numGames;
			_selectSize = selectSize;
			_valueSize = valueSize;
		}

		public List<double[]> StartEA()
		{
			List<double[]> pop = new List<double[]>();

			pop = InitPop();

			pop = Evaluate(pop);

			for (int i = 0; i < _iterations; i++)
			{
				List<double[]> selection = Selection(pop);
				List<double[]> crossPop = CrossOver(selection);

				Mutate(crossPop);

				List<double[]> evaPop = new List<double[]>();
				evaPop.AddRange(pop);
				evaPop.AddRange(crossPop);

				evaPop = Evaluate(evaPop);

				pop = new List<double[]>();
				for (int j = 0; j < _popSize; j++)
				{
					pop.Add(evaPop[j]);
				}

				if (i % 10 == 0)
				{
					foreach (var item in pop)
					{
						Console.WriteLine();
						Console.Write("[");
						foreach (var value in item)
						{
							Console.Write(value + ",");
						}
						Console.Write("]");
					}
				}
			}

			return pop;
		}

		public void Mutate(List<double[]> pop)
		{
			foreach (var item in pop)
			{
				for (int i = 0; i < item.Length; i++)
				{
					double rand = _rnd.NextDouble();
					if (rand <= _mutChance)
					{
						item[i] += _rnd.NextDouble() * (_mutRange / 2) - _mutRange;
					}
				}
			}
		}

		public List<double[]> CrossOver(List<double[]> pop)
		{
			List<double[]> crossPop = new List<double[]>();
			for (int i = 0; i < pop.Count; i += 2)
			{
				crossPop.Add(Cross(pop[i], pop[i + 1]));
			}

			return crossPop;
		}

		public double[] Cross(double[] first, double[] second)
		{
			double[] result = new double[first.Length];

			int crossPoint = _rnd.Next(result.Length);
			bool zone = _rnd.Next(2) == 1;
			if (zone)
				for (int i = 0; i < result.Length; i++)
					result[i] = i < crossPoint ? first[i] : second[i];
			else
				for (int i = 0; i < result.Length; i++)
					result[i] = i < crossPoint ? second[i] : first[i];

			return result;
		}

		public List<double[]> Selection(List<double[]> pop)
		{
			double maxValue = 0;
			foreach (var item in pop)
				maxValue += item[item.Length - 1];

			List<double[]> selection = new List<double[]>();

			for (int i = 0; i < _selectSize; i++)
			{
				double value = _rnd.NextDouble() * maxValue;
				double help = 0;
				for (int j = 0; j < pop.Count; j++)
				{
					help += pop[j][pop[j].Length - 1];
					if (value <= help)
					{
						selection.Add(pop[j]);
						break;
					}
				}
			}

			return selection;
		}



		private List<double[]> InitPop()
		{
			List<double[]> pop = new List<double[]>();

			for (int i = 0; i < _popSize; i++)
			{
				double[] values = new double[_valueSize + 1];
				for (int j = 0; j < values.Length; j++)
				{
					values[j] = _rnd.NextDouble() * 50;
				}
				values[values.Length - 1] = 0;

				pop.Add(values);
			}

			return pop;
		}

		private List<double[]> Evaluate(List<double[]> pop)
		{
			foreach (var item in pop)
				item[item.Length - 1] = 0;

			foreach (var player1 in pop)
			{
				AbstractAgent agent1 = new GreedyAgent();
				AbstractAgent agent2 = new KISSBot();

				Random rnd = new Random(Guid.NewGuid().GetHashCode());



				var gameHandler1 = new POGameHandler(gameConfig1, agent1, agent2, repeatDraws: false);
				var gameHandler2 = new POGameHandler(gameConfig2, agent1, agent2, repeatDraws: false);
				var gameHandler3 = new POGameHandler(gameConfig3, agent1, agent2, repeatDraws: false);
				//gameHandler.PlayGame(debug: true);
				gameHandler1.PlayGames(nr_of_games: _numGames, addResultToGameStats: true, debug: false);
				gameHandler2.PlayGames(nr_of_games: _numGames, addResultToGameStats: true, debug: false);
				gameHandler3.PlayGames(nr_of_games: _numGames, addResultToGameStats: true, debug: false);

				player1[player1.Length - 1] += gameHandler1.getGameStats().PlayerB_Wins;
				player1[player1.Length - 1] += gameHandler2.getGameStats().PlayerB_Wins;
				player1[player1.Length - 1] += gameHandler3.getGameStats().PlayerB_Wins;
			}

			return pop.OrderByDescending(x => x[x.Length - 1]).ToList();
		}
	}
}
