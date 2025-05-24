using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EightPuzzleLibrary
{
    public static class RBFS_Algorithm
    {            
        public static List<Node> Solve(int[,] puzzle)
        {            
            Node start = new Node(puzzle, 0, null);                                
            (Node? solution, _) = RBFS(start, int.MaxValue);                        
            return Node.ReturnPath(solution);
        }
        private static (Node?, int) RBFS(Node current, int f_limit)
        {           
            if (current.IsGoal())            
                return (current, f_limit);

            List<Node> successors = current.GetNeighbors();                    

            while (true)
            {
                successors.Sort();
                Node best = successors[0];               

                if (best.Full_cost > f_limit)
                    return (null, best.Full_cost);

                int alternative = successors[1].Full_cost;
                
                (Node? result, best.Full_cost) = RBFS(successors[0], Math.Min(f_limit, alternative));                
                if (result != null)
                    return (result, best.Full_cost);
            }
        }                
    }
}
