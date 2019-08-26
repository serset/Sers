/*
 * ConcurrentQueue.h
 *
 *  Created on: 2019Äê3ÔÂ19ÈÕ
 *      Author: root
 */

#ifndef SERS_CORE_UTIL_CONCURRENT_CONCURRENTQUEUE_H_
#define SERS_CORE_UTIL_CONCURRENT_CONCURRENTQUEUE_H_

#include <mutex>
#include <queue>

namespace Sers {




template <typename T>
class ConcurrentQueue
{
	private:
	std::queue<T> _queue;
	std::mutex  _lock;
public:

	ConcurrentQueue()
    {}

    ~ConcurrentQueue()
    {
    }


    void EnQueue(T& t)
    {
    	std::lock_guard<std::mutex> lock(_lock);
    	_queue.push(t);
    }


    bool TryDeQueue(T& t)
    {
    	std::lock_guard<std::mutex> lock(_lock);

    	if(_queue.empty()) return false;

    	t=_queue.front();
    	_queue.pop();
    	return true;
    }

     bool IsEmpty()
     {
    	 //std::lock_guard<std::mutex> lock(_lock);
         return _queue.empty();
     }


};




} /* namespace Sers */

#endif /* SERS_CORE_UTIL_CONCURRENT_CONCURRENTQUEUE_H_ */
