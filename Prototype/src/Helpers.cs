using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AvrixelPrototype
{
    public static class Helpers
    {

        public static Dictionary<string, Image> CreatedHighlightButtons = new Dictionary<string, Image>();


        /// <summary>
        /// Creates a ItemButton Highlight for the given itemUID and stores this in a dictionary for retrieval
        /// </summary>
        /// <param name="itemUID"></param>
        /// <param name="ItemDisplay"></param>
        /// <param name="TintColor"></param>
        /// <param name="Size"></param>
        /// <param name="Parent"></param>
        /// <param name="SetActive"></param>
        /// <param name="PushToBack"></param>
        /// <returns></returns>
        public static Image CreateButtonHighlight(string itemUID, ItemDisplay ItemDisplay, Color TintColor, Vector2 Size, Transform Parent = null, bool SetActive = true, bool PushToBack = true)
        {
            if (!CreatedHighlightButtons.ContainsKey(itemUID))
            {
                Image NewImage = ItemDisplay.gameObject.GetComponent<UISelectable>().CreateHighlightGameObject();
                (NewImage.transform as RectTransform).sizeDelta = Size;
                NewImage.color = TintColor;
                CreatedHighlightButtons.Add(itemUID, NewImage);
                if (Parent != null) NewImage.transform.SetParent(Parent, false);

                NewImage.gameObject.SetActive(SetActive);
                if (PushToBack) NewImage.transform.SetAsFirstSibling();
                return NewImage;
            }

            return null;
        }

        public static bool HasButtonHighLight(string itemUID)
        {
            return CreatedHighlightButtons.ContainsKey(itemUID);
        }

        public static Image GetButtonHighlight(string itemUID)
        {
            if (CreatedHighlightButtons.ContainsKey(itemUID))
            {
                return CreatedHighlightButtons[itemUID];
            }

            return null;
        }

        public static void DestroyButtonHighLight(string itemUID)
        {
            if (CreatedHighlightButtons.ContainsKey(itemUID))
            {
                GameObject.Destroy(CreatedHighlightButtons[itemUID].gameObject);
                CreatedHighlightButtons.Remove(itemUID);
            }
        }

        public static T GetFromAssetBundle<T>(string SLPackName, string AssetBundle, string key) where T : UnityEngine.Object
        {
            if (!SL.PacksLoaded)
            {
                return default(T);
            }

            return SL.GetSLPack(SLPackName).AssetBundles[AssetBundle].LoadAsset<T>(key);
        }

    }
}
