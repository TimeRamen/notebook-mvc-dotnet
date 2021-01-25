using System;
using System.Linq;
using System.Collections.Generic;

namespace Z01.Models
{
    public class NotepadModel
    {
        public int ID {get; set;}
        public string Name { get; set; }
        public DateTime Date{get; set;}
        public string[] Categories{get; set;}

        public bool Markdown{get; set;}

        public string Content{get; set;}


    }



}

/*
//    public class Paginate
 //   {
  //      public NotepadModel[][] pageList{get; set;}
        //the first number is the page number, the second number are the notes.
   // }
        string NoteFile(NotepadModel X){
            string curNote = "Title: ";
            string endSpace = "\n";
            curNote = curNote + X.Name + endSpace;
            curNote += "Date: ";
            curNote = curNote + X.Date.ToString() + endSpace;

            curNote += "Categories: ";
            curNote = curNote + X.Categories.ToString() + endSpace;

            curNote = curNote + endSpace + X.Content;
            return curNote;
        }

        NotepadModel getNoteFromString(string X){
            string noteTitle = "";
            string noteDate = "";
            string[] noteCategories = new string[]{""};
            string noteContent = "";

            //make logic on how to get these values from the string, which is supposed to be the text file that is read
            string[] textLines = X.Split(new[]{ Environment.NewLine }, StringSplitOptions.None);

            //
            noteTitle = textLines[0].Substring(textLines[0].LastIndexOf(':') + 1);
            //
            noteDate = textLines[1].Substring(textLines[1].LastIndexOf(':') + 1);
            //
            string cur = textLines[2].Substring(textLines[2].LastIndexOf(':') + 1).Split('[', ']')[1];
            noteCategories = cur.Split(',');
            //

            //var lines = textLines.Skip(3);
            noteContent = string.Join(Environment.NewLine, textLines.Skip(3).ToArray());
          // return string.Join(Environment.NewLine, lines.ToArray());



            NotepadModel createdNote = new NotepadModel(){
                Name= noteTitle,
                Date= Convert.ToDateTime(noteDate),
                Categories= noteCategories,
                Content= noteContent
            };

            return createdNote;
        }

        void writeStringToPath(string Note, string Path){
            System.IO.File.WriteAllText(@Path, Note);
        }

        void writeNoteToPath(NotepadModel Note, string Path){
            writeStringToPath(NoteFile(Note),Path);
        }
*/
        
