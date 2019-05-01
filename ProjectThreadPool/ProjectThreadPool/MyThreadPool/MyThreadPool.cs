using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MyThreadPool
{
    public class MyThreadPool
    {
        protected int _poolCapacity;
        protected int _stopedThreadsCounter;
        protected object _lockObject;
        protected CancellationTokenSource _cts;

        protected List<Thread> _threadsList;
        protected ConcurrentQueue<Action> _jobsQueue;       

        protected AutoResetEvent _eventTaskAdded;
        protected AutoResetEvent _eventJobDone;

        public bool IsActive
        {
            get => !_cts.Token.IsCancellationRequested;
        }

        public MyThreadPool(int tasksQuantity)
        {
            _poolCapacity = tasksQuantity;
            _stopedThreadsCounter = 0;
            _lockObject = new object();
            _cts = new CancellationTokenSource();
            _jobsQueue = new ConcurrentQueue<Action>();
            _eventTaskAdded = new AutoResetEvent(false);
            _eventJobDone = new AutoResetEvent(false);

            StartPool();
        }

        protected void StartPool()
        {
            _threadsList = new List<Thread>(_poolCapacity);
            for (var i = 0; i < _poolCapacity; i++)
            {
                var thread = new Thread(() =>
                {
                    while (IsActive)
                    {
                        if (_jobsQueue.TryDequeue(out Action doJob))
                        {
                            doJob();
                        }
                        else
                        {
                            _eventTaskAdded.WaitOne();
                        }
                    }

                    lock (_lockObject)
                    {
                        _stopedThreadsCounter++;
                    }
                    _eventJobDone.Set();
                });

                _threadsList.Add(thread);
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

            _jobsQueue.Enqueue(task.DoJob);
            _eventTaskAdded.Set();

            return task;
        }

        public void Shutdown()
        {
            _cts.Cancel();
            _eventTaskAdded.Set();
            while (true)
            {
                _eventJobDone.WaitOne();
                _eventTaskAdded.Set();

                lock (_lockObject)
                {
                    if (_poolCapacity == _stopedThreadsCounter)
                    {
                        break;
                    }
                }
            }
            _jobsQueue = null;
        }
    }
}
