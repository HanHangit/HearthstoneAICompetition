using System;
using System.Collections.Generic;
using System.Text;
using SabberStoneCore.Tasks;
using SabberStoneCoreAi.Agent;
using SabberStoneCoreAi.POGame;
using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneCoreAi.src.AI_Networks.MCTS;

namespace SabberStoneCoreAi.src.Agent
{
	/// <summary>
	/// Simple MCTS Agent with Random Simulation
	/// </summary>
	class MCTSAgent_01 : AbstractAgent
	{

		public override void FinalizeAgent()
		{

		}

		public override void FinalizeGame()
		{

		}

		public override void InitializeAgent()
		{
		}

		public override void InitializeGame()
		{
		}

		public override PlayerTask GetMove(SabberStoneCoreAi.POGame.POGame poGame)
		{
			MCT mct = new MCT();
			PlayerTask nextPlayerMove = mct.FindNextMove(poGame);
			return nextPlayerMove;
		}
	}
}
