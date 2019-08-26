package Sers.Core.Module.Log;

import Sers.Core.Util.Common.CommonHelp;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.util.Date;

public class Logger {

    public interface IOnLog{
        void OnLog(String level,String finalMsg);
    }


    public static IOnLog onLog=null;

    //   /Logs/{yyyy-MM}/{yyyy-MM-dd}{level}.log
    private static void LogTxt(String level,String  finalMsg){
        Date now=new Date();
        //(x.1) get log file path
        String foldPath= CommonHelp.GetAbsPathByRealativePath("Logs")+ File.separatorChar +  CommonHelp.DateToString(now,"yyyy-MM");
        String logFilePath = foldPath+ File.separatorChar+"["+CommonHelp.DateToString(now,"yyyy-MM-dd")+"]"+level+".log";

        //(x.2) make sure the fold  exits
//        File file=new File(foldPath);
//        if(!file.exists()){//如果文件夹不存在
//            file.mkdir();//创建文件夹
//        }

        //(x.3) write log content to file
        CommonHelp.File_AppendText(logFilePath, finalMsg);        

    }

    private static void Log(String level,String  message){
        //(x.1) build finalMsg
        String finalMsg="["+ CommonHelp.DateToString( new Date(),"HH:mm:ss.SSS")+"]"+message+CommonHelp.NewLine;

        //(x.2) output log
        try {

            LogTxt(level,finalMsg);

            if(onLog!=null){
                onLog.OnLog(level,finalMsg);
            }
        }catch(Exception e){

        }
    }


    public static void Info(String message){
        Log("info",message);
    }

    public static void Error(String message){
        Log("error",message);
    }


    public static void Error(String message,Exception ex){
            String strMsg = "";
            if (!CommonHelp.StringIsNullOrEmpty(message)) strMsg += " message:" + message;

            if (null != ex)
            {
                strMsg += CommonHelp.NewLine + " StackTrace:";
                for(StackTraceElement elem : ex.getStackTrace()) {
                    strMsg += CommonHelp.NewLine + elem;
                }
            }
            Error(strMsg);
    }

    public static void Error(Exception ex){
        Error(null,ex);
    }

    /*
    *
    *



public:

	static FuncOnLog OnLog;

	//print the log to console
	static void OnLog_Printf(const  string & level,const  string &  finalMsg){
		printf("[%s]%s",level.c_str(),finalMsg.c_str());
	}

    *
    * */



}
