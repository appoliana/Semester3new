using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MyThreadPool
{
    public class MyTask<TResult> : IMyTask<TResult>
    {
        protected TResult _result;
        protected Func<TResult> _job;
        protected Exception _innerException;
        protected MyThreadPool _threadPool;
        protected object _lockObject;

        private AutoResetEvent _getResultsEvent;
        private ManualResetEvent _taskAddedEvent;
        private ConcurrentQueue<Action> _queueJobs;
        

        public MyTask(MyThreadPool threadPool, Func<TResult> job)
        {
            _job = job;
            _threadPool = threadPool;
            _lockObject = new object();
            _getResultsEvent = new AutoResetEvent(false);
            _taskAddedEvent = new ManualResetEvent(true);
            _queueJobs = new ConcurrentQueue<Action>();
        }

        public bool IsCompleted { get; private set; }

        public TResult Result
        {
            get
            {
                _getResultsEvent.WaitOne();

                if (_innerException == null)
                {
                    return _result;
                }

                throw new AggregateException(_innerException);
            }
        }

        public void DoJob()
        {
            try
            {
                _result = _job();
            }

            catch (Exception ex)
            {
                _innerException = ex;
            }

            lock (_lockObject)
            {
                _getResultsEvent.Set();
                IsCompleted = true;
                FinishAndContinueJobs();
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> job)
        {
            var task = new MyTask<TNewResult>(_threadPool, () => job(_result));
            lock (_lockObject)
            {
                if (!IsCompleted)
                {
                    _queueJobs.Enqueue(() => _threadPool.AddTask<TNewResult>(task));
                    return task;
                }
            }
            return _threadPool.AddTask(task);
        }

        private void FinishAndContinueJobs()
        {
            if (_queueJobs.Count == 0)
            {
                return;
            }

            if (_innerException == null)
            {
                if (!_threadPool.IsActive)
                {
                    _queueJobs = null;
                    return;
                }

                foreach (Action job in _queueJobs)
                {
                    job();
                }

                _queueJobs = null;
            }
            else
            {
                throw new AggregateException(_innerException);
            }
        }
    }
}
