# CLAUDE.md - Officer

## 项目概述
这是一个基于 Unity `2022.3.22f1` 的 VR 办公室互动项目，集成了 `SteamVR`、`XR Interaction Toolkit`、`OpenXR` 与 `PICO XR` 相关能力。项目核心目标是实现玩家在办公室场景中的移动、转向、物品交互、数据/UI 展示，以及基于 VR 手柄的交互流程。

## 关键命令（必须准确）
- 打开项目：使用 `Unity 2022.3.22f1` 打开目录 `E:\UnityProject\LowPoly-Project\Officer`
- 打开主场景：`Assets/Scenes/SteamVer.unity`
- 打开 SteamVR 示例场景：`Assets/SteamVR/Simple Sample.unity`
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
- 场景资源与流程验证 → `Assets/Scenes/SteamVer.unity`、`Assets/SteamVR/Simple Sample.unity`
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

## 项目结构建议
- `Assets/Scripts/Game/`：玩法、流程、玩家状态、关卡逻辑
- `Assets/Scripts/SteamVRScripts/`：手柄输入、射线交互、抓取、转向、传送回调
- `Assets/Scripts/Data/`：`DataCenter`、`SnackData`、`ItemData` 等数据定义
- `Assets/Scripts/Event/`：项目内事件广播与解耦
- `Assets/Scripts/Tools/`：单例、时间管理、调试辅助、通用计算
- `Assets/Scenes/SteamVer.unity`：项目主场景
- `Assets/SteamVR/Simple Sample.unity`：VR 输入/交互调试样例场景

## 当前项目特征
- Unity 版本固定为 `2022.3.22f1`
- 同时存在 `SteamVR`、`OpenXR`、`XR Interaction Toolkit`、`PICO` 相关依赖
- 主业务脚本集中在 `Assets/Scripts/`
- 当前项目已有 VR 输入控制脚本，如 `TestInput.cs`、`LaserPointerHandler.cs`、`PlayerSteamVRManager.cs`
- 当前项目同时保留了官方示例和自定义实现，修改前需分清“示例代码”与“项目实际使用代码”

## 修改策略
- 小改动：优先改单个脚本并保持 Inspector 字段兼容
- 交互改动：先确认目标场景实际挂载的是哪个脚本，再动代码
- UI/Toggle/按钮改动：优先提供可从 Inspector 绑定的方法，如 `public void OnXChanged(bool value)`
- VR 输入改动：明确区分左手/右手输入源、平滑移动、Snap Turn、传送、抓取
- 场景相关问题：优先查组件引用、事件回调、Layer/Collider、Toggle 状态，再改代码
- 第三方代码必须改时：尽量做最小补丁，并在本文件记录原因

## Compact Instructions（上下文压缩时必须保留）
- 这是 Unity VR 项目，主逻辑在 `Assets/Scripts/`
- 优先改项目脚本，不优先改 `Assets/SteamVR/` 或 `Assets/Samples/`
- 变更前先确认目标场景和真实挂载脚本
- VR 改动必须区分输入源、场景引用、Toggle/Inspector 绑定
- 提交结果时说明：改了哪些文件、为什么改、如何在 Unity 里验证

## 验证闭环（每次修改后必须执行）
1. 等待 Unity 脚本编译完成，确认 `Console` 没有新增编译错误
2. 打开目标场景确认引用未丢失
3. 验证相关交互链路是否成立，例如：
   - 移动是否正常
   - 平滑转向 / Snap Turn 是否符合预期
   - 射线悬停、点击、抓取是否正常
   - 相关 UI Toggle / Button 是否触发正确逻辑
4. 检查是否误改第三方 SDK 或样例资源
5. 若修改的是输入或场景绑定，至少说明一个可复现的手动验证步骤

## 常见任务模板
- **改输入逻辑**：先确认使用的是 `SteamVR` 还是 `XRI` 输入链路，再改对应脚本
- **改 UI 交互**：优先增加 Inspector 可绑定入口，不要把 UI 查找逻辑写死
- **改场景行为**：先确认对象挂载脚本、事件回调、引用对象
- **修 SDK 问题**：先尝试项目层兜底；只有项目层无法解决时再改 SDK
- **修 NaN/Transform 报错**：优先在输入数据入口和赋值出口做合法性保护

## 更新规则
- 每次较大功能修改后，回顾本文件是否需要补充新的“架构边界”或“Never 列表”
- 若发现某类错误重复出现，将“错误现象 + 根因 + 预防方式”追加到本文件
- 若新增核心场景、核心管理器或输入链路，更新本文件的“项目结构建议”和“验证闭环”

---
**记住**：你是在维护一个真实使用中的 Unity VR 项目。优先做最小、稳定、可验证的修改；先确认场景和挂载对象，再改代码；优先保持 Inspector 兼容和现有工作流不被破坏。
