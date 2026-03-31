# Black Hole Singularity: DOTS-based Accretion Disk (v0.3.0)

本项目基于 **Unity 6 (URP)** 与 **Entities (DOTS)** 架构，实现了一个高性能、高动态密度的黑洞吸积盘模拟系统。

## 🚀 核心亮点 (Core Highlights)

* **140,000+ 实体渲染**：通过 `IJobEntity` 与 `Graphics.RenderMeshInstanced` 实现极致的并行计算与渲染性能，在移动端及 PC（测试环境：Lenovo Legion Y9000P）均可保持 60+ FPS。
* **非对称视觉对冲 (Matrix Offset Strategy)**：针对 Unity 旋转矩阵与大缩放冲突导致的渲染闪烁，自创了“数据脱壳”方案——在 C# 端将速度数据压缩进 Scale 通道，并在 Shader 顶点阶段通过矩阵逆向抵消还原真实几何形状。
* **天体物理密度模拟**：摒弃了均匀随机分布，引入了 **指数级分布算法 (Exponential Distribution)**。通过 $f(x) = x^n$ 公式重新排布粒子重生点，模拟了吸积盘内圈极密高亮、外圈空灵稀疏的真实黑洞视觉层级。
* **动态动感拉伸**：粒子长度根据实时速度动态线性映射（Remap），实现慢速星尘（点状）与高速光束（线状）的并存，增强了引力拉伸的视觉沉浸感。

---

## 🛠️ 技术方案 (Technical Solutions)

### 1. 渲染抗闪烁 (Anti-Flicker)
为了解决 DOTS 渲染中粒子重生瞬间产生的“时空拉伸”闪烁，在 `CalculateSingularityGravityJob` 中实现了位置与缩放数据的**强行帧同步**，确保每一帧数据的视觉一致性。

### 2. HDR 变色逻辑
利用 Shader Graph 实现了速度到 HDR 颜色的动态映射：
* **深红 (Slow)**：模拟吸积盘外围物质的低温红移。
* **冰蓝 (Fast)**：模拟 ISCO（最内层稳定轨道）物质被加速至近光速产生的高能蓝移。

### 3. 后台静默运行
支持打包后后台运行（Background Running），通过锁定全局帧率与可见性设置，实现在非活跃窗口状态下依然保持 14 万颗粒子的稳定模拟。

---

## 🏗️ 开发环境
* **Engine**: Unity 6 (6000.0.x)
* **Pipeline**: Universal Render Pipeline (URP)
* **Packages**: Entities, Mathematics, Burst Compiler, Collections

---

## 📅 未来展望 (Roadmap)
* [ ] 接入物理碰撞检测（基于 Spatial Partitioning）。
* [ ] 增加运行时 GUI 参数调整面板（基于 UIToolkit）。
* [ ] 实现广义相对论下的引力透镜光行差模拟。

---

> *"The event horizon is not just a limit, but a convergence of data and beauty."*

---
