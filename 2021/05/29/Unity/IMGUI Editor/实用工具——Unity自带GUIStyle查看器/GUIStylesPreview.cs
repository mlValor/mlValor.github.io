using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace EditorToolKits
{
    public class GUIStylesPreview : EditorWindow
    {
        [MenuItem("Editor Extension/GUIStyles Preview", priority = 2)]
        public static void ShowWindow()
        {
            GUIStylesPreview w = EditorWindow.GetWindow<GUIStylesPreview>("GUIStyles Preview");
            w.Show();
        }

        private struct Drawing
        {
            public Rect Rect;
            public System.Action Draw;
        }

        private List<Drawing> Drawings;

        private List<Object> _objects;
        private float _scrollPos;
        private float _maxY;
        private Rect _oldPosition;

        private bool _showingStyles = true;

        private string _search = "";

        void OnGUI()
        {
            if (!position.width.Equals(_oldPosition.width) && Event.current.type == EventType.Layout)
            {
                Drawings = null;
                _oldPosition = position;
            }
            GUILayout.Button("Styles");
            string newSearch = GUILayout.TextField(_search);
            if (newSearch != _search)
            {
                _search = newSearch;
                Drawings = null;
            }

            float top = 36;

            if (Drawings == null)
            {
                string lowerSearch = _search.ToLower();

                Drawings = new List<Drawing>();

                GUIContent inactiveText = new GUIContent("inactive");
                GUIContent activeText = new GUIContent("active");

                float x = 5.0f;
                float y = 5.0f;

                if (_showingStyles)
                {
                    foreach (GUIStyle ss in GUI.skin)
                    {
                        if (lowerSearch != "" && !ss.name.ToLower().Contains(lowerSearch))
                            continue;

                        GUIStyle thisStyle = ss;

                        Drawing draw = new Drawing();

                        float width = Mathf.Max(
                            100.0f,
                            GUI.skin.button.CalcSize(new GUIContent(ss.name)).x,
                            ss.CalcSize(inactiveText).x + ss.CalcSize(activeText).x
                        ) + 16.0f;

                        float height = 60.0f;

                        if (x + width > position.width - 32 && x > 5.0f)
                        {
                            x = 5.0f;
                            y += height + 10.0f;
                        }

                        draw.Rect = new Rect(x, y, width, height);

                        width -= 8.0f;

                        draw.Draw = () =>
                        {
                            if (GUILayout.Button(thisStyle.name, GUILayout.Width(width)))
                                CopyText("(GUIStyle)\"" + thisStyle.name + "\"");

                            GUILayout.BeginHorizontal();
                            GUILayout.Toggle(true, inactiveText, thisStyle, GUILayout.Width(width / 2));
                            GUILayout.Toggle(false, activeText, thisStyle, GUILayout.Width(width / 2));
                            GUILayout.EndHorizontal();
                        };

                        x += width + 18.0f;

                        Drawings.Add(draw);
                    }
                }

                _maxY = y;
            }

            Rect r = position;
            r.y = top;
            r.height -= r.y;
            r.x = r.width - 16;
            r.width = 16;

            float areaHeight = position.height - top;
            _scrollPos = GUI.VerticalScrollbar(r, _scrollPos, areaHeight, 0.0f, _maxY);

            Rect area = new Rect(0, top, position.width - 16.0f, areaHeight);
            GUILayout.BeginArea(area);
            //int count = 0;
            foreach (Drawing draw in Drawings)
            {
                Rect newRect = draw.Rect;
                newRect.y -= _scrollPos;

                if (newRect.y + newRect.height > 0 && newRect.y < areaHeight)
                {
                    GUILayout.BeginArea(newRect, GUI.skin.textField);
                    draw.Draw();
                    GUILayout.EndArea();
                }
            }

            GUILayout.EndArea();
        }

        void CopyText(string pText)
        {
            TextEditor editor = new TextEditor();
            
            editor.text = pText;

            editor.SelectAll();
            editor.Copy();
        }
    }
}