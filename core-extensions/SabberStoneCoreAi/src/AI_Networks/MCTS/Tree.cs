using System;
using System.Collections.Generic;
using System.Text;

namespace SabberStoneCoreAi.src.AI_Networks.MCTS
{
	class Tree
	{
		public Node Root { get; set; }

		public Tree(POGame.POGame game)
		{
			Root = new Node(new State(game.getCopy(false), null));
		}

	}
}
