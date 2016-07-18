using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;

namespace ironsource {
    public class EventTaskPoolException : Exception {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTaskPoolException"/>
        /// </summary>
        public EventTaskPoolException() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTaskPoolException"/>
        /// </summary>
        /// <param name="message">
        /// <see cref="string"/> error message
        /// </param>
        public EventTaskPoolException(string message) 
            : base(message) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTaskPoolException"/>
        /// </summary>
        /// <param name="message">
        /// <see cref="string"/> error message
        /// </param>
        /// <param name="inner">
        /// <see cref="Exception"/> inner exception
        /// </param>
        public EventTaskPoolException(string message, Exception inner) 
            : base(message, inner) {
        }
    }


    public class EventTaskPool {
        private ConcurrentQueue<Action> events_;
        private bool isRunning_;

        private List<Thread> workers_;

        private int maxEvents_;

        /// <summary>
        /// Initializes a new instance of the <see cref="ironsource.EventTaskPool"/> class.
        /// </summary>
        /// <param name="maxThreads">
        /// <see cref="int"/> max thread for event pool
        /// </param>
        /// <param name="maxEvents">
        /// <see cref="int"/> max events for event pool
        /// </param>
        public EventTaskPool(int maxThreads, int maxEvents) {
            maxEvents_ = maxEvents;
            events_ = new ConcurrentQueue<Action>();
            isRunning_ = true;

            workers_ = new List<Thread>();
            ThreadStart threadMethodHolder = new ThreadStart(this.TaskWorker);

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
        /// Tasks the worker.
        /// </summary>
        private void TaskWorker() {
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
        /// Adds the event.
        /// </summary>
        /// <param name="eventAction">
        /// <see cref="Action"/> event callback action
        /// </param>
        public void addEvent(Action eventAction) {
            if (events_.Count > maxEvents_) {
                throw new EventTaskPoolException("Exceeded max event count in Event Task Pool!");
            }
            events_.Enqueue(eventAction);
        }
    }
}

