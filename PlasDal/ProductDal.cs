﻿using PlasCommon.SqlCommonQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasDal
{
    public class ProductDal
    {

        #region 产品详情：查询型号名称及物性
        public DataSet GetModelInfo(string pguid)
        {
            var ds = SqlHelper.GetSqlDataSet(string.Format(@"exec PROC_GetInfoByPguid '{0}' ", pguid));
            return ds;
        }

        #endregion


        #region 热门产品
        /// <summary>
        /// 按点击次数，获取指定数量热门产品，默认5条
        /// </summary>
        /// <param name="showNum"></param>
        /// <returns></returns>
        public DataTable HotProducts(int showNum = 5)
        {
      
            var ds = SqlHelper.GetSqlDataTable(string.Format(@"select a.*,(select top 1 ProductID from dbo.ProductLevel1 b where a.SmallClassId=b.SmallClassID) as ProductID from
  (SELECT TOP {0} BigClassId,SmallClassId,ProModel,PlaceOrigin,ProductGuid,HitCount,Brand,Edition
  FROM dbo.Product where isShow=1 order by HitCount desc) a ", showNum));
            return ds;

        }
        #endregion


        #region 感兴趣的产品和同一个厂家产出的产品

        public DataSet GetCompanyAndLiveProduct(int topnum, string productID)
        {
            //---同一个厂家
            //---感兴趣的产品，同类产品
            string sql = string.Format(@"select top {0} a.ProModel,a.ProductGuid from Product as a 
                inner join Product as b on a.PlaceOrigin=b.PlaceOrigin
                where b.productguid='{1}'
                order by NewID()
                select top {2} a.ProModel,a.ProductGuid from Product as a 
                inner join Product as b on a.SmallClassID=b.SmallClassID
                where b.productguid='{3}'
                order by NewID()", topnum, productID, topnum, productID);
            var ds = SqlHelper.GetSqlDataSet(sql.ToString());
            return ds;
        }

        #endregion


        #region 超级搜索查询数据
        /// <summary>
        /// 数据库中根据中英文根据首字母排序
        /// </summary>
        /// <param name="type"></param>
        /// <param name="showNum"></param>
        /// <returns></returns>
        public DataTable GetSearchParam(int type, int showNum = 10)
        {
            var ds = new DataSet();
            switch (type)
            {
                case 1://产品种类
                    ds = Sys_GetSmallClassList(showNum);
                    break;
                case 2://特性
                    ds = Sys_GetSearchParam("Sys_character", showNum, " substring(dbo.[fn_ChineseToSpell](name),1,1) as fw,* ", " and name<>'''' and name<>''--''", " substring(dbo.[fn_ChineseToSpell](name),1,1),name ");
                    break;
                case 3://阻燃等级
                    ds = Sys_GetSearchParam("Prd_ZRDJSort", showNum, "  substring(dbo.[fn_ChineseToSpell](KeyWord),1,1) as fw,KeyWord as Name,* ", "", " value desc ,substring(dbo.[fn_ChineseToSpell](KeyWord),1,1),KeyWord ");
                    break;
                case 4://厂家
                    ds = Sys_GetSearchParam("Sys_Manufacturer", showNum, "  substring(dbo.[fn_ChineseToSpell](AliasName),1,1) as fw,AliasName as Name,*", " and AliasName<>'''' and AliasName<>''--''", "  substring(dbo.[fn_ChineseToSpell](AliasName),1,1),AliasName ");
                    break;
                case 5://加工方法
                    ds = Sys_GetSearchParam("Sys_Method", showNum, " substring(dbo.[fn_ChineseToSpell](name),1,1) as fw,* ", " and name<>'''' and name<>''--''", " substring(dbo.[fn_ChineseToSpell](name),1,1),name ");
                    break;
                case 6://安全级别(阻燃等级的二级属性)
                    ds = Sys_GetSearchParam("Prd_SmallClass", showNum, " substring(dbo.[fn_ChineseToSpell](name),1,1) as fw,* ", " and name<>'''' and name<>''--''", " substring(dbo.[fn_ChineseToSpell](name),1,1),name ");//Weigth字段加在了Prd_SmallClass_l表
                    break;
                case 7://用途
                    ds = Sys_GetSearchParam("Sys_ForUse", showNum, " substring(dbo.[fn_ChineseToSpell](name),1,1) as fw,* ", " and name<>'''' and name<>''--''", " substring(dbo.[fn_ChineseToSpell](name),1,1),name ");
                    break;
                case 8://填料
                    ds = Sys_GetSearchParam("Sys_filler", showNum, " substring(dbo.[fn_ChineseToSpell](name),1,1) as fw,* ", " and name<>'''' and name<>''--''", "  substring(dbo.[fn_ChineseToSpell](name),1,1),name ");
                    break;
                case 9://添加剂
                    ds = Sys_GetSearchParam("Sys_Additive", showNum, " substring(dbo.[fn_ChineseToSpell](name),1,1) as fw,* ", " and name<>'''' and name<>''--''", "  substring(dbo.[fn_ChineseToSpell](name),1,1),name ");
                    break;
                default:
                    ds = Sys_GetSearchParam("Prd_SmallClass", showNum, " substring(dbo.[fn_ChineseToSpell](name),1,1) as fw,* ", " and name<>'''' and name<>''--''", "  substring(dbo.[fn_ChineseToSpell](name),1,1),name ");
                    break;
            }
            return ds.Tables[0];
        }

        public DataSet Sys_GetSearchParam(string tableName, int pageSize = 10, string filterColumns = "", string whereStr = "", string orderBy = "weight desc", int pageIndex = 1)
        {
            string sql = string.Format("exec pr_common_page '{0}','{1}','{2}','{3}',{4},{5}", tableName, filterColumns, whereStr, orderBy, pageIndex, pageSize);
            var ds = SqlHelper.GetSqlDataSet(sql);
            return ds;
        }

        public DataSet Sys_GetSmallClassList(int pageSize = 10, int pageIndex = 1)
        {
            string sql = string.Format("exec PROC_GetSmallClassList {0},{1}", pageIndex, pageSize);
            var ds = SqlHelper.GetSqlDataSet(sql);
            return ds;
        }

        //属性值
        public DataSet Sys_GetSuperSearchParam(int parentID = -1)
        {
            string sql = string.Format("exec proc_supersearch_getParam {0}", parentID);
            var ds = SqlHelper.GetSqlDataSet(sql);
            return ds;
        }


        //超级搜索
        public DataSet Sys_SuperSearch(string searchStr = "", int languageid = 2052, int pageCount = 0, int pageSize = 20, string guidstr = "")
        {
            //{产品种类=>''尼龙 6 弹性体;MMS;EA'',0,0}{产品特性=>''Broad Seal Range ;半结晶'',0,0}	
            // { 机械性能 =)拉伸模量 => '''',0,10600}
            TaskFactory taskfactory = new TaskFactory();
            List<Task> taskList = new List<Task>();
            //为每个条件开启一个线程
            int taskts = searchStr.Length - searchStr.Replace("}", "").Length;//Environment.ProcessorCount
            string ver = guidstr;//Guid.NewGuid().ToString();
            if (pageCount < 2)//分页的时候，不执行数据查询
            {
                for (int i = 1; i <= taskts; i++)
                {
                    //产生一个查询条件
                    string onesql = searchStr.Substring(1, searchStr.IndexOf("}") - 1);
                    //一个线程执行一个查询条件
                    string execsql = string.Format("exec suppersearchone '{0}','{1}',{2},{3}", ver, onesql, languageid, i);
                    taskList.Add(taskfactory.StartNew(() =>
                    {
                        SqlHelper.ExectueNonQuery(SqlHelper.ConnectionStrings, execsql, null);
                    }));
                    searchStr = searchStr.Replace(string.Format("{0}{1}{2}", "{", onesql, "}"), "");
                }
            }
            //等待所有线程执行完成
            Task.WaitAll(taskList.ToArray());
            //所有线程执行完成后再将全部结果求交集
            string sql = string.Format("exec suppersearchmerge '{0}',{1},{2},{3}", ver, taskts, pageCount, pageSize);
            //Common.Common.AddLog("system", "执行超级搜索", sql.ToString(), "");
            var ds = SqlHelper.GetSqlDataSet(sql);
            return ds;
        }

        #endregion

        #region 普通搜索

        /// <summary>
        /// DataTable dt2 = dv.ToTable(true, "name,age,hobby");去重
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="filedNames"></param>
        /// <returns></returns>
        public static DataTable Distinct(DataTable dt, string[] filedNames)
        {
            DataView dv = dt.DefaultView;
            DataTable DistTable = dv.ToTable("Dist", true, filedNames);
            return DistTable;
        }

        //普通搜索
        public DataSet GetGeneralSearch(string key = "", int pageIndex = 1, int pageSize = 20, string strGuid = "")
        {
            string sql = string.Format("exec headsearch '{0}','{1}',{2},{3},{4}", strGuid, key, 2052, pageIndex, pageSize);
            var ds = SqlHelper.GetSqlDataSet(sql);
            return ds;
        }

        //       @pageIndex int,
        //@pageSize int,
        //@ver varchar(100),
        //@Characteristic varchar(300),--特性
        //@Used varchar(300),--用途
        //@Kind varchar(300),--种类
        //@Method varchar(300),--方法
        //@Factory varchar(300),--厂家
        //@Additive varchar(300),--添加剂
        //@AddingMaterial varchar(300)--增料
        public DataSet GetTwoSearch(int pageIndex, int pageSize, string ver, string Characteristic, string Used, string Kind, string Method, string Factory, string Additive, string AddingMaterial)
        {
            string sql = string.Format("exec Get_TwoHeadSearchTmpData {0},{1},'{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'", pageIndex, pageSize, ver, Characteristic, Used, Kind, Method, Factory, Additive, AddingMaterial);
            var ds = SqlHelper.GetSqlDataSet(sql);
            return ds;
        }

        #endregion

    }
}
