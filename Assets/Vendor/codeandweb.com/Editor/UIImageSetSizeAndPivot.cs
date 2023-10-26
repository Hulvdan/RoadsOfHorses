﻿using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class UIImageMenu {
    [MenuItem("GameObject/UI/Set Native Size + Pivot", false, 10000)]
    static void setSizeAndPivot() {
        foreach (var gameObject in Selection.gameObjects) {
            // process all selected game objects which have a RectTransform + Image
            var transform = gameObject.GetComponent<RectTransform>();
            var image = gameObject.GetComponent<Image>();

            if (transform && image && image.sprite) {
                // set size as it is defined by source image sprite
                image.SetNativeSize();

#if UNITY_2018_1_OR_NEWER
                // use mesh defined by source sprite to render UI image
                image.useSpriteMesh = true;
#endif

                // set pivot point as defined by source sprite
                var size = transform.sizeDelta * image.pixelsPerUnit;
                var pixelPivot = image.sprite.pivot;
                // sprite pivot point is defined in pixel, RectTransform pivot point is normalized
                transform.pivot = new Vector2(pixelPivot.x / size.x, pixelPivot.y / size.y);
            }
        }
    }
}
