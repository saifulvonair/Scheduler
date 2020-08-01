
//######################################################################
//#
//# DESCRIPTION:
//# 
//#
//# AUTHOR:		Mohammad Saiful Alam
//# POSITION:   Senior General Manager
//# E-MAIL:		saiful.alam@ bjitgroup.com
//# CREATE DATE: 
//#
//# Copyright (c): 
//######################################################################

using System;
using System.Collections.Generic;
using System.Threading;

namespace Scheduler
{
    class SchedulerModel: Object
    {
        public delegate void SchedulerModelCallback(object information);

        public enum Scheduler
        {
            EveryMinutes = 1,
            EveryHour,
            EveryHalfDay,
            EveryDay,
            EveryWeek,
            EveryMonth,
            EveryYear,
        }

        //
        private static Timer mTimer;
        private DateTime mNextDateValue;
        private IDictionary<int, bool> mDictionaryScheduleType = new Dictionary<int, bool>();
        private Scheduler mScheduler;
        private bool mRunning = false;
        //
        // transfer values from controls to schedule week days array
        private string[] mStrArrayScheduleWeekDays = { "", "", "", "", "", "", "" };

        private void setScheduleWeekDays()
        {
            //
        }

        //
        public SchedulerModel mSchedulerModel;
        int mNumMinsStart = 0;
        // Set the minutes Values.. how much 
        int mMinuteValue = 1;
        //
        SchedulerModelCallback mSchedulerModelCallback;
        //
        public SchedulerModel()
        {
        }

        //
        public void cancel()
        {
            mTimer.Dispose();
            mRunning = false;
            mTimer = null;
            mNumMinsStart = 0;
            mMinuteValue = 1;
            mNextDateValue = DateTime.Now;
            if (mSchedulerModelCallback != null)
            {
                mSchedulerModelCallback.Invoke("Scheduled stopped!!");
            }
        }

        public bool Running { get { return mRunning; } }
        
        //
        public void setScheduleType(Scheduler type)
        {
            mScheduler = type;
        }

        //
        public void initScheduler(int interval)
        {
            mRunning = true;
            mTimer = new Timer(new TimerCallback(dispatchEvents), // main timer
                                             null,
                                             0,
                                             interval);
            mSchedulerModel = this;
            mScheduler = Scheduler.EveryMinutes;
        }

        //
        public void start(SchedulerModelCallback callBack, int interval)
        {
            if(mTimer == null)
            {
                initScheduler(interval);
            }

            mSchedulerModelCallback = callBack;
            //retrieve hour and minute from the form
            // int hour = DateTime.Now.Hour;
            // int minutes = DateTime.Now.Minute;
            var dateNow = DateTime.Now;
            var date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, dateNow.Hour, dateNow.Minute, 0);
            mNumMinsStart = dateNow.Minute;
            //get nex date the code need to run
            mNextDateValue = getNextDate(date, getScheduler());
        }

        // call back for the timer function
        private void dispatchEvents(object aObj) // obj ignored
        {
            DateTime dateNow = DateTime.Now;
            dateNow = dateNow.AddSeconds(-dateNow.Second);

            if (mNextDateValue.Year != dateNow.Year &&
                mNextDateValue.Month != dateNow.Month &&
                mNextDateValue.Day != dateNow.Day)
            {
                return;
            }

            if (mNextDateValue.Hour == dateNow.Hour && mNextDateValue.Minute == dateNow.Minute)
            {
                mNextDateValue = methodToCall(dateNow);
            }
            else if (mNextDateValue < dateNow)
            {
                // Adjut Minutes
                int nextMinute = getNextMinute(mNumMinsStart, dateNow.Minute);
                dateNow = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, dateNow.Hour, nextMinute, 0);
                //TRY Execute now..
                if (mNextDateValue.Hour == dateNow.Hour && mNextDateValue.Minute == nextMinute)
                {
                    mNextDateValue = methodToCall(dateNow);
                }
                else
                {
                    //Later...
                    mNextDateValue = dateNow;//getNextDate(dateNow, getScheduler());
                }
            }
        }

        //
        private Scheduler getScheduler()
        { 
            return mScheduler;
        }

        //
        private DateTime methodToCall(DateTime time)
        {
            //setup next call
            var nextTimeToCall = getNextDate(time, getScheduler());
            var strText = string.Format("Method is called at {0}. The next call will be at {1}", time, nextTimeToCall);

            if (mSchedulerModelCallback != null)
            {
                mSchedulerModelCallback.Invoke(strText);
            }

            /*
            this.BeginInvoke((Action)(() =>
            {
                var strText = string.Format("Method is called at {0}. The next call will be at {1}", time, nextTimeToCall);
                listBox1.Items.Add(strText);
                //MessageBox.Show(strText);
            }));
            */

            return nextTimeToCall;
        }
        
        //
        int getNextMinute(int start, int currentMinute)
        {
            //date.AddMinutes(2);

            if (start == currentMinute)
            {
                return start;
            }

            if (currentMinute < start)
            {
                currentMinute += 60;
            }

            for (int i = start; i <= currentMinute;)
            {
                i += mMinuteValue;
                if (i >= currentMinute)
                {
                    return i % 60;
                }
            }

            return 0;
        }

        //
        private DateTime getNextDate(DateTime date, Scheduler scheduler)
        {
            //TODO need to update for othat values...
            switch (scheduler)
            {
                case Scheduler.EveryMinutes:
                    return date.AddMinutes(mMinuteValue);
                case Scheduler.EveryHour:
                    return date.AddHours(1);
                case Scheduler.EveryHalfDay:
                    return date.AddHours(12);
                case Scheduler.EveryDay:
                    return date.AddDays(1);
                case Scheduler.EveryWeek:
                    return date.AddDays(7);
                case Scheduler.EveryMonth:
                    return date.AddMonths(1);
                case Scheduler.EveryYear:
                    return date.AddYears(1);
                default:
                    throw new Exception("Invalid scheduler");
            }

        }
    }
}
