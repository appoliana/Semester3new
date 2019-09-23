using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MyThreadPool
{
    /// <summary>
    /// Класс MyTrheadPool.
    /// </summary>
    public class MyThreadPool
    { 
        // ёмкость пула (общее кол-во)
        private int poolCapacity;

        // счётчик остановленных нитей
        private int stoppedThreadsCounter;

        // объект для блокировки элементов (защита от гонки данных)
        private object lockObject;

        // спец объект для отмены выполнения нитей
        private CancellationTokenSource cancellationToken;

        private List<Thread> threadsList;

        // очередь из методов на обработку в наших нитях
        private ConcurrentQueue<Action> jobsQueue;

        // сигнальные объекты
        private AutoResetEvent eventTaskAdded;

        private AutoResetEvent eventJobDone;

        public bool IsActive => !cancellationToken.Token.IsCancellationRequested;

        /// <summary>
        /// Конструктор класса MyThreadPool.
        /// </summary>
        public MyThreadPool(int tasksQuantity)
        {
            poolCapacity = tasksQuantity;
            stoppedThreadsCounter = 0;
            lockObject = new object();
            cancellationToken = new CancellationTokenSource();
            jobsQueue = new ConcurrentQueue<Action>();
            eventTaskAdded = new AutoResetEvent(false);
            eventJobDone = new AutoResetEvent(false);

            StartPool();
        }

        /// <summary>
        /// Метод, который запускает пулл.
        /// </summary>
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
                        stoppedThreadsCounter++;
                    }
                    eventJobDone.Set();
                });

                threadsList.Add(thread);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        /// <summary>
        /// Метод, который вернет добавленную задачу.
        /// </summary>
        public IMyTask<TResult> AddJob<TResult>(Func<TResult> job)
        {
            var task = new MyTask<TResult>(this, job);
            return AddTask(task);
        }

        private IMyTask<TResult> AddTask<TResult>(MyTask<TResult> task)
        {
            lock (lockObject)
            {
                if (!IsActive)
                {
                    throw new MyThreadPoolCanceledException();
                }
                jobsQueue.Enqueue(task.DoJob);
                eventTaskAdded.Set();
                return task;
            }
        }

        /// <summary>
        /// Метод, который завершит работу потоков.
        /// </summary>
        public void Shutdown()
        {
            cancellationToken.Cancel();
            eventTaskAdded.Set();
            while (true) // ждет, пока закончат выполнение все нити
            {
                eventJobDone.WaitOne();
                eventTaskAdded.Set();
                lock (lockObject)
                {
                    if (poolCapacity == stoppedThreadsCounter)
                    {
                        break;
                    }
                } 
                jobsQueue = null;
            }
        }
    }
}
