#pragma once


#include "../../Data/ByteData.hpp"
#include <vector>

namespace Sers {

using namespace std;


class SersFile {
public:
	 vector<ArraySegment> Files;


	 void Unpack(ArraySegment & oriData){
		 Files.clear();
		 int index=0;
		 int len=oriData.len;
		 char* byte=oriData.GetData();


		 int * fileLen;
		 while(index+4<=len){

			 //(x.x.1)len
			 fileLen=(int*)(byte+index);
			 index+=4;

			 //(x.x.2)content
			 ArraySegment file;
			 file.CopyFrom(byte+index,*fileLen);
			 index+=(*fileLen);

			 Files.push_back(file);
		 }
	 }

	 ArraySegment Package(){
		 ArraySegment data;
		 //(x.1) get byte len
		 int len=0;
		 for(int t=0;t<Files.size();t++){
			 ArraySegment& file=Files[t];
			 len+=file.len+4;
		 }

		 //(x.2) copy byte
		 data.Malloc(len);

		 char * cur=data.GetData();
		 for(int t=0;t<Files.size();t++){
			 ArraySegment& file=Files[t];
			 len=file.len;
			 memcpy(cur,&len,4);
			 cur+=4;

			 memcpy(cur,file.GetData(),len);
			 cur+=len;
		 }
		 return data;

	 }


};


} /* namespace Sers */


