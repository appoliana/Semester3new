using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MyThreadPool
{
    public class MyTask<TResult> : IMyTask<TResult>
    {
        protected TResult result;
        protected Func<TResult> job;

        protected Exception innerException;
        protected MyThreadPool threadPool; //ссылка на наш пул
        protected object lockObject;

        //для обмена сообщениями между потоками
        private AutoResetEvent getResultsEvent;

        // нужно для ContinueWith
        private ManualResetEvent taskAddedEvent;
        private ConcurrentQueue<Action> queueJobs;
        

        public MyTask(MyThreadPool threadPool1, Func<TResult> job1)
        {
            job = job1;
            threadPool = threadPool1;
            lockObject = new object();
            getResultsEvent = new AutoResetEvent(false);
            taskAddedEvent = new ManualResetEvent(true);
            queueJobs = new ConcurrentQueue<Action>();
        }

        public bool IsCompleted { get; private set; }

        public TResult Result
        {
            get
            {
                getResultsEvent.WaitOne(); //свойство вернет результат только когда получит сигнал о том что есть результат

                if (innerException == null)
                {
                    return result;
                }

                throw new AggregateException(innerException); //собирает со всех дочерних потоков исклбчения
            }
        }

        public void DoJob()
        {
            try
            {
                result = job();
            }

            catch (Exception ex)
            {
                innerException = ex;
            }

            lock (lockObject)
            {
                getResultsEvent.Set();
                IsCompleted = true;
                FinishAndContinueJobs();
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> job)
        {
            // оболочка для функции, которая продолжает вычисление на основе полученного результата в том же пуле
            var task = new MyTask<TNewResult>(threadPool, () => job(result));
            lock (lockObject)
            {
                if (!IsCompleted) //если не закончена работа
                {
                    // помещаем новую задачу в наш пул на выполнение
                    queueJobs.Enqueue(() => threadPool.AddTask<TNewResult>(task));
                    return task;
                }
            }
            return threadPool.AddTask(task); // если закончена, то сразу в пул
        }

        private void FinishAndContinueJobs()
        {
            if (queueJobs.Count == 0)
            {
                return;
            }

            if (innerException == null)
            {
                if (!threadPool.IsActive)
                {
                    queueJobs = null;
                    return;
                }

                foreach (Action job in queueJobs)
                {
                    job(); //помещаем в пул на выполнение и задача начинает считаться
                }

                queueJobs = null;
            }
            else
            {
                throw new AggregateException(innerException); //место для всех исключений
            }
        }
    }
}
