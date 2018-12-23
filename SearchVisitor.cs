using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomDataProvider_Demo
{
  public  class SearchVisitor : QueryExpressionVisitorBase
    {
        public string SearchKeyWords { get; private set; }
        public override QueryExpression Visit(QueryExpression query)
        {

            if (query.Criteria.Conditions.Count == 0)
                return null;

            ////Get the first filter vallue
            SearchKeyWords = query.Criteria.Conditions[0].Values[0].ToString();

            //var filter = query.Criteria;
            //foreach (ConditionExpression condition in filter.Conditions)
            //{
            //  //  if (condition.Operator == ConditionOperator.Like && condition.Values.Count > 0)
            //    if (condition.Values.Count > 0)
            //        {
            //        string exprVal = (string)condition.Values[0];

            //        if (exprVal.Length > 2)
            //        {
            //            this.SearchKeyWord += "," + exprVal.Substring(1, exprVal.Length - 2);
            //        }
            //    }
            //}

            return query;
        }

    }
   


    public class QueryResult
    {
        [JsonProperty]
        public Guid StudentId { get; set; }

        [JsonProperty]
        public string name { get; set; }

        [JsonProperty]
        public string familyaddress { get; set; }

        [JsonProperty]
        public string phoneno { get; set; }

        [JsonProperty]
        public int age { get; set; }

        [JsonProperty]
        public int grade { get; set; }
    }

    public class OData
    {
        [JsonProperty]
        public string Metadata { get; set; }
        public List<QueryResult> Value { get; set; }
    }
}

