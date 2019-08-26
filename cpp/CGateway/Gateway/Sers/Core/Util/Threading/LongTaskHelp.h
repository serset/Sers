/*
 * LongTaskHelp.h
 *
 *  Created on: 2019Äê3ÔÂ18ÈÕ
 *      Author: lith
 */

#ifndef SERS_CORE_UTIL_THREADING_LONGTASKHELP_H_
#define SERS_CORE_UTIL_THREADING_LONGTASKHELP_H_

#include <pthread.h>
#include <signal.h>
#include <error.h>
#include <functional>

#include "Lock.h"


namespace Sers {

using Action = std::function<void(void*arg)>;

#ifndef ESRCH
#define	ESRCH		 3	/* No such process */
#define	EINVAL		22	/* Invalid argument */
#endif

class TaskItem{
public:
	TaskItem()
	{

	}
	~TaskItem()
	{
		Stop();
	}



	void Action_Set(Action & action,void * arg)
	{
		this->action_arg=arg;
		this->action=action;
	}


	bool Start()
	{
		//if(IsAlive()) return true;
		int ret_thrd = pthread_create(&pthread, NULL,  &_Run, this);

		if(0==ret_thrd)
		{
			//success
			return true;

		}else
		{
			//fail
			return false;
		}
	}

	void Stop()
	{
		if(IsAlive())
			pthread_cancel(pthread);
	}

	bool IsAlive()
	{
		int kill_rc = pthread_kill(pthread,0);

		if(kill_rc == ESRCH){
			//printf("the specified thread did not exists or already quit\n");
			return false;
		}else if(kill_rc == EINVAL){
//			printf("signal is invalid\n");
			return false;
		}else {
	//		printf("the specified thread is alive\n");
			return true;
		}
	}
private:
	pthread_t pthread;


	Action action;
	void* action_arg;

	static void* _Run(void*lp)
	{
		TaskItem * self=(TaskItem *)lp;
		pthread_setcancelstate(PTHREAD_CANCEL_ENABLE, NULL);

		while(1)
		{
			pthread_testcancel();/*the thread can be killed only here*/
			self->action(self->action_arg);
		}
	}
};


class LongTaskHelp {
private :

	Action action;
	void* action_arg;


	int threadCount=1;

	TaskItem* tasks;
	Lock lock;

public:

	LongTaskHelp()
	{
		tasks=0;
	}
	virtual ~LongTaskHelp()
	{
		Stop_Temp();
	}

	void Action_Set(void action(void *),void * arg)
	{
		this->action_arg=arg;
		this->action=action;
	}

	void Action_Set(Action & action,void * arg)
	{
		this->action_arg=arg;
		this->action=action;
	}


	void Start()
	{
		lock.lock();
		tasks=new TaskItem[threadCount];
		for(int t=0;t<threadCount;t++)
		{
			tasks[t].Action_Set(action,action_arg);
			tasks[t].Start();
		}
		lock.unlock();
	}

	void Stop_Temp()
	{
		return;
		lock.lock();
		if(tasks!=0)
		{
			delete []tasks;
			tasks=0;
		}
		lock.unlock();
	}

	bool IsAlive()
	{
		if(!tasks)
		return false;
		for(int t=0;t<threadCount;t++)
		{
			if(tasks[t].IsAlive())
				return true;
		}
		return false;
	}

	bool ThreadCount_Set(int threadCount)
	{
		bool success=false;
		lock.lock();
		if(!IsAlive())
		{
			this->threadCount=threadCount;
			success=true;
		}
		lock.unlock();
		return success;
	}
	int ThreadCount_Get()
	{
		return threadCount;
	}




};


} /* namespace Sers */

#endif /* SERS_CORE_UTIL_THREADING_LONGTASKHELP_H_ */
