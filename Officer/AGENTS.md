# CLAUDE.md - Officer

> Last updated: 2026-09-04 14:30 +08

## 项目概述
这是一个基于 Unity `2022.3.22f1` 的 VR 办公室互动项目，集成了 `SteamVR`、`XR Interaction Toolkit`、`OpenXR` 与 `PICO XR` 相关能力。项目核心目标是实现玩家在办公室场景中的移动、转向、物品交互、数据/UI 展示，以及基于 VR 手柄的交互流程。

## 关键命令（必须准确）
- 打开项目：使用 `Unity 2022.3.22f1` 打开目录 `E:\UnityProject\LowPoly-Project\Officer`
- 打开当前正常作业场景：`Assets/SteamVR/Simple Sample.unity`
- 打开备用主场景：`Assets/Scenes/SteamVer.unity`
- 打开 C# 工程：双击 `Officer.sln`
- 查看包依赖：检查 `Packages/manifest.json`
- 查看 Unity 版本：检查 `ProjectSettings/ProjectVersion.txt`
- 重新导入输入/脚本后验证：回到 Unity Editor 等待编译完成并检查 `Console`；`unity-log` MCP 助手可能读不到编译错误，以 `EditorUtility.scriptCompilationFailed` 和 `%LOCALAPPDATA%\Unity\Editor\Editor.log` 为准
- Unity MCP：Editor 里 `puerts-mcp` 默认监听 `http://127.0.0.1:3100/mcp`（工具 `evalJsCode`）；Cursor 内置 MCP 列表里没有它，需直连该端口
- Android 打包入口：在 Unity Editor 中使用 `File > Build Settings`

## 架构边界（严格遵守）
- 业务逻辑 → `Assets/Scripts/Game/`（含第 1 天教学 `DayOneTutorialDirector`）
- VR 交互与输入逻辑 → `Assets/Scripts/SteamVRScripts/`
- 数据定义与数据中心 → `Assets/Scripts/Data/`
- 事件系统 → `Assets/Scripts/Event/`
- 状态机辅助逻辑 → `Assets/Scripts/FSM/`
- 通用工具/单例/时间工具 → `Assets/Scripts/Tools/`
- 场景资源与流程验证 → 优先 `Assets/SteamVR/Simple Sample.unity`，其次 `Assets/Scenes/SteamVer.unity`
- 第三方 SDK/样例 → `Assets/SteamVR/`、`Assets/Samples/`、`Assets/XR/`、`Assets/XRI/`
- **禁止默认修改**：第三方 SDK、Samples、自动生成输入代码、外部资源包，除非问题明确出在这些文件且无法在项目脚本层修复。
- **优先原则**：优先在 `Assets/Scripts/` 下新增适配层、管理器或桥接脚本，不要直接大面积改动第三方包源码。

## 编码约定（Never 列表）
- 始终优先在项目自有脚本目录中修复问题，不先改 `SteamVR` 或 `Samples` 源文件
- 不随意修改 `.meta` 文件，除非明确涉及重命名、GUID 绑定或资源移动
- 不把多个职责混进一个 `MonoBehaviour`：输入、UI、业务、数据应尽量分开
- 不在 Update 中重复做高成本查找，如 `FindObjectOfType`、`GameObject.Find`
- 不把场景对象名写死为核心逻辑依赖，优先走 Inspector 引用或明确桥接
- 不无条件输出 `Debug.Log` 作为最终方案，调试日志应可控、可移除
- 不在未验证场景引用的情况下直接修改大型 `.unity` 文件
- 不混用 `SteamVR` 与 `XRI` 的同类控制逻辑而不加说明
- 不因为小需求顺手重构整套交互系统
- 不在未确认文件真实编码前，直接覆盖出现乱码的文档文件
- 不在 Play 时用脚本创建主监视器百分制 UI，也不要用脚本写死进度条/`WorkProgressPercent` 的 RectTransform；布局留在 Hierarchy/Inspector
- 不做低压版本时删除高压代码；只用 `GameManager.LowStressVersion` 旁路
- 不把 `GameManager.LowStressVersion` 做成运行时切换；它只用于打包前勾选低压或高压

## 项目结构建议
- `Assets/Scripts/Game/`：玩法、流程、玩家状态、关卡逻辑、第 1 天教学（`DayOneTutorialDirector.cs`、`GuideAnimationLoop.cs`）
- `Assets/Scripts/SteamVRScripts/`：手柄输入、射线交互、抓取、转向、传送回调
- `Assets/Scripts/Data/`：`DataCenter`、`SnackData`、`ItemData`、`MainMonitorData` 等数据定义
- `Assets/Scripts/Event/`：项目内事件广播与解耦
- `Assets/Scripts/Tools/`：单例、时间管理、调试辅助、通用计算
- `Assets/SteamVR/Simple Sample.unity`：当前正常作业场景，优先用于 VR 输入、抓取、吃零食等交互验证
- `Assets/Scenes/SteamVer.unity`：备用/并行保留场景，涉及 XRI 链路时再单独确认

## 当前项目特征
- Unity 版本固定为 `2022.3.22f1`
- 同时存在 `SteamVR`、`OpenXR`、`XR Interaction Toolkit`、`PICO` 相关依赖
- 主业务脚本集中在 `Assets/Scripts/`
- 当前项目已有 VR 输入控制脚本，如 `TestInput.cs`、`LaserPointerHandler.cs`、`PlayerSteamVRManager.cs`
- 当前 `GameManager` 已暴露 `countDown` 倒计时参数，默认 60 秒；从第 2 天开始，阶段内会在 `Update` 中持续计时
- 当前 `GameManager` 已暴露 `currentWorkProgress` 调试字段，并与 `DataCenter.GameData.PlayerData.workProgress` 和主监视器进度条、百分制保持同步
- 当前 `GameManager` 已改为通过场景内 `No snack` 对象做超时后的次日提示，不再依赖外部音频资源路径；该提示只会被触发一次
- 当前项目已新增“头显中心视野检测”项目侧实现：统一射线入口在 `PlayerSteamVRManager`，目标回调组件为 `CenterGazeCallback`
- 当前项目同时保留了官方示例和自定义实现，修改前需分清“示例代码”与“项目实际使用代码”
- `Assets/SteamVR/Simple Sample.unity` 是目前正常作业和默认回归测试场景
- `Assets/SteamVR/Simple Sample.unity` 当前实际验证链路使用 `PlayerSteamVRManager` 与 `MyInteractableSteamVR`，不是 `XRGrabInteractable`
- 当前射线交互已改为项目侧过滤：`LaserPointerHandler` 使用 `RaycastAll` + 距离排序，允许穿透非目标 Tag 物体，只对目标 Tag 生效
- 当前射线目标 Tag 白名单：`RayInteractable`、`Snack`、`Snacks`
- 当前近手悬停已被禁用：手部靠近不会触发 `OnHandHoverBegin/End` 交互，悬停高亮与 UI 由射线专用回调 `OnRayHoverBegin/End` 驱动
- 当前 `GameManager` 已暴露 `playerRayLength`，可统一控制玩家左右手射线长度
- 当前道具 TTS 链路为“双通道”：首次拿起走 `onObjectAttachedOnce -> ItemData.TryPlayPickupTTSOnce()`；按 Trigger 走 `onTriggerDown -> ItemData.PlayPickupTTS()`
- 当前手部交互为“双入口”：远距用 `LaserPointerHandler`（射线），近距用 `HandGrabCollider`（手部碰撞球）；两者都能抓取、Trigger、触发 guide
- 当前 `HandGrabCollider` 挂在场景 `RightHand` / `LeftHand` 上，依赖该手已有的触发器碰撞体收 `OnTriggerEnter/Exit`；它**完全独立于 laser 的悬停状态**，不调用 `OnRayHoverBegin/End`，因此不会干扰射线
- 当前 `MyInteractableSteamVR` 暴露 `DispatchTriggerFromExternalHand(Hand)` 作为碰撞球的 Trigger 入口，内部仍走 `TryDispatchTriggerFromHand` 的同帧去重，避免与 laser 重复触发
- 当前 `HandGrabCollider` 释放物体后会把它加入“忽略集”，直到它真正离开碰撞球才允许再次抓取，防止吸附在手上的物体被反复重抓
- ⚠️ 已知坑：`LaserPointerHandler` 的 `raycastLayers`（Inspector 里的 Raycast Layers）会反复被序列化成 `Nothing`（`m_Bits: 0`），导致 `RaycastAll` 打不到任何东西、laser 完全失效（不悬停/不抓取/不点击 guide），并连带让零食 guide outline 卡在常亮；**注意：Unity 开着场景时改磁盘 `.unity` 文件无效，会被内存版本覆盖回 0**
- ⚠️ 已知坑：`KeyboardController` 若 `using System`，`Random.Range` 会和 `System.Random` 冲突（CS0104），必须写 `UnityEngine.Random.Range`
- ⚠️ 已知坑：Unity MCP 的 `unity-log` 在编译失败时可能返回 0 条 error；以 `EditorUtility.scriptCompilationFailed` 和 `%LOCALAPPDATA%\Unity\Editor\Editor.log` 为准
- ⚠️ 已知坑：第 1 天导演 `Start` 会先 `ClearCurrentSnack()`。若 `SnackManager` 还没记下出生点，`ResetSnackRootState` 会把世界坐标写成 `(0,0,0)`，薯片会出现在场景原点/沙发附近地板上。出生点必须在 `Awake` 用 `Snacks` 相对 `SnacksRespawnPoint` 的 local pose 捕获，复位也走 local，不要用尚未赋值的世界坐标
- ⚠️ 已知坑：传送进 `StartTriggerBox` 时 Unity 可能不发 `OnTriggerEnter`，键盘会等玩家再走动才出现。WaitForWorkArea 在 `Teleport.Player` 完成时查一次 overlap，走路仍走 `OnTriggerEnter`
- ⚠️ 已知坑：晚上吃完零食后若手还抓着 `Snacks` 根，下一包会被带到手上。刷第二包前必须 `ForceReleaseIfHolding` 再复位到 `SnacksRespawnPoint`；`VanishEffect` 会把 `Container` 缩到 0.5，复位时要停动画并还原 scale
- 当前零食复位已走 `SnackManager`：`Awake` 记相对 `SnacksRespawnPoint` 的 local pose；`ResetSnackRootState` 会松手、停 `VanishEffect`、还原 local 坐标和 `Container` scale
- 当前已在代码层自愈：`LaserPointerHandler.GetEffectiveRaycastLayers()` 在 mask 为空(0)时自动回退到 `Physics.DefaultRaycastLayers`，因此不管场景序列化成什么，laser 都能命中；理想 Inspector 值仍为 `Everything`（除 Ignore Raycast，`m_Bits: 4294967291`）
- 当前零食有“两层 outline”：绿色为旧 guide 介绍 outline（`SnackGuideIntroTrigger.guideOutlines`），黄色为出生提示 outline（`SnackManager` 控制，挂在 `_curSnacks` 子树）；二者独立。第 1 天旧绿色 guide 开局即被强制关闭
- 当前睡觉入口为 `GameManager.SleepToNextDayFromInteraction()`（床 trigger 绑定）；非晚上播放 `TTS/ItemGet/NotTimeToSleep`；第 1 天晚上喂完（仓鼠关闭则吃完）后站在床 trigger 互动即可睡觉
- 键盘有效 slap 在进度条加满的那一次就 `PREPARE_CHANGE_TIME "work"`，不要再依赖“打满后再多拍一拍”
- 当前项目用 `GameManager.LowStressVersion` 区分高压/低压两个打包版本：默认 false 为高压；勾选后开局走低压旁路，**不要做成运行中途切换，也不要删除高压代码**。当前 `Simple Sample` 默认 `LowStressVersion: 0`（高压）

## 第 1 天教学
- 只认 `DayOneTutorialDirector`。`GameManager.Awake` 若场景里没有该组件会运行时 `AddComponent`；要在 Inspector 挂 TTS，必须在 `GameManager` 上保存一份该组件，并配置 `stepHooks`
- 流程（仅 `days == 1`）：`StartTriggerBox` 到达工位 → 早班只出键盘（循环 `Shining`；`guideDismissHitCount` 默认 3 次有效 slap 关动画；打满场景 `requireHit`，当前 Simple Sample 为 10，才换阶段）→ 仓鼠开启则下午只出仓鼠（摸满 `stayRequireTime` 进晚上）→ 晚上先只出 `Chips` 给玩家吃，仓鼠此时不出；吃完后仓鼠才出现，再刷一包只喂、禁止抚摸 → 喂完后站在床 trigger 按 InteractUI/GrabGrip 睡觉进第 2 天（注视床仍会关引导，但不再是睡觉前提）
- 仓鼠关闭（`GameManager.enableHamster == false`）：早班完成后 `_skipToNightOnNextChange` 一次过场直接晚上，不进下午、不刷第二包、不喂
- 教学结算计入总进度：早班工作进度、下午抚摸好感；晚上喂仓鼠后保留 `isOut`，第 2 天照常 `MainItemManager.RandomItem()`
- 第 1 天 `SnackManager.Start` 不调用 `RandomSnack()`；晚上用 `SpawnSnackByName("Chips")`。吃/喂门禁：`TutorialSnackRule.PlayerEatOnly` / `HamsterFeedOnly`
- 开局关闭旧并行 guide（`enableGuideIntro` / `TryTriggerGuideIntro`）。进第 2 天：`FinishTutorial()` 停循环动画、关触发盒、清交互锁，再 `ForceCompleteAllGuidesAsLearned()`。只关 outline 不够
- TTS 接口：`stepHooks` 的 `onEntered` / `onGuideDismissed` / `onCompleted`，或 `PlayTutorialTTS(string path)` / `PlayTutorialTTS(AudioClip)`（播时 `PushPlayerInteractionLock`，播完 `Pop`）
- 场景引用可留空，导演运行时查找 `StartTriggerBox`、键盘（优先带 `Work` 子物体的父节点）、仓鼠、`SnackManager`、名为 `Bed` 的 Animator。不要在 Unity 开着时改磁盘 `.unity` 去绑这些引用

## 当前交互链路说明
- 工作进度链路：`GameManager.currentWorkProgress` ↔ `DataCenter.GameData.PlayerData.workProgress` → `EventCommon.UPDATE_MONITOR` → `MainMonitorData.UpdateInfo()` → 主监视器 `Scrollbar` + `workInfo/WorkProgressPercent` 百分制（对象在 `Assets/Prefab/Item/MainMonitor.prefab` 层级里；脚本只改文字，布局在 Inspector 调 `Scrollbar` 与 `WorkProgressPercent`）
- 阶段倒计时链路：`GameManager.countDown` → `GameManager.Update()` → `TickStageCountDown()`；仅从第 2 天开始生效，每次推进到下一阶段后重置
- 超时奖惩链路：当天任意阶段超时后，`GameManager` 会记录当日超时状态；跨到下一天时调用 `SnackManager.SetContainerVisible(bool)`，超时则隐藏 `Container`，未超时则显示 `Container`
- 超时提示链路：首次发生“前一天超时”并进入下一天时，`GameManager` 会显示场景内 `No snack` 对象；该对象依赖自身 `AudioSource.playOnAwake` 播放提示，并且只触发一次，后续不再由 `GameManager` 重复 show/hide
- 零食提示链路：`MyInteractableSteamVR` → `EventCommon.PLAYER_SNACK_HINT` → `PlayerSteamVRManager.SetSnackHintVisible(bool)`
- 射线悬停链路：`LaserPointerHandler.RaycastAll`（穿透非目标 Tag）→ `MyInteractableSteamVR.OnRayHoverBegin/End` → Interactable 高亮 / Item UI / Hover 回调
- 射线点击链路：`LaserPointerHandler.HandleFilteredClick()` → `pointerDown/pointerClick/pointerUp`（仅对过滤后的目标触发）
- Trigger 播放链路：`MyInteractableSteamVR` 在“射线悬停中”或“已抓在手上”时监听 `InteractUI` 抬起并触发 `onTriggerDown`
- 手部碰撞球抓取链路：`HandGrabCollider.OnTriggerEnter/Exit` 收集 `canBeMoved` 的 `MyInteractableSteamVR` 候选（先进先抓）→ `GrabGrip` 按下抓取/跟随、抬起释放
- 手部碰撞球 Trigger 链路：`HandGrabCollider.DispatchTrigger()` → `MyInteractableSteamVR.DispatchTriggerFromExternalHand()` → `onTriggerDown`（与 laser 帧级去重，不重复）
- 手部碰撞球 guide 链路：`HandGrabCollider` 仍会在重叠时调用 `TryTriggerGuideIntro()`，但第 1 天开局已 `ForceCompleteGuideIntro()`，该入口是空操作；教学引导改由 `DayOneTutorialDirector` 循环 `Shining`
- 零食出生提示 outline 隐藏链路：射线指到走 `LaserPointerHandler -> SnackManager.HideSpawnOutlineForRayTarget()`；零食 guide 完成走 `SnackGuideIntroTrigger.TryTriggerGuideIntro() -> SnackManager.HideCurrentSnackSpawnOutline()`（按 `_curSnacks` 根隐藏，避免 guide 挂在子物体时漏掉根上的黄色 outline）
- 零食 guide 未完成时，`SnackManager` 会用 `IsCurrentSnackGuidePending()` 拦截出生提示的隐藏，保证 guide 期间提示常亮，直到 guide 被触发
- 第一天教学链路：`StartTriggerBox.Entered` → `DayOneTutorialDirector.NotifyArrivedAtWorkArea()` → 早/午/晚步骤；进第 2 天前 `HandleDayOneGuideFallbackBeforeDayIncrement()` → `FinishTutorial()` + `ForceCompleteAllGuidesAsLearned()`。高压是晚→次日，低压是下午→次日，同一套收尾
- 睡觉交互链路：床 trigger → `GameManager.SleepToNextDayFromInteraction()` → 非晚上播放 `NotTimeToSleep`；晚上且（非教学或 `CanSleep`）→ `TrySleepToNextDay()` → `CHANGE_TIME` 推进到次日。低压版永不进入晚上，因此床会一直走 `NotTimeToSleep`，换天只靠工作/玩耍
- 提示区域挂点：`Player/Container/SteamVRObjects/VRCamera/FollowHead/HeadCollider`
- `HeadCollider` 下会在运行时创建 `HeadColliderVisual` 子物体，并挂载球体 MeshRenderer 作为提示显示
- 当前提示样式为淡蓝色半透明球体；默认隐藏，玩家拿起零食时显示，放下或吃掉后隐藏
- 头显中心视野链路：`PlayerSteamVRManager` 统一缓存头显中心射线与命中结果 → `CenterGazeCallback` 复用该结果判断是否看向目标 → 触发 `onGazeEnter` / `onGazeExit`
- 当前中心视野检测优先使用 `Player.instance.hmdTransform`，拿不到时回退到 `Camera.main`
- 传送区域回调链路：`TeleportAreaCallback` 监听 `Teleport.Player`；玩家传送到当前区域时触发 `onTeleportComplete`，离开当前区域时触发 `onPlayerExitArea`
- 当前离开区域判定同时覆盖两种情况：传送到其他 `TeleportArea`，或玩家传送到该区域后通过房间尺度实际走出区域边界
- `TeleportAreaCallback` 当前还支持在离开区域时直接调用 `TeleportArea.SetLocked(true)` 锁定自身区域，适合做一次性传送点
- 当前可视提示与正常玩法链路都以 `Assets/SteamVR/Simple Sample.unity` 为准；若要同步到 `SteamVer.unity`，需分别确认 XRI 链路

## 低压版本（LowStressVersion）
- 开关：`GameManager` Inspector 的 `Low Stress Version`（字段 `LowStressVersion`）；其他脚本读 `GameManager.Instance.IsLowStressVersion`
- 打包：同一套场景打两个包。勾选后打低压包，不勾选打高压包。只在 `Start -> ApplyLowStressVersionState()` 生效，**不要做成运行中途切换**
- 原则：高压逻辑全部保留，只在关键路径用 `if (LowStressVersion)` 旁路；不要删 `stageAdvanceCallbacks`、疲劳、晚上阶段或原有结局分支
- 开局：`ApplyLowStressVersionState()` 隐藏 `GameManager/OutsideWalls`（`outsideWalls` 为空时 `transform.Find("OutsideWalls")`）；解锁场景内全部 `TeleportArea`（含未激活的 `TeleportAreaLock2`）；把各 `TeleportAreaCallback.lockAreaOnExit` 设为 false
- 探索：`TeleportAreaCallback.LockCurrentTeleportArea()` 在低压下直接 return，离开区域不会再锁传送点
- 阶段：高压仍是早→午→晚→次日早。低压 `ChangeTime()` 为早→午→次日早（`curTimeStage == 1 && LowStressVersion` 走换天，不把天空盒切到 night，监视器时间不会到 21:00）。换天仍由键盘工作 / 仓鼠玩耍发 `PREPARE_CHANGE_TIME`，不依赖睡觉
- 零食：换天时 `shouldShowSnackContainerNextDay = LowStressVersion || !_hasTimedOutToday`，始终 `SetContainerVisible(true)` + `RandomSnack()`，不走剥夺、不 `ShowNoSnackObject()`
- 事件：`InvokeStageAdvanceCallbacks()` 开头 return，Inspector 里 Stage Advance Callbacks 绑定的方法（发光、减进度、激活物体等）都不触发
- 疲劳：换天不 `AddFatigue` / `ResetFatigue`；`ApplyHalfOnByFatigue()` 强制 `gray=false`；`EvaluateDailyFatigueState()` 返回 false，不触发猝死结局
- 结局：`EndingManager.HandleEndingTextAndGameObjects()` 固定 `TTS/Ending/Work/WorkStandard`，不拼仓鼠结局、不走 Workaholic / WorkFailed / WorkDead。若误入 `EndingDeath()` 也改走 `Ending()`
- 第 1 天：低压不进晚上，教学的晚上吃/喂/睡觉步骤不会走到；换天靠下午工作/玩耍，收尾仍走 `HandleDayOneGuideFallbackBeforeDayIncrement()`
- 涉及脚本：`Assets/Scripts/Game/GameManager.cs`、`Assets/Scripts/Game/EndingManager.cs`、`Assets/Scripts/SteamVRScripts/TeleportAreaCallback.cs`

## 修改策略
- 小改动：优先改单个脚本并保持 Inspector 字段兼容
- 交互改动：先确认目标场景实际挂载的是哪个脚本，再动代码
- UI/Toggle/按钮改动：优先提供可从 Inspector 绑定的方法，如 `public void OnXChanged(bool value)`
- 数据调试改动：若要暴露可手动修改的运行时数据，优先在 `GameManager` 或对应管理器提供 Inspector 可见字段，并保持与 `DataCenter` 和现有 UI 刷新链路同步
- VR 输入改动：明确区分左手/右手输入源、平滑移动、Snap Turn、传送、抓取
- 视线/注视改动：优先复用 `PlayerSteamVRManager` 中统一的中心视线结果，不要在每个目标组件里重复发同样的 `Raycast`
- 射线可交互改动：优先在 `LaserPointerHandler` 维护“命中筛选 + 可视长度 + 点击派发”统一逻辑，避免在多个目标脚本重复判定
- 手部碰撞球改动：只改 `HandGrabCollider`，保持它独立于 laser 的悬停状态（不要调用 `OnRayHoverBegin/End`），避免两套交互互相污染 `isRayHovering`
- laser “完全没反应”时：先查 `LaserPointerHandler.raycastLayers` 是否被设成 `Nothing`，再查目标 Tag 与 Layer；代码已有 `GetEffectiveRaycastLayers()` 兜底，**不要**在 Unity 开着场景时改磁盘 `.unity` 去修 mask（会被内存版本覆盖）
- 改睡觉/换天逻辑：非晚上反馈走 `SleepToNextDayFromInteraction()`；实际推进仍由 `TrySleepToNextDay()` 负责，注意 `_isStageAdvanceRequested` 空档期；第 1 天晚上要过 `DayOneTutorialDirector.CanSleep`（已进入 `NightLookAtBed`，站在床 trigger 互动即可）。低压版不进晚上，换天只改 `ChangeTime()` 的下午旁路，不要让床在低压里直接 `TrySleepToNextDay()`
- 改第 1 天教学：只改 `DayOneTutorialDirector` 及它调用的门禁（键盘 slap、仓鼠摸/打、零食吃/喂、`GameManager._skipToNightOnNextChange`）。不要重新打开旧 `enableGuideIntro`。`requireHit` 跟场景值；3 次 slap 只关动画；晚上先吃后出仓鼠，禁摸只喂
- 改进度百分制：只改 `MainMonitorData.workProgressPercent` 的刷新文字；对象必须已在 `MainMonitor/workInfo/WorkProgressPercent`，不要运行时 `new GameObject`，不要改进度条坐标
- 改低压版本：只加 `LowStressVersion` 判断，不删高压分支；打包靠 Inspector 勾选区分两个版本。零食走 `LowStressVersion || !_hasTimedOutToday`，回调走 `InvokeStageAdvanceCallbacks()` 开头 return，阶段走下午直接换天、不进晚上，结局走 `EndingManager` 的 WorkStandard 早退
- 改 guide 生命周期：第 1 天只认导演循环动画；进第 2 天必须 `FinishTutorial()` 停 `Shining` 并清 lock，不能只靠 `ForceCompleteGuideIntro()`
- 场景相关问题：优先查组件引用、事件回调、Layer/Collider、Toggle 状态，再改代码
- 第三方代码必须改时：尽量做最小补丁，并在本文档记录原因、影响范围、回退方式

## 已记录的第三方补丁
- `Packages/puerts-mcp/Resources/McpServer/main.cjs`：为兼容当前 Codex/RMCP 调用链，调整 POST 响应为 JSON，避免 `text/event-stream` 导致客户端解码失败
- `Packages/puerts-mcp/Editor/McpServerWindow.cs`：增加 MCP Server 自动启动状态持久化，减少 Unity 重载后服务未恢复的问题
- 上述补丁仅用于恢复 Unity MCP 联调能力；后续若上游修复，需要优先评估是否回退到官方实现

## Compact Instructions（上下文压缩时必须保留）
- 这是 Unity VR 项目，主逻辑在 `Assets/Scripts/`
- 优先改项目脚本，不优先改 `Assets/SteamVR/` 或 `Assets/Samples/`
- 变更前先确认目标场景和真实挂载脚本
- VR 改动必须区分输入源、场景引用、Toggle/Inspector 绑定
- 第 1 天教学走 `DayOneTutorialDirector`，不要再启用旧的并行 `enableGuideIntro`
- 低压版开关是 `GameManager.LowStressVersion`：勾选打包低压，不勾选打包高压；开局生效，不支持中途切换；不删高压代码
- 低压旁路：隐藏 `OutsideWalls`、解锁传送区、不剥夺零食、不触发 `stageAdvanceCallbacks`、不进晚上（下午直接换天）、不判定熬夜灰屏/死亡、结局固定 WorkStandard
- 晚上仓鼠在玩家吃完薯片后才出现；零食复位走 local pose + 松手 + 还原 scale
- 主监视器百分制在预制体 `workInfo/WorkProgressPercent`，脚本只改文字
- 提交结果时说明：改了哪些文件、为什么改、如何在 Unity 里验证
- 文档文件默认使用 UTF-8（建议带 BOM 以兼容 Windows PowerShell），更新后必须复查是否存在乱码

## 验证闭环（每次修改后必须执行）
1. 等待 Unity 脚本编译完成，确认 `Console` 没有新增编译错误
2. 打开目标场景确认引用未丢失
3. 验证相关交互链路是否成立，例如：
   - 移动是否正常
   - 平滑转向 / Snap Turn 是否符合预期
   - 射线悬停、点击、抓取是否正常
   - 手部靠近物体时不应触发交互高亮/Trigger；仅射线命中时触发
   - 射线前方存在非目标 Tag 遮挡时，应能穿透并命中后方目标 Tag 物体
   - 若涉及 laser：确认 `LaserPointerHandler.raycastLayers` 不为 `Nothing`；laser 应能悬停/抓取/Trigger。第 1 天教学不依赖点击旧 guide
   - 若涉及手部碰撞球：手贴近道具按 `GrabGrip` 抓起/松开；贴近物体按 `InteractUI` 触发 TTS。第 1 天不要指望碰撞球/射线点击再触发旧 `TryTriggerGuideIntro`
   - 若涉及手部碰撞球：碰撞球抓食物→松开→手移开（食物离开球）后再按 grab，不应无条件重抓同一食物
   - 加了碰撞球后，laser 自身的悬停/抓取/Trigger 不应被破坏（两套交互可共存）
   - 若涉及零食出生 outline（第 2 天起）：射线指到后黄色出生提示应熄灭；第 1 天教学零食走导演循环 `Shining`
   - 若涉及第一天教学：不到工位不应出键盘/仓鼠/食物；传送进 `StartTriggerBox` 也应立刻出键盘；第 3 次有效 slap 只停键盘引导动画，打满场景 `requireHit`（当前 10）才换阶段且工作进度增加；仓鼠开启时下午只出仓鼠、晚上先吃薯片时仓鼠不应出现，吃完后仓鼠才出来且不能摸只能喂，喂完次日有道具；仓鼠关闭时早班打满直接晚上、只吃一包薯片；喂完后站在床 trigger 互动即可进第 2 天；第 2 天无循环 `Shining`/绿圈/交互锁
   - 若涉及第 1 天晚上薯片：应出现在 `SnacksRespawnPoint`（`Snacks` local `(0,0,0)`），不要刷到世界原点或沙发旁地板；玩家吃完第一包后应松手，第二包出现在出生点而不是跟着手柄；`Container` scale 应还原，不要一直缩在 0.5
   - 若涉及睡觉：早上/下午 trigger 床应播放 `NotTimeToSleep`；第 1 天晚上未喂完（或仓鼠关闭时未吃完）不应进次日；喂完后站在床 trigger 互动应进第 2 天；第 2 天起晚上 trigger 床应正常进入次日
   - `GameManager.playerRayLength` 在运行时调节后，左右手射线可见长度与命中距离应同步变化
   - 抓起零食后按 Trigger 应播放对应零食 TTS；道具按 Trigger 应播放道具 TTS；首次拿起道具仍只触发一次首次拿起 TTS
   - 若涉及 `GameManager.currentWorkProgress`：运行时手动修改该值后，确认 `DataCenter.GameData.PlayerData.workProgress`、主监视器进度条与条右侧百分制同步变化
   - 若涉及 `LowStressVersion`：勾选后 Play，`OutsideWalls` 应隐藏、传送区应可到达、超时后次日仍有零食、`stageAdvanceCallbacks` 绑定方法不触发、一天只有早/下午（下午工作或玩耍后直接到次日早上，天空盒不为夜晚、监视器时间不为 21:00）、不睡觉不灰屏不死、天数到齐只出 WorkStandard 结局；取消勾选后上述高压逻辑应全部恢复
   - 若涉及阶段倒计时：从第 2 天开始确认阶段内倒计时会运行，推进到下一阶段后确认计时器重置
   - 若涉及超时奖惩：确认某天内任意阶段超时后，下一天 `SnackManager.container` 被隐藏；若当天全程未超时，则下一天显示
   - 若涉及 `No snack` 提示：确认第一次超时后的次日会显示场景内 `No snack` 对象并播放其自身音频；后续天数中不应被 `GameManager` 反复触发
   - 若涉及传送区域回调：传送到目标区域时，`onTeleportComplete` 是否触发；传送到其他区域或实际走出区域后，`onPlayerExitArea` 是否触发；若开启离开即锁定，确认该区域后续不可再次传送进入
   - 头显中心视线看向目标时，`onGazeEnter` 是否触发；移开后，`onGazeExit` 是否触发
   - 相关 UI Toggle / Button 是否触发正确逻辑
4. 检查是否误改第三方 SDK 或样例资源
5. 若修改的是输入或场景绑定，至少说明一个可复现的手动验证步骤
6. 若更新了文档，必须重新以正确编码打开并确认全文无乱码、标题和代码块可正常显示

## 常见任务模板
- **改输入逻辑**：先确认使用的是 `SteamVR` 还是 `XRI` 输入链路，再改对应脚本
- **改手部交互（射线/碰撞球）**：先分清是 `LaserPointerHandler`（远距射线）还是 `HandGrabCollider`（近距碰撞球）；两者各自维护抓取/Trigger/guide，碰撞球必须保持独立、不碰 laser 的 `isRayHovering`
- **修 laser 失效**：先确认 `GetEffectiveRaycastLayers()` 兜底是否生效；再查目标 Tag/Layer；不要依赖改磁盘 `.unity` 修 `raycastLayers`
- **改床/睡觉交互**：确认床 trigger 绑定的是 `SleepToNextDayFromInteraction()`；非晚上 TTS 为 `TTS/ItemGet/NotTimeToSleep`；第 1 天晚上还要过 `CanSleep`（已进入 `NightLookAtBed` 即可，不必先注视 2.5s）
- **改第 1 天教学**：先改 `DayOneTutorialDirector`；仓鼠开关走 `enableHamster`；3 次 slap 只关动画；`requireHit` 跟场景值；晚上先吃后出仓鼠、禁摸只喂；TTS 挂 `stepHooks` 或 `PlayTutorialTTS`
- **改中心视野逻辑**：优先修改 `PlayerSteamVRManager` 的统一视线检测参数或 `CenterGazeCallback` 的目标判定，不要为每个目标单独复制一套头显射线检测
- **改 UI 交互**：优先增加 Inspector 可绑定入口，不要把 UI 查找逻辑写死
- **改进度显示/调试入口**：优先复用 `DataCenter.GameData.PlayerData.workProgress` 与 `EventCommon.UPDATE_MONITOR` 链路；百分制对象已在预制体 `workInfo/WorkProgressPercent`，脚本只改文字；若需要手动调试，优先暴露 `GameManager.currentWorkProgress`
- **改低压版本**：只改 `GameManager.LowStressVersion` 旁路，不要删高压代码；零食走 `LowStressVersion || !_hasTimedOutToday`，回调走 `InvokeStageAdvanceCallbacks()` 开头 return，阶段走下午直接换天、不进晚上，结局走 `EndingManager.HandleEndingTextAndGameObjects()` 的 WorkStandard 早退
- **改场景行为**：先确认对象挂载脚本、事件回调、引用对象
- **修 SDK 问题**：先尝试项目层兜底；只有项目层无法解决时再改 SDK
- **修 NaN/Transform 报错**：优先在输入数据入口和赋值出口做合法性保护
- **修文档乱码**：先确认文件真实编码，再决定是否转码或重写；不要把控制台显示问题误判为文件损坏

## 更新规则
- 每次较大功能修改后，回顾本文档是否需要补充新的“架构边界”或“Never 列表”
- 若发现某类错误重复出现，将“错误现象 + 根因 + 预防方式”追加到本文档
- 若新增核心场景、核心管理器或输入链路，更新本文档的“项目结构建议”和“验证闭环”
- 每次更新文档内容后，必须执行一次乱码检查；至少确认文件编码、重新读取结果、中文标题与反引号代码片段均显示正常
- 若终端输出存在乱码，先区分是“终端编码问题”还是“文件编码问题”，确认前不得草率覆盖原文

---
**记住**：你是在维护一个真实使用中的 Unity VR 项目。优先做最小、稳定、可验证的修改；先确认场景和挂载对象，再改代码；优先保持 Inspector 兼容和现有工作流不被破坏。
