using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Funland.Attractions
{
    internal class DoubleOrNothing : MonoBehaviour
    {
        BoneMapper currentPlayer;
        public bool Join(BoneMapper joiner)
        {
            if (currentPlayer)
            {
                return false;
            }
            currentPlayer = joiner;
            //play sit animation on joiner
            //disable default bindings
            //A: Back out
            //D: Double
            return true;
        }
        public bool Leave(BoneMapper joiner)
        {
            if (currentPlayer == joiner)
            {
                currentPlayer = null;
                return true;
            }
            return false;
        }

        public int CheckDouble(int betAmount)
        {
            return betAmount * 2 * UnityEngine.Random.Range(0, 2);
        }
    }
}
