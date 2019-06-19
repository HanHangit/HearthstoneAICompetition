using SabberStoneCore.Tasks.PlayerTasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace SabberStoneCoreAi.src
{
	interface ITaskValues
	{
		float ScorePlayCard(PlayerTask task, POGame.POGame game);
		float ScoreSpell(PlayerTask task, POGame.POGame game);
		float ScoreAttack(PlayerTask task, POGame.POGame game);
	}
}
