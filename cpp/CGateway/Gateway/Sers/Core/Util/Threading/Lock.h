/*
 * Lock.h
 *
 *  Created on: 2019Äê6ÔÂ6ÈÕ
 *      Author: root
 */

#ifndef SERS_CORE_UTIL_THREADING_LOCK_H_
#define SERS_CORE_UTIL_THREADING_LOCK_H_

/* define _Sers_Lock_ begin */
#if defined  _WIN32                                                         //Windows
#include <Windows.h>
#define _Sers_Lock_Mutex_t                 HANDLE
#define _Sers_Lock_Mutex_Init(_mutex)      (_mutex = CreateSemaphore(NULL,1,1,NULL))
#define _Sers_Lock_Mutex_Lock(_mutex)      (WaitForSingleObject(_mutex, INFINITE))
#define _Sers_Lock_Mutex_UnLock(_mutex)    (ReleaseSemaphore(_mutex,1,NULL))
#define _Sers_Lock_Mutex_Destroy(_mutex)   (CloseHandle(_mutex))


#elif defined __linux                                                       //Linux
#include <pthread.h>
#define _Sers_Lock_Mutex_t                 pthread_mutex_t
#define _Sers_Lock_Mutex_Init(_mutex)      (pthread_mutex_init(&_mutex, NULL))
#define _Sers_Lock_Mutex_Lock(_mutex)      (pthread_mutex_lock(&_mutex))
#define _Sers_Lock_Mutex_UnLock(_mutex)    (pthread_mutex_unlock(&_mutex))
#define _Sers_Lock_Mutex_Destroy(_mutex)   (pthread_mutex_destroy(&_mutex))

#endif
/* define _Sers_Lock_ end */




namespace Sers {





/*
 *  recommend code:
 *    //(x.1)init
 *    std::mutex  _lock;
 *
 *
 *    //(x.2)lock
 *	  std::lock_guard<std::mutex> lock(_lock);
 * */
class Lock
   {

   public:
		Lock();
       ~Lock();

       void lock() ;
       void unlock() ;

   private:
       _Sers_Lock_Mutex_t _mutex;
   };


} /* namespace Sers */

#endif /* SERS_CORE_UTIL_THREADING_LOCK_H_ */
