using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infringed.Math
{
    public static class CubicInterpolation
    {
        /// <summary>
        /// Generate a smooth (interpolated) curve that follows the path of the given X/Y points
        /// </summary>
        public static Vector3[] InterpolateXZ(List<Vector3> rootPoints, int count)
        {
            if (rootPoints.Count < 2)
                return rootPoints.ToArray();

            int inputPointCount = rootPoints.Count;
            float[] inputDistances = new float[inputPointCount];

            for (int i = 1; i < inputPointCount; i++)
            {
                var from = rootPoints[i - 1];
                var to = rootPoints[i];
                from.y = 0f;
                to.y = 0f;
                
                var distance = Vector3.Distance(from, to);
                inputDistances[i] = inputDistances[i - 1] + distance;
            }

            float meanDistance = inputDistances.Last() / (count - 1);
            float[] evenDistances = Enumerable.Range(0, count).Select(x => x * meanDistance).ToArray();
 
            float[] xsOut = Interpolate(inputDistances, rootPoints.Select((point) => point.x).ToArray(), evenDistances);
            float[] zsOut = Interpolate(inputDistances, rootPoints.Select((point) => point.z).ToArray(), evenDistances);

            return xsOut.Select((x, index) => new Vector3(x, 0f, zsOut[index])).ToArray();
        }

        private static float[] Interpolate(float[] xOrig, float[] yOrig, float[] xInterp)
        {
            (float[] a, float[] b) = FitMatrix(xOrig, yOrig);

            float[] yInterp = new float[xInterp.Length];
            for (int i = 0; i < yInterp.Length; i++)
            {
                int j;
                for (j = 0; j < xOrig.Length - 2; j++)
                    if (xInterp[i] <= xOrig[j + 1])
                        break;

                float dx = xOrig[j + 1] - xOrig[j];
                float t = (xInterp[i] - xOrig[j]) / dx;
                float y = (1 - t) * yOrig[j] + t * yOrig[j + 1] +
                    t * (1 - t) * (a[j] * (1 - t) + b[j] * t);
                yInterp[i] = y;
            }

            return yInterp;
        }

        private static (float[] a, float[] b) FitMatrix(float[] x, float[] y)
        {
            int n = x.Length;
            float[] a = new float[n - 1];
            float[] b = new float[n - 1];
            float[] r = new float[n];
            float[] A = new float[n];
            float[] B = new float[n];
            float[] C = new float[n];

            float dx1, dx2, dy1, dy2;

            dx1 = x[1] - x[0];
            C[0] = 1.0f / dx1;
            B[0] = 2.0f * C[0];
            r[0] = 3 * (y[1] - y[0]) / (dx1 * dx1);

            for (int i = 1; i < n - 1; i++)
            {
                dx1 = x[i] - x[i - 1];
                dx2 = x[i + 1] - x[i];
                A[i] = 1.0f / dx1;
                C[i] = 1.0f / dx2;
                B[i] = 2.0f * (A[i] + C[i]);
                dy1 = y[i] - y[i - 1];
                dy2 = y[i + 1] - y[i];
                r[i] = 3 * (dy1 / (dx1 * dx1) + dy2 / (dx2 * dx2));
            }

            dx1 = x[n - 1] - x[n - 2];
            dy1 = y[n - 1] - y[n - 2];
            A[n - 1] = 1.0f / dx1;
            B[n - 1] = 2.0f * A[n - 1];
            r[n - 1] = 3 * (dy1 / (dx1 * dx1));

            float[] cPrime = new float[n];
            cPrime[0] = C[0] / B[0];
            for (int i = 1; i < n; i++)
                cPrime[i] = C[i] / (B[i] - cPrime[i - 1] * A[i]);

            float[] dPrime = new float[n];
            dPrime[0] = r[0] / B[0];
            for (int i = 1; i < n; i++)
                dPrime[i] = (r[i] - dPrime[i - 1] * A[i]) / (B[i] - cPrime[i - 1] * A[i]);

            float[] k = new float[n];
            k[n - 1] = dPrime[n - 1];
            for (int i = n - 2; i >= 0; i--)
                k[i] = dPrime[i] - cPrime[i] * k[i + 1];

            for (int i = 1; i < n; i++)
            {
                dx1 = x[i] - x[i - 1];
                dy1 = y[i] - y[i - 1];
                a[i - 1] = k[i - 1] * dx1 - dy1;
                b[i - 1] = -k[i] * dx1 + dy1;
            }

            return (a, b);
        }
    }
}