namespace EightPuzzleLibrary
{
    public class Node : IComparable<Node>
    {
        public int[,] State;
        public int Level;
        public int Price; 
        public int Full_cost;
        public Node? Parent;        

        public Node(int[,] state, int level, Node? parent)
        {
            State = new int[3, 3];
            Array.Copy(state, State, state.Length);

            Level = level;
            Parent = parent;
            Price = CalculateManhattan();

            Full_cost = Price + Level;            
        }        
        private int CalculateManhattan()
        {
            int destination = 0;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    int val = State[i, j];
                    if (val == 0) continue;
                    int goal_x = (val - 1) / 3;
                    int goal_y = (val - 1) % 3;
                    destination += Math.Abs(i - goal_x) + Math.Abs(j - goal_y);
                }
            return destination;            
        }

        public bool IsGoal()
        {
            return Price == 0;
        }
       

        public List<Node> GetNeighbors()
        {
            int[,] Moves = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
            List<Node> neighbors = new List<Node>();
            int x = 0, y = 0;

            bool flag = false;
            for (int i = 0; i < 3 && !flag; i++)
            {
                for (int j = 0; j < 3 && !flag; j++)
                {
                    if (State[i, j] == 0)
                    {
                        x = i;
                        y = j;
                        flag = true;
                    }
                }
            }

            for (int i = 0; i < Moves.GetLength(0); i++)
            {
                int new_x = x + Moves[i, 0];
                int new_y = y + Moves[i, 1];

                if (new_x >= 0 && new_x < 3 && new_y >= 0 && new_y < 3)
                {
                    int[,] newState = new int[3, 3];
                    Array.Copy(State, newState, newState.Length);

                    newState[x, y] = newState[new_x, new_y];
                    newState[new_x, new_y] = 0;

                    neighbors.Add(new Node(newState, Level + 1, this));
                }
            }
            return neighbors;
        }

        public static List<Node> ReturnPath(Node? node)
        {
            List<Node> path = new List<Node>();            
            while (node != null)
            {
                path.Add(node);
                node = node.Parent!;
            }
            path.Reverse();
            return path;
        }

        public int CompareTo(Node? other)
        {
            int comparison = Full_cost.CompareTo(other!.Full_cost);
            if (comparison != 0) return comparison;
            return Level.CompareTo(other.Level);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Node other) return false;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (State[i, j] != other.State[i, j])
                        return false;
                }                    
            }                
            return true;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (int num in State)
                hash = hash * 31 + num;
            return hash;
        }        
    }
}
