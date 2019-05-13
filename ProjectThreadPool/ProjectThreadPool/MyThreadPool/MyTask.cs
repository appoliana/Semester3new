using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MyThreadPool
{
    /// <summary>
    /// Класс MyTask.
    /// </summary>
    public class MyTask<TResult> : IMyTask<TResult>
    {
        private TResult result;
        private Func<TResult> job;

        private Exception innerException;
        private MyThreadPool threadPool; 
        private object lockObject;

        //для обмена сообщениями между потоками
        private AutoResetEvent getResultsEvent;

        private ConcurrentQueue<Action> queueJobs;
        

        /// <summary>
        /// Конструктор класса MyTask.
        /// </summary>
        public MyTask(MyThreadPool threadPool, Func<TResult> job)
        {
            this.job = job;
            this.threadPool = threadPool;
            lockObject = new object();
            getResultsEvent = new AutoResetEvent(false);
            queueJobs = new ConcurrentQueue<Action>();
        }

        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Cвойство, которое возвращает результат работы задачи.
        /// </summary>
        public TResult Result
        {
            get
            {
                getResultsEvent.WaitOne(); //свойство вернет результат только когда получит сигнал о том что есть результат

                if (innerException == null)
                {
                    return result;
                }

                throw new AggregateException(innerException); 
            }
        }

        /// <summary>
        /// Метод, который выполняет задачу.
        /// </summary>
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

        /// <summary>
        /// Метод, который помещает задачу в пулл на выполнение.
        /// </summary>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> job)
        {
            // оболочка для функции, которая продолжает вычисление на основе полученного результата в том же пуле
            var task = new MyTask<TNewResult>(threadPool, () => job(result));
            lock (lockObject)
            {
                if (!IsCompleted) //если не закончена работа
                {
                    // помещаем новую задачу в наш пул на выполнение
                    queueJobs.Enqueue(() => threadPool.AddJob<TNewResult>(() => job(result)));
                    return task;
                }
            }
            return threadPool.AddJob(() => job(result)); // если закончена, то сразу в пул
        }

        /// <summary>
        /// Метод, который помещает задачу в пулл и продолжает выполнение .
        /// </summary>
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
