using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Vino.Fragment.Components;
using Unity.Collections;
using Unity.Rendering;

namespace Vino.Global.Gravity.Jobs
{
    [BurstCompile]
    public partial struct CalculateSingularityGravityJob : IJobEntity
    {
        public float blackHoleRadiusSq;
        public float3 blackHolePos;

        public float GM;
        public float particleDrag;
        public float epsilonSq;
        public float deltaTme;


        public float eventHorizonSq;//消失的距离的平方
        public float spawnRadius;//刷新的半径
        public uint randomSeed;
        public float initialSpeed;//初始速度

        public float ringInnerRatio;
        public float ringOuterRatio;
        public int ringPercentage;

        //[BurstCompile]
        //public void Execute([EntityIndexInQuery] int entityIndex, RefRW<FragmentData> fragment, ref LocalTransform t)
        //{
        //    var r = blackHolePos - t.Position;
        //    var distSq = math.lengthsq(r);
        //    if (distSq < eventHorizonSq)
        //    {
        //        var random = Unity.Mathematics.Random.CreateFromIndex((uint)entityIndex + randomSeed + 1);

        //        float3 randomDir = math.normalize(random.NextFloat3(-1f, 1f));
        //        t.Position = blackHolePos + randomDir * (spawnRadius + random.NextFloat(-10.0f, 10.0f));

        //        float3 up = new float3(0, 1, 0);
        //        float3 tangent = math.normalize(math.cross(up, randomDir));
        //        fragment.ValueRW.Velocity = tangent * initialSpeed;

        //        return;
        //    }



        //    var a = GM * math.pow(distSq + epsilonSq, -1.5f) * r;

        //    float3 currentVel = fragment.ValueRO.Velocity;
        //    currentVel += a * deltaTme;
        //    currentVel *= math.exp(-particleDrag * deltaTme);

        //    float eventHorizonRadius = math.sqrt(eventHorizonSq);

        //    float safeMaxSpeed = (eventHorizonRadius * 0.8f) / deltaTme;

        //    if (math.lengthsq(currentVel) > safeMaxSpeed * safeMaxSpeed)
        //    {
        //        currentVel = math.normalize(currentVel) * safeMaxSpeed;
        //    }

        //    fragment.ValueRW.Velocity = currentVel;

        //    t.Position += fragment.ValueRW.Velocity;
        //}

        //以下是ai结合
        //[BurstCompile]
        //public void Execute([EntityIndexInQuery] int entityIndex, RefRW<FragmentData> fragment, ref LocalTransform t)
        //{
        //    var r = blackHolePos - t.Position;
        //    var distSq = math.lengthsq(r);

        //    // 获取事件视界的真实半径 (内径)，作为动态限速的基准
        //    float eventHorizonRadius = math.sqrt(eventHorizonSq);

        //    // ================== 分层重生逻辑 (核心修改点) ==================
        //    if (distSq < eventHorizonSq)
        //    {
        //        var random = Unity.Mathematics.Random.CreateFromIndex((uint)entityIndex + randomSeed + 1);

        //        // 🌟 核心作弊魔法：一个介于 0 到 1 的随机数，决定粒子命运
        //        float populationType = random.NextFloat();

        //        float3 newPosition;
        //        float3 newVelocity;

        //        // --- 秩序层：75% 的粒子生成为扁平吸积盘 (成环) ---
        //        if (populationType < 0.75f)
        //        {
        //            // 在 XZ 水平面生成角度
        //            float angle = random.NextFloat(0f, math.PI * 2f);

        //            // 给圆盘增加一点微小的厚度 (Y 轴起伏)
        //            float yThickness = random.NextFloat(-0.2f, 0.2f);

        //            // 算出水平面的方向向量 (成盘的方向)
        //            float3 diskDir = math.normalize(new float3(math.cos(angle), yThickness * 0.01f, math.sin(angle)));

        //            // 魔法分布：让盘子的密度更集中在靠近黑洞的区域 (不只是统一的 spawnRadius)
        //            // 距离 = (内径 + 一点缓冲区) 到 (最大刷新半径的 70%)
        //            float randomDist = random.NextFloat(eventHorizonRadius * 1.8f, spawnRadius * 0.7f);
        //            newPosition = blackHolePos + diskDir * randomDist;

        //            // 秩序速度：完美的逆时针环绕切线 (围绕全局 Y 轴)
        //            float3 up = new float3(0, 1, 0);
        //            float3 tangent = math.normalize(math.cross(up, diskDir));

        //            // 赋予高速，确保它能稳定环绕
        //            float speedNoise = random.NextFloat(0.9f, 1.1f);
        //            newVelocity = tangent * initialSpeed * speedNoise;
        //        }
        //        // --- 混沌层：25% 的粒子生成为从四面八方飞来的星辰云 ---
        //        else
        //        {
        //            // 传统的“球向随机”方向 (四面八方)
        //            float3 randomDir = math.normalize(random.NextFloat3(-1f, 1f));

        //            // 它们应该在更遥远的区域刷新，制造出缓缓坠落的感觉
        //            // 距离 = (最大刷新半径的 50%) 到 (最大刷新半径)
        //            float randomDist = random.NextFloat(spawnRadius * 0.5f, spawnRadius);
        //            newPosition = blackHolePos + randomDir * randomDist;

        //            // 混沌速度：速度不能太快，且不能完美切线，要让它们有种不稳定的、缓缓汇聚的感觉
        //            // 这里我们计算一个“带有随机偏移的切线”速度
        //            float3 arbitraryUp = (math.abs(randomDir.y) < 0.9f) ? new float3(0, 1, 0) : new float3(1, 0, 0);
        //            float3 semiTangent = math.normalize(math.cross(arbitraryUp, randomDir));

        //            // 速度大幅降低，有些快有些慢，方向有些混乱
        //            float speedNoise = random.NextFloat(0.3f, 2.0f);
        //            newVelocity = semiTangent * initialSpeed * speedNoise;
        //        }

        //        // 应用最终的位置和速度
        //        t.Position = newPosition;
        //        fragment.ValueRW.Velocity = newVelocity;
        //        return;
        //    }

        //    // ================== 引力、阻力与限速 (保持之前的完美代码) ==================
        //    var a = GM * math.pow(distSq + epsilonSq, -1.5f) * r;

        //    float3 currentVel = fragment.ValueRO.Velocity;
        //    currentVel += a * deltaTme;

        //    // 应用粒子阻力 (核心魔法，确保数值稳定且形成吸积盘螺旋坠落感)
        //    currentVel *= math.exp(-particleDrag * deltaTme);

        //    // 🌟 核心防穿透逻辑：基于黑洞半径的动态最高限速
        //    // 距离 = 速度 * 时间  =>  最大速度 = 最大距离 / 时间
        //    // 保证粒子一帧内最多只能飞过黑洞半径的 80%，它就永远跑不掉判定。
        //    float safeMaxSpeed = (eventHorizonRadius * 0.8f) / deltaTme;

        //    // 超速拦截 (使用 lengthsq 避免多余的开方运算，压榨性能)
        //    if (math.lengthsq(currentVel) > safeMaxSpeed * safeMaxSpeed)
        //    {
        //        currentVel = math.normalize(currentVel) * safeMaxSpeed;
        //    }

        //    // 最后写回速度，并真实移动粒子位置
        //    fragment.ValueRW.Velocity = currentVel;
        //    t.Position += currentVel * deltaTme;
        //}
        [BurstCompile]
        public void Execute([EntityIndexInQuery] int entityIndex, RefRW<FragmentData> fragment, ref LocalTransform t, RefRW<URPMaterialPropertyBaseColor> color)
        {
            var r = blackHolePos - t.Position;
            var distSq = math.lengthsq(r);
            float eventHorizonRadius = math.sqrt(eventHorizonSq);

            // 🌟 核心魔法：用实体的唯一 ID 算出一个哈希值，决定它这辈子的“阶级”
            // 保证同一个粒子在飞行过程中，身份永远不会变
            uint hash = math.hash((uint2)(uint)entityIndex);
            bool isRingParticle = (hash % 100) < ringPercentage; // 75% 是环带，25% 是混沌星辰

            // ================== 分层重生逻辑 ==================
            if (distSq < eventHorizonSq)
            {
                var random = Unity.Mathematics.Random.CreateFromIndex((uint)entityIndex + randomSeed + 1);
                float3 newPosition;
                float3 newVelocity;

                if (isRingParticle)
                {
                    // --- 秩序层：形成带有巨大中空地带的“环” ---
                    float angle = random.NextFloat(0f, math.PI * 2f);
                    float yThickness = random.NextFloat(-0.1f, 0.1f);
                    float3 diskDir = math.normalize(new float3(math.cos(angle), yThickness, math.sin(angle)));

                    // 🌟 制造“环”的关键：把内径推远！留出 ISCO 死亡空洞
                    // 假设黑洞半径是 2，那么环至少从 10 开始刷新
                    float minRingRadius = eventHorizonRadius * ringInnerRatio;
                    float randomDist = random.NextFloat(minRingRadius, spawnRadius * ringOuterRatio);
                    newPosition = blackHolePos + diskDir * randomDist;

                    float3 up = new float3(0, 1, 0);
                    float3 tangent = math.normalize(math.cross(up, diskDir));

                    // 🌟 解决笔直掉落：给予极其狂暴的初速度
                    float speedNoise = random.NextFloat(1.2f, 1.8f);
                    newVelocity = tangent * initialSpeed * speedNoise;
                }
                else
                {
                    // --- 混沌层：四面八方缓缓坠落的星辰 ---
                    float3 randomDir = math.normalize(random.NextFloat3(-1f, 1f));

                    // 混沌粒子在更远的外围刷新
                    float randomDist = random.NextFloat(spawnRadius * 0.6f, spawnRadius * 1.2f);
                    newPosition = blackHolePos + randomDir * randomDist;

                    float3 arbitraryUp = (math.abs(randomDir.y) < 0.9f) ? new float3(0, 1, 0) : new float3(1, 0, 0);
                    float3 semiTangent = math.normalize(math.cross(arbitraryUp, randomDir));

                    // 混沌粒子速度极慢，注定会被吸进去
                    float speedNoise = random.NextFloat(0.2f, 0.6f);
                    newVelocity = semiTangent * initialSpeed * speedNoise;
                }

                t.Position = newPosition;
                fragment.ValueRW.Velocity = newVelocity;
                return;
            }

            // ================== 引力、阻力与限速 ==================
            var a = GM * math.pow(distSq + epsilonSq, -1.5f) * r;
            float3 currentVel = fragment.ValueRO.Velocity;
            currentVel += a * deltaTme;

            // 🌟 阶级差异化阻力：
            // 环带粒子享受“真空待遇”（阻力极小），可以维持成百上千圈的环绕！
            // 混沌粒子身处“泥沼”（阻力极大），转一点点就会笔直坠落。
            float appliedDrag = isRingParticle ? (particleDrag * 0.02f) : (particleDrag * 2.0f);
            currentVel *= math.exp(-appliedDrag * deltaTme);

            float safeMaxSpeed = (eventHorizonRadius * 0.8f) / deltaTme;
            if (math.lengthsq(currentVel) > safeMaxSpeed * safeMaxSpeed)
            {
                currentVel = math.normalize(currentVel) * safeMaxSpeed;
            }

            fragment.ValueRW.Velocity = currentVel;
            t.Position += currentVel * deltaTme;

            // ================== 🌟 视觉特效核心逻辑 ==================

            float currentSpeed = math.length(currentVel);

            // 🌟 直接从当前粒子 (fragment) 身上读取它的专属配置！
            float colorT = math.smoothstep(
                fragment.ValueRO.MinColorSpeed,
                fragment.ValueRO.MaxColorSpeed,
                currentSpeed
            );

            // 同样从粒子身上读取它的专属冷热颜色
            color.ValueRW.Value = math.lerp(
                fragment.ValueRO.SlowColor,
                fragment.ValueRO.FastColor,
                colorT
            );

            //color.ValueRW.Value = new float4(10.0f, 0.0f, 0.0f, 1.0f);
            // 转向逻辑保持不变
            if (currentSpeed > 0.1f)
            {
                t.Rotation = quaternion.LookRotationSafe(currentVel, new float3(0, 1, 0));
            }
        }
    }
}
