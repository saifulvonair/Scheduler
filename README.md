# Scheduler
A scheduler to run some task in specific time, added for minute. hour, daily, weekly, monthly and yearly. Support Cancel operation.  We can say it as Task scheduler as well.

#Example to use

```C#
 mSchedulerModel.setScheduleType(SchedulerModel.Scheduler.EveryMinutes);
 mSchedulerModel.start(delegate (object p)
            {
                // CallBack....
                string strText = (string)p;              
                this.BeginInvoke((Action)(() =>
                {
                    listBox1.Items.Add(strText);
                }));
            }, 1000); 
```
