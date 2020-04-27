//Author: Ilan Segal
//File Name: Path.cs
//Project Name: RoboKnight
//Creation Date: December 24, 2015
//Modified Date: January 7, 2016
//Description: The class which represents an enemy's path to a target

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roboknight
{
    class Path
    {
        //Grid used to keep track of room positions
        private PathNode[,] grid;

        //Start and end of the path (pointers to coordinates on the grid)
        private PathNode start;
        private PathNode end;

        //Lists used to generate path
        private List<PathNode> open;
        private List<PathNode> closed;

        //Used to calculate an optimal path
        private const int EDGE_WEIGHT = 10;
        private const int CORNER_WEIGHT = 14;

        //List of all coordinates on the grid which are impassible
        private List<PathNode> obstacles;

        //The actual path
        //NOTE: Used stack because, when the path is being constructed, it starts backwards from the end node.
        //      So, once the path is reconstructed, the coordinates will be in the proper order.
        private Stack<PathNode> path;

        //Variable set by the programmer, which allows for this class to interpret locations properly onto the grid
        private int gridUnit;

        /// <summary>
        /// Constructor for the Path class
        /// </summary>
        /// <param name="gridUnit">Pixel size of each unit on the grid used for pathfinding</param>
        /// <param name="roomWidth">Actual pixel width of the room</param>
        /// <param name="roomHeight">Actual pixel height of the room</param>
        /// <param name="startLoc">Actual starting location of the path (not on the grid)</param>
        /// <param name="endLoc">Actual ending location of the path (not on the grid)</param>
        /// <param name="roomObstacles">List of all Obstacles in the room</param>
        public Path(int gridUnit, int roomWidth, int roomHeight, Vector2 startLoc, Vector2 endLoc, List<Obstacle> roomObstacles)
        {
            //Translating the given room into grid form
            grid = new PathNode[roomWidth / gridUnit, roomHeight / gridUnit];

            //Initally setting all nodes in the grid as disconnected from eachother
            PathNode gridNode;
            for (int y = 0; y < (int)(roomHeight / gridUnit); y++)
            {
                for (int x = 0; x < (int)(roomWidth / gridUnit); x++)
                {
                    gridNode = new PathNode(new Vector2(x * gridUnit, y * gridUnit), null);

                    grid[x, y] = gridNode;
                }
            }

            //Sets the start and end pointers
            start = grid[(int)(startLoc.X / gridUnit), (int)(startLoc.Y / gridUnit)];
            end = grid[(int)(endLoc.X / gridUnit), (int)(endLoc.Y / gridUnit)];

            //Setting up H-Costs of each node on the grid
            this.SetHCosts();

            //Instantiating all the generation lists
            open = new List<PathNode>();
            closed = new List<PathNode>();
            obstacles = new List<PathNode>();

            //Adds the "Start" coordinate to the Open list
            open.Add(start);

            //"Marks" each impassible coordinate on the grid by adding it to the Obstacle list
            foreach (Obstacle obs in roomObstacles)
            {
                //Does not consider the obstacles used to prohibit room movement, since those are outside the room
                if (!obs.GetObstacleType().Equals("door"))
                {
                    obstacles.Add(grid[(int)obs.GetHitbox().GetLocation().X / gridUnit, (int)obs.GetHitbox().GetLocation().Y / gridUnit]);
                }
            }

            //Instantates the path Stack
            path = new Stack<PathNode>();

            this.gridUnit = gridUnit;
        }

        /// <summary>
        /// Gets the "end" node on the path
        /// </summary>
        /// <returns>The "End" PathNode</returns>
        public PathNode GetEnd()
        {
            return end;
        }

        /// <summary>
        /// Gets the path
        /// </summary>
        /// <returns>The path, in Stack form</returns>
        public Stack<PathNode> GetPath()
        {
            return path;
        }

        /// <summary>
        /// Sets the start location
        /// </summary>
        /// <param name="newStart">A valid Vector2</param>
        public void SetStart(Vector2 newStart)
        {
            start = grid[(int)(newStart.X / gridUnit), (int)(newStart.Y / gridUnit)];
        }

        /// <summary>
        /// Sets the end location
        /// </summary>
        /// <param name="newEnd">A valid Vector2</param>
        public void SetEnd(Vector2 newEnd)
        {
            end = grid[(int)(newEnd.X / gridUnit), (int)(newEnd.Y / gridUnit)];

            //Updating heuristic costs
            this.SetHCosts();
        }

        /// <summary>
        /// Calculates path, using A*
        /// </summary>
        public void CalculatePath()
        {
            //Starts at the "Start" node (duh)
            PathNode current = start;
            current.SetHCost(this.ManhattanEstimate(current));

            //Used to compare if a neighbor PathNode's current path can be improved
            int tentativeGCost = 0;

            //Clears lists
            open.Clear();
            closed.Clear();
            path.Clear();

            //Adds start to the Open list
            open.Add(start);

            //Resets all connections between the PathNodes on the grid
            foreach (PathNode n in grid)
            {
                n.SetParent(null);
            }

            //Keeps looping until no nodes are left to explore, or the end node has been found
            while (open.Count != 0)
            {
                //Examines the best possible path of all PathNodes in the Open list
                current = this.GetLowestFCost(open);

                //The end node has been found in the most efficient path
                if (current == end)
                {
                    //Break out of the loop
                    break;
                }

                //The current PathNode is being examined, so move it to the Closed list
                open.Remove(current);
                closed.Add(current);

                //Examines all the PathNodes neighboring the current PathNode
                foreach (PathNode neighbor in this.GetNeighbors(current))
                {
                    //Skips any neighbor nodes that are under the Closed list, because they have already been examined
                    if (closed.Contains(neighbor))
                    {
                        continue;
                    }

                    //Projects what the G-Cost of the PathNode n would be if it's parent was the "current" PathNode
                    if (current.GetLocation().X != neighbor.GetLocation().X && current.GetLocation().Y != neighbor.GetLocation().Y)
                    {
                        //PathNode n is on a diagonal from current
                        tentativeGCost = current.GetGCost() + CORNER_WEIGHT;
                    }
                    else
                    {
                        //Pathnode n is adjacent to current
                        tentativeGCost = current.GetGCost() + EDGE_WEIGHT;
                    }

                    //If neighbor is in the Open list, check the next condition
                    if (open.Contains(neighbor))
                    {
                        //If neighbor's current G-Cost is better than the new one, ignore all following code and skip to the next neighbor
                        if (neighbor.GetGCost() <= tentativeGCost && neighbor.GetGCost() != -1)
                        {
                            continue;
                        }
                    }
                    //If neighbor is in the Closed list, check the next condition
                    else if (closed.Contains(neighbor))
                    {
                        //If neighbor's current G-Cost is better than the new one, ignore all following code and skip to the next neighbor
                        if (neighbor.GetGCost() <= tentativeGCost && neighbor.GetGCost() != -1)
                        {
                            continue;
                        }
                        //The neighbor's just been put on a new path, so reinvestigate it on the next loop
                        else
                        {
                            closed.Remove(neighbor);
                            open.Add(neighbor);
                        }
                    }
                    //The neighbor has not been found by the loop yet, so investigate it on the next loop
                    else
                    {
                        open.Add(neighbor);
                    }

                    //Set neighbor to be connected to current, and set its G-Cost as such
                    neighbor.SetGCost(tentativeGCost);
                    neighbor.SetParent(current);
                }
            }

            //Reconstructs path
            this.ReconstructPath(current);

            //Gets rid of the starting node, since that is the one that the enemy starts on
            path.Pop();
        }

        /// <summary>
        /// Constructs a path, after CalculatePath has made the proper connections
        /// </summary>
        /// <param name="node">A valid Node</param>
        private void ReconstructPath(PathNode node)
        {
            //Clears the existing path
            path.Clear();

            //Pushes the first node onto the stack
            path.Push(node);

            //Keeps looping until the the starting node is at the top of the path stack
            while (path.Peek() != start)
            {
                //Moves to the previous PathNode in the path
                node = node.GetParent();

                //Pushes the previous node onto the stack
                path.Push(node);
            }
        }

        /// <summary>
        /// Gets the PathNodes neighboring the given one on the grid
        /// </summary>
        /// <param name="centreNode">A valid PathNode</param>
        /// <returns>All valid neighboring PathNodes on the grid</returns>
        private List<PathNode> GetNeighbors(PathNode centreNode)
        {
            //Used to keep track of which corners are accessible and which ones are not
            bool canGetTopCorners = true;
            bool canGetBottomCorners = true;
            bool canGetLeftCorners = true;
            bool canGetRightCorners = true;

            //Creates a list of neighbors
            List<PathNode> returnList = new List<PathNode>();

            //Tries to add node above the given node
            if (centreNode.GetLocation().Y / gridUnit - 1 >= 0 && !obstacles.Contains(grid[(int)centreNode.GetLocation().X / gridUnit, (int)centreNode.GetLocation().Y / gridUnit - 1]))
            {
                returnList.Add(grid[(int)centreNode.GetLocation().X / gridUnit, (int)centreNode.GetLocation().Y / gridUnit - 1]);
            }
            else if (centreNode.GetLocation().Y / gridUnit - 1 >= 0)
            {
                canGetTopCorners = false;
            }

            //Tries to add node below the given node
            if (centreNode.GetLocation().Y / gridUnit + 1 < grid.GetLength(1) && !obstacles.Contains(grid[(int)centreNode.GetLocation().X / gridUnit, (int)centreNode.GetLocation().Y / gridUnit + 1]))
            {
                returnList.Add(grid[(int)centreNode.GetLocation().X / gridUnit, (int)centreNode.GetLocation().Y / gridUnit + 1]);
            }
            else if (centreNode.GetLocation().Y / gridUnit - 1 >= 0)
            {
                canGetBottomCorners = false;
            }

            //Tries to add node left of the given node
            if (centreNode.GetLocation().X / gridUnit - 1 >= 0 && !obstacles.Contains(grid[(int)centreNode.GetLocation().X / gridUnit - 1, (int)centreNode.GetLocation().Y / gridUnit]))
            {
                returnList.Add(grid[(int)centreNode.GetLocation().X / gridUnit - 1, (int)centreNode.GetLocation().Y / gridUnit]);
            }
            else if(centreNode.GetLocation().X / gridUnit - 1 >= 0)
            {
                canGetLeftCorners = false;
            }

            //Tries to add node right of the given node
            if (centreNode.GetLocation().X / gridUnit + 1 < grid.GetLength(0) && !obstacles.Contains(grid[(int)centreNode.GetLocation().X / gridUnit + 1, (int)centreNode.GetLocation().Y / gridUnit]))
            {
                returnList.Add(grid[(int)centreNode.GetLocation().X / gridUnit + 1, (int)centreNode.GetLocation().Y / gridUnit]);
            }
            else if(centreNode.GetLocation().X / gridUnit + 1 < grid.GetLength(0))
            {
                canGetRightCorners = false;
            }

            //Tries to add node to the upper left of the given node
            if (centreNode.GetLocation().Y / gridUnit - 1 >= 0 &&
                centreNode.GetLocation().X / gridUnit - 1 >= 0 &&
                !obstacles.Contains(grid[(int)centreNode.GetLocation().X / gridUnit - 1, (int)centreNode.GetLocation().Y / gridUnit - 1]) &&
                canGetTopCorners &&
                canGetLeftCorners)
            {
                returnList.Add(grid[(int)centreNode.GetLocation().X / gridUnit - 1, (int)centreNode.GetLocation().Y / gridUnit - 1]);
            }

            //Tries to add node to the upper right of the given node
            if (centreNode.GetLocation().Y / gridUnit - 1 >= 0 &&
                centreNode.GetLocation().X / gridUnit + 1 < grid.GetLength(0) &&
                !obstacles.Contains(grid[(int)centreNode.GetLocation().X / gridUnit + 1, (int)centreNode.GetLocation().Y / gridUnit - 1]) &&
                canGetTopCorners &&
                canGetRightCorners)
            {
                returnList.Add(grid[(int)centreNode.GetLocation().X / gridUnit + 1, (int)centreNode.GetLocation().Y / gridUnit - 1]);
            }

            //Tries to add node to the lower left of the given node
            if (centreNode.GetLocation().Y / gridUnit + 1 < grid.GetLength(1) &&
                centreNode.GetLocation().X / gridUnit - 1 >= 0 &&
                !obstacles.Contains(grid[(int)centreNode.GetLocation().X / gridUnit - 1, (int)centreNode.GetLocation().Y / gridUnit + 1]) &&
                canGetBottomCorners &&
                canGetLeftCorners)
            {
                returnList.Add(grid[(int)centreNode.GetLocation().X / gridUnit - 1, (int)centreNode.GetLocation().Y / gridUnit + 1]);
            }

            //Tries to add node to the lower right of the given node
            if (centreNode.GetLocation().Y / gridUnit + 1 < grid.GetLength(1) &&
                centreNode.GetLocation().X / gridUnit + 1 < grid.GetLength(0) &&
                !obstacles.Contains(grid[(int)centreNode.GetLocation().X / gridUnit + 1, (int)centreNode.GetLocation().Y / gridUnit + 1]) &&
                canGetBottomCorners &&
                canGetRightCorners)
            {
                returnList.Add(grid[(int)centreNode.GetLocation().X / gridUnit + 1, (int)centreNode.GetLocation().Y / gridUnit + 1]);
            }


            //All available neighbors have been collected
            return returnList;
        }

        /// <summary>
        /// Finds the PathNode with the lowest net cost out of the given List
        /// </summary>
        /// <param name="set">A valid List of PathNodes</param>
        /// <returns>The PathNode with the lowest F-Cost</returns>
        private PathNode GetLowestFCost(List<PathNode> set)
        {
            //Defaults to the first 
            PathNode lowest = set[0];

            //Checks each PathNode in the list except for the first one, because that one is the default
            foreach(PathNode node in set)
            {
                if (node.GetFCost() < lowest.GetFCost())
                {
                    //The current PathNode in set has a lower cost than the "lowest" PathNode, so the current is now marked as "lowest"
                    lowest = node;
                }
            }

            //Returs the PathNode in set with the lowest F-Cost
            return lowest;
        }

        /// <summary>
        /// Extimates distance to the end PathNode, using the Manhattan heuristic
        /// </summary>
        /// <param name="current">A valid PathNode</param>
        /// <returns>Estimated distance between the given node and the end node, without diagonals</returns>
        private int ManhattanEstimate(PathNode current)
        {
            //Calculates differences in x and y axes between end node and the given node
            int xDif = (int)Math.Abs(current.GetLocation().X - end.GetLocation().X);
            int yDif = (int)Math.Abs(current.GetLocation().Y - end.GetLocation().Y);

            //Returns the combined difference
            return xDif + yDif;
        }

        /// <summary>
        /// Sets up each grid node's H-Cost
        /// </summary>
        private void SetHCosts()
        {
            foreach (PathNode node in grid)
            {
                node.SetHCost(this.ManhattanEstimate(node));
            }
        }
    }
}