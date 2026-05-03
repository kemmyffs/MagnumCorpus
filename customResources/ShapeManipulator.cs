using Godot;

public static class ShapeManipulator
{
    public const int ExpandingPixels = 6;
    public static Shape2D CopyExpandedShape(Shape2D copiedShape)
    {
        if (copiedShape.GetType() == typeof(RectangleShape2D))
        {
            RectangleShape2D s = (RectangleShape2D)copiedShape.Duplicate();
            s.Size = new Vector2(s.Size.X + ExpandingPixels, s.Size.Y + ExpandingPixels);
            GD.Print(s);
            return s;
        }
        else if (copiedShape.GetType() == typeof(CapsuleShape2D))
        {
            CapsuleShape2D s = (CapsuleShape2D)copiedShape.Duplicate();
            s.Radius += ExpandingPixels / 2;
            s.Height += ExpandingPixels / 2;
            return s;
        }

        return null;
    }
}