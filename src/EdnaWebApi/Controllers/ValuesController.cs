using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections;
using InStep.eDNA.EzDNAApiNet;
using System.Globalization;
using EdnaWebApi.Dtos;

namespace EdnaWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {

        private readonly ILogger<ValuesController> _logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }

        // GET api/values
        [HttpGet(Name = "ValuesGetTest")]
        public object Get()
        {
            dynamic ret = new JObject();
            ret.name1 = new JArray { new[] { "name1", "name2" } };
            ret.name1.Add("name3");
            ret.name2 = "name4";
            return new RealResult { Dval = 10, Timestamp = DateTime.Now, Status = "asfsa", Units = "hrht" };
        }

        // GET api/values/history?type=snap&pnt=something&strtime=30/11/2016/00:00:00&endtime=30/11/2016/23:59:00&secs=60
        // GET api/values/real?pnt=something
        [HttpGet("{id}", Name = "GetData")]
        public object GetData(string id, [FromQuery] string pnt = "WRLDC.PHASOR.WRDC0783", [FromQuery] string strtime = "30/11/2016/00:00:00", [FromQuery] string endtime = "30/11/2016/23:59:00", [FromQuery] int secs = 60, [FromQuery] string type = "snap", [FromQuery] string service = "WRDCMP.SCADA1")
        {
            int nret = 0;
            string format = "dd/MM/yyyy/HH:mm:ss";
            if (id == "history")
            {
                //get history values
                ArrayList historyResults = new();
                try
                {
                    uint s = 0;
                    double dval = 0;
                    DateTime timestamp = DateTime.Now;
                    string status = "";
                    TimeSpan period = TimeSpan.FromSeconds(secs);
                    //history request initiation
                    if (type == "raw")
                    { nret = History.DnaGetHistRaw(pnt, DateTime.ParseExact(strtime, format, CultureInfo.InvariantCulture), DateTime.ParseExact(endtime, format, CultureInfo.InvariantCulture), out s); }
                    else if (type == "snap")
                    { nret = History.DnaGetHistSnap(pnt, DateTime.ParseExact(strtime, format, CultureInfo.InvariantCulture), DateTime.ParseExact(endtime, format, CultureInfo.InvariantCulture), period, out s); }
                    else if (type == "average")
                    { nret = History.DnaGetHistAvg(pnt, DateTime.ParseExact(strtime, format, CultureInfo.InvariantCulture), DateTime.ParseExact(endtime, format, CultureInfo.InvariantCulture), period, out s); }
                    else if (type == "min")
                    { nret = History.DnaGetHistMin(pnt, DateTime.ParseExact(strtime, format, CultureInfo.InvariantCulture), DateTime.ParseExact(endtime, format, CultureInfo.InvariantCulture), period, out s); }
                    else if (type == "max")
                    { nret = History.DnaGetHistMax(pnt, DateTime.ParseExact(strtime, format, CultureInfo.InvariantCulture), DateTime.ParseExact(endtime, format, CultureInfo.InvariantCulture), period, out s); }

                    while (nret == 0)
                    {
                        nret = History.DnaGetNextHist(s, out dval, out timestamp, out status);
                        if (status != null)
                        {
                            historyResults.Add(new HistResult { Dval = dval, Timestamp = timestamp, Status = status });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while fetching history results " + ex.Message);
                    historyResults = new ArrayList();
                }
                return historyResults;
            }
            else if (id == "real")
            {
                RealResult realVal;
                try
                {
                    nret = RealTime.DNAGetRTAll(pnt, out double dval, out DateTime timestamp, out string status, out string desc, out string units);//get RT value
                    if (nret == 0)
                    {
                        realVal = new RealResult { Dval = dval, Timestamp = timestamp, Status = status, Units = units };
                        return realVal;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while fetching realtime result " + ex.Message);
                    return null;
                }
                return null;
            }
            else if (id == "longtoshort")
            {
                string shortId;
                try
                {
                    Configuration.ShortIdFromLongId(service, pnt, out shortId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while fetching longtoshort result " + ex.Message);
                    shortId = "";
                }
                return new { shortId };
            }
            else
            {
                return null;
            }
        }
    }
}