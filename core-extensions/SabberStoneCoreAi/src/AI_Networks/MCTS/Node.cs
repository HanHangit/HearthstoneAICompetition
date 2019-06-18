using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SabberStoneCoreAi.src.AI_Networks.MCTS
{
	class Node
	{
		private Random _rnd = new Random(Guid.NewGuid().GetHashCode());

		public State State { get; private set; }
		public Node Parent { get; set; }
		public List<Node> Children { get; set; }
		public bool FullyExpanded { get; set; } = false;
		public bool IsLeaf { get; set; } = true;

		public Node(State state)
		{
			State = new State(state);
			Children = new List<Node>();
			IsLeaf = state.Game.State != SabberStoneCore.Enums.State.RUNNING;
		}

		public Node(Node other) : this(other.State)
		{
			if (other.Parent != null)
				Parent = other.Parent;
			foreach (Node item in other.Children)
				Children.Add(new Node(item));
			IsLeaf = other.IsLeaf;
			FullyExpanded = other.FullyExpanded;
		}

		public Node Expand()
		{
			List<State> possibleStates = State.GetAllPossibleStates();
			Node result = null;

			//--Entfernt alle Nachfolger, die schon vorhanden sind--//
			if (!FullyExpanded && !IsLeaf)
			{
				possibleStates.RemoveAll(x => Children.Any(n => n.State.LastMoves.Last().ToString() == x.LastMoves.Last().ToString()));
				if (possibleStates.Count == 0)
				{

				}
				result = new Node(possibleStates[_rnd.Next(possibleStates.Count())]);
				result.Parent = this;
				Children.Add(result);

				if (possibleStates.Count == 1)
					FullyExpanded = true; 
			}

			return result;
		}

		public Node ExpandFull()
		{
			List<State> possibleStates = State.GetAllPossibleStates();
			State nextState = possibleStates[_rnd.Next(possibleStates.Count)];
			Node result = null;

			foreach (var item in possibleStates)
			{
				result = new Node(item);
				result.Parent = this;
				Children.Add(result); 
			}
			
			FullyExpanded = true;
			return result;
		}
	}
}
