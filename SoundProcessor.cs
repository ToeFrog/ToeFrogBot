using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FrogBot
{
    public class SoundProcessor : IDisposable
    {
        public const int DefaultSleepTime = 1000;

        BlockingCollection<SoundCommand> Sounds = new BlockingCollection<SoundCommand>();
        CancellationTokenSource canceller = new CancellationTokenSource();
        static Lazy<SoundProcessor> _instance = new Lazy<SoundProcessor>(() => { return new SoundProcessor(); }, LazyThreadSafetyMode.ExecutionAndPublication);
        public int ProcessorSleepTime { get; set; }

        public static SoundProcessor Current
        {
            get
            {
                return _instance.Value;
            }
        }

        public SoundCommand CurrentlyPlaying
        {
            get;
            set;
        }

        public SoundProcessor()
        {
            Console.WriteLine("Initializing SoundProcessor...");
            this.ProcessorSleepTime = DefaultSleepTime;

            ThreadPool.QueueUserWorkItem(new WaitCallback(this.KeepProcessing), null);
        }

        public void KeepProcessing(object state)
        {
            while (!Sounds.IsCompleted)
            {
                try
                {
                    Process();
                }
                catch (OperationCanceledException ex)
                {
                    Console.WriteLine("The processor was forced to cancel");
                    break;
                }

                Thread.Sleep(1000);
            }
        }

        public void Process(int timeout = 100)
        {
            SoundCommand sound = null;

            try
            {
                while(Sounds.TryTake(out sound, timeout, canceller.Token))
                {
                    // Here we want to play the audio
                    this.CurrentlyPlaying = sound;
                    this.CurrentlyPlaying.Execute();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("We have encountered the following error: {0}", ex.Message);
            }
        }

        public void Queue(SoundCommand sound)
        {
            this.Sounds.Add(sound);
        }

        public void Flush()
        {
            while(this.Sounds.Count > 0)
            {
                this.Sounds.Take();
            }
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing of the SoundProcessor...");
            this.Flush();
            GC.SuppressFinalize(this);
        }
    }
}
