﻿//
// JAction.cs
//
// Author:
//       JasonXuDeveloper（傑） <jasonxudeveloper@gmail.com>
//
// Copyright (c) 2020 JEngine
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace JEngine.Core
{
    public class JAction : IDisposable
    {
        /// <summary>
        /// 复用池
        /// </summary>
        private static readonly Queue<JActionItem> Items = new Queue<JActionItem>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public JAction()
        {
            //池子里有就从池子里取
            _item = Items.Count > 0 ? Items.Dequeue() : new JActionItem();
        }

        /// <summary>
        /// 当前任务对象
        /// </summary>
        private readonly JActionItem _item;

        /// <summary>
        /// 是否已经释放
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// 检测是否被释放
        /// </summary>
        private void DisposeCheck()
        {
            if (_disposed)
            {
                //为了定位到正确的堆栈
                try
                {
                    throw new ObjectDisposedException("This JAction has been disposed, please create a new one");
                }
                catch (Exception e)
                {
                    Log.PrintError($"JAction错误: {e.Message}, {e.Data["StackTrace"]}");
                    throw e;
                }
            }
        }

        /// <summary>
        /// 延迟time秒后执行后续任务
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public JAction Delay(float time)
        {
            DisposeCheck();
            _item.Delay(time);
            return this;
        }

        /// <summary>
        /// 延迟frame帧后执行后续任务
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public JAction DelayFrame(int frame)
        {
            DisposeCheck();
            _item.DelayFrame(frame);
            return this;
        }

        /// <summary>
        /// 等待到condition = true, 每frequency秒检测一次，当timeout>-1时，如果超过timeout秒condition还是false就超时了
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="frequency"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public JAction Until(Func<bool> condition, float frequency = 0.5f, float timeout = -1)
        {
            DisposeCheck();
            _item.Until(condition, frequency, timeout);
            return this;
        }

        /// <summary>
        /// 当condition是true时每frequency秒执行一次action, timeout大于-1时会在执行timeout秒后超时
        /// </summary>
        /// <param name="action"></param>
        /// <param name="condition"></param>
        /// <param name="frequency"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public JAction RepeatWhen(Action action, Func<bool> condition, float frequency = 0.5f, float timeout = -1)
        {
            DisposeCheck();
            _item.RepeatWhen(action, condition, frequency, timeout);
            return this;
        }

        /// <summary>
        /// 当condition是false时每frequency秒执行一次action, timeout大于-1时会在执行timeout秒后超时
        /// </summary>
        /// <param name="action"></param>
        /// <param name="condition"></param>
        /// <param name="frequency"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public JAction RepeatUntil(Action action, Func<bool> condition, float frequency = 0.5f, float timeout = -1)
        {
            DisposeCheck();
            _item.RepeatUntil(action, condition, frequency, timeout);
            return this;
        }

        /// <summary>
        /// 重复执行action counts次，每次间隔duration秒
        /// </summary>
        /// <param name="action"></param>
        /// <param name="counts"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public JAction Repeat(Action action, int counts, float duration = 0)
        {
            DisposeCheck();
            _item.Repeat(action, counts, duration);
            return this;
        }

        /// <summary>
        /// 执行Action（会被同步执行）
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public JAction Do(Action action)
        {
            DisposeCheck();
            _item.Do(action);
            return this;
        }

        /// <summary>
        /// 执行Task（可异步）
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public JAction Do(Task action)
        {
            DisposeCheck();
            _item.Do(action);
            return this;
        }

        /// <summary>
        /// 运行被多次Execute
        /// </summary>
        /// <returns></returns>
        public JAction Parallel()
        {
            DisposeCheck();
            _item.Parallel();
            return this;
        }

        /// <summary>
        /// 被取消时回调
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public JAction OnCancel(Action action)
        {
            DisposeCheck();
            _item.OnCancel(action);
            return this;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="onMainThread"></param>
        /// <returns></returns>
        public JAction Execute(bool onMainThread = false)
        {
            DisposeCheck();
            Do(onMainThread).Coroutine();
            return this;
        }

        /// <summary>
        /// 异步并行执行，可回调
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="onMainThread"></param>
        /// <returns></returns>
        public JAction ExecuteAsyncParallel(Action callback = null, bool onMainThread = false)
        {
            DisposeCheck();
            _ExecuteAsync(callback, onMainThread).Coroutine();
            return this;
        }

        /// <summary>
        /// 异步执行，或ExecuteAsync().Coroutine()同步不阻塞执行
        /// </summary>
        /// <param name="onMainThread"></param>
        /// <returns></returns>
        public async ET.ETTask<JAction> ExecuteAsync(bool onMainThread = false)
        {
            DisposeCheck();
            return await Do(onMainThread);
        }

        /// <summary>
        /// 异步执行，或ExecuteAsync().Coroutine()同步不阻塞执行，可带回调
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="onMainThread"></param>
        /// <returns></returns>
        private async ET.ETTask<JAction> _ExecuteAsync(Action callback, bool onMainThread)
        {
            DisposeCheck();
            await Do(onMainThread);
            callback?.Invoke();
            return this;
        }

        /// <summary>
        /// 取消JAction
        /// </summary>
        /// <returns></returns>
        public JAction Cancel()
        {
            DisposeCheck();
            _item.cancel = true;
            _item.cancellationTokenSource.Cancel();
            _item.onCancel?.Invoke();
            return this;
        }

        /// <summary>
        /// 重置JAction
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        public JAction Reset(bool force = true)
        {
            DisposeCheck();
            if (force)
            {
                Cancel();
                _reset();
            }
            else
            {
                if (_item.executing)
                {
                    Log.PrintError("JAction is currently executing, if you want to force reset, call Reset(true)");
                }
                else
                {
                    Cancel();
                    _reset();
                }
            }
            return this;
        }

        /// <summary>
        /// 内部重置
        /// </summary>
        private void _reset()
        {
            _item.executing = false;
            _item.parallel = false;
            _item.cancel = false;
            _item.toDo.Clear();
            _item.onCancel = null;
            _item.delays.Clear();
            _item.waits.Clear();
            _item.frequency.Clear();
            _item.timeout.Clear();
            _item.whens.Clear();
            _item.whenCauses.Clear();
            _item.whenFrequency.Clear();
            _item.whenTimeout.Clear();
            _item.cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// 内部执行
        /// </summary>
        /// <param name="onMainThread"></param>
        /// <returns></returns>
        private async ET.ETTask<JAction> Do(bool onMainThread)
        {
            if (_item.executing && !_item.parallel)
            {
                Log.PrintError("JAction is currently executing, if you want to execute JAction multiple times at the same time, call Parallel() before calling Execute()");
                return this;
            }
            if (!_item.parallel)
            {
                _item.executing = true;
            }
            _item.cancel = false;
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            Loom.QueueOnMainThread(async () =>
            {
                for (int i = 0; i < _item.toDo.Count; i++)
                {
                    int index = i;

                    if (_item.cancel || !Application.isPlaying)
                    {
                        tcs.SetResult(false);
                        return;
                    }

                    //Delay
                    if (_item.delays.ContainsKey(index))
                    {
                        await Task.Delay((int)(_item.delays[index] * 1000));
                        continue;
                    }

                    //DelayFrames
                    if (_item.delayFrames.ContainsKey(index))
                    {
                        //计算1帧时间(ms)
                        var durationPerFrame = 1000 / (int)(Application.targetFrameRate <= 0 ? GameStats.FPS : Application.targetFrameRate);
                        var duration = durationPerFrame * _item.delayFrames[index];
                        await Task.Delay(duration);
                        continue;
                    }


                    //Wait Until
                    if (_item.waits.ContainsKey(index))
                    {
                        float time = 0;
                        while (!_item.waits[index]() && Application.isPlaying)
                        {
                            if (_item.cancel)
                            {
                                tcs.SetResult(false);
                                return;
                            }

                            if (_item.timeout[index] > 0 && time >= _item.timeout[index])
                            {
                                throw new TimeoutException();
                            }

                            await Task.Delay((int)(_item.frequency[index] * 1000));
                            time += _item.frequency[index];
                        }
                        continue;
                    }

                    //Repeat When
                    if (_item.whens.ContainsKey(index))
                    {
                        float time = 0;
                        Func<bool> condition = _item.whenCauses[index];
                        float frequency = _item.whenFrequency[index];
                        float timeout = _item.whenTimeout[index];
                        Action action = _item.whens[index];
                        while (condition() && Application.isPlaying)
                        {
                            if (_item.cancel)
                            {
                                tcs.SetResult(false);
                                return;
                            }

                            if (timeout > 0 && time >= timeout)
                            {
                                throw new TimeoutException();
                            }

                            if (onMainThread)
                            {
                                await Task.Run(() =>
                                {
                                    Loom.QueueOnMainThread(p =>
                                    {
                                        Execute(action);
                                    }, null);
                                }, _item.cancellationTokenSource.Token);
                            }
                            else
                            {
                                await Task.Run(action, _item.cancellationTokenSource.Token);
                            }

                            await Task.Delay((int)(frequency * 1000));
                            time += frequency;
                        }
                        continue;
                    }

                    //DO
                    if (!onMainThread)
                    {
                        await Task.Run(() =>
                        {
                            Execute(_item.toDo[index]);
                        }, _item.cancellationTokenSource.Token);
                    }
                    else
                    {
                        Execute(_item.toDo[index]);
                    }
                }
                tcs.SetResult(true);
            });
            await tcs.Task;
            _item.executing = false;
            return this;
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="act"></param>
        private void Execute(Action act)
        {
            try
            {
                act?.Invoke();
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    e = e.InnerException;
                }
                Log.PrintError($"JAction错误: {e.Message}, {e.Data["StackTrace"]}，已跳过");
            }
        }

        /// <summary>
        /// 折构参数，ILRuntime下无效
        /// </summary>
        ~JAction()
        {
            Dispose(false);
        }

        /// <summary>
        /// 释放JAction接口，释放后会把JAction任务队列加入对象池
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// 保护级别的释放接口
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            _disposed = true;
            _reset();
            Items.Enqueue(_item);
            GC.Collect();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// JAction任务队列
        /// </summary>
        private class JActionItem
        {
            public bool executing;
            public bool parallel;
            public bool cancel;

            public readonly List<Action> toDo = new List<Action>();
            public Action onCancel = () => { };

            public readonly Dictionary<int, float> delays = new Dictionary<int, float>();
            public readonly Dictionary<int, int> delayFrames = new Dictionary<int, int>();

            public readonly Dictionary<int, Func<bool>> waits = new Dictionary<int, Func<bool>>();
            public readonly Dictionary<int, float> frequency = new Dictionary<int, float>();
            public readonly Dictionary<int, float> timeout = new Dictionary<int, float>();

            public readonly Dictionary<int, Action> whens = new Dictionary<int, Action>();
            public readonly Dictionary<int, Func<bool>> whenCauses = new Dictionary<int, Func<bool>>();
            public readonly Dictionary<int, float> whenFrequency = new Dictionary<int, float>();
            public readonly Dictionary<int, float> whenTimeout = new Dictionary<int, float>();

            public CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();


            public void Delay(float time)
            {
                if (time > 0)
                {
                    delays.Add(toDo.Count, time);
                    toDo.Add(null);
                }
            }

            public void DelayFrame(int frame)
            {
                if (frame > 0)
                {
                    delayFrames.Add(toDo.Count, frame);
                    toDo.Add(null);
                }
            }

            public void Until(Func<bool> condition, float frequency = 0.5f, float timeout = -1)
            {
                waits.Add(toDo.Count, condition);
                this.frequency.Add(toDo.Count, frequency);
                this.timeout.Add(toDo.Count, timeout);
                toDo.Add(null);
            }

            public void RepeatWhen(Action action, Func<bool> condition, float frequency = 0.5f, float timeout = -1)
            {
                whens.Add(toDo.Count, action);
                whenCauses.Add(toDo.Count, condition);
                whenFrequency.Add(toDo.Count, frequency);
                whenTimeout.Add(toDo.Count, timeout);
                toDo.Add(null);
            }

            public void RepeatUntil(Action action, Func<bool> condition, float frequency = 0.5f, float timeout = -1)
            {
                var temp = condition;
                condition = () => !temp();
                RepeatWhen(action, condition, frequency, timeout);
            }

            public void Repeat(Action action, int counts, float duration = 0)
            {
                for (int i = 0; i < counts; i++)
                {
                    Do(action);
                    Delay(duration);
                }
            }

            public void Do(Action action)
            {
                toDo.Add(() =>
                {
                    action();
                });
            }

            public void Do(Task action)
            {
                toDo.Add(async () =>
                {
                    await action;
                });
            }

            public void Parallel()
            {
                parallel = true;
            }

            public void OnCancel(Action action)
            {
                onCancel = () =>
                {
                    action();
                };
            }
        }
    }
}