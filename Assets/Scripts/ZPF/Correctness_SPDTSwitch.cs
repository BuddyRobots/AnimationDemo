using System.Collections.Generic;
using UnityEngine;

namespace MagicCircuit
{
    public class Correctness_SPDTSwitch
    {
        List<CircuitItem> circuitItems;
        Connectivity[,] originalConn;

        int count;
        int boundary;

        int ID_battery;
        int ID_bulb;
        int ID_switch_1;
        int ID_switch_2;


        public bool computeCorrectness(List<CircuitItem> _circuitItems, Connectivity[,] _originalConn)
        {
            circuitItems = _circuitItems;
            originalConn = _originalConn;

            count = circuitItems.Count;
            // Find boundary between cards & lines
            boundary = 0;
            while (boundary < count)
            {
                if (circuitItems[boundary].type == ItemType.CircuitLine)
                    break;
                boundary++;
            }
           
            // Group 1
            if (!checkComponets()) return false;

            // Group 2
            if (!(checkNoParrallel() && checkNoCrossing())) return false;

            // Group 3
            if (!(checkBothSideConnected() && checkMiddle() && check2ComponentConnected())) return false;

            return true;
        }

        // Group 1
        private bool checkComponets()
        {
            // Have and only have 4 components
            if (boundary != 4) return false;

            // Have 1 battery & 1 Bulb & 2 SPDTSwitches
            bool haveBattery = false;
            bool haveBulb = false;
            int countSwitch = 0;
            // Set components IDs
            for (var i = 0; i < boundary; i++)
            {
                if (circuitItems[i].type == ItemType.Battery) { haveBattery = true; ID_battery = i; }
                if (circuitItems[i].type == ItemType.Bulb) { haveBulb = true; ID_bulb = i; }
                if (circuitItems[i].type == ItemType.SPDTSwitch)
                {
                    countSwitch++;
                    if (countSwitch == 1) ID_switch_1 = i;
                    if (countSwitch == 2) ID_switch_2 = i;
                }
            }
            if (!(haveBattery && haveBulb && (countSwitch == 2))) return false;
            return true;
        }

        // Group 2
        private bool checkNoParrallel()
        {
            for (var i = 0; i < boundary; i++)
            {
                int countL = 0;
                int countM = 0;
                int countR = 0;

                for (var j = boundary; j < count; j++)
                {
                    if (originalConn[i, j] == Connectivity.l) countL++;
                    if (originalConn[i, j] == Connectivity.m) countM++;
                    if (originalConn[i, j] == Connectivity.r) countR++;
                }
                if ((countL > 1) || (countM > 1) || (countR > 1)) return false;
            }
            return true;
        }

        private bool checkNoCrossing()
        {
            for (var i = boundary; i < count; i++)
                for (var j = boundary; j < count; j++)
                    if (originalConn[i, j] != Connectivity.zero) return false;
            return true;
        }

        // Group 3
        private bool checkBothSideConnected()
        {
            if (((switch_1_to_2(Connectivity.l) == Connectivity.l) && (switch_1_to_2(Connectivity.r) == Connectivity.r)) ||
                ((switch_1_to_2(Connectivity.l) == Connectivity.r) && (switch_1_to_2(Connectivity.r) == Connectivity.l)))
                return true;
            else return false;
        }

        private bool checkMiddle()
        {
            if (((switch_middle_to_component(ID_switch_1) == ID_battery) && (switch_middle_to_component(ID_switch_2) == ID_bulb)) ||
                ((switch_middle_to_component(ID_switch_1) == ID_bulb) && (switch_middle_to_component(ID_switch_2) == ID_battery)))
                return true;
            else return false;
        }

        private bool check2ComponentConnected()
        {
            // Find a line that connect ID_battery to ID_bulb
            for (var i = boundary; i < count; i++)
            {
                if ((originalConn[i, ID_battery] != Connectivity.zero) && (originalConn[i, ID_bulb] != Connectivity.zero))
                    return true;
            }
            return false;
        }

        // Switch_1 input <-> Switch_2 result
        private Connectivity switch_1_to_2(Connectivity c)
        {
            Vector2 next;
            next.x = ID_switch_1;
            next.y = ID_switch_1;
            for (var j = boundary; j < count; j++)
            {
                if (originalConn[ID_switch_1, j] == c)
                {
                    next.y = j;
                    break;
                }
            }
            for (var j = 0; j < boundary; j++)
            {
                if (j == (int)next.x) continue;
                if (originalConn[(int)next.y, j] != Connectivity.zero)
                {
                    next.x = next.y;
                    next.y = j;
                }
            }
            if ((int)next.y != ID_switch_2) return Connectivity.zero;
            else return originalConn[(int)next.y, (int)next.x];
        }

        // Switch ID input <-> Component ID result
        private int switch_middle_to_component(int ID_switch)
        {
            Vector2 next;
            next.x = ID_switch;
            next.y = ID_switch;

            for (var j = boundary; j < count; j++)
            {
                if (originalConn[ID_switch, j] == Connectivity.m)
                {
                    next.y = j;
                    break;
                }
            }

            for (var j = 0; j < boundary; j++)
            {
                if (j == (int)next.x) continue;
                if (originalConn[(int)next.y, j] != Connectivity.zero)
                {
                    next.x = next.y;
                    next.y = j;
                }
            }
            return (int)next.y;
        }
    }
}