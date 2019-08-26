#pragma once

#include <stdio.h>
#include <string.h>
#include <string>
#include <unistd.h>
#include "../Threading/Lock.h"
#include <vector>
#include <time.h>

namespace Sers {


using  namespace std;

class CommonHelp{

	private:


	public :

		static long NewGuidLong()
		{
			static long curId=10;
			static Lock NewGuidLong_lock;

			long guid;
			NewGuidLong_lock.lock();
			guid= (++curId);
			NewGuidLong_lock.unlock();
			return guid;
		}

		static string NewGuid()
		{
			return to_string(NewGuidLong());
		}

		// return demo: /root/ws/Gateway/Debug/Gateway
		static string GetBaseDirectory()
		{
				string path;
//*
			    char szBuf[128];
			    getcwd(szBuf, sizeof(szBuf)-1);
			    //printf("buf:%s\n", szBuf);
			    path=szBuf;
			    return path;

				/*/
			    char szPath[500];
			    int ret =  readlink("/proc/self/exe", szPath, sizeof(szPath)-1 );
//			    printf("ret:%d\n", ret);
//			    printf("path:%s\n", szPath);
			    path=szPath;
			    return path;
			    //*/
		}

		//path demo:    Data/Demo.json
		static string GetAbsPathByRealativePath(const string&  path){
			string s=GetBaseDirectory()+"/"+path;
			return s;
		}



		static std::vector<std::string> StringSplit(const char *s, const char *delim)
		{
		    std::vector<std::string> result;
		    if (s && strlen(s))
		    {
		        int len = strlen(s);
		        char *src = new char[len + 1];
		        strcpy(src, s);
		        src[len] = '\0';
		        char *tokenptr = strtok(src, delim);
		        while (tokenptr != NULL)
		        {
		            std::string tk = tokenptr;
		            result.emplace_back(tk);
		            tokenptr = strtok(NULL, delim);
		        }
		        delete[] src;
		    }
		    return result;
		}

		//pattern demo:  "%Y-%m-%d %H:%M:%S"
		static std::string TimeToString(const char * pattern){
			time_t timep;
			time (&timep);
			char tmp[64];
			strftime(tmp, sizeof(tmp), pattern,localtime(&timep) );
			return tmp;
		}


		static void FileReadAllText(const string & path,string& fileData)
		 {
			FILE *f;

			f=fopen(path.data(),"rb");
			if(f){
				long len;
				char *data;
				fseek(f,0,SEEK_END);
				len=ftell(f);
				fseek(f,0,SEEK_SET);
				data=(char*)malloc(len+1);

				fread(data,1,len,f);
				fclose(f);
				fileData=data;
				free(data);
			}else{
				fileData.empty();
				return;
			}
		}
		static string DoubleToString(double value){
			char str[256];
			sprintf(str,"%.0f",value);
			return str;
		}

	};




} /* namespace Sers */


