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
				result = nodeWinScore + 2 * Math.Sqrt(Math.Log(totalVisit) / nodeVisit);

			return result;
		}

		public static Node FindBestNodeUCT(Node node)
		{
			Node result = null;
			int parentVisit = node.State.VisitCount;
			var test2 = new List<double>();
			foreach (var item in node.Children)
			{
				test2.Add(UCT.UctValue(parentVisit, item.State.WinScore, item.State.VisitCount));
			}
			var test = node.Children.OrderByDescending(n => UctValue(parentVisit, n.State.WinScore, n.State.VisitCount));

			result = node.Children.OrderByDescending(n => UctValue(parentVisit, n.State.WinScore, n.State.VisitCount)).FirstOrDefault();

			return result;
		}
	}
}
