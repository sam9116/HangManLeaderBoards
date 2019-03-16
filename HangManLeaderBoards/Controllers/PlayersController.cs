using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HangManLeaderBoards.Controllers
{
    class Player
    {

        public string id { get; set; }
        public int Roundssurvived { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        // GET api/Players
        [HttpGet]
        public async Task<JsonResult> Get()
        {
            try
            {
                MobileServiceSQLiteStore Store = new MobileServiceSQLiteStore("local.db");
                Store.DefineTable<Player>();
                await Store.InitializeAsync();
                var result = await Store.ExecuteQueryAsync("Player", "Select * From Player", new Dictionary<string, object>());
                return new JsonResult(result);
            }
            catch(Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        // GET api/Players/Sam
        [HttpGet("{username}")]
        public async Task<JsonResult> Get(string username)
        {
            try
            {
                MobileServiceSQLiteStore Store = new MobileServiceSQLiteStore("local.db");
                Store.DefineTable<Player>();
                await Store.InitializeAsync();


                var result = await Store.LookupAsync("Player", username);
                Player p = null;
                try
                {
                    p = JsonConvert.DeserializeObject<Player>(result.ToString());
                    p.Roundssurvived++;
                }
                catch
                {

                }
                if (p == null)
                {
                    p = new Player() { id = username, Roundssurvived = 0 };
                }

                await Store.UpsertAsync("Player", new List<JObject>() { JObject.FromObject(p) }, true);

            }
            catch(Exception ex)
            {
                return new JsonResult(ex.Message);
            }
            return new JsonResult("200");
        }
    }
}
