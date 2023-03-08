using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FirebaseMvc.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseMvc.Controllers;

public class HomeController : Controller
{

    public IActionResult Index()
    {
        return View();
    }

    public async Task<ActionResult> About()
    {
        //Simulate test user data and login timestamp
        var userId = "12345";
        var currentLoginTime = DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");

        //Save non identifying data to Firebase
        var currentUserLogin = new LoginData() { TimestampUtc = currentLoginTime };
        var firebaseClient = new FirebaseClient("https://fir-mvc-9d77e-default-rtdb.europe-west1.firebasedatabase.app/");
        var result = await firebaseClient
          .Child("Users/" + userId + "/Logins")
          .PostAsync(currentUserLogin);

        //Retrieve data from Firebase
        var dbLogins = await firebaseClient
          .Child("Users")
          .Child(userId)
          .Child("Logins")
          .OnceAsync<LoginData>();

        var timestampList = new List<DateTime>();

        //Convert JSON data to original datatype
        foreach (var login in dbLogins)
        {
            timestampList.Add(Convert.ToDateTime(login.Object.TimestampUtc).ToLocalTime());
        }

        //Pass data to the view
        ViewBag.CurrentUser = userId;
        ViewBag.Logins = timestampList.OrderByDescending(x => x);
        return View();
    }


}
