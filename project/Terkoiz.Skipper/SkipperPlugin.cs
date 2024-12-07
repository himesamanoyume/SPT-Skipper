using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EFT.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace Terkoiz.Skipper
{
    [BepInPlugin("com.terkoiz.skipper", "姫様の夢汉化 Terkoiz.Skipper", "1.2.0")]
    public class SkipperPlugin : BaseUnityPlugin
    {
        internal const string SkipButtonName = "SkipButton";

        internal new static ManualLogSource Logger { get; private set; }

        private const string MainSectionName = "Main";
        internal static ConfigEntry<bool> ModEnabled;
        internal static ConfigEntry<bool> AlwaysDisplay;
        internal static ConfigEntry<KeyboardShortcut> DisplayHotkey;

        [UsedImplicitly]
        internal void Start()
        {
            Logger = base.Logger;
            InitConfiguration();

            new QuestObjectiveViewPatch().Enable();
        }

        private void InitConfiguration()
        {
            ModEnabled = Config.Bind(
                MainSectionName,
                "1. 启用",
                true,
                "全局模式切换. 是否需要重新打开任务窗口以使设置更改生效.");

            AlwaysDisplay = Config.Bind(
                MainSectionName,
                "2. 始终显示跳过按钮",
                false,
                "如果启用, 跳过按钮将始终可见.");

            DisplayHotkey = Config.Bind(
                MainSectionName,
                "3. 显示跳过按钮 快捷键",
                new KeyboardShortcut(KeyCode.LeftControl),
                "按住此键将使跳过按钮出现.");
        }

        [UsedImplicitly]
        internal void Update()
        {
            if (!ModEnabled.Value || AlwaysDisplay.Value)
            {
                return;
            }

            if (QuestObjectiveViewPatch.LastSeenObjectivesBlock == null || !QuestObjectiveViewPatch.LastSeenObjectivesBlock.activeSelf)
            {
                return;
            }

            if (DisplayHotkey.Value.IsDown())
            {
                ChangeButtonVisibility(true);
            }

            if (DisplayHotkey.Value.IsUp())
            {
                ChangeButtonVisibility(false);
            }
        }

        private static void ChangeButtonVisibility(bool setVisibilityTo)
        {
            foreach (var button in QuestObjectiveViewPatch.LastSeenObjectivesBlock.GetComponentsInChildren<DefaultUIButton>(includeInactive: true))
            {
                if (button.name != SkipButtonName) continue;

                button.gameObject.SetActive(setVisibilityTo);
            }
        }
    }
}
