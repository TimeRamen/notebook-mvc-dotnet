using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Z01.Models;
//using System.IO;
using System.Text;
using System.Data;
using System.Web;


namespace Z01.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(int curPage = 1, string dir="")
        {      
            
            string[] note = PaginateFromDir(@"Notes/");
            
            if(dir == "next"){
                //if we go back a page.
                if(curPage<note.Length){
                curPage++;
                }
            }else if(dir == "back"){
                if(curPage!=1){
                    curPage--;
                }
                //if we go forward a page.
            }
            ViewBag.HTMLString = note[curPage-1];
            ViewBag.PageNum = note.Length;
            string note2 = PrintDropDownFromDir(@"Notes/");
            ViewBag.HTMLCat = note2;
            ViewBag.curPage = curPage;
            return base.View();
        }

        public IActionResult DeleteNote(int id)
        {
            DeleteFileID(id, new System.IO.DirectoryInfo(@"Notes/"));
            return Redirect("../");
        }

        public IActionResult SaveNote()
        {
            //sadly doesnt work.
            var noteToSave = returnTxtStringFromInput(Request.Form["title"],Request.Form["markdown"],Request.Form["note"],Request.Form["categories"]);
          /*
          
          //string cat = Request.Form["categories"];
            string Name = Request.Form["title"];
            string Content= Request.Form["note"];
            string Markdown = Request.Form["markdown"];
            //bool mark = false;

            string cat;
            if(Request.Form["categories"] == ""){
                cat="NoCat";
            }else{
                cat = Request.Form["categories"];
            }

            var noteToSave = returnTxtStringFromInput(Name,Markdown,Content,cat);

          
           */  
            writeNoteToPath(noteToSave,@"Notes/");
            return Redirect("../");
        }
        public IActionResult Compose(int id = -1){
            string note;
            if(id == -1){
                note = RenderNoteToHTML(emptyNote());
            }else{
                note = RenderNoteToHTML(GetNoteListFromPath(@".\Notes\")[id]);
            }
            ViewBag.HTMLString = note;
                return base.View(); 
            //find a way to get info of [i] from view
        }



      //  public ActionResult NewNote(){
      //      return View("Compose");
      //  }


//-------------------------Private Methods------------------------//
        private static string[] Paginate(NotepadModel[] noteList, int pageLimit){
            if(pageLimit >= noteList.Length){
                string temp = RenderNoteListToHTML(noteList);
                var returnString = new string[1]{temp};
                return returnString;
            }else{
                int pageAmount = noteList.Length/pageLimit;
                int endList = noteList.Length%pageLimit; 
                if(endList!=0){
                    pageAmount++;    
                }
                var multiNoteList = new NotepadModel[pageAmount][];
                var copyList = new NotepadModel[pageLimit];
                var endCopyList = new NotepadModel[endList];
                var returnHTMLString = new string[pageAmount];
                int curIndex = 0;
                for(int i = 0;i<pageAmount;i++){
                    if(i == pageAmount - 1){
                        for(int j = 0; j < endList ; j++){
                            endCopyList[j] = noteList[curIndex];
                            curIndex++;
                        }
                        multiNoteList[i] = endCopyList;
                    }else{
                        for(int j = 0; j < pageLimit; j++){
                            copyList[j] = noteList[curIndex];
                            curIndex++;
                        }
                        multiNoteList[i] = copyList;
                    }
                returnHTMLString[i] = RenderNoteListToHTML(multiNoteList[i]);
                }
                return returnHTMLString;
            }
        }
        private static NotepadModel emptyNote(){
            NotepadModel createdNote = new NotepadModel()
            {
                Name = "",
                Date = DateTime.Today,
                Categories = new string[0],
                Content = "",
                Markdown = false
            };
            return createdNote;
        }

        private static string[] PaginateFromDir(string path,int limit = 7){
        return Paginate(GetNoteListFromPath(path),limit);
        }
        private static NotepadModel getNoteFromString(string X)
        //Returns a notepad class from a string.
        {
            string noteTitle = "";
            string noteDate = "";
            string[] noteCategories = new string[] { "" };
            string noteContent = "";
            
            string[] textLines = X.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            //
            noteTitle = textLines[0].Substring(textLines[0].LastIndexOf(':') + 1);
            //
            noteDate = textLines[1].Substring(textLines[1].LastIndexOf(':') + 1);
            //
            string cur = textLines[2].Substring(textLines[2].LastIndexOf(':') + 1).Split('[', ']')[1];
            noteCategories = cur.Split(',');
            
            noteContent = string.Join(Environment.NewLine, textLines.Skip(4).ToArray());
            
            NotepadModel createdNote = new NotepadModel()
            {
                Name = noteTitle,
                Date = Convert.ToDateTime(noteDate),
                Categories = noteCategories,
                Content = noteContent
            };

            return createdNote;
        }

        private static void writeStringToPath(string Note, string Path)
        //save the TXT file into the directory
        {
            System.IO.File.WriteAllText(Path, Note);

        }

        private static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static void writeNoteToPath(NotepadModel Note, string Path=@"Notes/")
        //save the Notepad class into the directory
        {
            string title = RemoveSpecialCharacters(Note.Name);

            Path += title;
            if(Note.Markdown){
                Path+=".md";
            }else{
                Path+=".txt";
            }
            writeStringToPath(NoteFile(Note), Path);
        }

        private static string NoteFile(NotepadModel X)
        //turn notepad class into string
        {
            string curNote = "Title: ";
            string endSpace = "\n";
            curNote = curNote + X.Name + endSpace;
            curNote += "Date: ";
            curNote = curNote + X.Date.ToString("MM/dd/yyyy") + endSpace;

            curNote += "Categories: ";

            //string concat = string.Join(",",X.Categories);

            curNote = curNote + string.Join(",",X.Categories) + endSpace;

            curNote = curNote + endSpace + X.Content;
            return curNote;
        }

        private static NotepadModel getModelFromFile(string path)
        //return a notepad class from a particular path of a TXT file
        {
            string text = System.IO.File.ReadAllText(path);
            var returnModel = new NotepadModel();
            returnModel = getNoteFromString(text);
            returnModel.Markdown = CheckMarkdown(path);
            return returnModel;
        }
        private static bool CheckMarkdown(string path)
        //returns true if file is MD
        { 
            System.IO.FileInfo fi = new System.IO.FileInfo(path);  
            // Get file extension   
            string extn = fi.Extension;  
            if(extn == ".md"){
                return true;
            }else{
                return false;
            }
        }



            public static string ConvertDataTableToHTML(DataTable dt)
            //turn DataTable into a HTML table
            {
            string html = "<table>";
            //add header row
            html += "<thead><tr>";
            for(int i=0;i<dt.Columns.Count;i++)
                html+="<td>"+dt.Columns[i].ColumnName+"</td>";
            
            html += "</tr></thead><tbody>";
            //add rows
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                html += "<tr>";
                for (int j = 0; j< dt.Columns.Count; j++)
                    html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                    html += "</tr>";
            }
            html += "</tbody></table>";
        return html;
    }

    public static DataTable ConvertArrayToDataTable(NotepadModel[] NoteList)
    //turns an array of notepad objects into datatable
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("Date");
        dt.Columns.Add("Title");
        dt.Columns.Add("Actions");

        int length = NoteList.Length;
        for(int i = 0; i < length; i++){
            DataRow _cur = dt.NewRow();
            _cur["Date"] = NoteList[i].Date.ToString("MM/dd/yyy");
            _cur["Title"] = NoteList[i].Name;
            //somehow get these buttons to go to a compose page. 
            _cur["Actions"] =   "<a href='/Home/Compose/"+
                                NoteList[i].ID.ToString()+
                                "'>Edit</a>"+
                                "<a href='/Home/DeleteNote/"+
                                NoteList[i].ID.ToString()+
                                "'>Delete</a>";
            dt.Rows.Add(_cur);
        }
        return dt;
    }

    public static NotepadModel[] ConvertFileDirectoryToNoteList(System.IO.DirectoryInfo Dir)
    // takes a file directory and turns it into an array of objects
    {
        System.IO.FileInfo[] Files = GetFileListFromDirectory(Dir);
        int length = Files.Length;
        var noteList = new NotepadModel[length];
        for (int i = 0; i < length; i++)
        {
            noteList[i] = getModelFromFile(Dir.ToString()+Files[i].ToString());
            noteList[i].ID=i;
        }
        return noteList;
    }

    public static System.IO.FileInfo[] GetFileListFromDirectory(System.IO.DirectoryInfo d)
    //returns proper files (TXT and MD) from directory
    {
        return d.GetFiles("*.txt").Union(d.GetFiles("*.md")).ToArray();
    }

    public static NotepadModel[] GetNoteListFromPath(string path)
    //returns notelist from a path
    {
        System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(path);
        return ConvertFileDirectoryToNoteList(d);
    }

    public static string RenderNoteListToHTML(NotepadModel[] noteList)
    //turns notelist into HTML table
    {
        var arr = ConvertArrayToDataTable(noteList);
        return ConvertDataTableToHTML(arr);
    }

    public static string RenderDirToHTML(string path)
    //given a path, it renders the notelist into html
    {
        return RenderNoteListToHTML(GetNoteListFromPath(path));
    }

    public static string RenderNoteToHTML(NotepadModel note)
    //renders notepad object into a editing page
    {
        string HTMLPage = "";

        HTMLPage += RenderTopBarHTML(note);
        HTMLPage += RenderMiddleContentHTML(note);
       // HTMLPage += RenderBottomBarHTML(note);

        return HTMLPage;
    }

    public static string RenderTopBarHTML(NotepadModel note)
    {
        string HTMLPage = "";

        HTMLPage += "<div class='contents'><label for='title'>Title of the note: </label>"+
        "<input type='text' name='title' id='title' value='" + note.Name +
        "'required><label for='markdown'>Markdown: </label><input type='checkbox' id='markdown' name='markdown'";
        if(note.Markdown){
        HTMLPage += "checked";
        }
        HTMLPage +=	"></div>";

        return HTMLPage;
    }

    public static string RenderMiddleContentHTML(NotepadModel note)
    {
        string HTMLPage = "";

        HTMLPage += "<div class='container'><label for='note'>Content of the note: </label><textarea id='note' name='note' rows='10' placeholder='Please input text here...'>"+
        note.Content+"</textarea></div>";


        HTMLPage += "<div class='container'><div class='row'>"+
        "<div class='col-6'><label for='categories'>In Categories: </label>"+
        "<textarea id='categories' name='categories' rows='4' placeholder='No categories added' readonly disabled>";
        if(note.Categories.Length>0){
            for(int i = 0; i < note.Categories.Length; i++){
    	        HTMLPage += note.Categories[i] + "\n";
            }
        }
        HTMLPage += "</textarea></div>";
      //  HTMLPage += "</textarea></div><div class='col-6'>"+
      //  "<label for='cname'>Category name: </label><input type='text' name='cname' id='cname' required>"+
      //  "<input type='submit' value='Add'><input type='submit' value='Remove'></div></div></div>";

        return HTMLPage;
    }

    public static string[] GetAllCategoriesFromList(NotepadModel[] noteList)
    {
        string[] categories = new string[GetCountCategories(noteList)];
        int allCat = 0;

        for(int i = 0; i < noteList.Length; i++){
            for(int j = 0; j < noteList[i].Categories.Length;j++){
                if(!categories.Contains(noteList[i].Categories[j])){
                    categories[allCat]=noteList[i].Categories[j];
                    allCat++;
                }
            }
        }
        return categories;
    }

    public static int GetCountCategories(NotepadModel[] noteList)
    {
        int sum = 0;
        for(int i = 0; i < noteList.Length; i++){
            for(int j = 0; j < noteList[i].Categories.Length;j++){
                sum++;
            }
        }
        return sum;
    }

    public static string[] GetAllCategoryFromDirectory(string path){
        return GetAllCategoriesFromList(GetNoteListFromPath(path));
    }

    public static string PrintCatDropDown(string[] catList){
string HTMLString ="<label for='category'>Category:</label><select id='category'>"+
"<option value=''>--Select--</option>";

for(int i = 0; i<catList.Length;i++){
    if(!String.IsNullOrEmpty(catList[i])){
        HTMLString+="<option value='"+ catList[i]+"'>"+catList[i]+"</option>";
    }
   
}
HTMLString += "</select>";

return HTMLString;
    }

    public static string PrintDropDownFromDir(string path){
        return PrintCatDropDown(GetAllCategoryFromDirectory(path));
    }

    public static string RenderFilteredNoteList(NotepadModel[] noteList, string filter){
        return RenderNoteListToHTML(ReturnCategoryFilter(noteList,filter));
    }

    public static NotepadModel[] ReturnCategoryFilter(NotepadModel[] noteList,string categoryFilter){
        //string[] categories = new string[GetCountCategories(noteList)];
        int cur = 0;
        var returnNoteList = new NotepadModel[CountMatchCategories(noteList,categoryFilter)];

        for(int i = 0; i < noteList.Length; i++){
            for(int j = 0; j < noteList[i].Categories.Length;j++){
                if(noteList[i].Categories[j]==categoryFilter){
                    returnNoteList[cur]=noteList[i];
                    cur++;
                }
            }
        }
        return returnNoteList;
    }

        public static int CountMatchCategories(NotepadModel[] noteList, string Match){
        int sum = 0;
        for(int i = 0; i < noteList.Length; i++){
            for(int j = 0; j < noteList[i].Categories.Length;j++){
                if(noteList[i].Categories[j]==Match){
                   sum++; 
                }
            }
        }
        return sum;
    }


   
public static NotepadModel[] RemoveNoteFromList(NotepadModel[] noteList, NotepadModel noteToRemove){
    var editList = new NotepadModel[noteList.Length];
    int cur = 0;
    for(int i = 0; i < noteList.Length; i++){
                if(noteList[i]!=noteToRemove){
                    editList[cur] = noteList[i];    
                    cur++;
                }
        }
    
    return editList;
}

    public static void DeleteFileOfPath(string path){
    //check if file exists
        if(System.IO.File.Exists(path)){
            System.IO.File.Delete(path); 
        }
    //check if file is txt or md
    }
    public static void DeleteFileOfTitle(string title){
        string path = @"Notes/" + title;
        //check if txt or md
        if(System.IO.File.Exists(path+".txt")){
            DeleteFileOfPath(path+".txt");
        }else if(System.IO.File.Exists(path+".md")){
            DeleteFileOfPath(path+".md");
        }else{
            //title not found
        }
    }
    public static void DeleteFileID(int ID, System.IO.DirectoryInfo Dir){
        System.IO.FileInfo[] Files = GetFileListFromDirectory(Dir);
        string path = Dir.ToString()+Files[ID].ToString();
        DeleteFileOfPath(path);
    }
    public static NotepadModel NoteFromID(NotepadModel[] noteList,int ID){
        var note = new NotepadModel();
        for(int i = 0; i<noteList.Length; i++){
            if(noteList[i].ID == ID){
                note = noteList[i];
            }
        }
        return note;
    }
    public static NotepadModel returnTxtStringFromInput(string title, string markdown, string content, string categories= ""){
        bool mark;
        if (markdown != null && markdown == "on"){
            mark = true;
        }
        else{
            mark = false;
        }
        var noteModel = new NotepadModel(){
            Name = title,
            Date = DateTime.Today,
            Markdown = mark,
            Content = content,
            Categories = categories.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
        };

        return noteModel;
    }



    

    }



}



/*
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }

            */