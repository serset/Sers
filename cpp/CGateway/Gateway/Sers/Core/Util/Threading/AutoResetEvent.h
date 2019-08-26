/*
 * AutoResetEvent.h
 *
 *  Created on: 2019年3月18日
 *      Author: root
 */

#ifndef SERS_CORE_UTIL_THREADING_AUTORESETEVENT_H_
#define SERS_CORE_UTIL_THREADING_AUTORESETEVENT_H_


#include <semaphore.h>
#include <sys/time.h>


namespace Sers {


class AutoResetEvent {
public:


	AutoResetEvent()
	{
		sem_init(&sem, 0, 0);
	}
	~AutoResetEvent(){  sem_destroy(&sem);  }

	void Set()
	{
		sem_post(&sem);
	}
	//true if the current instance receives a signal; otherwise, false.
	bool WaitOne(int msTimeout=0)
	{
		if(msTimeout<=0)
		{
			sem_wait(&sem);
			return true;
		}else
		{
			timespec timewait;
			getTimeMsec(&timewait,msTimeout);

			int ret = sem_timedwait(&sem, &timewait);
			return ret!=-1;
		}
	}

private:
		sem_t sem;


		// 时间 time 自加 ms 毫秒
		static void time_add_ms(struct timeval *time, int ms)
		{
		        time->tv_usec += ms * 1000; // 微秒 = 毫秒 * 1000
		        if(time->tv_usec >= 1000000) // 进位，1000 000 微秒 = 1 秒
		        {
		                time->tv_sec += time->tv_usec / 1000000;
		                time->tv_usec %= 1000000;
		        }
		}
		static void getTimeMsec(timespec * lpTimespec, int ms)
		{
				// 毫秒级别
				struct timeval time;
				gettimeofday(&time, NULL);
				time_add_ms(&time, ms);
				lpTimespec->tv_sec = time.tv_sec;
				lpTimespec->tv_nsec = time.tv_usec * 1000;
		}
};


} /* namespace Sers */

#endif /* SERS_CORE_UTIL_THREADING_AUTORESETEVENT_H_ */
