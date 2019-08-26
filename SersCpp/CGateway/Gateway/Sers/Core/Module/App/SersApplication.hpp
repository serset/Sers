#pragma once

#include <string>
#include <functional>

#include "../../../Core/Module/Log/Logger.hpp"
#include "../../../Core/Util/Common/CommonHelp.hpp"
#include "../../../Core/Util/Concurrent/ConcurrentQueue.h"
#include "../../../Core/Util/Threading/AutoResetEvent.h"

namespace Sers {



using FuncOnstop = std::function<void ()>;

class SersApplication {
private:
	static AutoResetEvent mEventOnstop;
	static ConcurrentQueue<FuncOnstop> actionsOnstop;

public:
	static bool isRunning;

	static void AddActionOnstop(FuncOnstop onstop){
		actionsOnstop.EnQueue(onstop);
	}


	static void Stop(){
		if(!isRunning) return;
		isRunning=false;

		Logger::Info("program will exit.");

		//(x.1) callback
		FuncOnstop action;
		while(!actionsOnstop.IsEmpty()){
			if(actionsOnstop.TryDeQueue(action)){
				action();
			}
		}


		//(x.2) send stop signal
		mEventOnstop.Set();

		//(x.3) exit program
		exit(0);
	}

	static void RunAwait(){
		if(!isRunning) return;
		mEventOnstop.WaitOne();
	}



};

} /* namespace Sers */

