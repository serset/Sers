using App.SersKit.Station.Controllers.ErrorCollector;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using Sers.Core.Module.Api;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.Api.Rpc;
using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.Log;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.Core.Util.Common;
using Sers.Framework.Orm.Dapper.SqlHelp;
using System;
using Dapper;
using Sers.Core.Util.SsError;
using Sers.Core.Extensions;
using Newtonsoft.Json.Linq;
using Sers.FrameWork.Util.Mail;
using System.Threading.Tasks;
using FrameWork.Net;

namespace App.StationDemo.Station.Controllers.Demo
{
 
    //路由前缀，可不指定
    [SsRoutePrefix("Ticket")]
    public class TicketController : IApiController
    {



        static readonly string dbPath = CommonHelp.GetAbsPathByRealativePath("Data", "SersKit", "Ticket.db");

        static readonly string[] handlerEmails = Sers.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<string[]>("Ticket.handler.emails");
        static readonly string closeUrl = Sers.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<string>("Ticket.handler.closeUrl");

        
        /// <summary>
        /// 推送工单
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        [SsRoute("push")]
        [SsCallerSource(ECallerSource.Internal)]
        public ApiReturn  Push(TicketItem ticket)
        {
            #region (x.1) 入库
            try
            {
                #region 整理数据
                ticket.collectTime = DateTime.Now;
                ticket.ticket_id = CommonHelp.NewGuidLong();
                ticket.ticket_state = "open";
              
                #endregion

                using (var conn = SqliteHelp.GetOpenConnection(dbPath))
                {
                    conn.Insert(ticket);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return ex;
            }
            #endregion

            #region (x.2)发送消息（Email）
            try
            {
                if (handlerEmails != null && handlerEmails.Length > 0)
                {
                    Task.Run(() =>
                    {
                        foreach (var email in handlerEmails)
                        {
                            SendOpenTicketEmail(ticket, email);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return ex;
            }
            #endregion

            return true;
        }



        /// <summary>
        /// 关闭工单
        /// </summary>
        /// <param name="ticket_id"></param>
        /// <param name="handler"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        [SsRoute("close")]
        public ApiReturn Close(long ticket_id,string handler, string remarks)
        {
            TicketItem ticket;
            #region (x.1) 获取工单
            try
            {                
                using (var conn = SqliteHelp.GetOpenConnection(dbPath))
                {
                    ticket = conn.QueryFirstOrDefault<TicketItem>($"select * from {TicketItem.tableName} where ticket_id={ticket_id}");                     
                }
                if (ticket == null)
                {
                    return new SsError { errorMessage="错误：工单不存在" };
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return ex;
            }
            #endregion

            #region (x.2)处理工单
            ticket.ticket_state = "close";
            try
            {
                //[{handleTime:"2019-02-02 02:02:02",handler:'123@163.com'}]
                var log = ticket.ticket_log.ConvertBySerialize<JArray>()??new JArray();
                var logItem = new JObject
                {
                    ["handleTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    ["handler"] = handler                    
                };

                if (!string.IsNullOrEmpty(remarks))
                {
                    logItem["remarks"] = remarks;
                }

                log.Add(logItem);

                ticket.ticket_log = log.ToString();
                ticket.ticket_state = "close";

                using (var conn = SqliteHelp.GetOpenConnection(dbPath))
                {
                    if (!conn.Update<TicketItem>(ticket))
                    {
                        return new SsError { errorMessage = "错误：更新工单失败" };
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return ex;
            }
            #endregion


            #region (x.3)发送消息（Email）
            try
            {
                if (handlerEmails != null && handlerEmails.Length > 0)
                {
                    Task.Run(() =>
                    {
                        foreach (var email in handlerEmails)
                        {
                            SendCloseTicketEmail(ticket, email, handler);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return ex;
            }
            #endregion

            return true;
        }


        #region email
        static void SendOpenTicketEmail(TicketItem ticket,string email)
        {
            try
            {
                var html = "<h1>新工单</h1>";

                html += "<br/>工单id：" + ticket.id;
                html += "<br/>工单等级：" + ticket.ticket_level;
                html += "<br/>工单超时时间（小时）：" + ticket.ticket_timeoutHours;

                html += "<br/>工单类型：" + ticket.errorType;
                html += "<br/>工单发生时机：" + ticket.when;
                html += "<br/>工单原因：" + ticket.reason;
                html += "<br/>推送工单的站点：" + ticket.stationName;
                html += "<br/>工单发生时间：" + ticket.occurTime?.ToString("yyyy-MM-dd HH:mm:ss");
                html += "<br/>工单描述：" + ticket.description;
                html += "<br/>工单如何处理：" + ticket.howToHandle;

                html += "<br/>关闭工单:  <a href=\""+closeUrl+ "?ticket_id="+ticket.ticket_id+ "&handler="+ HttpUtil.UrlEncode(email) + "\">关闭工单</a>";


                MailHelp.Send(email, "亮家网工单(" + ticket.id + ")-" + ticket.errorType, html,true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);            
            }
        }


        static void SendCloseTicketEmail(TicketItem ticket, string email, string handler)
        {
            try
            {
                var html = "<h1>工单已关闭</h1>";               

                html += "<br/>工单id：" + ticket.id;
                html += "<br/>工单等级：" + ticket.ticket_level;
                html += "<br/>工单超时时间（小时）：" + ticket.ticket_timeoutHours;

                html += "<br/>工单类型：" + ticket.errorType;
                html += "<br/>工单发生时机：" + ticket.when;
                html += "<br/>工单原因：" + ticket.reason;
                html += "<br/>推送工单的站点：" + ticket.stationName;
                html += "<br/>工单发生时间：" + ticket.occurTime?.ToString("yyyy-MM-dd HH:mm:ss");
                html += "<br/>工单描述：" + ticket.description;
                html += "<br/>工单如何处理：" + ticket.howToHandle;

                html += "<br/>关闭人：" + handler;

                MailHelp.Send(email, "亮家网工单(" + ticket.id + ")已关闭-" + ticket.errorType, html, true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        #endregion

    }
}
