# CLAUDE.md - Officer

## 项目概述
这是一个基于 Unity `2022.3.22f1` 的 VR 办公室互动项目，集成了 `SteamVR`、`XR Interaction Toolkit`、`OpenXR` 与 `PICO XR` 相关能力。项目核心目标是实现玩家在办公室场景中的移动、转向、物品交互、数据/UI 展示，以及基于 VR 手柄的交互流程。

## 关键命令（必须准确）
- 打开项目：使用 `Unity 2022.3.22f1` 打开目录 `E:\UnityProject\LowPoly-Project\Officer`
- 打开当前正常作业场景：`Assets/SteamVR/Simple Sample.unity`
- 打开备用主场景：`Assets/Scenes/SteamVer.unity`
- 打开 C# 工程：双击 `Officer.sln`
- 查看包依赖：检查 `Packages/manifest.json`
- 查看 Unity 版本：检查 `ProjectSettings/ProjectVersion.txt`
- 重新导入输入/脚本后验证：回到 Unity Editor 等待编译完成并检查 `Console`
- Android 打包入口：在 Unity Editor 中使用 `File > Build Settings`

## 架构边界（严格遵守）
- 业务逻辑 → `Assets/Scripts/Game/`
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

## 项目结构建议
- `Assets/Scripts/Game/`：玩法、流程、玩家状态、关卡逻辑
- `Assets/Scripts/SteamVRScripts/`：手柄输入、射线交互、抓取、转向、传送回调
- `Assets/Scripts/Data/`：`DataCenter`、`SnackData`、`ItemData` 等数据定义
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
- 当前 `GameManager` 已暴露 `currentWorkProgress` 调试字段，并与 `DataCenter.GameData.PlayerData.workProgress` 和主监视器进度条保持同步
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

## 当前交互链路说明
- 工作进度链路：`GameManager.currentWorkProgress` ↔ `DataCenter.GameData.PlayerData.workProgress` → `EventCommon.UPDATE_MONITOR` → `MainMonitorData.UpdateInfo()` → 主监视器 `Scrollbar`
- 阶段倒计时链路：`GameManager.countDown` → `GameManager.Update()` → `TickStageCountDown()`；仅从第 2 天开始生效，每次推进到下一阶段后重置
- 超时奖惩链路：当天任意阶段超时后，`GameManager` 会记录当日超时状态；跨到下一天时调用 `SnackManager.SetContainerVisible(bool)`，超时则隐藏 `Container`，未超时则显示 `Container`
- 超时提示链路：首次发生“前一天超时”并进入下一天时，`GameManager` 会显示场景内 `No snack` 对象；该对象依赖自身 `AudioSource.playOnAwake` 播放提示，并且只触发一次，后续不再由 `GameManager` 重复 show/hide
- 零食提示链路：`MyInteractableSteamVR` → `EventCommon.PLAYER_SNACK_HINT` → `PlayerSteamVRManager.SetSnackHintVisible(bool)`
- 射线悬停链路：`LaserPointerHandler.RaycastAll`（穿透非目标 Tag）→ `MyInteractableSteamVR.OnRayHoverBegin/End` → Interactable 高亮 / Item UI / Hover 回调
- 射线点击链路：`LaserPointerHandler.HandleFilteredClick()` → `pointerDown/pointerClick/pointerUp`（仅对过滤后的目标触发）
- Trigger 播放链路：`MyInteractableSteamVR` 在“射线悬停中”或“已抓在手上”时监听 `InteractUI` 抬起并触发 `onTriggerDown`
- 提示区域挂点：`Player/Container/SteamVRObjects/VRCamera/FollowHead/HeadCollider`
- `HeadCollider` 下会在运行时创建 `HeadColliderVisual` 子物体，并挂载球体 MeshRenderer 作为提示显示
- 当前提示样式为淡蓝色半透明球体；默认隐藏，玩家拿起零食时显示，放下或吃掉后隐藏
- 头显中心视野链路：`PlayerSteamVRManager` 统一缓存头显中心射线与命中结果 → `CenterGazeCallback` 复用该结果判断是否看向目标 → 触发 `onGazeEnter` / `onGazeExit`
- 当前中心视野检测优先使用 `Player.instance.hmdTransform`，拿不到时回退到 `Camera.main`
- 传送区域回调链路：`TeleportAreaCallback` 监听 `Teleport.Player`；玩家传送到当前区域时触发 `onTeleportComplete`，离开当前区域时触发 `onPlayerExitArea`
- 当前离开区域判定同时覆盖两种情况：传送到其他 `TeleportArea`，或玩家传送到该区域后通过房间尺度实际走出区域边界
- `TeleportAreaCallback` 当前还支持在离开区域时直接调用 `TeleportArea.SetLocked(true)` 锁定自身区域，适合做一次性传送点
- 当前可视提示与正常玩法链路都以 `Assets/SteamVR/Simple Sample.unity` 为准；若要同步到 `SteamVer.unity`，需分别确认 XRI 链路

## 修改策略
- 小改动：优先改单个脚本并保持 Inspector 字段兼容
- 交互改动：先确认目标场景实际挂载的是哪个脚本，再动代码
- UI/Toggle/按钮改动：优先提供可从 Inspector 绑定的方法，如 `public void OnXChanged(bool value)`
- 数据调试改动：若要暴露可手动修改的运行时数据，优先在 `GameManager` 或对应管理器提供 Inspector 可见字段，并保持与 `DataCenter` 和现有 UI 刷新链路同步
- VR 输入改动：明确区分左手/右手输入源、平滑移动、Snap Turn、传送、抓取
- 视线/注视改动：优先复用 `PlayerSteamVRManager` 中统一的中心视线结果，不要在每个目标组件里重复发同样的 `Raycast`
- 射线可交互改动：优先在 `LaserPointerHandler` 维护“命中筛选 + 可视长度 + 点击派发”统一逻辑，避免在多个目标脚本重复判定
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
   - `GameManager.playerRayLength` 在运行时调节后，左右手射线可见长度与命中距离应同步变化
   - 抓起零食后按 Trigger 应播放对应零食 TTS；道具按 Trigger 应播放道具 TTS；首次拿起道具仍只触发一次首次拿起 TTS
   - 若涉及 `GameManager.currentWorkProgress`：运行时手动修改该值后，确认 `DataCenter.GameData.PlayerData.workProgress` 与主监视器进度条同步变化
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
- **改中心视野逻辑**：优先修改 `PlayerSteamVRManager` 的统一视线检测参数或 `CenterGazeCallback` 的目标判定，不要为每个目标单独复制一套头显射线检测
- **改 UI 交互**：优先增加 Inspector 可绑定入口，不要把 UI 查找逻辑写死
- **改进度显示/调试入口**：优先复用 `DataCenter.GameData.PlayerData.workProgress` 与 `EventCommon.UPDATE_MONITOR` 链路；若需要手动调试，优先暴露 `GameManager.currentWorkProgress`
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
