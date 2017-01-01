using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;

namespace ironsource {
    public class BatchEventPoolException : Exception {
        /// <summary>
        /// Initializes a new instance of the <see cref="BatchEventPoolException"/>
        /// </summary>
        public BatchEventPoolException() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchEventPoolException"/>
        /// </summary>
        /// <param name="message">
        /// <see cref="string"/> error message
        /// </param>
        public BatchEventPoolException(string message) 
            : base(message) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchEventPoolException"/>
        /// </summary>
        /// <param name="message">
        /// <see cref="string"/> error message
        /// </param>
        /// <param name="inner">
        /// <see cref="Exception"/> inner exception
        /// </param>
        public BatchEventPoolException(string message, Exception inner) 
            : base(message, inner) {
        }
    }

    /// <summary>
    /// Handles concurrent event sending
    /// Handles the backlog of BatchEvents
    /// </summary>
    public class BatchEventPool {
        private ConcurrentQueue<Action> events_;
        private bool isRunning_;

        private List<Thread> workers_;

        private int maxEvents_;

        /// <summary>
        /// Initializes a new instance of the <see cref="ironsource.BatchEventPool"/> class.
        /// </summary>
        /// <param name="maxThreads">
        /// <see cref="int"/> max thread for event pool
        /// </param>
        /// <param name="maxEvents">
        /// <see cref="int"/> max events for event pool
        /// </param>
        public BatchEventPool(int maxThreads, int maxEvents) {
            maxEvents_ = maxEvents;
            events_ = new ConcurrentQueue<Action>();
            isRunning_ = true;

            workers_ = new List<Thread>();
            ThreadStart threadMethodHolder = new ThreadStart(this.BatchWorkerTask);

            for (int index = 0; index < maxThreads; ++index) {
                Thread workerThread = new Thread(threadMethodHolder);
                workers_.Add(workerThread);

                workerThread.Start();
            }
        }

        /// <summary>
        /// Stop this instance.
        /// </summary>
        public void Stop() {
            isRunning_ = false;

            foreach (Thread thread in workers_) {
                thread.Join();
            }
        }

        /// <summary>
        /// Batch worker task function - each worker (thread) is polling the Queue for a batch event 
        /// and handles the sending of the data
        /// </summary>
        private void BatchWorkerTask() {
            while (isRunning_) {
                Action eventAction;
                if (!events_.TryDequeue(out eventAction)) {
                    Thread.Sleep(25);
                    continue;
                }

                eventAction();
            }
        }

        /// <summary>
        /// Add batchEvent to pool
        /// </summary>
        /// <param name="batchEvent">
        /// <see cref="Action"/> event callback action
        /// </param>
        public void addEvent(Action batchEvent) {
            if (events_.Count > maxEvents_) {
                throw new BatchEventPoolException("Exceeded max event count in Batch Event Pool!");
            }
            events_.Enqueue(batchEvent);
        }
    }
}

