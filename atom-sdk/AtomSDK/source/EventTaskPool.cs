using System;

using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;

namespace ironsource {
    class EventTaskPool {
        private ConcurrentQueue<Action> events_;
        private bool isRunning_;

        private List<Thread> workers_;

        private int maxEvents_;

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
        /// <param name="eventAction">Event action.</param>
        public void addEvent(Action eventAction) {
            if (events_.Count > maxEvents_) {
                // fixme raise exception
                return;
            }
            events_.Enqueue(eventAction);
        }
    }
}

