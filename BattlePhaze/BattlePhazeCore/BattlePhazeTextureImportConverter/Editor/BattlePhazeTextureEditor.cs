using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace BattlePhaze.TextureSystem.EditorUnity
{
    /// <summary>
    /// Texture System
    /// </summary>
    public class BattlePhazeTextureEditor : EditorWindow
    {
        /// <summary>
        /// Textures Importer
        /// </summary>
        public List<TextureImporter> Textures = new List<TextureImporter>();
        private Color32 PrimaryWhiteColor = new Color32(2, 135, 155, 255);
        private Color32 SecondaryColor = new Color32(140, 140, 140, 255);
        private GUIStyle TextStyling;
        private GUIStyle TextLargeStyling;
        private GUIStyle StyleButton;
        private GUIStyle EnumStyling;
        private bool rerun;
        /// <summary>
        /// Texture Max Size
        /// </summary>
        public int TextureMaxSize = 2048;
        /// <summary>
        /// Mip Map
        /// </summary>
        public bool MipMapEnabled = true;
        /// <summary>
        /// Crunch Compression
        /// </summary>
        public bool CrunchCompression = true;
        /// <summary>
        /// Streaming Mipmaps
        /// </summary>
        public bool StreamingMipmaps = true;
        /// <summary>
        /// Texture Importer Compression method
        /// </summary>
        TextureImporterCompression TextureCompression = TextureImporterCompression.Compressed;
        /// <summary>
        /// Texture Stream Window
        /// </summary>
        [MenuItem("BattlePhaze/Auto Texture Convert")]
        static void TextureStreamConvert()
        {
            BattlePhazeTextureEditor window = (BattlePhazeTextureEditor)EditorWindow.GetWindow(typeof(BattlePhazeTextureEditor));
            window.Show();
        }
        /// <summary>
        /// On GUI
        /// </summary>
        void OnGUI()
        {
            if (rerun == false)
            {
                UIDesign();
            }
            GUILayout.Label("BattlePhaze Texture Importer", TextLargeStyling);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Mip Maps", TextStyling);
            MipMapEnabled = EditorGUILayout.Toggle(MipMapEnabled);
            GUILayout.EndVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Crunch Compression", TextStyling);
            CrunchCompression = EditorGUILayout.Toggle(CrunchCompression);
            GUILayout.EndVertical();
#if !UNITY_2017
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Streaming MipMap", TextStyling);
            StreamingMipmaps = EditorGUILayout.Toggle(StreamingMipmaps);
            GUILayout.EndVertical();
#endif
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Texture Max Size", TextStyling);
            TextureMaxSize = EditorGUILayout.IntField(TextureMaxSize);
            GUILayout.EndVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Texture Compression", TextStyling);
            TextureCompression = (TextureImporterCompression)EditorGUILayout.EnumPopup(TextureCompression, EnumStyling);
            GUILayout.EndVertical();
            if (GUILayout.Button("Run Texture Import",StyleButton))
            {
                TextureConvert();
            }
        }
        /// <summary>
        /// Texture Convert
        /// </summary>
        public void TextureConvert()
        {
            Textures.Clear();
            MeshRenderer[] Renders = FindObjectsOfType<MeshRenderer>();
            Debug.Log(Renders.Length);
            for (int RendersIndex = 0; RendersIndex < Renders.Length; RendersIndex++)
            {
                foreach (Object obj in EditorUtility.CollectDependencies(new UnityEngine.Object[] { Renders[RendersIndex] }))
                {
                    if (obj is Texture)
                    {
                        string path = AssetDatabase.GetAssetPath(obj);
                        if (AssetImporter.GetAtPath(path) is TextureImporter)
                        {
                            TextureImporter TextureIM = (TextureImporter)AssetImporter.GetAtPath(path);
                            if (TextureIM)
                            {
                                if (!Textures.Contains(TextureIM))
                                {
#if UNITY_2017
                                    if (TextureIM.mipmapEnabled == MipMapEnabled && TextureIM.crunchedCompression == CrunchCompression && TextureIM.textureCompression == TextureCompression)
                                    {
                                    }
                                    else
                                    {
                                        TextureIM.crunchedCompression = CrunchCompression;
                                        TextureIM.textureCompression = TextureCompression;
                                        TextureIM.mipmapEnabled = MipMapEnabled;
                                        if (TextureIM.maxTextureSize >= TextureMaxSize)
                                        {
                                            TextureIM.maxTextureSize = TextureMaxSize;
                                        }
                                        Textures.Add(TextureIM);

                                    }
                                }
#else
                                f(TextureIM.mipmapEnabled == MipMapEnabled && TextureIM.crunchedCompression == CrunchCompression && TextureIM.textureCompression == TextureCompression && TextureIM.streamingMipmaps == StreamingMipmaps)
                                {
                                }
                                else
                                {
                                    TextureIM.crunchedCompression = CrunchCompression;
                                    TextureIM.textureCompression = TextureCompression;
                                    TextureIM.mipmapEnabled = MipMapEnabled;
                                        if (TextureIM.maxTextureSize >= TextureMaxSize)
                                        {
                                            TextureIM.maxTextureSize = TextureMaxSize;
                                        }
                                    TextureIM.streamingMipmaps = StreamingMipmaps;
                                    Textures.Add(TextureIM);

                                }
                            }
                        }
#endif
                            }
                        }
                    }
                }
            }
            foreach (TextureImporter Texture in Textures)
            {
                if (Texture)
                {
                    EditorUtility.SetDirty(Texture);
                    Texture.SaveAndReimport();
                }
            }
            Textures.Clear();
        }
        /// <summary>
        /// UI Design
        /// </summary>
        public void UIDesign()
        {
            rerun = true;
            StyleButton = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
            StyleButton.normal.background = SetColor(40, 40, new Color(15, 15, 15, 255));
            StyleButton.normal.textColor = PrimaryWhiteColor;
            TextStyling = new GUIStyle(GUI.skin.textArea)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
            TextStyling.fixedWidth = 300;
            TextStyling.fixedHeight = 20;
            TextStyling.normal.background = SetColor(2, 2, new Color(15, 15, 15, 255));
            TextStyling.clipping = TextClipping.Overflow;
            TextStyling.normal.textColor = PrimaryWhiteColor;
            TextLargeStyling = new GUIStyle(GUI.skin.textArea)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 25,
                fontStyle = FontStyle.Bold
            };
            TextLargeStyling.clipping = TextClipping.Overflow;
            TextLargeStyling.normal.textColor = SecondaryColor;
            EnumStyling = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
                fontStyle = FontStyle.Bold
            };
            EnumStyling.normal.background = SetColor(2, 2, new Color(15, 15, 15, 255));
            EnumStyling.normal.textColor = PrimaryWhiteColor;
        }
        /// <summary>
        /// Used for setting a color of a Texture2D
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public Texture2D SetColor(int x, int y, Color32 color)
        {
            Texture2D tex = new Texture2D(x, y);
            Color[] pix = new Color[x * y];

            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = color;
            }
            tex.SetPixels(pix);
            tex.Apply();
            return tex;
        }
    }
}