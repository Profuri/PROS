using System;
public static class Ease
{
    #region sine
    public static float EaseInSine(float x) => 1 - MathF.Cos((x * MathF.PI) / 2);
    public static float EaseOutSine(float x) => MathF.Sin((x * MathF.PI) / 2);
    public static float EaseInOutSine(float x) => -(MathF.Cos(MathF.PI * x) - 1) / 2;
    #endregion
    #region cubic
    public static float EaseInCubic(float x) => x * x * x;
    public static float EaseOutCubic(float x) => 1 - MathF.Pow(1 - x, 3);
    public static float EaseInOutCubic(float x) => x < 0.5 ? 4 * x * x * x : 1 - MathF.Pow(-2 * x + 2, 3) / 2;
    #endregion
    #region Quint
    public static float EaseInQunit(float x) => x * x * x * x;
    public static float EaseOutQuint(float x) => 1 - MathF.Pow(1 - x, 5);
    public static float EaseInOutQuint(float x) => x < 0.5 ? 16 * x * x * x * x * x : 1 - MathF.Pow(-2 * x + 2, 5) / 2;
    #endregion
    #region Circ
    
    public static float EaseInCirc(float x) => 1 - MathF.Sqrt(1 - MathF.Pow(x, 2));
    public static float EaseOutCirc(float x) => MathF.Sqrt(1 - MathF.Pow(x - 1, 2));
    public static float EaseInOutCirc(float x)
    {
        return x < 0.5
            ? (1 - MathF.Sqrt(1 - MathF.Pow(2 * x, 2))) / 2
            : (MathF.Sqrt(1 - MathF.Pow(-2 * x + 2, 2)) + 1) / 2;
    }
    #endregion
    #region Elastic
    public static float EaseInElastic(float x)
    {
        float c4 = (2 * MathF.PI) / 3;

        return x == 0
            ? 0
            : x == 1
                ? 1
                : -MathF.Pow(2, 10 * x - 10) * MathF.Sin((x * 10 - 10.75f) * c4);
    }
    public static float EaseOutElastic(float x)
    {
        float c4 = (2 * MathF.PI) / 3;

        return x == 0
            ? 0
            : x == 1
                ? 1
                : MathF.Pow(2, -10 * x) * MathF.Sin((x * 10 - 0.75f) * c4) + 1;
    }
    public static float EaseInOutElastic(float x)
    {
        float c5 = (2 * MathF.PI) / 4.5f;

        return x == 0
            ? 0
            : x == 1
                ? 1
                : x < 0.5
                    ? -(MathF.Pow(2, 20 * x - 10) * MathF.Sin((20 * x - 11.125f) * c5)) / 2f
                    : (MathF.Pow(2, -20 * x + 10) * MathF.Sin((20 * x - 11.125f) * c5)) / 2f + 1;
    }
    #endregion
    #region Quad
    public static float EaseInQuad(float x) => x * x;
    public static float EaseOutQuad(float x) => 1 - (1 - x) * (1 - x);
    public static float EaseInOutQuad(float x) => x < 0.5 ? 2 * x * x : 1 - MathF.Pow(-2 * x + 2, 2) / 2;
    #endregion
    #region Quart
    public static float EaseInQuart(float x) => x * x * x * x;
    public static float EaseOutQuart(float x) => 1 - MathF.Pow(1 - x, 4);
    public static float EaseInOutQuart(float x) => x < 0.5 ? 8 * x * x * x * x : 1 - MathF.Pow(-2 * x + 2, 4) / 2;
    #endregion
    #region Expo
    public static float EaseInExpo(float x) => x == 0 ? 0 : MathF.Pow(2, 10 * x - 10);
    public static float EaseOutExpo(float x) => x == 1 ? 1 : 1 - MathF.Pow(2, -10 * x);
    public static float EaseInOutExpo(float x)
    {
        return x == 0
            ? 0
            : x == 1
                ? 1
                : x < 0.5 ? MathF.Pow(2, 20 * x - 10) / 2
                    : (2 - MathF.Pow(2, -20 * x + 10)) / 2;
    }
    #endregion
    #region Back
    public static float EaseInBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return c3 * x * x * x - c1 * x * x;
    }
    public static float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return 1 + c3 * MathF.Pow(x - 1, 3) + c1 * MathF.Pow(x - 1, 2);
    }
    public static float EaseInOutBack(float x)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        return x < 0.5
            ? (MathF.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
            : (MathF.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    }
    #endregion
    #region Bounce
    public static float EaseInBounce(float x) => 1 - EaseOutBounce(1 - x);
    public static float EaseOutBounce(float x)
    {
        float n1 = 7.5625f;
        float d1 = 2.75f;

        if (x < 1 / d1) {
            return n1 * x * x;
        } else if (x < 2 / d1) {
            return n1 * (x -= 1.5f / d1) * x + 0.75f;
        } else if (x < 2.5 / d1) {
            return n1 * (x -= 2.25f / d1) * x + 0.9375f;
        } else {
            return n1 * (x -= 2.625f / d1) * x + 0.984375f;
        }
    }
    public static float EaseInOutBounce(float x)
    {
        return x < 0.5
            ? (1 - EaseOutBounce(1 - 2 * x)) / 2
            : (1 + EaseOutBounce(2 * x - 1)) / 2;
    }

    #endregion
}
