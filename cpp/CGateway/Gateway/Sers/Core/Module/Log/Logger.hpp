/*
 * Logger.h
 *
 *  Created on: 2019Äê6ÔÂ9ÈÕ
 *      Author: root
 */

#ifndef SERS_CORE_MODULE_LOG_LOGGER_HPP_
#define SERS_CORE_MODULE_LOG_LOGGER_HPP_

#include <string>
#include "../../Util/Common/CommonHelp.hpp"
#include <unistd.h>
#include <sys/stat.h>

namespace Sers {


using namespace std;


typedef void (*FuncOnLog)(const  string & level,const  string &  finalMsg);


class Logger {

private:


	//   /Logs/{yyyy-MM}/{yyyy-MM-dd}{level}.log
	static void LogTxt(const  string & level,const  string &  finalMsg){
		//(x.1) get log file path
		string foldPath_Logs=CommonHelp::GetAbsPathByRealativePath("Logs");
		string foldPath_Log=foldPath_Logs + "/" + CommonHelp::TimeToString("%Y-%m");
		string logFilePath = foldPath_Log+"/["+CommonHelp::TimeToString("%Y-%m-%d")+"]"+level+".log";

		//(x.2) make sure the fold  exits
		if(access(foldPath_Logs.data(), F_OK) ==-1)
		{
			mkdir(foldPath_Logs.data(),0755);
		}
		if(access(foldPath_Log.data(), F_OK) ==-1)
		{
			mkdir(foldPath_Log.data(),0755);
		}


		//(x.3) write log content to file
		FILE *fd = fopen(logFilePath.data(), "a+");
		if (fd)
		{
			fwrite(finalMsg.data(), finalMsg.length(), 1, fd);
			fclose(fd);
		}
	}


	static void Log(const  string & level,const  string &  message){
		//(x.1) build finalMsg
		string finalMsg="["+CommonHelp::TimeToString("%H:%M:%S")+"]"+message+"\r\n";

		LogTxt(level,finalMsg);

		if(OnLog){
			OnLog(level,finalMsg);
		}
	}

public:

	static FuncOnLog OnLog;

	//print the log to console
	static void OnLog_Printf(const  string & level,const  string &  finalMsg){
		printf("[%s]%s",level.c_str(),finalMsg.c_str());
	}

	static void Info(const std::string & message){
		Log("info",message);
	}

	static void Info(char * message){
		Log("info",string(message));
	}

	static void Error(const std::string & message){
		Log("error",message);
	}

	static void Error(char * message){
		Log("error",string(message));
	}

};


} /* namespace Sers */

#endif /* SERS_CORE_MODULE_LOG_LOGGER_HPP_ */
