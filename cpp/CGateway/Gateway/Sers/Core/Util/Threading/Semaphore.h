/*
 * Semaphore.h
 *
 *  Created on: 2019Äê3ÔÂ18ÈÕ
 *      Author: root
 */

#ifndef SERS_CORE_UTIL_THREADING_SEMAPHORE_H_
#define SERS_CORE_UTIL_THREADING_SEMAPHORE_H_

#include <semaphore.h>

namespace Sers {


class Semaphore {
public:
	//
	Semaphore(int initCount)
	{
		sem_init(&sem, 0, initCount);
	}
	~Semaphore(){  sem_destroy(&sem);  }

	// after called WaitOne(),Release() must be called
	void Release() { sem_post(&sem);   }
	// after called WaitOne(),Release() must be called
	void WaitOne()
	{
		sem_wait(&sem);
	}

private:
		sem_t sem;

};


} /* namespace Sers */

#endif /* SERS_CORE_UTIL_THREADING_SEMAPHORE_H_ */
