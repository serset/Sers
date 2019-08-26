
#include "HttpServer.hpp"


namespace Sers {

	mg_mgr HttpServer::m_mgr;
	sock_t HttpServer::sock[2];

	bool HttpServer::isRunning=false;
	LongTaskHelp HttpServer::taskToPoll;
	string HttpServer::m_port;
	int HttpServer::workThreadCount=16;

	string HttpServer::responseDefaultContentType="application/json; charset=UTF-8";

	string HttpServer::responseExtHeaders;


	HttpRequestHandler HttpServer::OnHttpRequest;




} /* namespace Sers */
