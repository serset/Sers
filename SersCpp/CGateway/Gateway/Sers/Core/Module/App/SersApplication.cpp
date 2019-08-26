/*
 * SersApplication.cpp
 *
 *  Created on: 2019Äê6ÔÂ15ÈÕ
 *      Author: root
 */

#include "SersApplication.hpp"

namespace Sers {

	AutoResetEvent SersApplication::mEventOnstop;

	ConcurrentQueue<FuncOnstop> SersApplication::actionsOnstop;

	bool SersApplication::isRunning=true;

} /* namespace Sers */
