using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.Math
{
    public struct Rectangle
    {
        public Vector2Int bottomLeft;
        public Vector2Int topRight;

        public int DeltaX => topRight.x - bottomLeft.x;
        public int DeltaY => bottomLeft.y - topRight.y;

        public Rectangle(Vector2Int bottomLeft, Vector2Int topRight)
        {
            this.bottomLeft = bottomLeft;
            this.topRight = topRight;
        }

        public Rectangle(int blX, int blY, int trX, int trY)
        {
            this.bottomLeft = new Vector2Int(blX, blY);
            this.topRight = new Vector2Int(trX, trY);
        }

        public void MoveX(int dx)
        {
            bottomLeft.x += dx;
            topRight.x += dx;
        }

        public void MoveY(int dy)
        {
            bottomLeft.y += dy;
            topRight.y += dy;
        }

        public override string ToString()
        {
            return "[" + bottomLeft + ", " + topRight + "]";
        }
    }
}
