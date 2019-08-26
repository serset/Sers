#pragma once



#include "../../Data/ByteData.hpp"
#include "../../Module/Message/SersFile.hpp"
#include "../../Module/Api/RpcData.hpp"


namespace Sers {

typedef ArraySegment (*FuncSendRequest)(ArraySegment & request);


struct HttpApiContext{

	string route;
	// GET POST PUT ...
	string rpc_method;
	string rpc_callerSource;
	string rpc_url;

	//json
	Json request_header;
	ArraySegment request_body;

	//json
	Json response_header;
	ArraySegment response_body;

};



class ApiClient {

private:
	 static void CallApi(ArraySegment & request,ArraySegment& reply){
		 	 reply= OnSendRequest(request);
	 }

	 static void BuildApiRequestMessage(SersFile & apiRequestMessage,HttpApiContext & apiContext){
			ArraySegment  request_rpcData;

			//(x.1) build RpcContextData
			RpcContextData rpcData;
			rpcData.SetRouteAndCaller(apiContext.route,apiContext.rpc_callerSource);

			//(x.2) build RpcContextData header
			rpcData.SetHttp(apiContext.rpc_url,apiContext.rpc_method,apiContext.request_header);


			//(x.3) request_rpc
			string rpcString=rpcData.ToString();
			request_rpcData.CopyFrom(rpcString.c_str(),rpcString.length());


			//(x.4) ApiMessage_Package
			ApiMessage_Package(apiRequestMessage,request_rpcData, apiContext.request_body);
	}


	 static void CallApi(SersFile & apiRequestMessage,SersFile & apiReplyMessage){
		 ArraySegment request=apiRequestMessage.Package();
		 ArraySegment reply;
		 CallApi(request,reply);
		 apiReplyMessage.Unpack(reply);
	 }



	 static void ApiMessage_Package(SersFile & apiMessage,ArraySegment& rpcData,ArraySegment& value){
		 apiMessage.Files.clear();
		 //file0
		 apiMessage.Files.push_back(rpcData);
		 //file1
		 apiMessage.Files.push_back(value);
	 }

	 static void ApiMessage_Unpackage(SersFile & apiMessage,ArraySegment& rpcData,ArraySegment& value){
		//file0
		rpcData=apiMessage.Files[0];
		//file1
		value=apiMessage.Files[1];
	 }

public:
	 static FuncSendRequest OnSendRequest;


	 static void CallApi(HttpApiContext & apiContext){

		 //(x.1) build apiRequestMessage
		 SersFile  apiRequestMessage,apiReplyMessage;
		 BuildApiRequestMessage(apiRequestMessage,apiContext);

		 //(x.2) call  api
		 CallApi(apiRequestMessage,apiReplyMessage);

		 //(x.3) unpackage apiReplyMessage
		 ArraySegment  reply_rpcData;
		 ApiClient::ApiMessage_Unpackage(apiReplyMessage,reply_rpcData, apiContext.response_body);

		 //(x.4) get reply header
		 if(reply_rpcData.len>0){
			 RpcContextData rpcReplyData(reply_rpcData.GetData());
			 rpcReplyData.GetHttpHeaders( apiContext.response_header);
		 }else{
			 apiContext.response_header.Clear();
		 }

	 }

	 static void CallApi(string & route,string & request,string & reply){
		 	 HttpApiContext apiContext;
			 apiContext.route=route;

			 apiContext.rpc_callerSource="Internal";
			 apiContext.request_body.CopyFrom(request.c_str(),request.length());

			 CallApi(apiContext);

			 reply=apiContext.response_body.GetData();
	 }

	 static void CallApi(string & route,string & request,Json & reply){

		 string  strReply;
		 CallApi(route,request,strReply);

		 reply.Parse(strReply.c_str());
	 }





};


} /* namespace Sers */
