using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace MagicCircuit
{
    public enum Connectivity
    {
        // Card: l = left,  r = right, m = middle
        // Line: s = start, e = end
        zero,
        l, r, m, s, e
    }

    public class CurrentFlow_SPDTSwitch 
    {
        private List<CircuitItem> circuitItems;

        private Correctness_SPDTSwitch correctness;

        private Connectivity[,] connectivity;
        private Connectivity[,] originalConn;
        private Connectivity[,] modifiedConn;   // For remove switches

        private int count;
        private int boundary;                   // ID of the first CircuitLine
        private bool[] isOpened;                // Store information of which item is open circuited

        private const int region = 30;

   

        public bool compute(List<CircuitItem> itemList)
        {

			circuitItems = itemList;
            correctness = new Correctness_SPDTSwitch();

            if (!computeCircuitBranch())
            {                
                //Debug.Log("Wrong Circuit!");
                return false;
            }

            switchOnOff(3, false);
			switchOnOff(3, true);
            //switchOnOff(2, false);

            // Display circuitItems
            /*for (var i = 0; i < count; i++)
            {
                Debug.Log(i + ": ----------------");
                Debug.Log(circuitItems[i].powered);
                Debug.Log(circuitItems[i].list[0]);
            }*/
            return true;
        }

        private bool computeCircuitBranch()
        {
//            circuitItems = XmlCircuitItemCollection.Load(Path.Combine(Application.dataPath, "CircuitItems.xml")).toCircuitItems();

            count = circuitItems.Count;

            // Find boundary between cards & lines
            boundary = 0;
            while (boundary < count)
            {
                if (circuitItems[boundary].type == ItemType.CircuitLine)
                    break;
                boundary++;
            }

            connectivity = new Connectivity[count, count];
            originalConn = new Connectivity[count, count];
            modifiedConn = new Connectivity[count, count];

            computeConnectivity();

            simplifyCircuit();

            // Save connectivity to originalConn & modifiedConn
            Array.Copy(connectivity, originalConn, connectivity.Length);
            Array.Copy(connectivity, modifiedConn, connectivity.Length);

            /////////////////////////
            //@ computeCorrectness here
            if (!correctness.computeCorrectness(circuitItems, originalConn))
                return false;
            //correctness.computeCorrectness(circuitItems, originalConn);
            /////////////////////////

            removeSwitches();
		
            if (!computeCurrentFlow())
				for (var i = 0; i < count; i++)
					circuitItems[i].powered = false;

            return true;
        }

		public void switchOnOff(int ID, bool state) // State true: right false: left
        {
            for (var i = 0; i < count; i++)
                circuitItems[i].powered = false;

            if (state)
                switchRight(ID);
            else
                switchLeft(ID);

            if (!computeCurrentFlow())
                for (var i = 0; i < count; i++)
                    circuitItems[i].powered = false;
            else
                for (var i = 0; i < count; i++)
                    if (circuitItems[i].type == ItemType.SPDTSwitch)
                        circuitItems[i].powered = true;
        }

        private bool computeCurrentFlow()
        {
            // Traverse all the components to get current flow direction
            Vector2 next = new Vector2(0, 0);  // next : (next, current) ->

            // Start from battery to find Connectivity.r
            circuitItems[0].powered = true;
            for (var j = boundary; j < count; j++)
                if (modifiedConn[0, j] == Connectivity.r)
                    next.x = j;

            // Update powered, flip lines
            do
            {
                circuitItems[(int)next.x].powered = true;

                if (modifiedConn[(int)next.x, (int)next.y] == Connectivity.e)
                    flipLine((int)next.x);

                if (!findNext(ref next))
                    return false;

            } while ((int)next.x != 0);

            return true;
        }

        private void computeConnectivity()
        {
            // For each card, check which line is attached
            // Improve performance?
            for (var i = 0; i < boundary; i++)
                for (var j = boundary; j < count; j++)
                {
                    if (inRegion(circuitItems[i].connect_left, circuitItems[j].connect_left))
                    {
                        connectivity[i, j] = Connectivity.l;
                        connectivity[j, i] = Connectivity.s;
                        circuitItems[j].list.Insert(0, circuitItems[i].list[0]);
                        continue;
                    }
                    if (inRegion(circuitItems[i].connect_right, circuitItems[j].connect_left))
                    {
                        connectivity[i, j] = Connectivity.r;
                        connectivity[j, i] = Connectivity.s;
                        circuitItems[j].list.Insert(0, circuitItems[i].list[0]);
                        continue;
                    }
                    if (inRegion(circuitItems[i].connect_middle, circuitItems[j].connect_left))
                    {
                        connectivity[i, j] = Connectivity.m;
                        connectivity[j, i] = Connectivity.s;
                        circuitItems[j].list.Insert(0, circuitItems[i].list[0]);
                        continue;
                    }
                    if (inRegion(circuitItems[i].connect_left, circuitItems[j].connect_right))
                    {
                        connectivity[i, j] = Connectivity.l;
                        connectivity[j, i] = Connectivity.e;
                        circuitItems[j].list.Add(circuitItems[i].list[0]);
                        continue;
                    }
                    if (inRegion(circuitItems[i].connect_right, circuitItems[j].connect_right))
                    {
                        connectivity[i, j] = Connectivity.r;
                        connectivity[j, i] = Connectivity.e;
                        circuitItems[j].list.Add(circuitItems[i].list[0]);
                        continue;
                    }
                    if (inRegion(circuitItems[i].connect_middle, circuitItems[j].connect_right))
                    {
                        connectivity[i, j] = Connectivity.m;
                        connectivity[j, i] = Connectivity.e;
                        circuitItems[j].list.Add(circuitItems[i].list[0]);
                        continue;
                    }
                };

            // For each line, check which line it is connected to
            for (var i = boundary; i < count; i++)
            {
                for (var j = boundary; j < count; j++)
                {
                    if (i == j) continue;
                    if (isConnected(circuitItems[i].connect_left, circuitItems[j].connect_left))
                    { connectivity[i, j] = Connectivity.s; connectivity[j, i] = Connectivity.s; }
                    if (isConnected(circuitItems[i].connect_left, circuitItems[j].connect_right))
                    { connectivity[i, j] = Connectivity.s; connectivity[j, i] = Connectivity.e; }
                }
                for (var j = boundary; j < count; j++)
                {
                    if (i == j) continue;
                    if (isConnected(circuitItems[i].connect_right, circuitItems[j].connect_left))
                    { connectivity[i, j] = Connectivity.e; connectivity[j, i] = Connectivity.s; }
                    if (isConnected(circuitItems[i].connect_right, circuitItems[j].connect_right))
                    { connectivity[i, j] = Connectivity.e; connectivity[j, i] = Connectivity.e; }
                }
            }
        }

        // Construct modifiedConn
        private void removeSwitches()
        {
            for (var i = 0; i < boundary; i++)
                if (circuitItems[i].type == ItemType.SPDTSwitch)
                {
                    circuitItems[i].powered = true;
                    int m = 0;
                    int r = 0;
                    for (var j = 0; j < count; j++)
                    {
                        modifiedConn[i, j] = Connectivity.zero;
                        modifiedConn[j, i] = Connectivity.zero;

                        if (originalConn[i, j] == Connectivity.m) m = j;
                        if (originalConn[i, j] == Connectivity.r) r = j;
                    }
                    modifiedConn[m, r] = originalConn[m, i];
                    modifiedConn[r, m] = originalConn[r, i];
                }
        }

        // Check if linePoint is in a box region of cardPoint
        private bool inRegion(Vector2 cardPoint, Vector2 linePoint)
        {
            if ((linePoint.x > (cardPoint.x - region)) && (linePoint.x < (cardPoint.x + region))
                && (linePoint.y > (cardPoint.y - region)) && (linePoint.y < (cardPoint.y + region)))
                return true;
            else
                return false;
        }

        private bool isConnected(Vector2 point_1, Vector2 point_2)
        {
            if (point_1.x == point_2.x && point_1.y == point_2.y)
                return true;
            else
                return false;
        }

        private bool simplifyCircuit()
        {
            bool flag;
            bool[] delete = new bool[count];
            bool haveOpenCircuit = false;

            // Initialize isOpened
            isOpened = new bool[count];
            for (var i = 0; i < count; i++)
                isOpened[i] = false;

            do
            {
                flag = false;
                // Initialize delete[] to all false            
                for (var i = 0; i < count; i++)
                    delete[i] = false;

                for (var i = 0; i < count; i++)
                    if (isNotValid(i))
                    {
                        delete[i] = true;
                        isOpened[i] = true;
                        flag = true;
                        haveOpenCircuit = true;
                    }

                for (var i = 0; i < count; i++)
                    if (delete[i])
                        for (var j = 0; j < count; j++)
                        {
                            connectivity[i, j] = Connectivity.zero;
                            connectivity[j, i] = Connectivity.zero;
                        }
            } while (flag);

            if (haveOpenCircuit) return false;
            else return true;
        }

        private bool isNotValid(int i)
        {
            bool haveStart = false;
            bool haveEnd = false;
            bool haveMiddel = false;

            // Check if SPDTSwitch's middle point is connected
            if (circuitItems[i].type == ItemType.SPDTSwitch)
            {
                for (var j = boundary; j < count; j++)
                    if (connectivity[i, j] == Connectivity.m)
                        haveMiddel = true;
                if (!haveMiddel)
                    return false;
            }

            for (var j = 0; j < count; j++)
            {
                if (connectivity[i, j] == Connectivity.l || connectivity[i, j] == Connectivity.s)
                    haveStart = true;
                if (connectivity[i, j] == Connectivity.r || connectivity[i, j] == Connectivity.e)
                    haveEnd = true;
            }

            // We consider 0 0 and 1 1 as valid
            //             1 0 and 0 1 as invalid
            // So return true when invalid
            if (haveStart ^ haveEnd) // XOR operator
                return true;
            else
                return false;
        }

        private void flipLine(int ID)
        {
            circuitItems[ID].list.Reverse();



			Debug.Log("CurrentFlow.cs flipLine : circuitItem["+ID+"] flipped : list[0] = " + circuitItems[ID].list[0] + " list[Count] = " + circuitItems[ID].list[circuitItems[ID].list.Count-1]);



            for (var j = 0; j < count; j++)
            {
                if (modifiedConn[ID, j] == Connectivity.s)
                    modifiedConn[ID, j] = Connectivity.e;
                else
                if (modifiedConn[ID, j] == Connectivity.e)
                    modifiedConn[ID, j] = Connectivity.s;

				if (originalConn[ID, j] == Connectivity.s)
					originalConn[ID, j] = Connectivity.e;
				else
				if (originalConn[ID, j] == Connectivity.e)
					originalConn[ID, j] = Connectivity.s;
            }
        }

        private void switchLeft(int ID)
        {
            int l = 0;
            int r = 0;
            int m = 0;
            for (var j = boundary; j < count; j++)
            {
                if (originalConn[ID, j] == Connectivity.l) l = j;
                if (originalConn[ID, j] == Connectivity.r) r = j;
                if (originalConn[ID, j] == Connectivity.m) m = j;
            }
            modifiedConn[m, r] = Connectivity.zero;
            modifiedConn[r, m] = Connectivity.zero;

            modifiedConn[m, l] = originalConn[m, ID];
            modifiedConn[l, m] = originalConn[l, ID];
        }

        private void switchRight(int ID)
        {
            int l = 0;
            int r = 0;
            int m = 0;
            for (var j = boundary; j < count; j++)
            {
                if (originalConn[ID, j] == Connectivity.l) l = j;
                if (originalConn[ID, j] == Connectivity.r) r = j;
                if (originalConn[ID, j] == Connectivity.m) m = j;
            }
            modifiedConn[m, l] = Connectivity.zero;
            modifiedConn[l, m] = Connectivity.zero;

            modifiedConn[m, r] = originalConn[m, ID];
            modifiedConn[r, m] = originalConn[r, ID];
        }

        private bool findNext(ref Vector2 next)
        {
            for (var j = 0; j < count; j++)
            {
                if (j == (int)next.y) continue;
                if (modifiedConn[(int)next.x, j] != Connectivity.zero)
                {
                    next.y = next.x;
                    next.x = j;
                    return true;
                }
            }
            return false;
        }
    }
}