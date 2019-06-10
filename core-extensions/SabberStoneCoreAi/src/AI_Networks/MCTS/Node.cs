using System;
using System.Collections.Generic;
using System.Text;

namespace SabberStoneCoreAi.src.AI_Networks.MCTS
{
	class Node
	{
		public State State { get; private set; }
		public Node Parent { get; set; }
		public List<Node> Children { get; set; }

		public Node(State state)
		{
			State = new State(state);
			Children = new List<Node>();
		}

		public Node(Node other)
		{
			State = new State(other.State);
			Children = new List<Node>();
			if (other.Parent != null)
				Parent = other.Parent;
			foreach (Node item in other.Children)
				Children.Add(new Node(item));
		}
	}
}
