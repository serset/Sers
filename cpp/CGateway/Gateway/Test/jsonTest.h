#pragma once


#include "../Sers/Core/Module/json/Json.hpp"




using namespace Sers;

void JsonTest(){

	 Json json;
	 json.Parse("{\"a\":4}");

	 auto s=json.GetStringByPath("a");
	 auto s2=json.GetIntByPath("a");
	 auto s3=json.GetDoubleByPath("a");


}
