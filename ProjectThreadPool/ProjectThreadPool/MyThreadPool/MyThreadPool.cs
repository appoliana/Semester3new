using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MyThreadPool
{
    public class MyThreadPool
    { 
        // ёмкость пула (общее кол-во)
        protected int poolCapacity;
        // счётчик оставновленных нитей
        protected int stopedThreadsCounter;
        // объект для блокировки элементов (защита от гонки данных)
        protected object lockObject;
        // спец объект для отмены выполнения нитей
        protected CancellationTokenSource cts;

        protected List<Thread> threadsList;

        // очередь из методов на обработку в наших нитях
        protected ConcurrentQueue<Action> jobsQueue;       

        // сигнальные объекты
        protected AutoResetEvent eventTaskAdded;
        protected AutoResetEvent eventJobDone;

        public bool IsActive
        {
            get => !cts.Token.IsCancellationRequested;
        }

        public MyThreadPool(int tasksQuantity)
        {
            poolCapacity = tasksQuantity;
            stopedThreadsCounter = 0;
            lockObject = new object();
            cts = new CancellationTokenSource();
            jobsQueue = new ConcurrentQueue<Action>();
            eventTaskAdded = new AutoResetEvent(false);
            eventJobDone = new AutoResetEvent(false);

            StartPool();
        }

        protected void StartPool()
        {
            threadsList = new List<Thread>(poolCapacity);
            for (var i = 0; i < poolCapacity; i++)
            {
                var thread = new Thread(() =>
                {
                    while (IsActive)
                    {
                        if (jobsQueue.TryDequeue(out Action doJob))
                        {
                            doJob();
                        }
                        else
                        {
                            eventTaskAdded.WaitOne();
                        }
                    }

                    lock (lockObject)
                    {
                        stopedThreadsCounter++;
                    }
                    eventJobDone.Set();
                });

                threadsList.Add(thread);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        public IMyTask<TResult> AddJob<TResult>(Func<TResult> job)
        {
            var task = new MyTask<TResult>(this, job);
            return AddTask<TResult>(task);
        }

        public IMyTask<TResult> AddTask<TResult>(MyTask<TResult> task)
        {
            if (!IsActive)
            {
                throw new MyThreadPoolCanceledException();
            }

            jobsQueue.Enqueue(task.DoJob);
            eventTaskAdded.Set();

            return task;
        }

        public void Shutdown()
        {
            cts.Cancel();
            eventTaskAdded.Set();
            while (true) // ждет, пока закончат выполнение все нити
            {
                eventJobDone.WaitOne();
                eventTaskAdded.Set();

                lock (lockObject)
                {
                    if (poolCapacity == stopedThreadsCounter)
                    {
                        break;
                    }
                }
            }
            jobsQueue = null;
        }
    }
}
