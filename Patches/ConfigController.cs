﻿using EFT.InventoryLogic;
using EFT.UI;
using EFT.UI.DragAndDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using QuickSell.Patches;
using Comfort.Common;
using EFT;
namespace QuickSell.Patches
{
    public class ConfigController : MonoBehaviour
    {

        void Update()
        {
            if (Singleton<GameWorld>.Instantiated && Singleton<GameWorld>.Instance is not HideoutGameWorld) return;
            if (Singleton<MenuUI>.Instantiated && Singleton<MenuUI>.Instance.HideoutAreaTransferItemsScreen.isActiveAndEnabled) return;

            if (Input.GetKeyDown(Plugin.KeybindFlea.Value.MainKey))
            {
                Utils.SendDebugNotification("Flea Market keybind pressed");

                Item item = SelectItem();
                if (item == null)
                {
                    Utils.SendDebugNotification("No item selected");
                    return;
                }

                ContextMenuPatch.ConfirmWindow((i) => ContextMenuPatch.UIFixesHandler((i) => ContextMenuPatch.SellFlea(i), i), "on the flea", item);
                

            }

            if (Input.GetKeyDown(Plugin.KeybindTraders.Value.MainKey))
            {
                Utils.SendDebugNotification("Traders Market keybind pressed");

                Item item = SelectItem();
                if (item == null)
                {
                    Utils.SendDebugNotification("No item selected");
                    return;
                }
                ContextMenuPatch.ConfirmWindow((i) => ContextMenuPatch.UIFixesHandler((i) => ContextMenuPatch.SellTrader(i), i), "to the traders", item);
            }
        }

        private static Item SelectItem()
        {
            var preloader = GameObject.Find("/Preloader UI/Preloader UI");
            if (preloader == null)
            {
                Utils.SendDebugNotification("Preloader not found");
                return null;
            }
            var raycaster = preloader.GetComponent<GraphicRaycaster>() ?? preloader.AddComponent<GraphicRaycaster>();
            PointerEventData eventData = new(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = [];
            raycaster.Raycast(eventData, results);

            Utils.SendDebugNotification($"{results.Count} results");

            if (results.Count != 0)
            {
                Utils.SendDebugNotification("Cast blocked");
                return GetItemFromRaycastResults(results);
            }

            var target = GameObject.Find("InventoryScreen");
            if (target == null)
            {
                Utils.SendDebugNotification("InventoryScreen not found");
                return null;
            }

            raycaster = target.GetComponent<GraphicRaycaster>() ?? target.AddComponent<GraphicRaycaster>();

            List<RaycastResult> results2 = [];
            raycaster.Raycast(eventData, results2);
            Utils.SendDebugNotification($"{results2.Count} results");
            return GetItemFromRaycastResults(results2); 
        }

        private static Item GetItemFromRaycastResults(List<RaycastResult> results)
        {
            foreach (GameObject gameObject in results.Select(r => r.gameObject))
            {
                Utils.SendDebugNotification(gameObject.name);
                if (gameObject.name == "Color Panel")
                {
                    Utils.SendDebugNotification("Item Found");
                    var itemObject = gameObject.transform.parent.gameObject;
                    GridItemView gridItemView = itemObject.GetComponent<GridItemView>();
                    Utils.SendDebugNotification("Item Returning: " + gridItemView.Item.Name);
                    return gridItemView.Item;
                }
            }
            return null;
        }
    }
}
 