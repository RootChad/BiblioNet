using BackOffice.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BackOffice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment webHostEnvironment;
        private string messageSuccess;
        private string messageError;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            webHostEnvironment = hostEnvironment;
        }
        public async Task<IActionResult> Login()
        {
           
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Authentification(LoginViewModel loginView)
        {


            System.Diagnostics.Debug.WriteLine(loginView);
            using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(loginView), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync("https://localhost:44385/api/Books/login", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        if (apiResponse == "true")
                        {
                            CheckSession.sess = true;
                        return RedirectToAction("Index");
                    }
                    else
                        {
                            CheckSession.sess = false;
                       return RedirectToAction("Login");
                        }
                        
                    }
                }
            return RedirectToAction("Index");
        }
        public IActionResult Logout()
        {
            CheckSession.sess = false;
            return RedirectToAction("Login");
        }
        [CheckSession]
        public async Task<IActionResult> Index()
        {
            List<Book> bookList = new List<Book>();
            dynamic mymodel = new ExpandoObject();
            
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:44385/api/Books"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    bookList = JsonConvert.DeserializeObject<List<Book>>(apiResponse);
                }
            }
            //mymodel.Book = bookList;
            //mymodel.BookViewModel = new BookViewModel();
            ViewBag.Book = bookList;
            ViewBag.success = messageSuccess;
            ViewBag.error = messageError;
           // ViewBag.BookViewModel = new BookViewModel();
            // return View(bookList);
            // return View(mymodel);
            return View();
        }
        public ViewResult GetBook() => View();
        [CheckSession]
        [HttpPost]
        public async Task<IActionResult> GetBook(int id)
        {
            Book book = new Book();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:44385/api/Books/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    book = JsonConvert.DeserializeObject<Book>(apiResponse);
                }
            }
            return View(book);
        }
        public ViewResult AddBook() => View();
        [CheckSession]
        [HttpPost]
        //public async Task<IActionResult> AddBook(Book book)
        //{
        //    Book newBook = new Book();
        //    using (var httpClient = new HttpClient())
        //    {
        //        StringContent content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");

        //        using (var response = await httpClient.PostAsync("https://localhost:44385/api/Books", content))
        //        {
        //            string apiResponse = await response.Content.ReadAsStringAsync();
        //            newBook = JsonConvert.DeserializeObject<Book>(apiResponse);
        //        }
        //    }
        //    //return View(newBook);
        //    return RedirectToAction("Index");
        //}

        public async Task<IActionResult> AddBook(BookViewModel bookView)
        {
            
            if (ModelState.IsValid)
            {
                
                this.messageSuccess = "Insertion reussie";
                this.messageError = "Erreur lors de l'insertion";
                string uniqueFileName = UploadedFileImage(bookView);
                string filepath= UploadedFile(bookView);
                Book newBook = null;
                Book book = new Book();
                book.Author = bookView.Author;
                book.Description = bookView.Description;
                book.ReleaseDate = bookView.ReleaseDate;
                book.Titre = bookView.Titre;
                book.Image = uniqueFileName;
                book.FileName = filepath;
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync("https://localhost:44385/api/Books", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        newBook = JsonConvert.DeserializeObject<Book>(apiResponse);
                    }
                }
                //return View(newBook);
                //if (newBook != null)
                //   this.messageSuccess = "Insertion reussie";
                //else
                //    this.messageError = "Erreur lors de l'insertion";
                TempData["msg"] = "Insertion reussie";
                
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> UpdateBook(int id)
        {
            Book reservation = new Book();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:44385/api/Books/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reservation = JsonConvert.DeserializeObject<Book>(apiResponse);
                }
            }
            return View(reservation);
        }
        [CheckSession]
        [HttpPost]
        public async Task<IActionResult> UpdateBook(BookViewModel bookView)
        {
            Book newBook = new Book();
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFileImage(bookView);
                string filepath = UploadedFile(bookView);
                Book book = new Book();
                book.BookID = bookView.BookID;
                book.Author = bookView.Author;
                book.Description = bookView.Description;
                book.ReleaseDate = bookView.ReleaseDate;
                book.Titre = bookView.Titre;
                book.Image = uniqueFileName;
                book.FileName = filepath;
                if (uniqueFileName == null)
                {
                    book.Image = bookView.Image;
                }
                else
                {
                    book.Image = uniqueFileName;

                }
                if (filepath == null)
                {
                    book.FileName = bookView.FileName;
                }
                else
                {
                    book.FileName = filepath;

                }
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PutAsync("https://localhost:44385/api/Books/" + book.BookID, content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        ViewBag.Result = "Success";
                        newBook = JsonConvert.DeserializeObject<Book>(apiResponse);
                    }
                }
                //return View(newBook);
                TempData["msg"] = "Mise a jour reussie";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
            //Book newBook = new Book();
            //using (var httpClient = new HttpClient())
            //{
            //    StringContent content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
            //    using (var response = await httpClient.PutAsync("https://localhost:44385/api/Books/"+book.BookID, content))
            //    {
            //        string apiResponse = await response.Content.ReadAsStringAsync();
            //        ViewBag.Result = "Success";
            //        newBook = JsonConvert.DeserializeObject<Book>(apiResponse);
            //    }
            //}
            //return RedirectToAction("Index");
        }
        [CheckSession]
        [HttpPost]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync("https://localhost:44385/api/Books/" + bookId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            TempData["msg"] = "Suppression reussie";
            return RedirectToAction("Index");
        }
        private string UploadedFileImage(BookViewModel model)
        {
            string uniqueFileName = null;

            if (model.ProfileImage != null)
            {
                string uploadsFolder = @"D:\EtudesMaster\M2\Naina\ProjetNet\FrontOffice\Content\images\upload";
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        private string UploadedFile(BookViewModel model)
        {
            string uniqueFileName = null;

            if (model.File != null)
            {
                string uploadsFolder = @"D:\EtudesMaster\M2\Naina\ProjetNet\FrontOffice\Content\pdf";
                uniqueFileName = model.File.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.File.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
