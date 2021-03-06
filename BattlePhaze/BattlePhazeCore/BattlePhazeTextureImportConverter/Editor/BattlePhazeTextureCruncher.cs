﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace BattlePhaze.TextureSystem.EditorUnity
{
    /// <summary>
    /// Texture System
    /// </summary>
    public class BattlePhazeTextureCruncher : EditorWindow
    {
        /// <summary>
        /// Textures Importer
        /// </summary>
        public List<TextureImporter> Textures = new List<TextureImporter>();
        public List<string> LightMapDirectories = new List<string>();
        private Color32 DarkPrimaryColor = new Color32(140, 140, 140, 255);
        private Color32 LightPrimaryColor = new Color32(80, 80, 80, 255);
        private Color32 AccentColor = new Color32(252, 152, 103, 255);
        private Color White = new Color32(255, 255, 255, 255);
        private GUIStyle TextStyling;
        private GUIStyle TextLargeStyling;
        private GUIStyle StyleButton;
        private GUIStyle EnumStyling;
        private bool? rerun = false;
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
            BattlePhazeTextureCruncher window = (BattlePhazeTextureCruncher)EditorWindow.GetWindow(typeof(BattlePhazeTextureCruncher));
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
            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = LightPrimaryColor;
            GUILayout.Label("BattlePhaze Texture Importer", TextLargeStyling);
            GUI.backgroundColor = oldColor;
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
            GUI.backgroundColor = AccentColor;
            if (GUILayout.Button("Convert Textures", StyleButton))
            {
                IgnoreLightmaps();
                TextureConvert();
            }
            if (GUILayout.Button("Convert Skinned Mesh Renders Textures", StyleButton))
            {
                IgnoreLightmaps();
                ConvertSkinnedMeshRenders();
            }
            if (GUILayout.Button("Convert Skybox", StyleButton))
            {
                IgnoreLightmaps();
                SceneRender();
            }
            if (GUILayout.Button("Convert Particle System", StyleButton))
            {
                ParticleConvert();
            }
            GUI.backgroundColor = oldColor;
        }
        /// <summary>
        /// Ignores Lightmaps
        /// </summary>
        public void IgnoreLightmaps()
        {
            LightMapDirectories.Clear();
            for (int LightIndex = 0; LightIndex < LightmapSettings.lightmaps.Length; LightIndex++)
            {
                string path = AssetDatabase.GetAssetPath(LightmapSettings.lightmaps[LightIndex].lightmapColor);
                if (path != "")
                {
                    LightMapDirectories.Add(path);
                }
                path = AssetDatabase.GetAssetPath(LightmapSettings.lightmaps[LightIndex].lightmapDir);
                if (path != "")
                {
                    LightMapDirectories.Add(path);
                }
            }
        }
        /// <summary>
        /// Texture Convert
        /// </summary>
        public void TextureConvert()
        {
            Textures.Clear();
            MeshRenderer[] Renders = FindObjectsOfType<MeshRenderer>();
            for (int RendersIndex = 0; RendersIndex < Renders.Length; RendersIndex++)
            {
                foreach (Object obj in EditorUtility.CollectDependencies(new UnityEngine.Object[] { Renders[RendersIndex] }))
                {
                    if (obj is Texture)
                    {
                        string path = AssetDatabase.GetAssetPath(obj);
                        bool wasexisting = true;
                        for (int LightIndex = 0; LightIndex < LightMapDirectories.Count; LightIndex++)
                        {
                            if (path == LightMapDirectories[LightIndex])
                            {
                                wasexisting = false;
                            }
                            if (path.Contains("BakeryLightmaps"))
                            {
                                wasexisting = false;
                            }
                        }
                        if (wasexisting)
                        {
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
                                            if (TextureIM.maxTextureSize > TextureMaxSize)
                                            {
                                                TextureIM.maxTextureSize = TextureMaxSize;
                                            }
                                         Textures.Add(TextureIM);

                                        }
                                    }
#else
                                    if (TextureIM.mipmapEnabled == MipMapEnabled && TextureIM.crunchedCompression == CrunchCompression && TextureIM.textureCompression == TextureCompression && TextureIM.streamingMipmaps == StreamingMipmaps)
                                    {
                                    }
                                    else
                                    {
                                        TextureIM.crunchedCompression = CrunchCompression;
                                        TextureIM.textureCompression = TextureCompression;
                                        TextureIM.mipmapEnabled = MipMapEnabled;
                                        if (TextureIM.maxTextureSize > TextureMaxSize)
                                        {
                                            TextureIM.maxTextureSize = TextureMaxSize;
                                        }
                                        TextureIM.streamingMipmaps = StreamingMipmaps;
                                        Textures.Add(TextureIM);
                                    }
                                }
#endif
                                }
                            }
                        }
                    }
                }
            }
            TextureConvert(Textures.ToArray());
            Textures.Clear();
        }
        public void ParticleConvert()
        {
            Textures.Clear();
            ParticleSystem[] Particle = FindObjectsOfType<ParticleSystem>();
            for (int ParticleIndex = 0; ParticleIndex < Particle.Length; ParticleIndex++)
            {
                foreach (Object obj in EditorUtility.CollectDependencies(new UnityEngine.Object[] { Particle[ParticleIndex] }))
                {
                    if (obj is Texture)
                    {
                        string path = AssetDatabase.GetAssetPath(obj);
                        bool wasexisting = true;
                        for (int LightIndex = 0; LightIndex < LightMapDirectories.Count; LightIndex++)
                        {
                            if (path == LightMapDirectories[LightIndex])
                            {
                                wasexisting = false;
                            }
                            if (path.Contains("BakeryLightmaps"))
                            {
                                wasexisting = false;
                            }
                        }
                        if (wasexisting)
                        {
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
                                            if (TextureIM.maxTextureSize > TextureMaxSize)
                                            {
                                                TextureIM.maxTextureSize = TextureMaxSize;
                                            }
                                            Textures.Add(TextureIM);

                                        }
                                    }
#else
                                    if (TextureIM.mipmapEnabled == MipMapEnabled && TextureIM.crunchedCompression == CrunchCompression && TextureIM.textureCompression == TextureCompression && TextureIM.streamingMipmaps == StreamingMipmaps)
                                    {
                                    }
                                    else
                                    {
                                        TextureIM.crunchedCompression = CrunchCompression;
                                        TextureIM.textureCompression = TextureCompression;
                                        TextureIM.mipmapEnabled = MipMapEnabled;
                                        if (TextureIM.maxTextureSize > TextureMaxSize)
                                        {
                                            TextureIM.maxTextureSize = TextureMaxSize;
                                        }
                                        TextureIM.streamingMipmaps = StreamingMipmaps;
                                        Textures.Add(TextureIM);
                                    }
                                }
#endif
                                }
                            }
                        }
                    }
                }
            }
            TextureConvert(Textures.ToArray());
            Textures.Clear();
        }
        /// <summary>
        /// Converts Skinned mesh Renders
        /// </summary>
        public void ConvertSkinnedMeshRenders()
        {
            Textures.Clear();
            SkinnedMeshRenderer[] Renders = FindObjectsOfType<SkinnedMeshRenderer>();
            for (int RendersIndex = 0; RendersIndex < Renders.Length; RendersIndex++)
            {
                foreach (Object obj in EditorUtility.CollectDependencies(new UnityEngine.Object[] { Renders[RendersIndex] }))
                {
                    if (obj is Texture)
                    {
                        string path = AssetDatabase.GetAssetPath(obj);
                        bool wasexisting = true;
                        for (int LightIndex = 0; LightIndex < LightMapDirectories.Count; LightIndex++)
                        {
                            if (path == LightMapDirectories[LightIndex])
                            {
                                wasexisting = false;
                            }
                            if (path.Contains("BakeryLightmaps"))
                            {
                                wasexisting = false;
                            }
                        }
                        if (wasexisting)
                        {
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
                                            if (TextureIM.maxTextureSize > TextureMaxSize)
                                            {
                                                TextureIM.maxTextureSize = TextureMaxSize;
                                            }
                                            Textures.Add(TextureIM);

                                        }
                                    }
#else
                                    if (TextureIM.mipmapEnabled == MipMapEnabled && TextureIM.crunchedCompression == CrunchCompression && TextureIM.textureCompression == TextureCompression && TextureIM.streamingMipmaps == StreamingMipmaps)
                                    {
                                    }
                                    else
                                    {
                                        TextureIM.crunchedCompression = CrunchCompression;
                                        TextureIM.textureCompression = TextureCompression;
                                        TextureIM.mipmapEnabled = MipMapEnabled;
                                        if (TextureIM.maxTextureSize > TextureMaxSize)
                                        {
                                            TextureIM.maxTextureSize = TextureMaxSize;
                                        }
                                        TextureIM.streamingMipmaps = StreamingMipmaps;
                                        Textures.Add(TextureIM);
                                    }
                                }
#endif
                                }
                            }
                        }
                    }
                }
            }
            TextureConvert(Textures.ToArray());
            Textures.Clear();
        }
        /// <summary>
        /// Scene Render 
        /// </summary>
        public void SceneRender()
        {
           Material Skybox = RenderSettings.skybox;
            foreach (Object obj in EditorUtility.CollectDependencies(new UnityEngine.Object[] { Skybox }))
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
                                    if (TextureIM.maxTextureSize > TextureMaxSize)
                                    {
                                        TextureIM.maxTextureSize = TextureMaxSize;
                                    }
                                    Textures.Add(TextureIM);

                                }
                            }
#else
                                    if (TextureIM.mipmapEnabled == MipMapEnabled && TextureIM.crunchedCompression == CrunchCompression && TextureIM.textureCompression == TextureCompression && TextureIM.streamingMipmaps == StreamingMipmaps)
                                    {
                                    }
                                    else
                                    {
                                        TextureIM.crunchedCompression = CrunchCompression;
                                        TextureIM.textureCompression = TextureCompression;
                                        TextureIM.mipmapEnabled = MipMapEnabled;
                                        if (TextureIM.maxTextureSize > TextureMaxSize)
                                        {
                                            TextureIM.maxTextureSize = TextureMaxSize;
                                        }
                                        TextureIM.streamingMipmaps = StreamingMipmaps;
                                        Textures.Add(TextureIM);
                                    }
                                }
#endif
                        }
                    }
                }
            }
            TextureConvert(Textures.ToArray());
            Textures.Clear();
        }
        /// <summary>
        /// Texture Convert
        /// </summary>
        /// <param name="Textures"></param>
        public void TextureConvert(TextureImporter[] Textures)
        {
            foreach (TextureImporter Texture in Textures)
            {
                if (Texture)
                {
                    EditorUtility.SetDirty(Texture);
                    Texture.SaveAndReimport();
                }
            }
        }
        /// <summary>
        /// UI Design
        /// </summary>
        public void UIDesign()
        {
            rerun = true;
            bool darkmode = EditorGUIUtility.isProSkin;
            Color32 PrimaryColor = darkmode ? DarkPrimaryColor : LightPrimaryColor;
            StyleButton = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
            };
            StyleButton.normal.textColor = White;
            TextStyling = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 11,
                fontStyle = FontStyle.Normal,
                stretchWidth = true,
                fixedWidth = 200,
                fixedHeight = 20,
                clipping = TextClipping.Overflow
            };
            TextStyling.normal.textColor = PrimaryColor;
            TextLargeStyling = EditorStyles.toolbar;
            TextLargeStyling.alignment = TextAnchor.MiddleCenter;
            TextLargeStyling.fontSize = 18;
            TextLargeStyling.fontStyle = FontStyle.Bold;
            TextLargeStyling.fixedHeight = 40;
            TextLargeStyling.stretchWidth = true;
            TextLargeStyling.clipping = TextClipping.Overflow;
            TextLargeStyling.normal.textColor = AccentColor;
            EnumStyling = EditorStyles.popup;
            EnumStyling.normal.textColor = PrimaryColor;
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