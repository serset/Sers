/*
 * Lock.cpp
 *
 *  Created on: 2019Äê6ÔÂ6ÈÕ
 *      Author: root
 */

#include "Lock.h"

namespace Sers {




Lock::Lock() {
	// TODO Auto-generated constructor stub
	_Sers_Lock_Mutex_Init(_mutex);
}

Lock::~Lock() {
	// TODO Auto-generated destructor stub
	_Sers_Lock_Mutex_Destroy(_mutex);
}

void Lock::lock() {_Sers_Lock_Mutex_Lock(_mutex);}

void Lock::unlock() {_Sers_Lock_Mutex_UnLock(_mutex);}





} /* namespace Sers */
