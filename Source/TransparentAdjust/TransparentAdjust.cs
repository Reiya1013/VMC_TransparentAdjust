using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using VMCMod;

namespace TransparentAdjust
{
    [VMCPlugin(
    Name: "TransparentAdjust",
    Version: "0.0.1",
    Author: "Reiya",
    Description: "Transparentを使用時透過ｳｨﾝﾄﾞｳを使用していると裏が透けるのを防止する",
    AuthorURL: "https://twitter.com/Reiya__",
    PluginURL: "https://github.com/Reiya1013/VMC_TransparentAdjust")]

    public class TransparentAdjust : MonoBehaviour
    {

        /// <summary>
        /// ロードしたモデル
        /// </summary>
        private GameObject Model;
        /// <summary>
        /// Alpha値をちょうどよく調整するマテリアル
        /// </summary>
        private Material AlphaOn;

        private void Awake()
        {
            VMCEvents.OnModelLoaded += OnModelLoaded;
        }

        void Start()
        {
            AssetBundleLoad();
        }

        /// <summary>
        /// AssetBundleから必要データを読み込む
        /// </summary>
        private void AssetBundleLoad()
        {
            //AlphaOnMaskMaterialロード
            AssetBundle assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("TransparentAdjust.alphaon"));
            var Assets = assetBundle.LoadAllAssets();
            assetBundle.Unload(false);
            foreach (var asset in Assets)
            {
                if (asset is Material material)
                {
                    AlphaOn = material;
                }
            }

        }


        /// <summary>
        /// ModelLoadedでModelを取得しておく
        /// </summary>
        /// <param name="currentModel"></param>
        private void OnModelLoaded(GameObject currentModel)
        {
            if (currentModel == null) return;
            Model = currentModel;
        }


        /// <summary>
        /// 前メッシュに透過防止用Alpha上書きMaterialを割り当てる
        /// </summary>
        private void OnRenderObject()
        {
            AlphaOnMaterialPatch();
        }

        /// <summary>
        /// Alpha上書きマテリアルの反映
        /// </summary>
        private void AlphaOnMaterialPatch()
        {
            if (AlphaOn == null)
            {
                return;
            }
            if (Model == null) return;
            foreach (Renderer renderer in Model.GetComponentsInChildren<Renderer>(true))
            {
                var meshFilter = renderer.GetComponent<MeshFilter>();
                Mesh mesh;
                if (meshFilter != null)
                {
                    mesh = meshFilter.sharedMesh;
                }
                else
                {
                    var smr = (SkinnedMeshRenderer)renderer;
                    mesh = (smr).sharedMesh;
                    smr.BakeMesh(mesh);
                }

                AlphaOn.SetPass(0);
                Graphics.DrawMeshNow(mesh, renderer.transform.position, renderer.transform.rotation);
            }
        }
    }
}
