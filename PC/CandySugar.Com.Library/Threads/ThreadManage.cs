using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CandySugar.Com.Library.Threads
{
    public class ThreadManage
    {
        private static ThreadManage _Instance;
        public static ThreadManage Instance
        {
            get {
                if (_Instance != null) return _Instance;
                else
                {
                    _Instance = new ThreadManage();
                    return _Instance;
                }
            }
        }

        public int RestartInterval { get; set; } = 2000;
        ConcurrentDictionary<string, TaskModel> Threads = new ConcurrentDictionary<string, TaskModel>();
        ConcurrentDictionary<string, Action> Actions = new ConcurrentDictionary<string, Action>();

        /// <summary>
        /// 停止指定任务
        /// </summary>
        /// <param name="key">任务名</param>
        /// <param name="ExitCallback">任务结束的回调</param>
        public void StopTask(string key, Action ExitCallback = null)
        {
            if (Threads.ContainsKey(key))
            {
                Actions.TryAdd(key, ExitCallback);
                Threads[key].Cts?.Cancel();
            }
        }
        /// <summary>
        /// 长任务，带 while true 的循环
        /// </summary>
        /// <param name="action"></param>
        /// <param name="key"></param>
        public void StartLong(Action action, string key, bool IsRestart = false, Action RunComplete = null)
        {
            if (!Threads.ContainsKey(key))
            {
                Threads.TryAdd(key, new TaskModel());
                Threads[key].RunTask = action;
                Threads[key].ThreadTask = Task.Factory.StartNew(new Action(() =>
                {
                    Thread.CurrentThread.Name = key;
                    while (!Threads[key].Cts.IsCancellationRequested)
                    {
                        if (IsRestart)
                        {
                            try
                            {
                                Threads[key].RunTask?.Invoke();
                            }
                            catch (Exception)
                            {
                                if (IsRestart) Thread.Sleep(RestartInterval);
                            }
                        }
                        else Threads[key].RunTask?.Invoke();
                    }
                }), Threads[key].Cts.Token).ContinueWith(new Action<Task, object?>((t, o) =>
                {
                    ThreadStatus(t, o.ToString());
                    if (RunComplete != null) RunComplete();
                }), key);
            }
        }


        /// <summary>
        /// 不带 while true 的循环任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="key"></param>
        public void Start(Action action, string key, bool isRestart = false)
        {
            if (!Threads.ContainsKey(key))
            {
                Threads.TryAdd(key, new TaskModel());
                Threads[key].RunTask = action;
                Threads[key].ThreadTask = Task.Factory.StartNew(new Action(() =>
                {
                    Thread.CurrentThread.Name = key;
                    while (true)
                    {
                        try
                        {
                            Threads[key].RunTask?.Invoke();
                            break;
                        }
                        catch (Exception)
                        {
                            if (isRestart) Thread.Sleep(RestartInterval);
                            if (!isRestart) break;
                        }
                    }
                }), Threads[key].Cts.Token).ContinueWith(new Action<Task, object?>((t, o) =>
                {
                    ThreadStatus(t, o.ToString());
                }), key);
            }
        }

        private void ThreadStatus(Task task, string key)
        {
            bool IsRemove = false;
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
                    IsRemove = true;
                    break;
                case TaskStatus.Faulted:
                    IsRemove = true;
                    break;
                case TaskStatus.Canceled:
                    IsRemove = true;
                    break;
                default:
                    break;
            }

            if (IsRemove)
            {
                if (Threads.ContainsKey(key)) Threads.TryRemove(key, out _);
                if (Actions.ContainsKey(key))
                {
                    Actions[key]?.Invoke();
                    Actions.TryRemove(key, out _);
                }

            }
        }

        /// <summary>
        /// 释放所有线程资源
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < Threads.Count; i++)
            {
                Threads.ElementAt(i).Value.Cts.Cancel();
                Threads.ElementAt(i).Value.RunTask = null;
            }

        }

        /// <summary>
        /// 判断指定线程是否完成
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsComplete(string key)
        {
            if (Threads.ContainsKey(key)) return Threads[key].ThreadTask.IsCompleted;
            return false;
        }
    }
}
