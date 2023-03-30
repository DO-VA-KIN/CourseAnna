using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CurseAnna.Classes
{
    public struct Report
    {
        public int HorseNum;
        public int[] Traveled;
        public int[] Speed;
    }

    public class Game
    {
        private Report Report1;
        public Report GetReport()
        { return Report1; }

        public delegate void End(int num);
        /// <summary>
        /// Событие завершения
        /// </summary>
        public event End EndGame;

        private Controls.PBRoad PBHorse;
        private Label LHorse;
        private int MinSpeed;
        private int MaxSpeed;
        private int R;
        private bool Cancel = false;
        public void CancelAsync()
        { Cancel = true; }
        public bool IsBusy()
        { return Back.IsBusy; }

        public int Distance;
        public int StagesCount;

        BackgroundWorker Back;

        public Game(Controls.PBRoad pbHorse, Label lHorse, int minSpeed, int maxSpeed, int r)
        {
            PBHorse = pbHorse;
            LHorse = lHorse;
            MinSpeed = minSpeed;
            MaxSpeed = maxSpeed;
            R = r;

            Distance = pbHorse.GetMaximum();
            StagesCount = pbHorse.GetStageCount();

            Report1 = new Report()
            {
                HorseNum = r,
                Traveled = new int[StagesCount],
                Speed = new int[StagesCount]
            };

            Back = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            Back.DoWork += Back_DoWork;
            Back.ProgressChanged += Back_ProgressChanged;
            Back.RunWorkerCompleted += Back_RunWorkerCompleted;

            Back.RunWorkerAsync();
        }

        private void Back_DoWork(object sender, DoWorkEventArgs e)
        {
            Random rand = new Random((int)DateTime.Now.TimeOfDay.Ticks + R);
            int speed =  rand.Next(MinSpeed, MaxSpeed);

            Report1.Speed[0] = speed;
            int rep = 1;

            int change = Distance / StagesCount;
            for (int i = 0; i < Distance; i += speed)
            {
                if (i > change)
                {
                    speed = rand.Next(MinSpeed, MaxSpeed);
                    change += Distance / StagesCount;
                    Report1.Speed[rep] = speed;
                    Report1.Traveled[rep - 1] = i;
                    rep++;
                }
                
                Back.ReportProgress(i, speed);
                Thread.Sleep(1000);

                if (Cancel)
                {
                    Back.CancelAsync();
                    return;
                }
            }
        }

        private void Back_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PBHorse.SetValue(e.ProgressPercentage);
            LHorse.Content = e.UserState;
        }

        private void Back_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Cancel)
            {
                PBHorse.SetValue(0);
                LHorse.Content = "-";
            }
            else
            {
                int max = PBHorse.GetMaximum();
                Report1.Traveled[StagesCount - 1] = max;
                PBHorse.SetValue(max);
                LHorse.Content = "Ф";
            }
            if(EndGame != null)
                EndGame.Invoke(R);
        }

    }
}
