using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace MagicCircuit
{
    public class CurrentFlow_new : MonoBehaviour
    {
        private List<CircuitItem> circuitItemList;
        private List<List<int>> circuitBranch;  // Store the branches of the whole circuit for CircuitCompare

        private Connectivity[,] connectivity;   // Will modify this when handling switch on/off
		private Connectivity[,] originalConn;   // Connectivity for the whole circuit when all switches on
		private Connectivity[,] currentConn;    // Store the current state of connectivity for switchOn/Off
		private Connectivity[,] L_Matrix;
        private Connectivity[,] R_Matrix;        
        
        private int count;                      // Total number of items
        private int boundary;                   // ID of the first CircuitLine
		bool[] isOpened;


        public bool compute(List<CircuitItem> _circuitItemList, int _level)
        {
			circuitItemList = _circuitItemList;

            initMembers();

			allPowerOff();

			computeConnectivity();

			if (removeOpenedCircuit())
				Debug.Log("CurrentFlow.cs compute() : have open circuit!");

			// Store the connectivity when all switches are on            
			Array.Copy(connectivity, originalConn, connectivity.Length);

			if (!computeLRMatrix())
			{
				Debug.Log("CurrentFlow.cs compute() : Error when computing LRMatrix!");
				return false;
			}

			if (computeCircuitBranch())
			{
				Debug.Log("CurrentFlow.cs compute() : Working Circuit!");
			}
			else
			{
				Debug.Log("CurrentFlow.cs compute() : Error when computing CircuitBranch!");
				return false;
			}

			// Determine whether circuit is correct
			if (!Correctness.computeCorrectness(circuitItemList, _level, circuitBranch))
			{
				Debug.Log("CurrentFlow.cs compute() : Wrong Circuit!");
				allPowerOff();
				return false;
			}

			if (haveNoSwitches())
				allPowerOn();
			else
			{
				allPowerOff();
				// Save to currentConn
				Array.Copy(connectivity, currentConn, connectivity.Length);
			}

			return true;
        }

		public void switchOnOff(int ID, bool state) // State true: on false: off
		{
			// Restore connectivity to current state
			Array.Copy(currentConn, connectivity, currentConn.Length);

			// Reset all circuitItems.powered to false
			for (var i = 0; i < count; i++)
				circuitItemList[i].powered = false;

			if (state)
				switchOn(ID);
			else
				switchOff(ID);

			// Save new current state
			Array.Copy(connectivity, currentConn, connectivity.Length);

			// Re-compute LR Matrices
			if (!computeLRMatrix())
			{
				Debug.Log("CurrentFlow.cs switchOnOff() : Error when computing LRMatrix!");
				return;
			}

			// Will modify connectivity & generate circuitItems as result
//			process();
		}

        private void initMembers()
		{
			count = circuitItemList.Count;
			// Find boundary between cards & lines
			boundary = 0;
			while (boundary < count)
			{
				if (circuitItemList[boundary].type == ItemType.CircuitLine)
					break;
				boundary++;
			}

			circuitBranch = new List<List<int>>();
			circuitBranch.Add(new List<int>());			          

			connectivity = new Connectivity[count, count];
			originalConn = new Connectivity[count, count];
			currentConn  = new Connectivity[count, count];
			L_Matrix = new Connectivity[boundary, boundary];
			R_Matrix = new Connectivity[boundary, boundary];

			// Set default value of matrices to zeros
			for (var i = 0; i < count; i++)
				for (var j = 0; j < count; j++)
					connectivity[i, j] = Connectivity.zero;
			for (var i = 0; i < boundary; i++)
				for (var j = 0; j < boundary; j++)
				{
					L_Matrix[i, j] = Connectivity.zero;
					R_Matrix[i, j] = Connectivity.zero;
				}

			// bool[] are all false by default
			isOpened = new bool[count];
		}

		private void allPowerOn()
		{
			for (var i = 0; i < count; i++)
			{
				if (isOpened[i]) continue;
				circuitItemList[i].powered = true;
			}
		}

        private void allPowerOff()
        {
            for (var i = 0; i < count; i++)
				circuitItemList[i].powered = false;
        }

		private void computeConnectivity()
		{
			// For each card, check which line is attached
			// Improve performance?
			for (var i = 0; i < boundary; i++)
				for (var j = boundary; j < count; j++)
				{
					if (inBoxRegion(circuitItemList[i].connect_left, circuitItemList[j].connect_left))
					{
						connectivity[i, j] = Connectivity.l;
						connectivity[j, i] = Connectivity.s;
						circuitItemList[j].list.Insert(0, circuitItemList[i].list[0]);
						continue;
					}
					if (inBoxRegion(circuitItemList[i].connect_right, circuitItemList[j].connect_left))
					{
						connectivity[i, j] = Connectivity.r;
						connectivity[j, i] = Connectivity.s;
						circuitItemList[j].list.Insert(0, circuitItemList[i].list[0]);
						continue;
					}
					if (inBoxRegion(circuitItemList[i].connect_left, circuitItemList[j].connect_right))
					{
						connectivity[i, j] = Connectivity.l;
						connectivity[j, i] = Connectivity.e;
						circuitItemList[j].list.Add(circuitItemList[i].list[0]);
						continue;
					}
					if (inBoxRegion(circuitItemList[i].connect_right, circuitItemList[j].connect_right))
					{
						connectivity[i, j] = Connectivity.r;
						connectivity[j, i] = Connectivity.e;
						circuitItemList[j].list.Add(circuitItemList[i].list[0]);
						continue;
					}
				};

			// For each line, check which line it is connected to
			for (var i = boundary; i < count; i++)
			{
				for (var j = boundary; j < count; j++)
				{
					if (i == j) continue;
					if (isConnected(circuitItemList[i].connect_left, circuitItemList[j].connect_left))
					{ connectivity[i, j] = Connectivity.s; connectivity[j, i] = Connectivity.s; }
					if (isConnected(circuitItemList[i].connect_left, circuitItemList[j].connect_right))
					{ connectivity[i, j] = Connectivity.s; connectivity[j, i] = Connectivity.e; }
				}
				for (var j = boundary; j < count; j++)
				{
					if (i == j) continue;
					if (isConnected(circuitItemList[i].connect_right, circuitItemList[j].connect_left))
					{ connectivity[i, j] = Connectivity.e; connectivity[j, i] = Connectivity.s; }
					if (isConnected(circuitItemList[i].connect_right, circuitItemList[j].connect_right))
					{ connectivity[i, j] = Connectivity.e; connectivity[j, i] = Connectivity.e; }
				}
			}
		}

		// Check if linePoint is in a box region of cardPoint
		private bool inBoxRegion(Vector2 cardPoint, Vector2 linePoint)
		{
			if ((linePoint.x > (cardPoint.x - Constant.POINT_CONNECT_REGION)) && (linePoint.x < (cardPoint.x + Constant.POINT_CONNECT_REGION))
				&& (linePoint.y > (cardPoint.y - Constant.POINT_CONNECT_REGION)) && (linePoint.y < (cardPoint.y + Constant.POINT_CONNECT_REGION)))
				return true;
			else
				return false;
		}

		// Check if two line points are connected
		private bool isConnected(Vector2 point_1, Vector2 point_2)
		{
			if (point_1.x == point_2.x && point_1.y == point_2.y)
				return true;
			else
				return false;
		}

		// Return true if have open circuit
		private bool removeOpenedCircuit()
		{			
			bool haveItemDeleted = false;
			bool haveOpenCircuit = false;

			do
			{
				haveItemDeleted = false;

				for (var i = 0; i < count; i++)
				{
					if (isOpened[i]) continue;

					if (isNotFullyConnected(i))
					{
						isOpened[i] = true;
						removeItemFromConnectivity(i);
						haveItemDeleted = true;
						haveOpenCircuit = true;
						break;
					}
				}
			} while (haveItemDeleted);

			return haveOpenCircuit;
		}

		private bool isNotFullyConnected(int i)
		{
			bool haveStart = false;
			bool haveEnd = false;

			for (var j = 0; j < count; j++)
			{
				if (connectivity[i, j] == Connectivity.l || connectivity[i, j] == Connectivity.s)
					haveStart = true;
				if (connectivity[i, j] == Connectivity.r || connectivity[i, j] == Connectivity.e)
					haveEnd = true;
			}

			// We consider 1 1                 as valid
			//             0 0 and 1 0 and 0 1 as invalid
			// So return true when invalid
			if (haveStart && haveEnd)
				return false;
			else
				return true;
		}

		private void removeItemFromConnectivity(int i)
		{
			for (var j = 0; j < count; j++)
			{
				connectivity[i, j] = Connectivity.zero;
				connectivity[j, i] = Connectivity.zero;
			}
		}

		private bool computeLRMatrix()
		{
			L_Matrix = new Connectivity[boundary, boundary];
			R_Matrix = new Connectivity[boundary, boundary];

			bool LCorrect = false;
			bool RCorrect = false;
			for (var i = 0; i < boundary; i++)
			{
				LCorrect = compute1ComponentLR(i, Connectivity.l);
				RCorrect = compute1ComponentLR(i, Connectivity.r);
				if (!(LCorrect || RCorrect))
					return false;
			}
			return (LCorrect && RCorrect);
		}

		private bool compute1ComponentLR(int ID, Connectivity dir)
		{
			Stack<Vector2> stack = new Stack<Vector2>();

			bool[] visited = new bool[count];
			for (var i = 0; i < count; i++)
				visited[i] = false;

			for (var j = boundary; j < count; j++)
				if (connectivity[ID, j] == dir)
					stack.Push(new Vector2(j, ID));

			while (stack.Count > 0)
			{
				Vector2 v = stack.Pop();

				if (visited[(int)v.x])
					continue;
				visited[(int)v.x] = true;

				for (var i = 0; i < count; i++)
				{
					if (i == (int)v.y || i == ID) continue;

					if (connectivity[i, (int)v.x] != Connectivity.zero)
					{
						if (i < boundary)
						{
							if (dir == Connectivity.l)
							{
								if (L_Matrix[ID, i] == Connectivity.r) return false; // Short circuit
								L_Matrix[ID, i] = connectivity[i, (int)v.x];
							}
							if (dir == Connectivity.r)
							{
								if (R_Matrix[ID, i] == Connectivity.l) return false; // Short circuit
								R_Matrix[ID, i] = connectivity[i, (int)v.x];
							}
						}
						else
							stack.Push(new Vector2(i, (int)v.x));
					}
				}
			}
			return true;
		}

		private bool batteryAdjacency(ref Connectivity c, ref int lineID)
		{
			int num = 0;
			bool flag = false;
			c = Connectivity.zero;
			lineID = 0;

			// Adjacent right
			for (var j = boundary; j < count; j++)
				if (connectivity[0, j] == Connectivity.r)
				{
					num++;
					if (connectivity[1, j] == Connectivity.l)
					{
						c = Connectivity.r;
						lineID = j;
						flag = true;
					}
				}
			if (num == 1 && flag)
				return true;

			// Adjacent left
			num = 0;
			flag = false;
			c = Connectivity.zero;
			lineID = 0;

			for (var j = boundary; j < count; j++)
				if (connectivity[0, j] == Connectivity.l)
				{
					num++;
					if (connectivity[1, j] == Connectivity.r)
					{
						c = Connectivity.l;
						lineID = j;
						flag = true;
					}
				}
			if (num == 1 && flag)
				return true;

			return false;
		}

		private void deleteBattery1(Connectivity c, int lineID)
		{
			for (var j = boundary; j < count; j++)
				if (connectivity[1, j] == c)
				{
					connectivity[lineID, j] = connectivity[lineID, 1];
					connectivity[j, lineID] = connectivity[j, 1];
				}
			for (var j = boundary; j < count; j++)
			{
				connectivity[1, j] = Connectivity.zero;
				connectivity[j, 1] = Connectivity.zero;
			}
		}

		private bool computeCircuitBranch()
		{
			if (!combineBattery()) return false;

			return process();
		}

		private bool process()
		{
			// Traverse all the components to get current flow direction
			Vector2 next = new Vector2(0, 0);  // next : (next, current) ->

			Stack<Vector2> parrallel_start = new Stack<Vector2>();

			// Main circuit
			do
			{
				circuitItemList[(int)next.x].powered = true;
				circuitBranch[0].Add((int)next.x);

				// Modify Connectivity & L & R_Matrix
				if (R_Matrix[(int)next.y, (int)next.x] == Connectivity.r)
					flipComponent((int)next.x);

				parrallel_start = findR(next);

				// Branch circuit
				if (parrallel_start.Count > 1)
				{
					// next = deParrallel
					if (!deParrallel(ref parrallel_start, ref next))
						return false;
				}
				else if (parrallel_start.Count == 1)
				{
					next = parrallel_start.Pop();
				}
				else // (parrallel_start.Count == 0)
					return false;

			} while ((int)next.x != 0);

			// Determine current flow direcion of lines
			flipLineFromComponent();
			while (flipLineFromLine()) ;

			return true;
		}

		private bool combineBattery()
		{
			int num = 0;
			for (var i = 0; i < boundary; i++)
				if (circuitItemList[i].type == ItemType.Battery)
					num++;

			switch (num)
			{
			case 0:
				return false;
			case 1:
				return true;
			case 2:
				Connectivity c = Connectivity.zero;
				int lineID = 0;
				if (!batteryAdjacency(ref c, ref lineID)) return false;

				deleteBattery1(c, lineID);
				circuitBranch[0].Add(1);

				return true;
			default:
				return false;
			}
		}

		private void flipComponent(int ID)
		{
			// Modify IDth col of R_Matrix
			for (var i = 0; i < boundary; i++)
			{
				if (R_Matrix[i, ID] == Connectivity.l)
					R_Matrix[i, ID] = Connectivity.r;
				else if (R_Matrix[i, ID] == Connectivity.r)
					R_Matrix[i, ID] = Connectivity.l;
			}
			// Modify IDth col of L_Matrix
			for (var i = 0; i < boundary; i++)
			{
				if (L_Matrix[i, ID] == Connectivity.l)
					L_Matrix[i, ID] = Connectivity.r;
				else if (L_Matrix[i, ID] == Connectivity.r)
					L_Matrix[i, ID] = Connectivity.l;
			}
			// Switch IDth row of L/R_Matrix
			Connectivity[] tmp = new Connectivity[boundary];
			for (var j = 0; j < boundary; j++)
			{
				tmp[j] = R_Matrix[ID, j];
				R_Matrix[ID, j] = L_Matrix[ID, j];
				L_Matrix[ID, j] = tmp[j];
			}
			// Modify IDth row of Connectivity
			for (var j = 0; j < count; j++)
			{
				if (connectivity[ID, j] == Connectivity.l)
					connectivity[ID, j] = Connectivity.r;
				else if (connectivity[ID, j] == Connectivity.r)
					connectivity[ID, j] = Connectivity.l;
			}
		}

		private Stack<Vector2> findR(Vector2 ID)
		{
			Stack<Vector2> res = new Stack<Vector2>();
			for (var j = 0; j < boundary; j++)
			{
				if (R_Matrix[(int)ID.x, j] != Connectivity.zero)
					res.Push(new Vector2(j, (int)ID.x));
			}
			return res;
		}

		private bool deParrallel(ref Stack<Vector2> parrallel_start, ref Vector2 next)
		{
			List<int> parrallel_end = new List<int>();

			while (parrallel_start.Count > 0)
			{
				// In a new branch
				circuitBranch.Add(new List<int>());
				// Add compnents to new branch in deSeries
				int end = deSeries(parrallel_start.Pop());

				parrallel_end.Add(end);
			}

			// Construct result matrix
			int[,] result = new int[parrallel_end.Count + 1, boundary];

			for (var i = 0; i < parrallel_end.Count; i++)
				for (var j = 0; j < boundary; j++)
				{
					if (R_Matrix[parrallel_end[i], j] != Connectivity.zero)
						result[i, j] = 1;
					else
						result[i, j] = 0;
				}

			for (var j = 0; j < boundary; j++)
			{
				result[parrallel_end.Count, j] = 0;

				for (var i = 0; i < parrallel_end.Count; i++)
					result[parrallel_end.Count, j] += result[i, j];
			}

			// Determine whether this is a correct parrallel circuit
			int[] statistics = new int[3] { 0, 0, 0 };

			for (var j = 0; j < boundary; j++)
			{
				if (result[parrallel_end.Count, j] == parrallel_end.Count)
				{
					next = new Vector2(j, parrallel_end[0]);
					statistics[0]++;
				}
				if (result[parrallel_end.Count, j] == parrallel_end.Count - 1)
					statistics[1]++;
				if (result[parrallel_end.Count, j] == 0)
					statistics[2]++;
			}

			if (statistics[0] == 1 &&
				statistics[1] == parrallel_end.Count &&
				statistics[2] == (boundary - parrallel_end.Count - 1))
				return true;
			else
				return false;
		}

		// Branch circuit
		private int deSeries(Vector2 next)
		{
			Stack<Vector2> stack = new Stack<Vector2>();

			while (true)
			{
				circuitItemList[(int)next.x].powered = true;
				circuitBranch[circuitBranch.Count - 1].Add((int)next.x);

				if (R_Matrix[(int)next.y, (int)next.x] == Connectivity.r)
					flipComponent((int)next.x);

				stack = findR(next);

				if (stack.Count > 1)
					break;
				else if (stack.Count == 1)
					next = stack.Pop();
			}
			return (int)next.x;
		}

		private void flipLineFromComponent()
		{
			for (var i = 0; i < boundary; i++)
				for (var j = boundary; j < count; j++)
				{
					circuitItemList[j].powered = true;

					if (connectivity[i, j] == Connectivity.r)
					{
						if (connectivity[j, i] == Connectivity.e)
							flipLine(j);
					}
					else if (connectivity[i, j] == Connectivity.l)
					{
						if (connectivity[j, i] == Connectivity.s)
							flipLine(j);
					}
				}
		}

		private bool flipLineFromLine()
		{
			bool modified = false;

			for (var i = boundary; i < count; i++)
				if (!circuitItemList[i].powered)
				{
					circuitItemList[i].powered = true;
					modified = true;

					int flip = 0; // For voting
					int dont_flip = 0;

					for (var j = boundary; j < count; j++)
					{
						if (!circuitItemList[j].powered) continue;

						if (connectivity[j, i] == Connectivity.e)
						{
							if (connectivity[i, j] == Connectivity.e)
								flip++;
							else
								dont_flip++;
						}
						else if (connectivity[j, i] == Connectivity.s)
						{
							if (connectivity[i, j] == Connectivity.s)
								flip++;
							else
								dont_flip++;
						}
					}
					if (flip > dont_flip)
						flipLine(i);
				}
			return modified;
		}

		private void flipLine(int ID)
		{
			circuitItemList[ID].list.Reverse();

			for (var j = 0; j < count; j++)
			{
				if (connectivity[ID, j] == Connectivity.s)
					connectivity[ID, j] = Connectivity.e;
				else if (connectivity[ID, j] == Connectivity.e)
					connectivity[ID, j] = Connectivity.s;
			}
		}

		private bool haveNoSwitches()
		{			
			bool haveSwitch = false;
			for (var i = 0; i < boundary; i++)
				if (circuitItemList[i].type == ItemType.Switch ||
					circuitItemList[i].type == ItemType.LightActSwitch ||
					circuitItemList[i].type == ItemType.VoiceOperSwitch ||
					circuitItemList[i].type == ItemType.VoiceTimedelaySwitch)
				{
					haveSwitch = true;
					break;
				}                    
			return !haveSwitch;
		}

		private void switchOff(int ID)
		{
			for (var i = 0; i < count; i++)
			{
				connectivity[ID, i] = Connectivity.zero;
				connectivity[i, ID] = Connectivity.zero;
			}
		}

		private void switchOn(int ID)
		{
			for (var i = 0; i < count; i++)
			{
				connectivity[ID, i] = originalConn[ID, i];
				connectivity[i, ID] = originalConn[i, ID];
			}
		}
    }
}