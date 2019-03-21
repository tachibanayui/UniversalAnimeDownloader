using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace UniversalAnimeDownloader
{
    /// <summary>
    /// Collection that remove the item after an delay
    /// </summary>
    /// <typeparam name="T">The Type of the items</typeparam>
    class LifeSpanObservableCollection<T> : ObservableCollection<T>, IDisposable
    {
        public TimeSpan ItemLifeSpan { get; set; } = TimeSpan.FromSeconds(20);
        public Thread LifeSpanTimerThread { get; private set; }
        public LifeSpanState State { get; private set; }
        private ManualResetEvent pauseThread = new ManualResetEvent(true);
        private Dispatcher UIThread;

        public LifeSpanObservableCollection() : base()
        {
            LifeSpanTimerThread = new Thread(RemoveItemEndOfLife);
            State = LifeSpanState.Running;
        }

        private void RemoveItemEndOfLife()
        {
            while(true)
            {
                pauseThread.WaitOne();
                Thread.Sleep(1000);
                if(Items.Count > 0)
                {
                    Thread.Sleep(ItemLifeSpan - TimeSpan.FromSeconds(1));
                    if(Items.Count > 0)
                    {
                        var tmp = Items[0];
                        Items.RemoveAt(0);
                        UIThread.Invoke(() => OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, tmp)));
                    }
                }
            }
        }

        public void ResumeLifeSpan()
        {
            pauseThread.Set();
            State = LifeSpanState.Running;
        }

        public void PauseLifeSpan()
        {
            pauseThread.Reset();
            State = LifeSpanState.Pasuing;
        }

        public void RemoveAll()
        {
            while (Items.Count != 0)
            {
                Items.RemoveAt(0);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Dispose()
        {
            LifeSpanTimerThread.Abort();
            RemoveAll();
        }
    }

    public enum LifeSpanState
    {
        Running, Pasuing
    }
}
