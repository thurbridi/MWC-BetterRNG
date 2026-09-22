using System.Collections.Generic;
using UnityEngine;

namespace BetterRNG
{
    public class DebugGraphWindow : MonoBehaviour
    {
        public List<float> Xs;
        public List<float> Ys;
        public string Title = "ValueNoise Graph";

        private Rect windowRect = new Rect(1490, 40, 400, 300);
        private Vector2 graphPadding = new Vector2(40, 40);
        private Texture2D lineTex;
        private bool isOpen = true; // Track window open state

        public void Init(List<float> xs, List<float> ys, string title = "ValueNoise Graph")
        {
            Xs = xs;
            Ys = ys;
            Title = title;
            lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            lineTex.SetPixel(0, 0, Color.white); // Use white so GUI.color tints correctly
            lineTex.Apply();
        }

        private void OnGUI()
        {
            if (!isOpen) return; // Only draw if open

            // Update windowRect with the new position after dragging
            windowRect = GUI.Window(GetInstanceID(), windowRect, DrawWindow, Title);

            // Now draw the graph using the updated windowRect
            DrawGraph();
        }

        private void DrawWindow(int id)
        {
            GUI.DragWindow(new Rect(0, 0, windowRect.width, 20));
            // Close button at top right
            float btnSize = 20f;
            Rect closeBtnRect = new Rect(windowRect.width - btnSize - 4, 2, btnSize, btnSize);
            if (GUI.Button(closeBtnRect, "X"))
            {
                isOpen = false;
            }
            // Do not draw the graph here!
        }
        private void DrawGraph()
        {
            if (Xs == null || Ys == null || Xs.Count != Ys.Count || Xs.Count < 2)
                return;

            float minX = Mathf.Min(Xs.ToArray());
            float maxX = Mathf.Max(Xs.ToArray());
            float minY = Mathf.Min(Ys.ToArray());
            float maxY = Mathf.Max(Ys.ToArray());

            float graphWidth = windowRect.width - 2 * graphPadding.x;
            float graphHeight = windowRect.height - 2 * graphPadding.y;

            // Axis positions (left and bottom of the graph area)
            Vector2 origin = new Vector2(
                windowRect.x + graphPadding.x,
                windowRect.y + windowRect.height - graphPadding.y
            );
            Vector2 xAxisEnd = new Vector2(
                windowRect.x + graphPadding.x + graphWidth,
                windowRect.y + windowRect.height - graphPadding.y
            );
            Vector2 yAxisEnd = new Vector2(
                windowRect.x + graphPadding.x,
                windowRect.y + graphPadding.y
            );

            // Draw X axis (horizontal) in white
            DrawLine(origin, xAxisEnd, Color.white, 2f);
            // Draw Y axis (vertical) in white
            DrawLine(origin, yAxisEnd, Color.white, 2f);

            // Reference line color (semi-transparent white)
            Color refLineColor = new Color(1f, 1f, 1f, 0.3f);

            // Draw X axis value labels and vertical reference lines
            int xTicks = 5;
            for (int i = 0; i <= xTicks; i++)
            {
                float t = (float)i / xTicks;
                float xValue = Mathf.Lerp(minX, maxX, t);
                float x = windowRect.x + graphPadding.x + t * graphWidth;
                float y = windowRect.y + windowRect.height - graphPadding.y + 4; // Slightly below axis

                // Draw vertical reference line
                Vector2 refStart = new Vector2(x, windowRect.y + graphPadding.y);
                Vector2 refEnd = new Vector2(x, windowRect.y + windowRect.height - graphPadding.y);
                DrawLine(refStart, refEnd, refLineColor, 1f);

                // Draw label
                string label = xValue.ToString("0.##");
                Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent(label));
                GUI.Label(new Rect(x - labelSize.x / 2, y, labelSize.x, labelSize.y), label);
            }

            // Draw Y axis value labels and horizontal reference lines
            int yTicks = 5;
            for (int i = 0; i <= yTicks; i++)
            {
                float t = (float)i / yTicks;
                float yValue = Mathf.Lerp(minY, maxY, 1 - t); // 0 at bottom, 1 at top
                float y = windowRect.y + graphPadding.y + t * graphHeight;
                float x = windowRect.x + graphPadding.x - 40; // To the left of axis

                // Draw horizontal reference line
                Vector2 refStart = new Vector2(windowRect.x + graphPadding.x, y);
                Vector2 refEnd = new Vector2(windowRect.x + graphPadding.x + graphWidth, y);
                DrawLine(refStart, refEnd, refLineColor, 1f);

                // Draw label
                string label = yValue.ToString("0.##");
                Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent(label));
                GUI.Label(new Rect(x + 2, y - labelSize.y / 2, 38, labelSize.y), label);
            }

            Vector2 prev = Vector2.zero;
            for (int i = 0; i < Xs.Count; i++)
            {
                float xNorm = (Xs[i] - minX) / (maxX - minX + Mathf.Epsilon);
                float yNorm = (Ys[i] - minY) / (maxY - minY + Mathf.Epsilon);

                Vector2 pt = new Vector2(
                    windowRect.x + graphPadding.x + xNorm * graphWidth,
                    windowRect.y + windowRect.height - graphPadding.y - yNorm * graphHeight
                );

                if (i > 0)
                {
                    DrawLine(prev, pt, Color.green, 2f);
                }
                prev = pt;
            }
        }

        private void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
        {
            Matrix4x4 savedMatrix = GUI.matrix;
            Color savedColor = GUI.color;

            float angle = Mathf.Atan2(pointB.y - pointA.y, pointB.x - pointA.x) * Mathf.Rad2Deg;
            float length = Vector2.Distance(pointA, pointB);

            GUI.color = color;
            GUIUtility.RotateAroundPivot(angle, pointA);
            GUI.DrawTexture(new Rect(pointA.x, pointA.y - width / 2, length, width), lineTex);

            GUI.matrix = savedMatrix;
            GUI.color = savedColor;
        }
    }
}
