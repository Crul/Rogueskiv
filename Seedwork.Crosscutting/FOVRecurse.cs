// https://github.com/AndyStobirski/RogueLike/blob/master/FOVRecurse.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Seedwork.Crosscutting
{

    /// <summary>
    /// Implementation of "FOV using recursive shadowcasting - improved" as
    /// described on http://roguebasin.roguelikedevelopment.org/index.php?title=FOV_using_recursive_shadowcasting_-_improved
    /// 
    /// The FOV code is contained in the region "FOV Algorithm".
    /// The method GetVisibleCells() is called to calculate the cells
    /// visible to the player by examing each octant sequantially. 
    /// The generic list VisiblePoints contains the cells visible to the player.
    /// 
    /// GetVisibleCells() is called everytime the player moves, and the event playerMoved
    /// is called when a successful move is made (the player moves into an empty cell)
    /// 
    /// </summary>
    public class FOVRecurse
    {
        public Size MapSize { get; set; }
        public int[,] Map { get; private set; }

        /// <summary>
        /// Radius of the player's circle of vision
        /// </summary>
        public int VisualRange { get; set; }

        /// <summary>
        /// List of points visible to the player
        /// </summary>
        public List<Point> VisiblePoints { get; private set; }  // Cells the player can see

        private Point player;
        public Point Player { get { return player; } set { player = value; } }

        /// <summary>
        /// The octants which a player can see
        /// </summary>
        List<int> VisibleOctants = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };

        public FOVRecurse(int width, int height, int visualRange)
        {
            MapSize = new Size(width, height);
            Map = new int[MapSize.Width, MapSize.Height];
            VisualRange = visualRange;
        }


        /// <summary>
        /// Move the player in the specified direction provided the cell is valid and empty
        /// </summary>
        /// <param name="pX">X offset</param>
        /// <param name="pY">Y Offset</param>
        public void SetPlayerPos(int x, int y)
        {
            if (Point_Valid(x, y) && Point_Get(x, y) == 0)
            {
                player.X = x;
                player.Y = y;
                GetVisibleCells();
            }
        }

        #region map point code

        /// <summary>
        /// Check if the provided coordinate is within the bounds of the mapp array
        /// </summary>
        /// <param name="pX"></param>
        /// <param name="pY"></param>
        /// <returns></returns>
        private bool Point_Valid(int pX, int pY)
        {
            return pX >= 0 & pX < Map.GetLength(0)
                    & pY >= 0 & pY < Map.GetLength(1);
        }

        /// <summary>
        /// Get the value of the point at the specified location
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <returns>Cell value</returns>
        public int Point_Get(int _x, int _y)
        {
            return Map[_x, _y];
        }

        /// <summary>
        /// Set the map point to the specified value
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_val"></param>
        public void Point_Set(int _x, int _y, int _val)
        {
            if (Point_Valid(_x, _y))
                Map[_x, _y] = _val;
        }

        #endregion

        #region FOV algorithm

        //  Octant data
        //
        //    \ 1 | 2 /
        //   8 \  |  / 3
        //   -----+-----
        //   7 /  |  \ 4
        //    / 6 | 5 \
        //
        //  1 = NNW, 2 =NNE, 3=ENE, 4=ESE, 5=SSE, 6=SSW, 7=WSW, 8 = WNW

        /// <summary>
        /// Start here: go through all the octants which surround the player to
        /// determine which open cells are visible
        /// </summary>
        public void GetVisibleCells()
        {
            VisiblePoints = new List<Point> { new Point(player.X, player.Y) };
            foreach (int o in VisibleOctants)
                ScanOctant(1, o, 1.0, 0.0);

            VisiblePoints = VisiblePoints.Distinct().ToList();

        }

        /// <summary>
        /// Examine the provided octant and calculate the visible cells within it.
        /// </summary>
        /// <param name="pDepth">Depth of the scan</param>
        /// <param name="pOctant">Octant being examined</param>
        /// <param name="pStartSlope">Start slope of the octant</param>
        /// <param name="pEndSlope">End slope of the octance</param>
        protected void ScanOctant(int pDepth, int pOctant, double pStartSlope, double pEndSlope)
        {

            int visrange2 = VisualRange * VisualRange;
            int x = 0;
            int y = 0;

            switch (pOctant)
            {

                case 1: //nnw
                    y = player.Y - pDepth;
                    if (y < 0) return;

                    x = player.X - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x < 0) x = 0;

                    while (GetSlope(x, y, player.X, player.Y, false) >= pEndSlope)
                    {
                        if (GetVisDistance(x, y, player.X, player.Y) <= visrange2)
                        {
                            if (Map[x, y] == 1) //current cell blocked
                            {
                                if (x - 1 >= 0 && Map[x - 1, y] == 0) //prior cell within range AND open...
                                                                      //...incremenet the depth, adjust the endslope and recurse
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y + 0.5, player.X, player.Y, false));
                            }
                            else
                            {

                                if (x - 1 >= 0 && Map[x - 1, y] == 1) //prior cell within range AND open...
                                                                      //..adjust the startslope
                                    pStartSlope = GetSlope(x - 0.5, y - 0.5, player.X, player.Y, false);

                                VisiblePoints.Add(new Point(x, y));
                            }
                        }
                        x++;
                    }
                    x--;
                    break;

                case 2: //nne

                    y = player.Y - pDepth;
                    if (y < 0) return;

                    x = player.X + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x >= Map.GetLength(0)) x = Map.GetLength(0) - 1;

                    while (GetSlope(x, y, player.X, player.Y, false) <= pEndSlope)
                    {
                        if (GetVisDistance(x, y, player.X, player.Y) <= visrange2)
                        {
                            if (Map[x, y] == 1)
                            {
                                if (x + 1 < Map.GetLength(0) && Map[x + 1, y] == 0)
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y + 0.5, player.X, player.Y, false));
                            }
                            else
                            {
                                if (x + 1 < Map.GetLength(0) && Map[x + 1, y] == 1)
                                    pStartSlope = -GetSlope(x + 0.5, y - 0.5, player.X, player.Y, false);

                                VisiblePoints.Add(new Point(x, y));
                            }
                        }
                        x--;
                    }
                    x++;
                    break;

                case 3:

                    x = player.X + pDepth;
                    if (x >= Map.GetLength(0)) return;

                    y = player.Y - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y < 0) y = 0;

                    while (GetSlope(x, y, player.X, player.Y, true) <= pEndSlope)
                    {

                        if (GetVisDistance(x, y, player.X, player.Y) <= visrange2)
                        {

                            if (Map[x, y] == 1)
                            {
                                if (y - 1 >= 0 && Map[x, y - 1] == 0)
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y - 0.5, player.X, player.Y, true));
                            }
                            else
                            {
                                if (y - 1 >= 0 && Map[x, y - 1] == 1)
                                    pStartSlope = -GetSlope(x + 0.5, y - 0.5, player.X, player.Y, true);

                                VisiblePoints.Add(new Point(x, y));
                            }
                        }
                        y++;
                    }
                    y--;
                    break;

                case 4:

                    x = player.X + pDepth;
                    if (x >= Map.GetLength(0)) return;

                    y = player.Y + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y >= Map.GetLength(1)) y = Map.GetLength(1) - 1;

                    while (GetSlope(x, y, player.X, player.Y, true) >= pEndSlope)
                    {

                        if (GetVisDistance(x, y, player.X, player.Y) <= visrange2)
                        {

                            if (Map[x, y] == 1)
                            {
                                if (y + 1 < Map.GetLength(1) && Map[x, y + 1] == 0)
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y + 0.5, player.X, player.Y, true));
                            }
                            else
                            {
                                if (y + 1 < Map.GetLength(1) && Map[x, y + 1] == 1)
                                    pStartSlope = GetSlope(x + 0.5, y + 0.5, player.X, player.Y, true);

                                VisiblePoints.Add(new Point(x, y));
                            }
                        }
                        y--;
                    }
                    y++;
                    break;

                case 5:

                    y = player.Y + pDepth;
                    if (y >= Map.GetLength(1)) return;

                    x = player.X + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x >= Map.GetLength(0)) x = Map.GetLength(0) - 1;

                    while (GetSlope(x, y, player.X, player.Y, false) >= pEndSlope)
                    {
                        if (GetVisDistance(x, y, player.X, player.Y) <= visrange2)
                        {

                            if (Map[x, y] == 1)
                            {
                                if (x + 1 < Map.GetLength(0) && Map[x + 1, y] == 0)
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y - 0.5, player.X, player.Y, false));
                            }
                            else
                            {
                                if (x + 1 < Map.GetLength(0)
                                        && Map[x + 1, y] == 1)
                                    pStartSlope = GetSlope(x + 0.5, y + 0.5, player.X, player.Y, false);

                                VisiblePoints.Add(new Point(x, y));
                            }
                        }
                        x--;
                    }
                    x++;
                    break;

                case 6:

                    y = player.Y + pDepth;
                    if (y >= Map.GetLength(1)) return;

                    x = player.X - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (x < 0) x = 0;

                    while (GetSlope(x, y, player.X, player.Y, false) <= pEndSlope)
                    {
                        if (GetVisDistance(x, y, player.X, player.Y) <= visrange2)
                        {

                            if (Map[x, y] == 1)
                            {
                                if (x - 1 >= 0 && Map[x - 1, y] == 0)
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y - 0.5, player.X, player.Y, false));
                            }
                            else
                            {
                                if (x - 1 >= 0
                                        && Map[x - 1, y] == 1)
                                    pStartSlope = -GetSlope(x - 0.5, y + 0.5, player.X, player.Y, false);

                                VisiblePoints.Add(new Point(x, y));
                            }
                        }
                        x++;
                    }
                    x--;
                    break;

                case 7:

                    x = player.X - pDepth;
                    if (x < 0) return;

                    y = player.Y + Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y >= Map.GetLength(1)) y = Map.GetLength(1) - 1;

                    while (GetSlope(x, y, player.X, player.Y, true) <= pEndSlope)
                    {

                        if (GetVisDistance(x, y, player.X, player.Y) <= visrange2)
                        {

                            if (Map[x, y] == 1)
                            {
                                if (y + 1 < Map.GetLength(1) && Map[x, y + 1] == 0)
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y + 0.5, player.X, player.Y, true));
                            }
                            else
                            {
                                if (y + 1 < Map.GetLength(1) && Map[x, y + 1] == 1)
                                    pStartSlope = -GetSlope(x - 0.5, y + 0.5, player.X, player.Y, true);

                                VisiblePoints.Add(new Point(x, y));
                            }
                        }
                        y--;
                    }
                    y++;
                    break;

                case 8: //wnw

                    x = player.X - pDepth;
                    if (x < 0) return;

                    y = player.Y - Convert.ToInt32((pStartSlope * Convert.ToDouble(pDepth)));
                    if (y < 0) y = 0;

                    while (GetSlope(x, y, player.X, player.Y, true) >= pEndSlope)
                    {

                        if (GetVisDistance(x, y, player.X, player.Y) <= visrange2)
                        {

                            if (Map[x, y] == 1)
                            {
                                if (y - 1 >= 0 && Map[x, y - 1] == 0)
                                    ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y - 0.5, player.X, player.Y, true));

                            }
                            else
                            {
                                if (y - 1 >= 0 && Map[x, y - 1] == 1)
                                    pStartSlope = GetSlope(x - 0.5, y - 0.5, player.X, player.Y, true);

                                VisiblePoints.Add(new Point(x, y));
                            }
                        }
                        y++;
                    }
                    y--;
                    break;
            }


            if (x < 0)
                x = 0;
            else if (x >= Map.GetLength(0))
                x = Map.GetLength(0) - 1;

            if (y < 0)
                y = 0;
            else if (y >= Map.GetLength(1))
                y = Map.GetLength(1) - 1;

            if (pDepth < VisualRange & Map[x, y] == 0)
                ScanOctant(pDepth + 1, pOctant, pStartSlope, pEndSlope);

        }

        /// <summary>
        /// Get the gradient of the slope formed by the two points
        /// </summary>
        /// <param name="pX1"></param>
        /// <param name="pY1"></param>
        /// <param name="pX2"></param>
        /// <param name="pY2"></param>
        /// <param name="pInvert">Invert slope</param>
        /// <returns></returns>
        private double GetSlope(double pX1, double pY1, double pX2, double pY2, bool pInvert)
        {
            if (pInvert)
                return (pY1 - pY2) / (pX1 - pX2);
            else
                return (pX1 - pX2) / (pY1 - pY2);
        }


        /// <summary>
        /// Calculate the distance between the two points
        /// </summary>
        /// <param name="pX1"></param>
        /// <param name="pY1"></param>
        /// <param name="pX2"></param>
        /// <param name="pY2"></param>
        /// <returns>Distance</returns>
        private int GetVisDistance(int pX1, int pY1, int pX2, int pY2)
        {
            return ((pX1 - pX2) * (pX1 - pX2)) + ((pY1 - pY2) * (pY1 - pY2));
        }

        #endregion

    }
}
