/*
 * JsonFile.h
 *
 *  Created on: 2019Äê6ÔÂ4ÈÕ
 *      Author: lith
 */

#ifndef SERS_CORE_UTIL_CONFIGURATIONMANAGER_JSONFILE_HPP_
#define SERS_CORE_UTIL_CONFIGURATIONMANAGER_JSONFILE_HPP_

#include <stdio.h>
#include <string>
#include "vector"
#include "../Common/CommonHelp.hpp"
#include "../../Module/Log/Logger.hpp"

#include "../../Module/json/Json.hpp"

namespace Sers {



class JsonFile {

private:
	Json json;
	string fileData;


public:
	static  JsonFile  ConfigurationManager;


	//absPath demo:  "appsettings.json"
	JsonFile(string absPath){
		string   path=CommonHelp::GetAbsPathByRealativePath(absPath);

		CommonHelp::FileReadAllText(path,fileData);

		const char * text=fileData.c_str();
		//const char * text="{\n\"name\": \"Jack (\\\"Bee\\\") Nimble\", \n\"format\": {\"type\":       \"rect\", \n\"width\":      1920, \n\"height\":     1080, \n\"interlace\":  false,\"frame rate\": 24\n}\n}";
		//text= "{\"Sers\":{\"test\":\"UTF8\"}}";
		json.Parse(text);
	}






	string GetStringByPath(const char * path)
	{
		return json.GetStringByPath(path);
	}

	int GetIntByPath(const char * path)
	{
		return json.GetIntByPath(path);
	}

};





} /* namespace Sers */

#endif /* SERS_CORE_UTIL_CONFIGURATIONMANAGER_JSONFILE_HPP_ */
