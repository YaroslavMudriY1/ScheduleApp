//This code was generated by a tool.
//Changes to this file will be lost if the code is regenerated.
// See the blog post here for help on using the generated code: http://erikej.blogspot.dk/2014/10/database-first-with-sqlite-in-universal.html
using SQLite;
using System;

namespace Model
{
    public class SQLiteDb
    {
        string _path;
        public SQLiteDb(string path)
        {
            _path = path;
        }
        
         public void Create()
        {
            using (SQLiteConnection db = new SQLiteConnection(_path))
            {
                db.CreateTable<Groups>();
                db.CreateTable<Schedule>();
                db.CreateTable<Subjects>();
                db.CreateTable<Teachers>();
            }
        }
    }
    public partial class Groups
    {
        [PrimaryKey]
        public Int64 group_id { get; set; }
        
        public String group_name { get; set; }
        
    }
    
    public partial class Schedule
    {
        [PrimaryKey]
        public Int64 schedule_id { get; set; }
        
        public Int64? group_id { get; set; }
        
        public Int64? subject_id { get; set; }
        
        public Int64? teacher_id { get; set; }
        
        public String classroom { get; set; }
        
        public String day_of_week { get; set; }
        
        public String time_start { get; set; }
        
        public String time_end { get; set; }
        
    }
    
    public partial class Subjects
    {
        [PrimaryKey]
        public Int64 subject_id { get; set; }
        
        public String subject_name { get; set; }
        
    }
    
    public partial class Teachers
    {
        [PrimaryKey]
        public Int64 teacher_id { get; set; }
        
        public String teacher_name { get; set; }
        
    }
    
}
