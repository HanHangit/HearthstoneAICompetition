﻿using SabberStoneCore.Model;
using SabberStoneCore.Model.Entities;
using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneCoreAi.Agent;
using SabberStoneCoreAi.POGame;
using SabberStoneCoreAi.src.AI_Networks.MCTS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SabberStoneCoreAi.src.Agent
{
	class HeuristicBot : AbstractAgent
	{
		private double[] _values;

		public HeuristicBot(double[] values)
		{
			_values = values;
		}

		public override void FinalizeAgent()
		{

		}

		public override void FinalizeGame()
		{

		}

		public override PlayerTask GetMove(POGame.POGame poGame)
		{
			PlayerTask result = null;

			List<PlayerTask> allTasks = poGame.CurrentPlayer.Options();

			List<(PlayerTask, double)> scoreTasks = new List<(PlayerTask, double)>();

			var simGames = poGame.Simulate(allTasks).Where(x => x.Value != null);

			foreach (var item in simGames)
				scoreTasks.Add((item.Key, ScoreGame(item.Key, item.Value, poGame.CurrentPlayer.PlayerId)));

			scoreTasks = scoreTasks.OrderByDescending(x => x.Item2).ToList();

			//Console.ForegroundColor = ConsoleColor.Cyan;
			//foreach (var item in scoreTasks)
			//	Console.WriteLine($"[{item.Item2}] " + item.Item1.ToString());
			//Console.ResetColor();

			if (scoreTasks.Any())
				result = scoreTasks.First().Item1;
			else
				result = allTasks.First(x => x.PlayerTaskType == PlayerTaskType.END_TURN);

			return result;
		}

		public override void InitializeAgent()
		{

		}

		public override void InitializeGame()
		{

		}

		#region Helpers

		private double ScoreGame(PlayerTask task, POGame.POGame game, int playerId)
		{
			double result = 0;
			Controller player = game.CurrentPlayer.PlayerId == playerId ? game.CurrentPlayer : game.CurrentOpponent;
			Controller enemy = game.CurrentPlayer.PlayerId == playerId ? game.CurrentOpponent : game.CurrentPlayer;

			if (task.HasSource && task.Source.Card.Name.Contains("Coin"))
				result += CoinValue(game, player);
			if (task.HasSource && task.Source.Card.Type == SabberStoneCore.Enums.CardType.SPELL
				&& task.HasTarget && task.Target.Health > 10)
				result -= 5;

			if (task.HasSource && task.Source.Card.Type == SabberStoneCore.Enums.CardType.SPELL)
				result += SpellOnMinion(task, game, player);

			result += player.Hero.Health;
			result -= enemy.Hero.Health;

			result += MinionAttackOnBoard(player);
			result += MinionHealthOnBoard(player);
			result -= MinionAttackOnBoard(enemy) * 2;
			result -= MinionHealthOnBoard(enemy) * 2;
			result += player.Hero.AttackDamage;

			if (MinionAttackOnBoard(enemy) == 0)
				result += 10;

			if (CanDefeatEnemyHeroWithMinions(player, enemy, canAttack: false) && task.HasTarget && task.Target.Card.Type == SabberStoneCore.Enums.CardType.HERO)
				result += 10000;
			else if (CanDefeatEnemyHeroWithMinions(enemy, player))
				result -= 10000;

			return result;
		}

		private float SpellOnMinion(PlayerTask task, POGame.POGame game, Controller ctrl)
		{
			float result = 0;

			if (task.HasTarget)
			{
				if (task.Target.Health == task.Source.Card.ATK)
					result += 20;
				else if (task.Target.Health == task.Source.Card.ATK - 1)
					result += 5;
				else
					result -= 100;
			}

			return result;
		}

		private float CoinValue(POGame.POGame game, Controller ctrl)
		{
			float result = 0;

			int mana = ctrl.RemainingMana + 1;

			result += MinionWithMana(ctrl, mana) * 3;

			return result;
		}

		private int MinionWithMana(Controller ctrl, int mana)
		{
			int result = -10;

			foreach (var item in ctrl.HandZone)
			{
				if (item.Card.Type == SabberStoneCore.Enums.CardType.MINION && item.Card.Cost == mana)
					result = 1;
			}

			return result;
		}

		private double MinionAttackOnBoard(Controller ctrl)
		{
			double result = 0;

			foreach (var item in ctrl.BoardZone)
				result += item.AttackDamage;

			return result;
		}

		private double MinionHealthOnBoard(Controller ctrl)
		{
			double result = 0;

			foreach (var item in ctrl.BoardZone)
				result += item.Health;

			return result;
		}

		public bool CanDefeatEnemyHeroWithMinions(Controller player, Controller enemy, bool withSpell = true, bool canAttack = true)
		{
			var damagePoints = 0;
			if (withSpell)
				damagePoints += MaxSpellDamage(player, enemy);
			foreach (var item in player.BoardZone)
			{
				if (!item.CantAttackHeroes && (item.CanAttack || canAttack))
				{
					damagePoints += item.AttackDamage;
					if (damagePoints >= enemy.Hero.Health)
					{
						return true;
					}
				}
			}
			return false;
		}

		public int MaxSpellDamage(Controller player, Controller enemy)
		{
			var playerMana = player.RemainingMana;
			var spellCards = new List<Card>();

			foreach (var item in player.HandZone)
			{
				if (item.Card.Type == SabberStoneCore.Enums.CardType.SPELL && item.Card.ATK > 0)
				{
					spellCards.Add(item.Card);
				}
			}
			spellCards = spellCards.OrderByDescending(x => x.ATK).ToList();

			var cost = 0;
			var damage = 0;

			for (int i = 0; i < spellCards.Count; i++)
			{
				if (spellCards[i].Cost <= playerMana - cost)
				{
					cost += spellCards[i].Cost;
					damage += spellCards[i].ATK;
				}
			}

			return damage;
		}

		#endregion
	}
}
