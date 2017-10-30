using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ExpirableListLib
{
    public class ExpirableList<T>
    {
        private List<T> items;
        private Timer timer;
        private object thisLock = new object();

        public event ListFinishedEventHandler<T> ListFinished;   

        public ExpirableList(int timeoutInMs, int capacity)
        {
            this.items = new List<T>(capacity);
            this.timer = new Timer(timeoutInMs);
            this.timer.Elapsed += Timer_Elapsed;
        }

        public void Add(T item)
        {
            if (this.items.Count == 0)
            {
                this.timer.Start();
            }

            if (this.timer.Enabled)
            {
                lock (thisLock)
                {
                    this.items.Add(item);

                    if (this.items.Count == this.items.Capacity)
                    {
                        this.timer.Stop();
                        if (this.ListFinished != null)
                        {
                            this.ListFinished(this, new ListFinishedEventArgs<T>(this.items, true));
                            this.ListFinished = null;
                        }
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Cannot add items while list stopped.");
            }
        }

        public IEnumerable<T> Items
        {
            get
            {
                return this.items;
            }
        }

        public int Capacity
        {
            get
            {
                return this.items.Capacity;
            }
        }

        public bool IsFinished
        {
            get
            {
                return !this.timer.Enabled;
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.timer.Stop();

            if (this.ListFinished != null)
            {
                this.ListFinished(this, new ListFinishedEventArgs<T>(this.items, this.items.Count == this.items.Capacity));
                this.ListFinished = null;
            }
        }
    }

    public class ListFinishedEventArgs<T> : EventArgs
    {
                
        public ListFinishedEventArgs(IEnumerable<T> items, bool isListComplete)
        {
            this.Items = items;
            this.IsListComplete = isListComplete;
        }

        public IEnumerable<T> Items { get; private set; }

        public bool IsListComplete { get; private set; }
    }

    public delegate void ListFinishedEventHandler<T>(ExpirableList<T> sender, ListFinishedEventArgs<T> e);
}
