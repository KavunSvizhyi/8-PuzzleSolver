using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EightPuzzleLibrary
{
    public static class A_Star_Algorithm
    {       
        public static List<Node> Solve(int[,] puzzle)
        {         
            PriorityQueue<Node, int> open_list = new PriorityQueue<Node, int>();

            Node root = new Node(puzzle, 0, null);
            open_list.Enqueue(root, root.Full_cost);

            HashSet<Node> closed_list = new HashSet<Node>();

            while (open_list.Count > 0)
            {            
                Node current = open_list.Dequeue();
                closed_list.Add(current);
            
                if (current.IsGoal())
                    return Node.ReturnPath(current);

                foreach (Node neighbor in current.GetNeighbors())
                {                    
                    if (!closed_list.Contains(neighbor))
                        open_list.Enqueue(neighbor, neighbor.Full_cost);                   
                }
            }
            return new List<Node>();
        }
    }
}
