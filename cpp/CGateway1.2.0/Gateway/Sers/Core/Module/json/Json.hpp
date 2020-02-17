#pragma once

#include <string>
#include <string.h>
#include <unordered_map>
#include <unordered_set>
#include <functional>
#include <stdio.h>
#include "cJSON/cJSON.h"
#include <unistd.h>

#include "../Log/Logger.hpp"

using namespace std;

namespace Sers {


class Json
{
private:
	cJSON * root;
	bool isRoot;





	cJSON * GetChildByPath(const char * path){
		cJSON * cur=root;

		if(!cur){
			return 0;
		}

		auto items=CommonHelp::StringSplit(path,".");
		for(int t=0;t<items.size();t++)
		{
			cur=cJSON_GetObjectItem(cur,items[t].data());
			if(!cur){
				return 0;
			}
		}
		return cur;
	}

	static string cJSON_GetValueString(cJSON *cur){

		string ret;

		if(cur){

			if(cur->type==cJSON_String){
				ret=cur->valuestring;
			}else if(cur->type==cJSON_Number){
				ret=CommonHelp::DoubleToString(cur->valuedouble);
			}else if(cur->type==cJSON_True){
				ret="true";
			}else if(cur->type==cJSON_False){
				ret="false";
			}else if(cur->type==cJSON_Array || cur->type==cJSON_Object){

				char * out=	cJSON_Print(cur);
				if(out){
					ret=out;
					free(out);
				}
			}
//			else if(cur->type==cJSON_NULL){
//				ret="";
//			}

		}
		return ret;
	}



public:
	Json() {
		root=0;
		isRoot=false;
	}
	~Json() {
		Clear();
	}

	void Clear(){
		if(root && isRoot){
			cJSON_Delete(root);
			root=0;
		}
	}


	bool IsEmpty(){
		return root==0;
	}

	void Copy(const Json& json){
			this->Clear();
			isRoot=false;
			root=json.root;
		}

	bool Parse(const char * text){
		Clear();
		isRoot=true;
		root=cJSON_Parse(text);
		if (!root) {
			Logger::Error( string("cJSON_Parse Error:")+cJSON_GetErrorPtr());
		}
		return root;
	}

	bool CreateObject(){
		Clear();
		isRoot=true;
		root = cJSON_CreateObject();
		return root;
	}

	string ToString(){
		return cJSON_GetValueString(root);
	}

	string GetName(){
		string name;
		if(root){
			name=root->string;
		}
		return name;
	}


	void SetValue(const char*name,Json& value){
		if(!value.root)value.CreateObject();
		cJSON_AddItemToObject(root, name, value.root);
		value.isRoot=false;
	}




	void SetValue(const char*name,const char* value){
		cJSON_AddItemToObject(root, name, cJSON_CreateString(value));
	}

	void SetValue(const char*name,double value){
		cJSON_AddItemToObject(root, name, cJSON_CreateNumber(value));
	}

	void SetValue(const char*name,string& value){
		SetValue(name, value.c_str());
	}

	void SetValue(string & name,string& value){
		SetValue(name.c_str(), value.c_str());
	}

	void GetValue(const char*name,string & value){
		cJSON * cur=cJSON_GetObjectItem(root,name);
		value=cJSON_GetValueString(cur);
	}
	void GetValue(const char*name,Json & value){
		value.Clear();
		if(!root) return;
		cJSON * cur=cJSON_GetObjectItem(root,name);

		if(cur){
			value.isRoot=false;
			value.root=cur;
		}
	}

	string GetValueString(const char*name){
		string value;
		GetValue(name,value);
		return value;
	}


	bool GetValue(const char*name,bool & value){
			cJSON * cur=cJSON_GetObjectItem(root,name);

			if(cur){
				if(cur->type==cJSON_True){
						value=true;
						return true;
				}else if(cur->type==cJSON_False){
					value=false;
					return true;
				}
			}
			return false;
		}

//	void GetValue(const char*name,double & value){
//		cJSON * cur=cJSON_GetObjectItem(root,name);
//		if(cur){
//			value= cur->valueint;
//		}else
//			value=0;
//	}




	string GetStringByPath(const char * path)
	{
		cJSON * cur=GetChildByPath(path);
		return cJSON_GetValueString(cur);
	}

	int GetIntByPath(const char * path)
	{
		cJSON * cur=GetChildByPath(path);
		if(cur){
			return cur->valueint;
		}
		return 0;
	}

	double GetDoubleByPath(const char * path)
	{
		cJSON * cur=GetChildByPath(path);
		if(cur){
			return cur->valuedouble;
		}
		return 0;
	}



	void Foreach(void func(Json& item,void* lp),void* lp){
		if(!root) return;
		Json item;
		item.isRoot=false;


		cJSON * cur=root->child;
		while(cur){
			item.root=cur;

			func(item,lp);

			cur=cur->next;
		}
	}

};

}/* namespace Sers */
