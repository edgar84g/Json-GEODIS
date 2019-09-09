using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using Newtonsoft.Json;

namespace GEODIS.Controllers
{
    public class TextController : Controller
    {
        [HttpGet("api/Nick")]
        public IActionResult Get()
        {
            return Ok(new { name = "Nick" });
        }



        [HttpPut("api/users")]
        public string users([FromBody] string s)
        {
            if (s == null)
                return "";

            return WorkJson(s.ToString());

        }

        [HttpPut("api/userALL")]
        public string userALL([FromBody] List<all> s)
        {
            
            if (s == null)
                return "";

            return WorkJson(s.ToString());
        }

        [HttpPut("api/userJ")]
        public string put([FromBody] JArray s)
        {
          
            if (s == null)
                return "";

            
            return WorkJson(s.ToString());

        }

        
        [HttpPut("api/user")]
        public IActionResult GetAction([FromBody]string s)
        {
            if (s == null)
                return Ok();

            return Ok(WorkJson(s));
        }

    
        private string WorkJson(string sJosn)
        {

            List<string> list = sJosn.Split(',').ToList();


            List<int> matchingIndexes = (from current in list.Select((value, index) => new { value, index })
                                         where current.value.Contains("_entity") select current.index).ToList();

            matchingIndexes.Add(list.Count);

            List<stUser> lUser = new List<stUser>();
            List<stBlogs> lBlogs = new List<stBlogs>();
            List<stComments> lComments = new List<stComments>();

   
            for (int i = 0; matchingIndexes.Count - 1 > i; i++)
            {
                string _entity = list[matchingIndexes[i]];

                if (list[matchingIndexes[i]].Contains("User"))
                {
                    stUser nUser = addUser(list, matchingIndexes[i], matchingIndexes[i + 1]);
                    if (!lUser.Exists(u => u.id == nUser.id))
                        lUser.Add(nUser);

                }

                if (list[matchingIndexes[i]].Contains("Blog"))
                    lBlogs.Add(addBlogs(list, matchingIndexes[i], matchingIndexes[i + 1], lUser[lUser.Count - 1].id));

                if (list[matchingIndexes[i]].Contains("Comment"))
                {
                    stComments comments = addComments(list, matchingIndexes[i], matchingIndexes[i + 1]);

                    i++;

                    stUser nUserCom = addUser(list, matchingIndexes[i], matchingIndexes[i + 1]);
                    
                    if (!lUser.Exists(u => u.id == nUserCom.id))
                        lUser.Add(nUserCom);

                    comments.UserId = nUserCom.id;
                    comments.blogId = lBlogs[lBlogs.Count - 1].id;

                    lComments.Add(comments);
                }
            }

            string rJson="";
            lUser = lUser.OrderBy(o=> o.id).ToList();

            rJson = "\"Users\":"+ JsonConvert.SerializeObject(lUser) ;

            rJson += ",\"blogs\":" + JsonConvert.SerializeObject(lBlogs) ;

            rJson += ",\"Comments\":" + JsonConvert.SerializeObject(lComments)  ;

            return"["+ rJson+"]";

        }

        private stUser addUser(List<string> lst, int idx, int idxM)
        {
            stUser st_User = new stUser();
            st_User.blogs = new List<stBlogs>();

            List<string> Nlist = lst.GetRange(idx, idxM - idx);
 
            var sourceProperties = typeof(stUser).GetProperties();

            foreach (PropertyInfo info in sourceProperties)
            {
                string Nm = info.Name;
                Type type = info.PropertyType;

                string Row = Nlist.Find(n => n.Contains(Nm));

                if (string.IsNullOrEmpty(Row))
                    continue;

                string[] spRow = Row.Split(':');

                string value = spRow[spRow.Length - 1].Replace("/t", "")
                                   .Replace("]", "")
                                   .Replace("}", "")
                                   .Replace('"',' ')
                                   .Trim() ;

                if(info.PropertyType == typeof(int))
                {
                    int ivalue = int.Parse(value);
                    info.SetValue(st_User, ivalue, null);
                }
                if (info.PropertyType == typeof(string))
                {
                    string svalue = value;
                    info.SetValue(st_User, svalue, null);
                }
            }
            
            return st_User;
        }

        private stBlogs addBlogs(List<string> lst, int idx, int idxM, int userId)
        {
            stBlogs newst = new stBlogs();
            newst.userId=userId;

            List<string> Nlist = lst.GetRange(idx, idxM - idx);

            var stBlogsProperties = typeof(stBlogs).GetProperties();

            foreach (PropertyInfo info in stBlogsProperties)
            {
                string Nm = info.Name;
                Type type = info.PropertyType;

                string Row = Nlist.Find(n => n.Contains(Nm));

                if (string.IsNullOrEmpty(Row))
                    continue;

                string[] spRow = Row.Split(':');

                string value = spRow[spRow.Length-1].Replace("/t", "")
                                   .Replace("]", "")
                                   .Replace("}", "")
                                   .Replace('"', ' ')
                                   .Trim();

                if (info.PropertyType == typeof(int))
                {
                    int ivalue = int.Parse(value);
                    info.SetValue(newst, ivalue, null);
                }
                if (info.PropertyType == typeof(string))
                {
                    string svalue = value;
                    info.SetValue(newst, svalue, null);
                }
            }

            return newst;
        }

        private stComments addComments(List<string> lst, int idx, int idxM )
        {
            stComments newst = new stComments();
            
            List<string> Nlist = lst.GetRange(idx,idxM-idx);

            var sourceProperties = typeof(stComments).GetProperties();

            foreach (PropertyInfo info in sourceProperties)
            {
                string Nm = info.Name;
                Type type = info.PropertyType;

                string Row = Nlist.Find(n => n.Contains(Nm));

                if (string.IsNullOrEmpty(Row))
                    continue;

                string[] spRow = Row.Split(':');

                string value = spRow[spRow.Length-1].Replace("/t", "")
                                   .Replace("]", "")
                                   .Replace("}", "")
                                   .Replace('"', ' ')
                                   .Trim();

                if (info.PropertyType == typeof(int))
                {
                    int ivalue = int.Parse(value);
                    info.SetValue(newst, ivalue, null);
                }
                if (info.PropertyType == typeof(string))
                {
                    string svalue = value;
                    info.SetValue(newst, svalue, null);
                }
            }

            return newst;
        }

    }

    public class all
    {
        public List<stUser> luser { get; set; }

    }

    public class stUser
    {
        public string _entity { get; set; }
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int age { get; set; }
        public List<stBlogs> blogs { get; set; }
    }
    public class stBlogs
    {
        public string _entity { get; set; }
        public int id { get; set; }
        public int userId { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        

    }
    public class stComments
    {
        public string _entity { get; set; }
        public int id { get; set; }
        public int blogId { get; set; }
        public string body { get; set; }
        public int UserId { get; set; }
        
    }
}
