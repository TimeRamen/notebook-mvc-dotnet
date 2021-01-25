using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Z01.Models;

namespace Z01.Controllers
{
    public class NotepadController : Controller
    {

//GET: Notepad
        public IActionResult Random()
        {
            var note = new NotepadModel(){ Name ="Lorem", Categories=new string[]{"Hello","World!"}};


            return View(note);
        }
    }
}