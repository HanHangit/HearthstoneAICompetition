using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SabberStoneCoreAi.src.AI_Networks.MCTS
{
	class UCT
	{
		public static double UctValue(int totalVisit, double nodeWinScore, int nodeVisit)
		{
			double result = 0;

			if (nodeVisit == 0)
				result = Int32.MaxValue;
			else
				result = (nodeWinScore / nodeVisit) + 1.41f * Math.Sqrt(Math.Log(totalVisit) / nodeVisit);

			return result;
		}

		public static Node FindBestNodeUCT(Node node)
		{
			Node result = null;
			int parentVisit = node.State.VisitCount;

			result = node.Children.OrderByDescending(n => UctValue(parentVisit, n.State.WinScore, n.State.VisitCount)).FirstOrDefault();

			return result;
		}
	}
}
